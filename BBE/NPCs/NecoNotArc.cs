using BBE.Helpers;
using MTM101BaldAPI.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBE.NPCs
{
    public class NecoNotArc : NPC
    {
        public ItemObject item;
        public AudioManager audMan;
        public CustomSpriteAnimator animator;
        public override void Initialize()
        {
            base.Initialize();
            CreateSprites();
            audMan = this.GetComponent<AudioManager>();
            behaviorStateMachine.ChangeState(new NecoNotArc_Wandering(this, this));
        }
        public void CreateSprites()
        {
            animator = gameObject.AddComponent<CustomSpriteAnimator>();
            animator.spriteRenderer = this.spriteRenderer[0];
            animator.animations.Add("Idle", new CustomAnimation<Sprite>(60, new Sprite[] { AssetsHelper.SpriteFromFile("Textures/NPCs/NecoNotArc/Base.png", 150f) }));
        }
        public void SetDefaultSprite()
        {
            animator.Play("Idle", 1f);
        }
        public void StartCooldown()
        {
            behaviorStateMachine.ChangeState(new NecoNotArc_Cooldowm(this, this));
        }
        public void EndCooldown()
        {
            behaviorStateMachine.ChangeState(new NecoNotArc_Wandering(this, this));
        }
        public void StartPersuingPlayer(PlayerManager player)
        {
            behaviorStateMachine.ChangeNavigationState(new NavigationState_TargetPlayer(this, 63, player.transform.position));
        }
        public void PersuePlayer(PlayerManager player)
        {
            behaviorStateMachine.CurrentNavigationState.UpdatePosition(player.transform.position);
        }
        public void ReturnItem(PlayerManager player)
        {
            if (!player.itm.InventoryFull())
            {
                player.itm.AddItem(item);
                Singleton<CoreGameManager>.Instance.AddPoints(item.price/3, 0, true);
                audMan.PlaySingle("NecoNotArcReturnItem");
                item = null;
            }
            StartCooldown();
        }
        public void StealItem(PlayerManager player)
        {
            ItemObject[] items = player.itm.items.Where(x => x != player.itm.nothing).ToArray();
            if (items.Length > 0)
            {
                ItemObject itemToSteal = items.ChooseRandom();
                player.itm.Remove(itemToSteal.itemType);
                item = itemToSteal;
            }
            StartCooldown();
        }
    }
    public class NecoNotArc_StateBase : NpcState
    {
        public NecoNotArc_StateBase(NPC npc, NecoNotArc neco) : base(npc)
        {
            necoNotArc = neco;
        }

        protected NecoNotArc necoNotArc;
    }
    public class NecoNotArc_Cooldowm : NecoNotArc_StateBase
    {
        private float time = 45f;
        public NecoNotArc_Cooldowm(NPC npc, NecoNotArc necoNotArc)
        : base(npc, necoNotArc)
        {
            time = 45f;
        }
        public override void Enter()
        {
            base.Enter();
            necoNotArc.SetDefaultSprite();
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }
        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * npc.TimeScale;
            if (time <= 0)
            {
                necoNotArc.EndCooldown();
            }
        }
    }
    public class NecoNotArc_Wandering : NecoNotArc_StateBase
    {
        public NecoNotArc_Wandering(NPC npc, NecoNotArc necoNotArc)
        : base(npc, necoNotArc)
        {
        }
        public override void Enter()
        {
            base.Enter();
            necoNotArc.SetDefaultSprite();
            npc.looker.Blink();
            if (!npc.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            }
        }
        public override void OnStateTriggerEnter(Collider other)
        {
            base.OnStateTriggerEnter(other);
            if (other.CompareTag("Player") && necoNotArc.item.IsNull())
            {
                PlayerManager component = other.GetComponent<PlayerManager>();
                necoNotArc.StealItem(component);
            }
            else if (other.CompareTag("Player"))
            {
                PlayerManager component = other.GetComponent<PlayerManager>();
                necoNotArc.ReturnItem(component);
            }
        }
        public override void PlayerSighted(PlayerManager player)
        {
            base.PlayerSighted(player);
            if (!player.Tagged)
            {
                necoNotArc.StartPersuingPlayer(player);
            }
        }
        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            if (!player.Tagged)
            {
                necoNotArc.PersuePlayer(player);
            }
        }
    }
}
