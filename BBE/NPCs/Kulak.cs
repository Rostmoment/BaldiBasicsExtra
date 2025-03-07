using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BBE.Helpers;
using MTM101BaldAPI.Registers;

namespace BBE.NPCs
{
    public class Kulak : NPC, INPCPrefab
    {
        public void SetupAssets()
        {
            sprites = AssetsHelper.CreateSpriteSheet(5, 1, 50, "Textures", "NPCs", "Kulak", "BBE_KulakAngry.png").ToArray();
     
            if (getOut == null)
                getOut = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Kulak", "BBE_Kulak_GetOut.wav"), SoundType.Voice, "f5de5b", subtitleKey: "BBE_Kulak_GetOut");
            if (smash == null)
                smash = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Kulak", "BBE_Kulak_Smash.wav"), SoundType.Voice, "f5de5b", subtitleKey: "BBE_Kulak_Smash");
            if (hiya == null)
                hiya = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Kulak", "BBE_Kulak_Hiya.wav"), SoundType.Voice, "f5de5b", subtitleKey: "BBE_Kulak_Hiya");
            if (animator == null) animator = gameObject.AddComponent<CustomSpriteAnimator>();
            spriteRenderer[0].sprite = sprites[0];
            animator.spriteRenderer = this.spriteRenderer[0];
            audMan = this.GetComponent<AudioManager>();
            audMan.overrideSubtitleColor = false;
            Navigator.passableObstacles.Add(PassableObstacle.Window);
            Navigator.passableObstacles.Add(PassableObstacle.Bully);
            Navigator.passableObstacles.Add(PassableObstacle.LockedDoor);
            charactersToIgnore.Add(Character.Principal);
            charactersToIgnore.Add(Character.Chalkles);
            charactersToIgnore.Add(Character.Bully);
        }
        [SerializeField]
        private Sprite[] sprites;
        public List<Character> charactersToIgnore = new List<Character>() { };
        public List<Window> toIgnore = new List<Window>() { };
        public AudioManager audMan;
        public CustomSpriteAnimator animator;
        public SoundObject getOut;
        public SoundObject smash;
        public SoundObject hiya;
        public override void Initialize()
        {
            base.Initialize();
            if (!animator.animations.ContainsKey("Angry"))
                animator.animations.Add("Angry", new CustomAnimation<Sprite>(6, sprites));
            if (!animator.animations.ContainsKey("Idle"))
                animator.animations.Add("Idle", new CustomAnimation<Sprite>(6, sprites.Reverse().ToArray()));
            behaviorStateMachine.ChangeState(new Kulak_Wandering(this));
        }
        public void SetSprite(string name)
        {
            animator.Play(name, 1f);
        }
        public void Angry()
        {
            behaviorStateMachine.ChangeState(new Kulak_Angry(this));
        }
        public void WanderNormal()
        {
            behaviorStateMachine.ChangeState(new Kulak_Wandering(this));
        }
        public void Push(Entity entity)
        {
            SetGuilt(10f, "Bullying");
            entity.AddForce(new Force(transform.forward, 150, -50f));
        }
    }
    public class Kulak_StateBase : NpcState
    {
        public Kulak_StateBase(Kulak kulakk) : base(kulakk)
        {
            kulak = kulakk;
        }
        protected Kulak kulak;
    }
    public class Kulak_Wandering : Kulak_StateBase
    {
        private float timeLeft = 30f;
        public Kulak_Wandering(Kulak kulakk) : base(kulakk)
        {
            timeLeft = 30f;
        }
        public override void Enter()
        {
            base.Enter();
            kulak.SetSprite("Idle");
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
                if (!other.GetComponent<Window>().broken && !kulak.toIgnore.Contains(other.GetComponent<Window>()))
                {
                    other.GetComponent<Window>().Break(false);
                    kulak.audMan.PlaySingle(new List<SoundObject>() { kulak.smash, kulak.hiya }.ChooseRandom());
                }
                if (!other.GetComponent<Window>().broken) kulak.toIgnore.Add(other.GetComponent<Window>());
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
        public Kulak_Angry(Kulak kulakk) : base(kulakk)
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
                if (!other.GetComponent<Window>().broken && !kulak.toIgnore.Contains(other.GetComponent<Window>()))
                {
                    other.GetComponent<Window>().Break(false);
                    kulak.audMan.PlaySingle(new List<SoundObject>() { kulak.smash, kulak.hiya }.ChooseRandom());
                }
                if (!other.GetComponent<Window>().broken) kulak.toIgnore.Add(other.GetComponent<Window>());
            }
            if (other.CompareTag("Player"))
            {
                kulak.Push(other.GetComponent<PlayerManager>().plm.entity);
                kulak.audMan.PlaySingle(kulak.getOut);
                kulak.WanderNormal();
            }
            if (other.CompareTag("NPC"))
            {
                if (!other.GetComponent<NPC>().GetMeta().tags.Contains("BBE_KulakIgnoreCharacter"))
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
