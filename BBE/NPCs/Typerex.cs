using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BBE.Helpers;
using MTM101BaldAPI.Components;

namespace BBE.NPCs
{
    public class KeyboardGame : MonoBehaviour
    {
        private MovementModifier moveMod = new MovementModifier(default(Vector3), 0f);
        public PlayerManager player;
        public Typerex typerex;
        public EnvironmentController ec;
        public float time = 0;
        private List<string> words = new List<string>() { "Rost", "BAA", "Typerex", "Baldi", "Principal", "Keyboard", "Kostya", "Extra", "WestieNZ", "Jorietta", "Graysland" };
        private string word;
        private string playerAnswer = "";
        private TMP_Text text;
        private TMP_Text wordTMP;
        private GameObject canvas;

        private void Start()
        {
            canvas = CreateObjects.CreateCanvas("Typerex_Canvas", color: new Color(0, 0, 0, 0));
            player.plm.am.moveMods.Add(moveMod);
            word = words.ChooseRandom();
            time = word.Length * 1.5f;
            text = CreateObjects.CreateText("Typerex_Text", Singleton<LocalizationManager>.Instance.GetLocalizedText("Typerex_EnterWordOnKeyboard"),
                true, new Vector3(-3.9f, 1, 1), new Vector3(0.222f, 0.222f, 0.222f), canvas.transform, 24f);
            text.isOrthographic = false;
            wordTMP = CreateObjects.CreateText("Typerex_Text_Word", word, true, new Vector3(-1.5f, 0, 1), new Vector3(0.222f, 0.222f, 0.222f), canvas.transform, 24f);
            wordTMP.isOrthographic = false;
            StartCoroutine(CheckAnswer());
        }
        private void Update()
        {
            string coloredPart = $"<color=#03FC0BFF>{word.Substring(0, playerAnswer.Length)}</color>";
            string remainingPart = word.Substring(playerAnswer.Length);
            string blackPart = $"<color=#000000FF>{remainingPart}</color>";
            wordTMP.text = coloredPart + blackPart;
            time -= Time.deltaTime;
            if (!Input.anyKeyDown)
            {
                return;
            }
            if (Input.inputString.Length > 0)
            {
                char inputChar = Input.inputString[0];
                if (char.IsLetter(inputChar))
                {
                    string symbol = inputChar.ToString();
                    try
                    {
                        if (string.Equals(symbol, word[playerAnswer.Length].ToString(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            playerAnswer += symbol;
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                }
            }
        }
        private IEnumerator CheckAnswer()
        {
            while (playerAnswer.ToLower() != word.ToLower()) 
            {
                if (time <= 0f)
                {
                    End(false);
                    break;
                }
                yield return null;
            }
            End(true);
        }
        private void DestroyGame()
        {
            Destroy(canvas);    
            Destroy(wordTMP);
            Destroy(wordTMP.gameObject);
            Destroy(text);
            Destroy(text.gameObject);
        }
        private void End(bool done)
        {
            if (done)
            {
                Singleton<CoreGameManager>.Instance.AddPoints(word.Length * 10, player.playerNumber, playAnimation: true);
            }
            else
            {
                ec.MakeNoise(typerex.transform.position, 77);
            }
            player.plm.am.moveMods.Remove(moveMod);
            DestroyGame();
            typerex.EndGame();
        }
    }

    public class Typerex : NPC
    {
        private MovementModifier moveModForNPC = new MovementModifier(default(Vector3), 0.5f);
        public KeyboardGame game;
        public CustomSpriteAnimator animator;
        public override void Initialize()
        {
            CreateSprites();
            base.Initialize();
            behaviorStateMachine.ChangeState(new Typerex_Wandering(this, this));
        }
        public void CreateSprites()
        {
            animator = gameObject.AddComponent<CustomSpriteAnimator>();
            animator.spriteRenderer = this.spriteRenderer[0];
            animator.animations.Add("Idle", new CustomAnimation<Sprite>(60, new Sprite[] { AssetsHelper.SpriteFromFile("Textures/NPCs/Typerex/Base.png", 50f) }));
        }
        public void SetDefaultSprite()
        {
            animator.Play("Idle", 1f);
        }
        public void StartGame(PlayerManager player)
        {
            if (!game.IsNull())
            {
                Destroy(game);
            }
            if (player.jumpropes.Count > 0)
            {
                Playtime playtime = FindObjectOfType<Playtime>();
                playtime.EndJumprope(false);
            }
            foreach (NPC npc in ec.Npcs)
            {
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier))
                {
                    activityModifier.moveMods.Add(moveModForNPC);
                }
            }
            game = gameObject.AddComponent<KeyboardGame>();
            game.player = player;
            game.ec = ec;
            game.typerex = this;
            navigator.maxSpeed = 0f;
            navigator.SetSpeed(0f);
        }
        public void EndGame()
        {
            foreach (NPC npc in ec.Npcs)
            {
                ActivityModifier activityModifier;
                if (npc.TryGetComponent<ActivityModifier>(out activityModifier) && activityModifier.moveMods.Contains(moveModForNPC))
                {
                    activityModifier.moveMods.Remove(moveModForNPC);
                }
            }
            navigator.maxSpeed = 12f;
            navigator.SetSpeed(12f);
            behaviorStateMachine.ChangeState(new Typerex_Cooldown(this, this));
            Destroy(game);
        }
        public void EndCooldown()
        {
            behaviorStateMachine.ChangeState(new Typerex_Wandering(this, this));
        }
        public void StartPersuingPlayer(PlayerManager player)
        {
            behaviorStateMachine.ChangeNavigationState(new NavigationState_TargetPlayer(this, 63, player.transform.position));
        }
        public void PersuePlayer(PlayerManager player)
        {
            behaviorStateMachine.CurrentNavigationState.UpdatePosition(player.transform.position);
        }
    }

    public class Typerex_StateBase : NpcState
    {
        public Typerex_StateBase(NPC npc, Typerex typerexx) : base(npc)
        {
            typerex = typerexx;
        }

        protected Typerex typerex;
    }
    public class Typerex_Cooldown : Typerex_StateBase
    {
        private float time = 15f;
        public Typerex_Cooldown(NPC npc, Typerex typerex)
        : base(npc, typerex)
        {
            time = 15f;
        }
        public override void Enter()
        {
            base.Enter();
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            typerex.SetDefaultSprite();
        }
        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * npc.TimeScale;
            if (time <= 0)
            {
                typerex.EndCooldown();
            }
        }
    }
    public class Typerex_Wandering : Typerex_StateBase
    {
        public Typerex_Wandering(NPC npc, Typerex typerex)
        : base(npc, typerex)
        {
        }
        public override void Enter()
        {
            base.Enter();
            npc.looker.Blink();
            if (!npc.Navigator.HasDestination)
            {
                ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
            }
            typerex.SetDefaultSprite();
        }
        public override void OnStateTriggerEnter(Collider other)
        {
            base.OnStateTriggerEnter(other);
            if (other.CompareTag("Player"))
            {
                PlayerManager component = other.GetComponent<PlayerManager>();
                if (!component.Tagged)
                {
                    typerex.StartGame(component);
                }
            }
        }
        public override void PlayerSighted(PlayerManager player)
        {
            base.PlayerSighted(player);
            if (!player.Tagged)
            {
                typerex.StartPersuingPlayer(player);
            }
        }

        public override void PlayerInSight(PlayerManager player)
        {
            base.PlayerInSight(player);
            if (!player.Tagged)
            {
                typerex.PersuePlayer(player);
            }
        }
    }
}
