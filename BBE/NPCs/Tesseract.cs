using BBE.Extensions;
using BBE.Helpers;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BBE.NPCs
{
    class Tesseract : NPC, INPCPrefab
    {
        private int lastAnimationIndex;
        private int cooldown;
        public float EffectTime
        {
            get
            {
                float time = 15f;
                if (FunSettingsType.HardModePlus.IsActive())
                    time += 5;
                return time;
            }
        }
        public List<Entity> effectedRN = new List<Entity>();
        public Dictionary<Entity, float> timers = new Dictionary<Entity, float>();
        [SerializeField]
        private SoundObject corrupt;
        [SerializeField]
        private Sprite[] sprites;
        public void SetupAssets()
        {
            sprites = AssetsHelper.CreateSpriteSheet(6, 4, 40, "Textures", "NPCs", "Tesseract", "BBE_TesseractRotation.png");
            corrupt = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Tesseract", "BBE_TesseractCorrupt.ogg"), SoundType.Voice, "1a324c", subtitleKey: "BBE_TesseractCorrupt");
            spriteRenderer[0].sprite = sprites[0];
        }
        public void PlaySound(AudioManager audMan) => audMan?.PlaySingle(corrupt);
        public override void Initialize()
        {
            base.Initialize();
            behaviorStateMachine.ChangeState(new Tesseract_Wandering(this));
            behaviorStateMachine.ChangeNavigationState(new NavigationState_WanderRandom(this, 0));
        }
        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            UpdateSprite();
        }
        private void UpdateSprite()
        {
            cooldown -= 1;
            if (cooldown <= 0)
            {
                lastAnimationIndex++;
                if (lastAnimationIndex == sprites.Length)
                    lastAnimationIndex = 0;
                spriteRenderer[0].sprite = sprites[lastAnimationIndex];
                cooldown = 10;
            }
        }
        public bool CanBeEffected(Entity entity) => !timers.ContainsKey(entity) && !effectedRN.Contains(entity);
    }
    class Tesseract_Wandering : NpcState
    {
        private Tesseract tesseract;
        public Tesseract_Wandering(Tesseract npc) : base(npc)
        {
            tesseract = npc;
        }
        private IEnumerator EntityForce(Entity entity, float time)
        {
            float left = time;
            while (left > 0)
            {
                entity.AddForce(new Force(Directions.RandomDirection.ToVector3(), 0.1f, -0.05f));
                left -= Time.deltaTime * tesseract.ec.NpcTimeScale;
                yield return null;
            }
            tesseract.StartCoroutine(RemoveTimer(entity, 30));
            yield break;
        }
        private IEnumerator BlindNPC(NPC npc, float time)
        {
            npc.Navigator.Entity.SetBlinded(true);
            float left = time;
            while (left > 0)
            {
                left -= Time.deltaTime * tesseract.ec.NpcTimeScale;
                yield return null;
            }
            npc.Navigator.Entity.SetBlinded(false);
        }
        private IEnumerator FuckPlayerCamera(GameCamera camera, float time)
        {
            List<CameraModifier> modifiers = new List<CameraModifier>();
            float left = time/2f;
            while (left > 0)
            {
                CameraModifier modifier = new CameraModifier(new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)), 
                    new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)));
                camera.cameraModifiers.Add(modifier);
                modifiers.Add(modifier);
                left -= Time.deltaTime * tesseract.ec.NpcTimeScale;
                yield return null;
            }
            foreach (CameraModifier modifier in modifiers)
            {
                camera.cameraModifiers.Remove(modifier);
                yield return null;
            }
            yield break;
        }
        private IEnumerator RemoveTimer(Entity entity, float time)
        {
            float left = time;
            while (left > 0)
            {
                left -= Time.deltaTime * tesseract.ec.NpcTimeScale;
                yield return null;
            }
            tesseract.timers.Remove(entity);
        }
        public override void OnStateTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerManager player = other.GetComponent<PlayerManager>();
                if (!tesseract.CanBeEffected(player.plm.Entity))
                    return;
                tesseract.PlaySound(CoreGameManager.Instance.audMan);
                tesseract.StartCoroutine(EntityForce(player.plm.Entity, tesseract.EffectTime));
                tesseract.StartCoroutine(FuckPlayerCamera(CoreGameManager.Instance.GetCamera(player.playerNumber), tesseract.EffectTime));
            }
            if (other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();
                if (!tesseract.CanBeEffected(npc.Navigator.Entity) || (npc.GetMeta().tags.Contains("BBE_TesseractIgnoreNPC")))
                    return;
                tesseract.PlaySound(npc.GetComponent<AudioManager>());
                tesseract.StartCoroutine(EntityForce(npc.Navigator.Entity, tesseract.EffectTime));
                tesseract.StartCoroutine(BlindNPC(npc, tesseract.EffectTime));
            }
        }
    }
}
