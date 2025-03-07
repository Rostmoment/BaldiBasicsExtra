using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BBE.Creators;
using BBE.CustomClasses;
using System.Linq;
using HarmonyLib;
using BBE.Extensions;
using BBE.Helpers;

namespace BBE.NPCs
{
    public class KeyboardGame : MonoBehaviour
    {
        private MovementModifier moveMod = new MovementModifier(default(Vector3), 0f);
        public PlayerManager player;
        public Typerex typerex;
        public EnvironmentController ec;
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
            //text = CreateObjects.CreateText("Typerex_Text", "Typerex_EnterWordOnKeyboard".Localize(),
             //   true, new Vector3(-3.9f, 1, 1), Vector3.one, canvas.transform, 24f);
            text.isOrthographic = false;
            //wordTMP = CreateObjects.CreateText("Typerex_Text_Word", word, true, new Vector3(-1.5f, 0, 1), Vector3.one, canvas.transform, 24f);
            wordTMP.isOrthographic = false;
            StartCoroutine(CheckAnswer());
        }
        private void Update()
        {
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
                char inputChar = Input.inputString[0];
                if (char.IsLetter(inputChar) && typerex.possibleSymbols.Contains(inputChar.ToString().ToUpper()))
                {
                    try
                    {
                        if (inputChar == word.Substring(0, playerAnswer.Length)[0])
                        {
                            playerAnswer += inputChar;
                        }
                        else
                        {
                            End(false);
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
            End(true);
        }
        void OnDestroy()
        {
            Destroy(canvas);    
            Destroy(wordTMP);
            Destroy(wordTMP.gameObject);
            Destroy(text);
            Destroy(text.gameObject);
        }
        private void End(bool done)
        {
            StopAllCoroutines();
            if (done) CoreGameManager.Instance.AddPoints(word.Length * 10, player.playerNumber, true);
            else ec.MakeNoise(typerex.transform.position, 77);
            player.plm.am.moveMods.Remove(moveMod);
            typerex.EndGame(done);
        }
    }
    public class Typerex : NPC
    {
        private Dictionary<string ,Sprite> sprites = new Dictionary<string ,Sprite>();
        private float pushSpeed = 5f;
        private float pushAcceleration = -5f;
        public string possibleSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public KeyboardGame KeyboardGame;
        private int cellCount = 3;
        private List<Vector3> cells = new List<Vector3>();
        public override void Initialize()
        {
            base.Initialize();
            Setup();
            Navigator.maxSpeed = 0; // NO SPEED
            if (FunSettingsType.HardModePlus.IsActive()) cellCount = 5;
            cells = ec.AllTilesNoGarbage(false, false).Where(x => x.room.type == RoomType.Hall).ChooseRandom(cellCount).Select(x => x.CenterWorldPosition).ToList();
            behaviorStateMachine.ChangeState(new Typerex_Cooldown(this, this));
        }
        private void Setup()
        {
            sprites.Add("Base", AssetsHelper.CreateTexture("Textures", "NPCs", "Typerex", "BBE_TyperexBase.png").ToSprite(65));
            sprites.Add("Angry", AssetsHelper.CreateTexture("Textures", "NPCs", "Typerex", "BBE_TyperexAngry.png").ToSprite(65));
            sprites.Add("Close", AssetsHelper.CreateTexture("Textures", "NPCs", "Typerex", "BBE_TyperexClose.png").ToSprite(65));
            sprites.Add("Happy", AssetsHelper.CreateTexture("Textures", "NPCs", "Typerex", "BBE_TyperexHappy.png").ToSprite(65));
        }
        public void SetSprite(string key) => spriteRenderer[0].sprite = sprites[key];
        public void EndGame(bool done)
        {
            Destroy(KeyboardGame);
            KeyboardGame = null;
            if (done)
            {
                SetSprite("Happy");
                StartCooldown();
            }
            else
            {
                SetSprite("Angry");
                Angry();
            }
        }
        public void StartGame(PlayerManager player)
        {
            KeyboardGame = gameObject.AddComponent<KeyboardGame>();
            KeyboardGame.player = player;
            KeyboardGame.ec = ec;
            KeyboardGame.typerex = this;
        }
        public void Push(Entity entity)
        {
            entity.AddForce(new Force((((Component)entity).transform.position - base.transform.position).normalized, pushSpeed, pushAcceleration));
        }
        public void Angry()
        {
            this.behaviorStateMachine.ChangeState(new Typerex_Angry(this, this));
        }
        public void EndCooldown()
        {
            SetSprite("Base");
            StartCoroutine(Timer(true));
        }
        public void StartCooldown()
        {
            SetSprite("Close");
            StartCoroutine(Timer(false));
        }
        private IEnumerator Timer(bool state)
        {
            if (state)
            {
                Vector3 pos = cells.ChooseRandom();
                this.transform.position = new Vector3(pos.x, -5, pos.z);
                while (this.transform.position.y < 5)
                {
                    this.transform.position += new Vector3(0, 0.1f, 0);
                    yield return null;
                }
                if (this.transform.position.y > 5) this.transform.position = new Vector3(this.transform.position.x, 5, this.transform.position.z);
                this.behaviorStateMachine.ChangeState(new Typerex_Normal(this, this));
                yield break;
            }
            else
            {
                while (this.transform.position.y > -5)
                {
                    this.transform.position -= new Vector3(0, 0.1f, 0);
                    yield return null;
                }
                if (this.transform.position.y < -5) this.transform.position = new Vector3(this.transform.position.x, -5, this.transform.position.z);
                this.behaviorStateMachine.ChangeState(new Typerex_Cooldown(this, this));
                yield break;
            }
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
    public class Typerex_Normal : Typerex_StateBase
    {
        public Typerex_Normal(NPC npc, Typerex typerexx) : base(npc, typerexx)
        {
        }
        public override void OnStateTriggerEnter(Collider other)
        {
            base.OnStateTriggerStay(other);
            if (other.CompareTag("Player")) typerex.StartGame(other.GetComponent<PlayerManager>());
        }
    }
    public class Typerex_Angry : Typerex_StateBase
    {
        private float timeLeft;
        public Typerex_Angry(NPC npc, Typerex typerexx) : base(npc, typerexx)
        {
            timeLeft = UnityEngine.Random.Range(40f, 50f);
        }
        public override void Update()
        {
            base.Update();
            timeLeft -= Time.deltaTime * typerex.ec.NpcTimeScale;
            if (timeLeft < 0) typerex.StartCooldown();
        }
        public override void OnStateTriggerStay(Collider other)
        {
            base.OnStateTriggerStay(other);
            if (other.CompareTag("Player")) typerex.Push(other.GetComponent<PlayerManager>().plm.Entity);
        }

    }
    public class Typerex_Cooldown : Typerex_StateBase
    {
        private float timer;
        public Typerex_Cooldown(NPC npc, Typerex typerexx) : base(npc, typerexx)
        {
            timer = 30f;
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
            timer -= Time.deltaTime * typerex.ec.NpcTimeScale;
            if (timer <= 0) typerex.EndCooldown();
        }
    }
}
