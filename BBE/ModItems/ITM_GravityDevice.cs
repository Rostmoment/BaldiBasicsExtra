using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering;
using HarmonyLib;
using BBE.Extensions;
using MTM101BaldAPI;
using System.Collections.Generic;
using MTM101BaldAPI.Registers;

namespace BBE.ModItems
{
    public class ITM_GravityDevice : Item
    {
        private PlayerManager player;
        private List<NPC> toIgnore;
        private List<CameraModifier> modifiers;
        private GameCamera camera;
        private static bool isActive;
        public override bool Use(PlayerManager pm)
        {
            if (isActive || pm.ec.GetEvent(RandomEventType.Gravity) != null)
                return false;
            isActive = true;
            player = pm;
            camera = CoreGameManager.Instance.GetCamera(pm.playerNumber);
            toIgnore = new List<NPC>();
            modifiers = new List<CameraModifier>();
            foreach (NPC npc in player.ec.npcs)
            {
                if (npc.GetMeta().tags.Contains("BBE_NPCGravityDeviceIgnore"))
                    continue;
                npc.Navigator.Entity.IgnoreEntity(player.plm.Entity, true);
                toIgnore.Add(npc);
            }
            StartCoroutine(FlipPlayer());
            StartCoroutine(Timer(15));
            return true;
        }
        private IEnumerator FlipPlayer()
        {
            for (int i = 0; i < 180; i++)
            {
                CameraModifier modifier = new CameraModifier(Vector3.zero, new Vector3(0, 0, 1));
                camera.cameraModifiers.Add(modifier);
                if (i == 89)
                {
                    player.Reverse();
                    SubtitleManager.Instance.Reverse();
                }
                modifiers.Add(modifier);
                yield return null;
            }
            yield break;
        }
        private IEnumerator Disable()
        {
            foreach (CameraModifier modifier in modifiers)
            {
                camera.cameraModifiers.Remove(modifier);
            }
            modifiers.Clear();
            isActive = false;
            yield break;
        }
        private IEnumerator Timer(float time)
        {
            float left = time;
            while (left > 0) 
            {
                left -= Time.deltaTime * player.ec.EnvironmentTimeScale;
                yield return null;
            }
            StartCoroutine(Disable());
            yield return new WaitUntil(() => isActive);
            player.Reverse();
            SubtitleManager.Instance.Reverse();
            Destroy(gameObject);
        }
        void OnDestroy()
        {
            foreach (CameraModifier modifier in modifiers.Where(x => camera.cameraModifiers.Contains(x)))
                camera.cameraModifiers.Remove(modifier);
            foreach (NPC npc in toIgnore)
                npc.Navigator.Entity.IgnoreEntity(player.plm.Entity, false);
            toIgnore.Clear();
            modifiers.Clear();
            isActive = false;
        }
    }
}
