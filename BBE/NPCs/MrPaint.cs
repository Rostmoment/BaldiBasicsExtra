using BBE.Extensions;
using BBE.Creators;
using BBE.ModItems;
using MTM101BaldAPI.Registers;
using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.NPCs
{
    // Paint object
    public class PaintBase : MonoBehaviour
    {
        public GameObject item;
        public GameObject paintObject;
        protected AudioManager audMan;
        private float time;
        private SpriteRenderer sprite;
        public virtual float EffectTime => 10;
        public virtual void Initialize(Sprite spr, GameObject parent)
        {
            audMan = gameObject.AddAudioManager();
            audMan.PlaySingle("PaintSlap");
            time = EffectTime;
            sprite = gameObject.AddComponent<SpriteRenderer>();
            sprite.sprite = spr;
            gameObject.AddCollider(new Vector3(4.5f, 15f, 4.5f));
            SetPosition(parent);
        }
        private void SetPosition(GameObject parent)
        {
            Vector3 pos = new Vector3(parent.transform.position.x, 0.1f, parent.transform.position.z);
            Vector3 lookAt = new Vector3(parent.transform.position.x, 180, parent.transform.position.z);
            gameObject.transform.position = pos;
            gameObject.transform.LookAt(lookAt);
        }
        public virtual void Update() 
        {
            time -= Time.deltaTime;
            if (time < 0)
            {
                Destroy(this);
                Destroy(paintObject);
                if (item != null) Destroy(item);
            }
        }
        public virtual void OnDestroy() 
        {
            
        }
        public virtual void OnTriggerEnter(Collider other)
        {
        }
        public virtual void OnTriggerExit(Collider other)
        {

        }
    }
    public class BluePaint : PaintBase
    {
        private SoundObject AudSlipping => ((ITM_NanaPeel)ItemMetaStorage.Instance.FindByEnum(Items.NanaPeel).value.item).audSlipping;
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            Entity entity = other.GetComponent<Entity>();
            if (entity == null) return;
            if (!other.isTrigger) return;
            if (entity.Grounded)
            {
                entity.AddForce(new Force(entity.Velocity.normalized, 45f + entity.Velocity.magnitude, entity.Velocity.magnitude + (-45f * 0.9f)));
                audMan.PlaySingle(AudSlipping);
            }
        }
    }
    public class PinkPaint : YellowPaint
    {
        // Most lazy coded class ever I made
        public override float modifer => 0.5f;
    }
    public class PurplePaint : PaintBase
    {
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.TryGetComponent<Entity>(out Entity entity))
            {
                entity.Teleport(BaseGameManager.Instance.ec.RandomCell(false, false, true).TileTransform.position + Vector3.up * 5f);
                audMan.PlaySingle("Teleport");
            }
        }
    }
    public class YellowPaint : PaintBase
    {
        public virtual float modifer => 2f;
        public MovementModifier moveMod = null;
        private List<NPC> npcs = new List<NPC>() { };
        private List<PlayerManager> players = new List<PlayerManager>() { };
        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            Entity entity = other.GetComponent<Entity>();
            if (entity == null) return;
            if (!other.isTrigger || !entity.Grounded) return;
            if (other.CompareTag("Player"))
            {
                PlayerManager player = other.GetComponent<PlayerManager>();
                players.Add(player);
                if (!player.plm.am.moveMods.Contains(moveMod)) player.plm.am.moveMods.Add(moveMod);
            }
            if (other.CompareTag("NPC"))
            {
                NPC npc = other.GetComponent<NPC>();
                npcs.Add(npc);
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier)) activityModifier.moveMods.Add(moveMod);
            }
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            foreach (NPC npc in npcs)
            {
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier) && activityModifier.moveMods.Contains(moveMod))
                {
                    activityModifier.moveMods.Remove(moveMod);
                }
            }
            foreach (PlayerManager player in players)
            {
                if (player.Am.moveMods.Contains(moveMod))
                {
                    player.Am.moveMods.Remove(moveMod);
                }
            }
        }
        public override void Initialize(Sprite spr, GameObject parent)
        {
            base.Initialize(spr, parent);
            moveMod = new MovementModifier(default(Vector3), modifer);
        }
    }
    public class MrPaint : NPC, INPCPrefab
    {

        void IPrefab.SetupAssets()
        {
            spriteRenderer[0].sprite = BasePlugin.Asset.GetOrAdd("MrPaintBaseSprite", AssetsHelper.CreateTexture("Textures", "NPCs", "MrPaint", "BBE_MrPaintBase.png").ToSprite(50));
        }
        public override void Initialize()
        {
            base.Initialize();
            this.behaviorStateMachine.ChangeState(new MrPaint_Wandering(this));
        }
    }
    public class MrPaint_BaseState : NpcState
    {
        protected MrPaint mrPaint;
        public MrPaint_BaseState(MrPaint npc) : base(npc)
        {
            mrPaint = npc;
        }
    }
    public class MrPaint_Wandering : MrPaint_BaseState
    {
        private PaintBase paint;
        private static float Cooldown => 25f;
        private float time;
        public MrPaint_Wandering(MrPaint npc) : base(npc)
        {
            time = Cooldown;
        }
        public override void Enter()
        {
            base.Enter();
            if (!mrPaint.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(mrPaint, 0));
            }
        }
        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * mrPaint.ec.NpcTimeScale;
            if (time <= 0)
            {
                Color color = Color.white;
                GameObject game = new GameObject("MrPaintItem");
                switch (UnityEngine.Random.Range(0, 4))
                {
                    case 0:
                        paint = game.AddComponent<PinkPaint>();
                        color = Color.magenta;
                        break;
                    case 1:
                        paint = game.AddComponent<YellowPaint>();
                        color = Color.yellow;
                        break;
                    case 2:
                        paint = game.AddComponent<BluePaint>();
                        color = Color.blue;
                        break;
                    case 3:
                        paint = game.AddComponent<PurplePaint>();
                        color = AssetsHelper.ColorFromHex("761594");
                        break;
                }
                paint.Initialize(BasePlugin.Asset.Get<Sprite>("PaintBase").ReplaceColor(Color.white, color), mrPaint.gameObject);
                paint.paintObject = game;
                time = Cooldown;
            }
        }
    }
}
