using UnityEngine;
using System.Collections;
using BBE.Helpers;
using System.Linq;
using UnityEngine.Rendering;
using HarmonyLib;

namespace BBE.ModItems
{
    public class ITM_GravityDevice : Item
    {
        // Turns the player over as if a gravity chaos event had begun
        // TODO: Rewrite code and make it better
        public void FlipPlayer(bool value)
        {
            MirrorMode mirrorMode = null;
            if (gameObject.GetComponent<MirrorMode>().IsNull()) 
            {
                mirrorMode = gameObject.AddComponent<MirrorMode>();
            }
            else
            {   
                mirrorMode = gameObject.GetComponent<MirrorMode>();
            }
            if (mirrorMode.IsNull()) return;
            Camera[] cameraToMirror = mirrorMode.cameraToMirror;
            cameraToMirror[0] = Singleton<CoreGameManager>.Instance.GetCamera(0).camCom;
            cameraToMirror[1] = Singleton<CoreGameManager>.Instance.GetCamera(0).billboardCam;
            Camera[] array = cameraToMirror;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).Reverse();
            if (value)
            {
                OriginalProjectionMatrices = new Matrix4x4[cameraToMirror.Length];
                for (int i = 0; i < cameraToMirror.Length; i++)
                {
                    OriginalProjectionMatrices[i] = cameraToMirror[i].projectionMatrix;
                }
                foreach (Camera obj in array)
                {
                    Matrix4x4 projectionMatrix = obj.projectionMatrix;
                    projectionMatrix *= Matrix4x4.Scale(new Vector3(1f, -1f, 1f));
                    obj.projectionMatrix = projectionMatrix;
                }
                RenderPipelineManager.beginCameraRendering += mirrorMode.ReverseCulling;
                RenderPipelineManager.endCameraRendering += mirrorMode.ReturnCulling;
            }
            else
            {
                for (int i = 0; i < cameraToMirror.Length; i++)
                {
                    cameraToMirror[i].projectionMatrix = OriginalProjectionMatrices[i];
                }
                RenderPipelineManager.beginCameraRendering -= mirrorMode.ReverseCulling;
                RenderPipelineManager.endCameraRendering -= mirrorMode.ReturnCulling;
            }
        }

        public override bool Use(PlayerManager pm)
        {
            if (IsActive)
            {
                return false;
            }
            foreach (NPC npc in Singleton<BaseGameManager>.Instance.Ec.Npcs)
            {
                try
                {
                    if (!IgnorePlayerNPCs.Contains(npc))
                    {
                        IgnorePlayerNPCs = IgnorePlayerNPCs.AddToArray(npc);
                        npc.Navigator.Entity.IgnoreEntity(Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity, true);
                    }
                }
                catch { }
            }
            IsActive = true;
            FlipPlayer(true);
            StartCoroutine(EndEffect(15f));
            return true;
        }

        private IEnumerator EndEffect(float time)
        {
            float TimeToEnd = time;
            while (TimeToEnd > 0)
            {
                foreach (NPC npc in Singleton<BaseGameManager>.Instance.Ec.Npcs)
                {
                    try
                    {
                        if (!IgnorePlayerNPCs.Contains(npc))
                        {
                            IgnorePlayerNPCs = IgnorePlayerNPCs.AddToArray(npc);
                            npc.Navigator.Entity.IgnoreEntity(Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity, true);
                        }
                    }
                    catch { }
                }
                TimeToEnd -= Time.deltaTime;
                yield return null;
            }
            FlipPlayer(false);
            IsActive = false;
            foreach (NPC npc in Singleton<BaseGameManager>.Instance.Ec.Npcs)
            {
                try
                {
                    npc.Navigator.Entity.IgnoreEntity(Singleton<CoreGameManager>.Instance.GetPlayer(0).plm.Entity, false);
                }
                catch { }
            }
            IgnorePlayerNPCs = new NPC[] { };
            Destroy(gameObject);
            yield break;
        }
        void OnDestroy()
        {
            MirrorMode mirrorMode = null;
            if (gameObject.GetComponent<MirrorMode>().IsNull())
            {
                mirrorMode = gameObject.AddComponent<MirrorMode>();
            }
            else
            {
                mirrorMode = gameObject.GetComponent<MirrorMode>();
            }
            if (mirrorMode.IsNull()) return;
            Camera[] cameraToMirror = mirrorMode.cameraToMirror;
            cameraToMirror[0] = Singleton<CoreGameManager>.Instance.GetCamera(0).camCom;
            cameraToMirror[1] = Singleton<CoreGameManager>.Instance.GetCamera(0).billboardCam;
            Camera[] array = cameraToMirror;
            if (Singleton<CoreGameManager>.Instance.GetPlayer(0).reversed) Singleton<CoreGameManager>.Instance.GetPlayer(0).Reverse();
            for (int i = 0; i < cameraToMirror.Length; i++)
            {
                cameraToMirror[i].projectionMatrix = OriginalProjectionMatrices[i];
            }
            RenderPipelineManager.beginCameraRendering -= mirrorMode.ReverseCulling;
            RenderPipelineManager.endCameraRendering -= mirrorMode.ReturnCulling;
            IsActive = false;
        }
        private static NPC[] IgnorePlayerNPCs = new NPC[] { };
        public Matrix4x4[] OriginalProjectionMatrices;
        public static bool IsActive;
    }
}
