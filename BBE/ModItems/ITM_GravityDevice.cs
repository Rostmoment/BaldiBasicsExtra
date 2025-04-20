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
        private AudioManager audMan;
        private PlayerManager player;
        private List<NPC> toIgnore;
        private static List<CameraModifier> modifiers;
        private GameCamera camera;
        public override bool Use(PlayerManager pm)
        {
            if (modifiers == null)
                modifiers = new List<CameraModifier>();
            if (modifiers.Count != 0 || pm.ec.GetEvent(RandomEventType.Gravity) != null)
                return false;
            audMan = gameObject.AddAudioManager();
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
        private IEnumerator Timer(float time)
        {
            float left = time;
            while (left > 0) 
            {
                left -= Time.deltaTime * player.ec.EnvironmentTimeScale;
                if (!audMan.AnyAudioIsPlaying && left < 3.9f)
                    audMan.PlaySingle("GravityDeviceTimer");
                yield return null;
            }
            Destroy(gameObject);
        }
        void OnDestroy()
        {
            foreach (CameraModifier modifier in modifiers.Where(x => camera.cameraModifiers.Contains(x)))
                camera.cameraModifiers.Remove(modifier);
            foreach (NPC npc in toIgnore.Where(x => x != null))
                npc.Navigator.Entity.IgnoreEntity(player.plm.Entity, false);
            player.Reverse();
            SubtitleManager.Instance.Reverse();
            toIgnore.Clear();
            modifiers.Clear();
        }
    }
}
