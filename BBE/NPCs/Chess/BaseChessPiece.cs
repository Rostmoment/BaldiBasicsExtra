using BBE.Extensions;
using BBE.Creators;
using MTM101BaldAPI.UI;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace BBE.NPCs.Chess
{
    public abstract class BaseChessPiece
    {
        public King King => (King)ChessBoard.chessPieces.Find(x => x.Type == ChessPieces.King && x.Color == Color);
        public bool IsWhite => Color == PieceColor.White;
        public PieceColor Color { get; }
        public Image Image { get; private set; }
        public Position position;
        public Position StartPosition { get; }
        public abstract ChessPieces Type { get; }
        public ChessBoard ChessBoard { get; private set; }
        public virtual Position[] ValidMoves => new Position[0];

        public BaseChessPiece(bool isWhite, string position) : this(isWhite ? PieceColor.White : PieceColor.Black, position) { }
        public BaseChessPiece(PieceColor color, string position) : this(color, Position.Create(position)) { }
        private BaseChessPiece(PieceColor color, Position position)
        {
            StartPosition = position;
            Color = color;
            this.position = position;
        }
        public void SetupBoard(ChessBoard board)
        {
            if (Image == null)
                Image = UIHelpers.CreateImage(BasePlugin.Asset.Get<Sprite>(Color.ToLower() + Type.ToLower()), board.canvas.transform, Vector3.zero, true, 0.7f);
            Image.transform.localPosition = position.PiecePostion;
            Image.color = UnityEngine.Color.white;
            this.ChessBoard = board;
            ChessBoard.chessPieces.Add(this);
        }
        public void SetPosition(Position position)
        {
            this.position = position;
            Image.transform.localPosition = position.PiecePostion;
        }
        public virtual void Capture()
        {
            SetPosition(StartPosition);
            ChessBoard.chessPieces.Remove(this);
            Image.color = UnityEngine.Color.clear;
        }
    }
}
