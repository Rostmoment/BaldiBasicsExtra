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
using HarmonyLib;

namespace BBE.NPCs
{
    public class KeyboardGame : MonoBehaviour
    {
        private MovementModifier moveMod = new MovementModifier(default(Vector3), 0f);
        public PlayerManager player;
        public Typerex typerex;
        private List<string> words = new List<string>() { "Rost", "Typerex", "Baldi", "Principal", "Keyboard", "Kostya", "Extra", "WestieNZ" };
        private string word;
        private string playerAnswer = "";
        private TMP_Text text;
        private TMP_Text wordTMP; 
        private void Start()
        {
            player.plm.am.moveMods.Add(moveMod);
            word = words.ChooseRandom();
            text = Instantiate(PrivateDataHelper.GetVariable<TMP_Text[]>(Singleton<CoreGameManager>.Instance.GetHud(0), "textBox")[0]);
            text.gameObject.transform.position = new Vector3(-3.9f, 1, 1);
            text.gameObject.transform.localScale = new Vector3(0.222f, 0.222f, 0.222f);
            text.isOrthographic = false;
            text.gameObject.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).transform);
            text.gameObject.name = "Typerex_Text";
            text.text = Singleton<LocalizationManager>.Instance.GetLocalizedText("Typerex_EnterWordOnKeyboard");
            wordTMP = Instantiate(PrivateDataHelper.GetVariable<TMP_Text[]>(Singleton<CoreGameManager>.Instance.GetHud(0), "textBox")[0]);
            wordTMP.gameObject.transform.position = new Vector3(-1.5f, 0, 1);
            wordTMP.gameObject.transform.localScale = new Vector3(0.222f, 0.222f, 0.222f);
            wordTMP.isOrthographic = false;
            wordTMP.gameObject.transform.SetParent(Singleton<CoreGameManager>.Instance.GetHud(0).transform);
            wordTMP.gameObject.name = "Typerex_Text_Word";
            wordTMP.text = word;
            StartCoroutine(CheckAnswer());
        }
        private void Update()
        {
            if (player.jumpropes.Count > 0)
            {
                Playtime playtime = FindObjectOfType<Playtime>();
                playtime.EndJumprope(false);
            }
            string coloredPart = $"<color=#03FC0BFF>{word.Substring(0, playerAnswer.Length)}</color>";
            string remainingPart = word.Substring(playerAnswer.Length);
            string blackPart = $"<color=#000000FF>{remainingPart}</color>";
            wordTMP.text = coloredPart + blackPart;
            if (!Input.anyKeyDown)
            {
                return;
            }
            if (Input.inputString.Length > 0)
            {
                if (char.IsLetter(Input.inputString[0]))
                {
                    string symbol = Input.inputString[0].ToString();
                    try
                    {
                        if (symbol.ToLower() == word[playerAnswer.Length].ToString().ToLower())
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
                yield return null;
            }
            End();
        }
        private void DestroyGame()
        {
            Destroy(wordTMP);
            Destroy(wordTMP.gameObject);
            Destroy(text);
            Destroy(text.gameObject);
        }
        private void End()
        {
            Singleton<CoreGameManager>.Instance.AddPoints(word.Length * 25, player.playerNumber, playAnimation: true);
            player.plm.am.moveMods.Remove(moveMod);
            DestroyGame();
            typerex.EndGame();
        }
    }

    public class Typerex : NPC
    {
        private MovementModifier moveModForNPC = new MovementModifier(default(Vector3), 0.5f);
        public KeyboardGame game;
        public override void Initialize()
        {
            base.Initialize();
            behaviorStateMachine.ChangeState(new Typerex_Wandering(this, this));
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
        private float time = 15;

        public Typerex_Cooldown(NPC npc, Typerex typerex)
        : base(npc, typerex)
        {
        }
        public override void Enter()
        {
            base.Enter();
            time = 15;
            ChangeNavigationState(new NavigationState_WanderRandom(npc, 0));
        }
        public override void Update()
        {
            base.Update();
            time -= Time.deltaTime * npc.TimeScale;
            if (time <= 0f)
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
