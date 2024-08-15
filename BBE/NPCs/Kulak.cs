using BBE.Helpers;
using MTM101BaldAPI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBE.NPCs
{
    public class Kulak : NPC
    {
        public AudioManager audMan;
        public CustomSpriteAnimator animator;
        public SoundObject getOut;
        public SoundObject smash;
        public SoundObject hiya;
        public override void Initialize()
        {
            base.Initialize();
            Setup();
            audMan = this.GetComponent<AudioManager>();
            audMan.overrideSubtitleColor = false;
            Navigator.passableObstacles.Add(PassableObstacle.Window);
            Navigator.passableObstacles.Add(PassableObstacle.Bully);
            Navigator.passableObstacles.Add(PassableObstacle.LockedDoor);
            behaviorStateMachine.ChangeState(new Kulak_Wandering(this, this));
        }
        public void SetSprite(string name)
        {
            animator.Play(name, 1f);
        }
        public void Setup()
        {
            if (getOut.IsNull()) getOut = AssetsHelper.CreateSoundObject("Audio/NPCs/Kulak_GetOut.wav", SoundType.Voice, AssetsHelper.ColorFromHex("f5de5b"), SubtitleKey: "Kulak_GetOut");
            if (smash.IsNull()) smash = AssetsHelper.CreateSoundObject("Audio/NPCs/Kulak_Smash.wav", SoundType.Voice, AssetsHelper.ColorFromHex("f5de5b"), SubtitleKey: "Kulak_Smash");
            if (hiya.IsNull()) hiya = AssetsHelper.CreateSoundObject("Audio/NPCs/Kulak_Hiya.wav", SoundType.Voice, AssetsHelper.ColorFromHex("f5de5b"), SubtitleKey: "Kulak_Hiya");
            if (animator.IsNull()) animator = gameObject.AddComponent<CustomSpriteAnimator>();
            animator.spriteRenderer = this.spriteRenderer[0];
            animator.animations.Add("Angry", new CustomAnimation<Sprite>(6, AssetsHelper.CreateSpriteSheet("Textures/NPCs/Kulak/Angry.png", 5, 1, 50).ToArray()));
            animator.animations.Add("Idle", new CustomAnimation<Sprite>(6, AssetsHelper.CreateSpriteSheet("Textures/NPCs/Kulak/Angry.png", 5, 1, 50).ToArray().ReverseAsArray()));
        }
        public void Angry()
        {
            behaviorStateMachine.ChangeState(new Kulak_Angry(this, this));
        }
        public void WanderNormal()
        {
            behaviorStateMachine.ChangeState(new Kulak_Wandering(this, this));
        }
        public void Push(Entity entity)
        {
            SetGuilt(10f, "Bullying");
            entity.AddForce(new Force(transform.forward, 150, -50f));
        }
    }
    public class Kulak_StateBase : NpcState
    {
        public Kulak_StateBase(NPC npc, Kulak kulakk) : base(npc)
        {
            kulak = kulakk;
        }
        protected Kulak kulak;
    }
    public class Kulak_Wandering : Kulak_StateBase
    {
        private float timeLeft = 30f;
        public Kulak_Wandering(NPC npc, Kulak kulakk) : base(npc, kulakk)
        {
            timeLeft = 30f;
        }
        public override void Enter()
        {
            base.Enter();
            kulak.SetSprite("Idle");
            npc.looker.Blink();
            if (!npc.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            }
        }
        public override void OnStateTriggerStay(Collider other)
        {
            base.OnStateTriggerStay(other);
            if (other.CompareTag("Window"))
            {
                if (!other.GetComponent<Window>().broken)
                {
                    other.GetComponent<Window>().Break(false);
                    kulak.audMan.PlaySingle(new List<SoundObject>() { kulak.smash, kulak.hiya }.ChooseRandom());
                }
            }
        }
        public override void Update()
        {
            base.Update();
            timeLeft -= Time.deltaTime * npc.TimeScale;
            if (timeLeft <= 0)
            {
                kulak.Angry();
            }
        }
    }
    public class Kulak_Angry : Kulak_StateBase
    {
        public Kulak_Angry(NPC npc, Kulak kulakk) : base(npc, kulakk)
        {
        }
        public override void Enter()
        {
            base.Enter();
            kulak.SetSprite("Angry");
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }
        public override void OnStateTriggerStay(Collider other)
        {
            base.OnStateTriggerStay(other);
            if (other.CompareTag("Window"))
            {
                if (!other.GetComponent<Window>().broken)
                {
                    other.GetComponent<Window>().Break(false);
                    kulak.audMan.PlaySingle(new List<SoundObject>() { kulak.smash, kulak.hiya }.ChooseRandom());
                }
            }
            if (other.CompareTag("Player"))
            {
                kulak.Push(other.GetComponent<PlayerManager>().plm.entity);
                kulak.audMan.PlaySingle(kulak.getOut);
                kulak.WanderNormal();
            }
            if (other.CompareTag("NPC"))
            {
                if (other.GetComponent<NPC>().character != Character.Principal)
                {
                    kulak.Push(other.GetComponent<NPC>().GetComponent<Entity>());
                    kulak.audMan.PlaySingle(kulak.getOut);
                    kulak.WanderNormal();
                }
            }
        }
        public override void DoorHit(StandardDoor door)
        {
            if (door.locked)
            {
                door.Unlock();
            }
            door.OpenTimed(20, false);
        }
    }
}
