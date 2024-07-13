using BBE.Helpers;
using MTM101BaldAPI.Components;
using System;
using System.Collections.Generic;
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
            if (getOut.IsNull()) getOut = AssetsHelper.CreateSoundObject("Audio/NPCs/Kulak_GetOut.wav", SoundType.Voice, new Color(0.91796875f, 0.90625f, 0.35546875f), SubtitleKey: "Kulak_GetOut");
            if (smash.IsNull()) smash = AssetsHelper.CreateSoundObject("Audio/NPCs/Kulak_Smash.wav", SoundType.Voice, new Color(0.91796875f, 0.90625f, 0.35546875f), SubtitleKey: "Kulak_Smash");
            if (hiya.IsNull()) hiya = AssetsHelper.CreateSoundObject("Audio/NPCs/Kulak_Hiya.wav", SoundType.Voice, new Color(0.91796875f, 0.90625f, 0.35546875f), SubtitleKey: "Kulak_Hiya");
            if (animator.IsNull()) animator = gameObject.AddComponent<CustomSpriteAnimator>();
            animator.spriteRenderer = this.spriteRenderer[0];
            animator.animations.Add("Angry", new CustomAnimation<Sprite>(60, new Sprite[] { AssetsHelper.SpriteFromFile("Textures/NPCs/Kulak/Angry.png", 50f) }));
            animator.animations.Add("Idle", new CustomAnimation<Sprite>(60, new Sprite[] { AssetsHelper.SpriteFromFile("Textures/NPCs/Kulak/Base.png", 50f) }));
        }
        public void StartPersuingPlayer(PlayerManager player)
        {
            behaviorStateMachine.ChangeNavigationState(new NavigationState_TargetPlayer(this, 63, player.transform.position));
        }
        public void PersuePlayer(PlayerManager player)
        {
            behaviorStateMachine.CurrentNavigationState.UpdatePosition(player.transform.position);
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

        public override void DoorHit(StandardDoor door)
        {
            if (door.locked) 
            {
                door.Unlock();
            }
            door.OpenTimed(20, false);
        }
    }
    public class Kulak_Wandering : Kulak_StateBase
    {
        private float timeLeft = 10f;
        public Kulak_Wandering(NPC npc, Kulak kulakk) : base(npc, kulakk)
        {
            timeLeft = 10f;
        }
        public override void Enter()
        {
            base.Enter();
            kulak.SetSprite("Idle");
            if (kulak.Navigator.passableObstacles.Contains(PassableObstacle.Window))
            {
                kulak.Navigator.passableObstacles.Remove(PassableObstacle.Window);
            }
            if (kulak.Navigator.passableObstacles.Contains(PassableObstacle.Bully))
            {
                kulak.Navigator.passableObstacles.Remove(PassableObstacle.Bully);
            }
            if (kulak.Navigator.passableObstacles.Contains(PassableObstacle.LockedDoor))
            {
                kulak.Navigator.passableObstacles.Remove(PassableObstacle.LockedDoor);
            }
            npc.looker.Blink();
            if (!npc.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
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
        public override void PlayerSighted(PlayerManager player)
        {
            base.PlayerSighted(player);
            if (!player.Tagged)
            {
                kulak.StartPersuingPlayer(player);
            }
        }

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            if (!player.Tagged)
            {
                kulak.PersuePlayer(player);
            }
        }
        public override void Enter()
        {
            base.Enter();
            kulak.SetSprite("Angry");
            if (!kulak.Navigator.passableObstacles.Contains(PassableObstacle.Window))
            {
                kulak.Navigator.passableObstacles.Add(PassableObstacle.Window);
            }
            if (!kulak.Navigator.passableObstacles.Contains(PassableObstacle.Bully))
            {
                kulak.Navigator.passableObstacles.Add(PassableObstacle.Bully);
            }
            if (!kulak.Navigator.passableObstacles.Contains(PassableObstacle.LockedDoor))
            {
                kulak.Navigator.passableObstacles.Add(PassableObstacle.LockedDoor);
            }
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
                kulak.Push(other.GetComponent<NPC>().GetComponent<Entity>());
                kulak.audMan.PlaySingle(kulak.getOut);
                kulak.WanderNormal();
            }
        }
    }
}
