using BBE.Extensions;
using BBE.Creators;
using HarmonyLib;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BBE.Helpers;
using MTM101BaldAPI.Registers;

namespace BBE.NPCs.Chess
{
    public class ChessBoard : MonoBehaviour
    {
        private float time;
        private TimeScaleModifier zero = new TimeScaleModifier(0, 0, 0);
        private Stockfish stockfish;
        private PlayerManager pm;
        private ChessPuzzle puzzle;
        private int moveMade;
        private int totalMoves;
        public GameObject canvas;
        private Position[] positions;
        private GameObject cursor;
        private Sprite sprite;
        private Image boardImage;
        private TMP_Text info;
        private TMP_Text title;
        private CursorController cursorController;
        public Move CurrentMove
        {
            get
            {
                try
                {
                    return puzzle.Moves[totalMoves];
                }
                catch
                {
                    return new Move("A1", "A1");
                }
            }
        }
        public static bool Initialized { get; private set; } = false;
        private static BaseChessPiece selectedPiece;
        private static Position selectedTile;
        // 272; 272
        private Dictionary<Position, Vector4> positionSquares = new Dictionary<Position, Vector4>() 
        {
            
        };
        public List<BaseChessPiece> chessPieces;
        public int MovesLeft => (puzzle.Moves.Length + 1) / 2 - moveMade;
        public static Sprite GenerateBoardSprite(Position position)
        {
            if (position == null)
                return GenerateBoardSprite();
            if (BasePlugin.Asset.Exists<Sprite>("ChessBoard" + position.ToString(), out Sprite sprite))
                return sprite;
            if (!BasePlugin.Asset.Exists<Sprite>("ChessBoard"))
                BasePlugin.Asset.Add<Sprite>("ChessBoard", ChessBoard.GenerateBoardSprite());
            Texture2D reference = BasePlugin.Asset.Get<Sprite>("ChessBoard").texture.CopyTexture();
            foreach (Vector2 vector in position.Pixels)
                reference.SetPixel((int)vector.x, (int)vector.y, Color.yellow);
            reference.Apply();
            Sprite result = reference.ToSprite();
            BasePlugin.Asset.Add<Sprite>("ChessBoard" + position.ToString(), result);
            return result;
        }
        public static Sprite GenerateBoardSprite()
        {
            if (BasePlugin.Asset.Exists<Sprite>("ChessBoard", out Sprite sprite)) 
                return sprite;
            Texture2D result = new Texture2D(Constants.CHESS_TILE_SIZE * 8, Constants.CHESS_TILE_SIZE * 8);
            for (int x = 0; x < 512; x++)
            {
                for (int y = 0; y < 512; y++)
                {
                    int file = x / Constants.CHESS_TILE_SIZE + 1;
                    int rank = y / Constants.CHESS_TILE_SIZE + 1;
                    Position pos = Position.Create(file, rank);
                    Vector2 vector = new Vector2(x, y);
                    if (!pos.Pixels.Contains(vector))
                        pos.Pixels.Add(vector);
                    int localX = x % Constants.CHESS_TILE_SIZE;
                    int localY = y % Constants.CHESS_TILE_SIZE;
                    Color color = pos.Texture2D.GetPixel(localX, localY);   
                    result.SetPixel(x, y, color);
                }
            }
            result.Apply();
            Sprite res = result.ToSprite();
            BasePlugin.Asset.Add<Sprite>("ChessBoard", res);
            return res;
        }
        private bool CursorInside(Vector4 rect)
        {
            return cursor.transform.localPosition.x >= rect.x && cursor.transform.localPosition.x <= rect.z &&
                cursor.transform.localPosition.y >= rect.y && cursor.transform.localPosition.y <= rect.w;
        }
        private void Start() 
        {
        }
        public IEnumerator MakeMove(Move move, float time)
        {
            cursorController.image.color = cursorController.image.color.Change(a: 0.5f);
            cursorController.DisableClick(true);
            BaseChessPiece piece = chessPieces.Find(x => x.position == move.start);
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
            if (chessPieces.IfExists(x => x.position == move.end, out BaseChessPiece result))
            {
                result.Capture();
            }
            piece.SetPosition(move.end);
            cursorController.image.color = cursorController.image.color.Change(a: 1);
            cursorController.DisableClick(false);
            yield break;
        }
        public void MakeMove(Move move, bool playerMove)
        {
            totalMoves++;
            if (playerMove)
            {
                StartCoroutine(MakeMove(move, 0));
                moveMade++;
                if (totalMoves < puzzle.Moves.Length - 1)
                {
                    MakeMove(CurrentMove, false);
                }
            }
            else
                StartCoroutine(MakeMove(move, 0.5f));
        }
        private void UpdateVisual()
        {
            string tileText = "NO";
            if (selectedTile != null)
                tileText = selectedTile.ToString();
            string pieceName = "None";
            if (selectedPiece != null)
                pieceName = selectedPiece.Type.ToString();
            string textToSet = string.Format("BBE_ChessInfo".Localize(), tileText.ToUpper(), MovesLeft.ToString(), pieceName);
            ShowValidMoves();
            info.text = textToSet;
            cursorController.transform.SetAsLastSibling();
        }
        public void ShowValidMoves()
        {
            Position.HideAllPoints();
            if (selectedPiece == null)
                return;
            if (selectedPiece.ValidMoves.EmptyOrNull())
                return;
            selectedPiece.ValidMoves?.Do(x => x.ShowPoint(Color.blue));
        }
        private void ShowCorrectMove()
        {
            Position.HideAllPoints();
            if (totalMoves >= puzzle.Moves.Length)
                return;
            CurrentMove.start.ShowPoint(Color.green);
            CurrentMove.end.ShowPoint(Color.red);
        }
        private void OnTileClick()
        {
            if (positionSquares.Values.IfExists(x => CursorInside(x), out Vector4 result))
            {
                Position position = positionSquares.FindKey(result);
                if (position == selectedTile)
                    return;
                if (selectedTile == null || selectedPiece == null || selectedPiece.Color != puzzle.PlayerColor || position.PieceAtPosition?.Color == selectedPiece?.Color || !selectedPiece.ValidMoves.Any(x => x == position))
                {
                    selectedTile = position;
                    selectedPiece = position.PieceAtPosition;
                }
                else
                {
                    Move move = new Move(selectedTile, position);
                    if (move != CurrentMove)
                    {
                        End(false);
                        return;
                    }
                    MakeMove(move, true);
                    selectedTile = null;
                    selectedPiece = null;
                }
            }
            else
            {
                selectedPiece = null;
                selectedTile = null;
            }
            boardImage.sprite = GenerateBoardSprite(selectedTile);
            UpdateVisual();
        }
        private void Update()
        {
            if (puzzle == null)
                End(true);
            if (totalMoves == puzzle.Moves.Length)
            {
                cursorController.DisableClick(true);
                cursorController.image.color = cursorController.image.color.Change(a: 0.5f);
                if (float.IsNaN(time))
                    time = 1f;
                time -= Time.deltaTime;
                if (time <= 0f)
                    End(true);
            }
            if (CurrentMove.end != CurrentMove.start)
            {
                if (pm.itm.items.Any(x => x.itemType.Is(ModdedItems.ChessBook)))
                    ShowCorrectMove();
            }
            if (Input.GetKeyDown(KeyCode.C))
                End(false);
        }
        public void Initialize(ChessPuzzle puzzle, PlayerManager pm, Stockfish stockfish)
        {
            this.positions = Position.GetAll();
            if (positions.Length != positionSquares.Count)
            {
                foreach (Position position in positions)
                {
                    if (!positionSquares.ContainsKey(position))
                        positionSquares.Add(position, position.Square);
                }
            }
            this.sprite = BasePlugin.Asset.Get<Sprite>("ChessBoard");
            selectedTile = null;
            selectedPiece = null;
            this.puzzle = null;
            CoreGameManager.Instance.disablePause = true;
            Position.chessBoard = this;
            Initialized = true;
            this.sprite = GenerateBoardSprite();
            this.stockfish = stockfish;
            this.pm = pm;
            this.chessPieces = new List<BaseChessPiece>();
            this.puzzle = puzzle;
            if (zero == null)
                zero = new TimeScaleModifier(0, 0, 0);
            pm.ec.AddTimeScale(zero);
            if (canvas == null)
            {
                this.canvas = CreateObjects.CreateCanvas("ChessCanvas", color: Color.black.Change(a: 0.9f));
                this.boardImage = UIHelpers.CreateImage(sprite, canvas.transform, Vector3.zero, false, 0.55f);
                TMP_Text letters = CreateObjects.CreateText("Letters", "A  B  C  D  E  F  G  H", true, new Vector3(-135, -140, 0), Vector3.one, canvas.transform, BaldiFonts.BoldComicSans24);
                letters.color = Color.white;
                letters.fontSize = 25f;
                letters.ChangeizeTextContainerState(true);
                TMP_Text numbers = CreateObjects.CreateText("Numbers", "8\n7\n6\n5\n4\n3\n2\n1", true, new Vector3(-160, 16.5f), Vector3.one, canvas.transform, BaldiFonts.BoldComicSans24);
                numbers.color = Color.white;
                numbers.fontSize = 26;
                this.title = CreateObjects.CreateText("Title", "", true, new Vector3(-115, 180, 0), new Vector3(1.5f, 1.5f, 1.5f), canvas.transform, BaldiFonts.BoldComicSans12);
                this.title.color = Color.white;
                this.info = CreateObjects.CreateText("Info", "BBE_YourTurn".Localize(), canvas, new Vector3(143, 120), new Vector3(0.9f, 0.9f, 0.9f), canvas.transform, BaldiFonts.BoldComicSans24);
                this.info.color = Color.white;
                this.cursorController = CreateObjects.CreateCursor(canvas.transform, out cursor);
            }
            cursorController.DisableClick(false);
            cursorController.image.color = cursorController.image.color.Change(a: 1);
            StandardMenuButton m = canvas.GetComponentInChildren<Image>().gameObject.ConvertToButton();
            m.OnPress.AddListener(OnTileClick);
            string pieceColor = "BBE_Black".Localize();
            if (puzzle.PlayerIsWhite)
                pieceColor = "BBE_White".Localize();
            title.text = string.Format("BBE_ToMoveChess".Localize(), pieceColor, puzzle.MateIn.ToString());
            canvas.SetActive(true);
            if (!puzzle.Pieces.EmptyOrNull())
            {
                foreach (BaseChessPiece piece in puzzle.Pieces.OrderByDescending(x => x.position.Rank))
                {
                    piece.SetupBoard(this);
                }
            }
            cursorController.transform.SetAsLastSibling();
            CoreGameManager.Instance.GetHud(0).Hide(true);
            this.moveMade = 0;
            this.totalMoves = 0;
            this.time = float.NaN;
            Position.HideAllPoints();
            UpdateVisual();
        }
        public void End(bool corect)
        {
            CoreGameManager.Instance.disablePause = false;
            Position.chessBoard = null;
            CoreGameManager.Instance.GetHud(0).Hide(false);
            Destroy(canvas);
            canvas = null;
            try
            {
                if (pm.ec.timeScaleModifiers.Contains(zero))
                    pm.ec.RemoveTimeScale(zero);
            }
            catch (NullReferenceException) { }
            stockfish.EndGame(pm, this, corect);
            this.puzzle = null;
            this.pm = null;
            this.stockfish = null;
            Initialized = false;
            selectedPiece = null;
            this.sprite = GenerateBoardSprite();
            selectedTile = null;
            foreach (BaseChessPiece piece in chessPieces.ToList())
                piece.Capture();
        }
    }
}
