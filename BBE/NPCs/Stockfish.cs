using BBE.Extensions;
using BBE.Creators;
using BBE.NPCs.Chess;
using MTM101BaldAPI;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BBE.Helpers;
using System.IO;
using System.Collections;
using BBE.Events;
using UnityEngine.UI;
using Rewired;

namespace BBE.NPCs
{
    public class Stockfish : NPC, INPCPrefab
    {
        private PlayerManager playerManager;
        private bool disableWalkingAnim = false;
        private MovementModifier moveMod;
        private GameObject canvas;
        private Image Image => canvas.GetComponentInChildren<Image>();
        private int animationIndex = 0;
        private float animationDelay = 0.5f;
        private float animationDelayDefault = 0.5f;
        private ChessPuzzle puzzle;
        [SerializeField]
        private ChessBoard chessBoard;
        [SerializeField]
        private static List<ChessPuzzle> potentialPuzzles;
        [SerializeField]
        private Sprite[] walking, hit;
        [SerializeField]
        private SoundObject[] steps;
        [SerializeField]
        private AudioManager audMan;
        public void SetupAssets()
        {
            audMan = GetComponent<AudioManager>();
            walking = AssetsHelper.CreateSpriteSheet(4, 1, 45, "Textures", "NPCs", "Stockfish", "BBE_StockfishWalking.png");
            hit = AssetsHelper.CreateSpriteSheet(3, 1, 55, "Textures", "NPCs", "Stockfish", "BBE_StockfishPunishment.png");
            spriteRenderer[0].sprite = walking[1];
            steps = new SoundObject[]
            {
                AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Stockfish", "BBE_StockfishStep1.ogg"), SoundType.Effect, ""),
                AssetsHelper.CreateSoundObject(AssetsHelper.AudioFromFile("Audio", "NPCs", "Stockfish", "BBE_StockfishStep2.ogg"), SoundType.Effect, ""),
            };
            CreateChessPuzzles();
        }
        private IEnumerator BlinkingCanvas(float time, PlayerManager pm)
        {
            if (canvas != null)
                Destroy(canvas);
            if (moveMod != null)
                pm.Am.moveMods.Remove(moveMod);
            moveMod = new MovementModifier(default, 0.8f);
            pm.Am.moveMods.Add(moveMod);
            float left = time;
            int modifer = 1;
            float wait = float.NaN;
            canvas = CreateObjects.CreateCanvas(color: Color.black);
            while (left > 0) 
            {
                if (CoreGameManager.Instance.Paused)
                    yield return null;
                if (!float.IsNaN(wait))
                {
                    if (wait > 0)
                        wait -= Time.deltaTime;
                    else
                        wait = float.NaN;
                    yield return null;
                }
                Color color = Image.color;
                float a = color.a + modifer * 0.01f;
                if (a > 1)
                {
                    a = 1;
                    modifer *= -1;
                }
                if (a < 0)
                {
                    a = 0;
                    modifer *= -1;
                }
                if (a == 0 || a == 1)
                {
                    wait = 4;
                    yield return null;
                }
                color.a = a;
                Image.color = color;
                left -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }
            pm.Am.moveMods.Remove(moveMod);
            moveMod = null;
            Destroy(canvas);
            yield break;
        }
        private IEnumerator TurnPlayer(PlayerManager player, float time)
        {
            float left = time;
            while (left > 0f)
            {
                if (CoreGameManager.Instance.Paused)
                    yield return null;
                Vector3 vector = player.transform.rotation.eulerAngles + new Vector3(0, 0.5f, 0);
                if (vector.y > 360)
                    vector.y = 0;
                player.transform.rotation = Quaternion.Euler(vector);
                left -= Time.deltaTime * ec.EnvironmentTimeScale;
                yield return null;
            }
            yield break;
        }
        public void CreateChessPuzzles()
        {
            // 2, 1, 0
            potentialPuzzles = new List<ChessPuzzle>();
            foreach (string file in Directory.GetFiles(AssetsHelper.ModPath + Constants.CHESS_PUZZLES_PATH))
            {
                potentialPuzzles.Add(ChessPuzzle.FromJson(file));
            }
        }
        public override void Despawn()
        {
            base.Despawn();
            if (canvas != null)
                Destroy(canvas);
            canvas = null;
            chessBoard?.End(false);
        }
        public override void Initialize()
        {
            base.Initialize();
            behaviorStateMachine.ChangeState(new StockfishWandering(this));
        }
        public override void VirtualUpdate()
        {
            base.VirtualUpdate();
            if (!disableWalkingAnim) WalkingAnimation();
            else HitAnim();
        }
        public void SetDefaultSprite()
        {
            spriteRenderer[0].sprite = walking[2];
        }
        public void HitAnim()
        {
            animationDelay -= Time.deltaTime * ec.NpcTimeScale;
            if (animationDelay < 0)
            {
                if (animationIndex >= hit.Length)
                {
                    disableWalkingAnim = false;
                    StartCoroutine(BlinkingCanvas(15, playerManager));
                    StartCoroutine(TurnPlayer(playerManager, 15));
                    audMan.PlaySingle("Bang");
                    playerManager = null;
                    return;
                }
                spriteRenderer[0].sprite = hit[animationIndex];
                animationIndex++;
                animationDelay = animationDelayDefault;
            }
        }
        public void WalkingAnimation() 
        {
            animationDelay -= Time.deltaTime * ec.NpcTimeScale;
            if (animationDelay < 0)
            {
                if (audMan == null)
                    audMan = GetComponent<AudioManager>();
                SoundObject soundObject = steps.ChooseRandom();
                audMan.PlaySingle(soundObject);
                spriteRenderer[0].sprite = walking[animationIndex];
                animationIndex++;
                if (animationIndex >= walking.Length)
                    animationIndex = 0;
                animationDelay = animationDelayDefault;
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
        public void StartChessMinigame(PlayerManager pm)
        {
            if (potentialPuzzles.EmptyOrNull())
                CreateChessPuzzles();
            if (!potentialPuzzles.EmptyOrNull())
                puzzle = potentialPuzzles.ChooseRandom();
            if (puzzle == null)
                puzzle = new ChessPuzzle(true, new BaseChessPiece[] { });
            if (chessBoard == null)
                chessBoard = gameObject.GetOrAddComponent<ChessBoard>();
            if (ChessBoard.Initialized)
                return;
            puzzle = potentialPuzzles.ChooseRandom();
            chessBoard.Initialize(puzzle, pm, this);
        }
        public void EndGame(PlayerManager pm, ChessBoard chessBoard, bool correct)
        {
            if (correct)
                CoreGameManager.Instance.AddPoints(puzzle.MateIn * 100, 0, true);
            else
            {
                playerManager = pm;
                disableWalkingAnim = true;
            }
            puzzle = null;
            behaviorStateMachine.ChangeState(new StockfishWandering(this, 30));
            chessBoard = null;
            gameObject.DeleteComponent<ChessBoard>();
        }
    }
    class StockfishStateBase : NpcState
    {
        protected Stockfish stockfish;
        public StockfishStateBase(Stockfish npc) : base(npc)
        {
            stockfish = npc;
        }
    }
    class StockfishWandering : StockfishStateBase
    {
        private float time;
        public StockfishWandering(Stockfish npc) : this(npc, -1) { }
        public StockfishWandering(Stockfish npc, float time) : base(npc)
        {
            this.time = time;
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
            if (time > 0) 
                time -= Time.deltaTime * stockfish.ec.NpcTimeScale;
        }
        public override void OnStateTriggerEnter(Collider other)
        {
            if (time > 0)
                return;
            base.OnStateTriggerStay(other);
            if (other.CompareTag("Player"))
            {
                stockfish.StartChessMinigame(other.GetComponent<PlayerManager>());
            }
        }
        public override void PlayerSighted(PlayerManager player)
        {
            if (time > 0)
                return;
            base.PlayerSighted(player);
            if (!player.Tagged)
            {
                stockfish.StartPersuingPlayer(player);
            }
        }
        public override void PlayerInSight(PlayerManager player)
        {
            if (time > 0)
                return;
            base.PlayerInSight(player);
            if (!player.Tagged)
            {
                stockfish.PersuePlayer(player);
            }
        }
    }
}
