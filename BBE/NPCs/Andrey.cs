using BBE.Extensions;
using BBE.Creators;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BBE.Helpers;

namespace BBE.NPCs
{
    class Andrey : NPC, INPCPrefab 
    {
        void IPrefab.SetupAssets()
        {
            audMan = this.GetComponent<AudioManager>();
            BasePlugin.Asset.Add<Sprite>("AndreyAngry", AssetsHelper.CreateTexture("Textures", "NPCs", "Andrey", "BBE_AndreyAngry.png").ToSprite(40));

            BasePlugin.Asset.Add<Sprite>("AndreyTalkiing", AssetsHelper.CreateTexture("Textures", "NPCs", "Andrey", "BBE_AndreyTalking.png").ToSprite(40));

            BasePlugin.Asset.Add("AndreyBaseSprite", AssetsHelper.CreateTexture("Textures", "NPCs", "Andrey", "BBE_AndreyHappy.png").ToSprite(40));

            BasePlugin.Asset.Add<Sprite>("AndreyCry", AssetsHelper.CreateTexture("Textures", "NPCs", "Andrey", "BBE_AndreyCry.png").ToSprite(40));
            spriteRenderer[0].sprite = BasePlugin.Asset.AddAndReturn<Sprite>("AndreyBaseSprite", AssetsHelper.CreateTexture("Textures", "NPCs", "Andrey", "BBE_AndreyHappy.png").ToSprite(40));
            angrySounds = new List<SoundObject>() { };
            for (int i = 1; i < 5; i++)
            {
                angrySounds.Add(AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Andrey", $"BBE_Andrey_Angry{i}.wav"), SoundType.Voice, "612006", subtitleKey: "BBE_AndreyAngry" + i));
            }
            
            relaxSounds.Add(AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Andrey", "BBE_Andrey_Relax1.wav"), SoundType.Voice, "612006", subtitleKey: "BBE_AndreyRelax1"));
            studentMistreatingBaldi = AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Andrey", "BBE_Andrey_CallBaldi.wav"), SoundType.Voice, "612006", subtitleKey: "BBE_AndreyCallsBaldi");
        }
        /*
        private static SoundObject RandomSound
        {
            get
            {
                List<float> samples = new List<float>();
                for (int i = 0; i < UnityEngine.Random.Range(10000, 200000); i++)
                {
                    samples.Add(UnityEngine.Random.Range(-1f, 1f));
                }
                AudioClip clip = AudioClip.Create(
                    "RandomSound",
                    samples.Count,
                    1,
                    22050,
                    false
                );
                clip.SetData(samples.ToArray(), 0);
                return AssetsHelper.CreateSoundObject(clip, SoundType.Voice);
            }
        }*/
        public float normalSpeed = 12f;
        public float angrySpeed = 18f;
        [SerializeField]
        public List<SoundObject> angrySounds = new List<SoundObject>() { };
        [SerializeField]
        public List<SoundObject> relaxSounds = new List<SoundObject>() { };
        private SoundObject WhistleSound 
        {
            get
            {
                return AssetsHelper.LoadAsset<ITM_PrincipalWhistle>(x => x.audWhistle != null).audWhistle;
            }
        }
        private int MaxAnnoyCount
        {
            get
            {
                if (FunSettingsType.HardModePlus.IsActive()) return 5;
                return 3;
            }
        }
        private int annoyCount = 0;
        [SerializeField]
        private SoundObject studentMistreatingBaldi;
        private AudioManager audMan;
        public bool isClone = false;
        public string currentSprite;
        private Sprite angry;
        private Sprite talking;
        private Sprite happy;
        private Sprite cry;

        public override void Initialize()
        {
            base.Initialize();
            Color color = AssetsHelper.GenerateRandomColor(false);
            Color color2 = AssetsHelper.GenerateRandomColor(false);
            Color color3 = AssetsHelper.GenerateRandomColor(false);
            angry = BasePlugin.Asset.Get<Sprite>("AndreyAngry").ReplaceColor(Color.white, color).ReplaceColor(AssetsHelper.ColorFromHex("3030ff"), color2)
                .ReplaceColor(AssetsHelper.ColorFromHex("808080"), color3);

            talking = BasePlugin.Asset.Get<Sprite>("AndreyTalkiing").ReplaceColor(Color.white, color).
                ReplaceColor(AssetsHelper.ColorFromHex("3030ff"), color2).ReplaceColor(AssetsHelper.ColorFromHex("808080"), color3);

            happy = BasePlugin.Asset.Get<Sprite>("AndreyBaseSprite").ReplaceColor(Color.white, color).
                ReplaceColor(AssetsHelper.ColorFromHex("3030ff"), color2).ReplaceColor(AssetsHelper.ColorFromHex("808080"), color3);

            cry = BasePlugin.Asset.Get<Sprite>("AndreyCry").ReplaceColor(Color.white, color).
                ReplaceColor(AssetsHelper.ColorFromHex("3030ff"), color2).ReplaceColor(AssetsHelper.ColorFromHex("808080"), color3);
            behaviorStateMachine.ChangeState(new AndreyAnnoying(this));
        }
        public override void SentToDetention()
        {
            base.SentToDetention();
            SetSprite("cry");
        }
        public void AnnoyPlayer(PlayerManager player)
        {
            if (annoyCount >= MaxAnnoyCount)
            {
                StartCooldown();
                return;
            }
            annoyCount++;
            switch (UnityEngine.Random.Range(1, 5))
            {
                case 1:
                    CoreGameManager.Instance.AddPoints(-100, 0, true);
                    break;
                case 2:
                    player.plm.stamina -= 20f;
                    break;
                case 3:
                    ec.MakeNoise(transform.position, 112);
                    Talk(studentMistreatingBaldi);
                    break;
                case 4:
                    foreach (NPC npc in ec.npcs)
                    {
                        if (npc is Principal principal)
                        {
                            principal.WhistleReact(transform.position);
                        }
                    }
                    string old = WhistleSound.soundKey;
                    WhistleSound.soundKey = "*Whistling*";
                    audMan.PlaySingle(WhistleSound);
                    WhistleSound.soundKey = old;
                    break;
            }
        }
        private System.Collections.IEnumerator TalkingTimer(float time)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            SetSprite("happy");
            if (behaviorStateMachine.currentState is AndreyAngry)
                SetSprite("angry");
            yield break;
        }
        public void Talk(SoundObject sound)
        {
            if (audMan == null)
                audMan = GetComponent<AudioManager>();
            audMan.PlaySingle(sound);
            SetSprite("talking");
            if (behaviorStateMachine.currentState is AndreyAngry)
                SetSprite("angry");
            StartCoroutine(TalkingTimer(sound.soundClip.length + 1));
        }
        public void SetSprite(string name)
        {
            currentSprite = name;
            switch (name.ToLower()) {
                case "angry":
                    spriteRenderer[0].sprite = angry;
                    break;
                case "happy":
                    spriteRenderer[0].sprite = happy;
                    break;
                case "cry":
                    spriteRenderer[0].sprite = cry;
                    break;
                default:
                    spriteRenderer[0].sprite = talking;
                    break;
            }
        }
        public void StartPersuingPlayer(PlayerManager player)
        {
            behaviorStateMachine.ChangeNavigationState(new NavigationState_TargetPlayer(this, 63, player.transform.position));
        }
        public void PersuePlayer(PlayerManager player)
        {
            behaviorStateMachine.CurrentNavigationState.UpdatePosition(player.transform.position);
        }
        public void StartCooldown()
        {
            behaviorStateMachine.ChangeState(new AndreyCooldown(this));
            annoyCount = 0;
        }
        public void TurnCamera(PlayerManager player, float speed) => StartCoroutine(TurnPlayer(player, speed));
        private IEnumerator TurnPlayer(PlayerManager player, float speed)
        {
            float time = 0.5f;
            while (time > 0f)
            {
                Vector3 vector = Vector3.RotateTowards(player.transform.forward, (transform.position - player.transform.position).normalized, Time.deltaTime * 2f * (float)Math.PI * speed, 0f);
                player.transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
                time -= Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
    class AndreyStateBase : NpcState
    {
        protected Andrey andrey;
        public AndreyStateBase(Andrey npc) : base(npc)
        {
            this.andrey = npc;
        }
    }
    class AndreyWandering : AndreyStateBase
    {
        public AndreyWandering(Andrey npc) : base(npc)
        {
        }
        public override void Enter()
        {
            base.Enter();
            if (!npc.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            }
            andrey.SetSprite("happy");
            andrey.Navigator.SetSpeed(andrey.normalSpeed, andrey.normalSpeed);
        }
    }
    class AndreyAngry : AndreyWandering
    {
        private PlayerManager target;
        private float timer;
        public AndreyAngry(Andrey npc, PlayerManager player) : base(npc)
        {
            timer = 10f;
            target = player;
        }
        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                andrey.Talk(andrey.angrySounds.ChooseRandom());
                timer = 10f;
            }
        }
        public override void Enter()
        {
            base.Enter();
            andrey.SetSprite("angry");
            andrey.Navigator.SetSpeed(andrey.angrySpeed, andrey.angrySpeed);
        }
        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            if (player == target)
            {
                andrey.PersuePlayer(player);
            }
        }
        public override void PlayerSighted(PlayerManager player)
        {
            base.PlayerSighted(player);
            if (target == player)
            {
                andrey.StartPersuingPlayer(player);
            }
        }
        public override void OnStateTriggerEnter(Collider other)
        {
            base.OnStateTriggerStay(other);
            if (other.CompareTag("Player") && other.GetComponent<PlayerManager>() == target)
            {
                andrey.AnnoyPlayer(other.GetComponent<PlayerManager>());
            }
        }
    }
    class AndreyAnnoying : AndreyWandering
    {
        private PlayerManager target;
        public AndreyAnnoying(Andrey npc) : base(npc)
        {
            target = null;
        }
        public override void PlayerSighted(PlayerManager player)
        {
            base.PlayerSighted(player);
            if (!player.Tagged)
            {
                andrey.StartPersuingPlayer(player);
            }
        }
        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            if (!player.Tagged)
            {
                andrey.PersuePlayer(player);
            }
        }
        public override void PlayerLost(PlayerManager player)
        {
            base.PlayerLost(player);
            if (target != null)
            {
                if (player == target)
                {
                    andrey.behaviorStateMachine.ChangeState(new AndreyAngry(andrey, player));
                    for (int i = 0; i<UnityEngine.Random.Range(3, 8); i++)
                    {
                       /* Andrey a = andrey.SpawnClone();
                        a.behaviorStateMachine.ChangeState(new AndreyAngry(a, player));*/
                    }
                }
            }
        }
        public override void OnStateTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerManager player = other.GetComponent<PlayerManager>();
                if (target == null)
                {
                    target = player;
                }
                if (target == player)
                {
                    andrey.TurnCamera(player, 1);
                }
            }
            base.OnStateTriggerEnter(other);
        }
    }
    class AndreyCooldown : AndreyWandering
    {
        private float time;
        private float timer;
        public AndreyCooldown(Andrey npc) : base(npc)
        {
            timer = 15f;
            time = 120f;
        }
        public override void Enter()
        {
            base.Enter();
            // Bruh, why base AndreyWandering does not work
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            andrey.SetSprite("happy");
            andrey.Navigator.SetSpeed(andrey.normalSpeed, andrey.normalSpeed);
        }
        public override void Update()
        {
            base.Update();
            timer -= Time.deltaTime;
            time -= Time.deltaTime * andrey.ec.NpcTimeScale;
            if (time <= 0)
            {
                andrey.behaviorStateMachine.ChangeState(new AndreyAnnoying(andrey));
            }
            if (timer <= 0)
            {
                andrey.Talk(andrey.relaxSounds.ChooseRandom());
                timer = 15f;
            }
        }   
    }
}
