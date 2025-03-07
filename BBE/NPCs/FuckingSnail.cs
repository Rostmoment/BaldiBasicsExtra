using BBE.Extensions;
using BBE.Creators;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.NPCs
{
    class FuckingSnail : NPC, INPCPrefab
    {

        public void SetupAssets()
        {
            audMan = this.GetComponent<AudioManager>();
            audMan.overrideSubtitleColor = false;
            spriteRenderer[0].sprite = BasePlugin.Asset.GetOrAdd<Sprite>("SnailBaseSprite",
                AssetsHelper.CreateTexture("Textures", "NPCs", "Snail", "BBE_SnailBase.png").ToSprite(50)); 
            scream = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "BBE_SnailScream.mp3"), SoundType.Voice, "d4b585", subtitleKey: "BBE_Snail_Scream");
        }
        public AudioManager audMan;
        [SerializeField]
        public SoundObject scream;
        public override void Initialize()
        {
            base.Initialize();
            behaviorStateMachine.ChangeState(new FuckingSnailWandering(this));
        }
    }
    class FuckingSnailStateBase : NpcState
    {
        protected FuckingSnail snail;
        public FuckingSnailStateBase(FuckingSnail npc) : base(npc)
        {
            snail = npc;
        }
    }
    class FuckingSnailWandering : FuckingSnailStateBase
    {
        private float time;
        public FuckingSnailWandering(FuckingSnail npc) : base(npc)
        {
            snail.Navigator.SetSpeed(10, 10);
            time = 30f;
        }
        public override void Enter()
        {
            base.Enter();
            if (!npc.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            }
        }
        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * snail.ec.NpcTimeScale;
            if (time <= 0)
            {
                snail.behaviorStateMachine.ChangeState(new FuckingSnailDontStepOnHim(snail));
            }
        }
    }
    class FuckingSnailDontStepOnHim : FuckingSnailWandering
    {
        private float time;
        public FuckingSnailDontStepOnHim(FuckingSnail npc) : base(npc)
        {
        }
        public override void OnStateTriggerStay(Collider other)
        {
            base.OnStateTriggerStay(other);
            if (other.CompareTag("Player"))
            {
                snail.ec.MakeNoise(snail.transform.position, 126);
                snail.audMan.PlaySingle(snail.scream);
                snail.behaviorStateMachine.ChangeState(new FuckingSnailCooldown(snail));
            }
        }
    }
    class FuckingSnailCooldown : FuckingSnailStateBase
    {
        private float time;
        public FuckingSnailCooldown(FuckingSnail npc) : base(npc)
        {
            time = 9.6f;
            snail.Navigator.SetSpeed(0, 0);
        }
        public override void Update()
        {
            time -= Time.deltaTime * snail.ec.NpcTimeScale;
            if (time <= 0)
            {
                snail.behaviorStateMachine.ChangeState(new FuckingSnailWandering(snail));
            }
        }
    }
}
