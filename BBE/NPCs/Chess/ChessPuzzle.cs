using BBE.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BBE.NPCs.Chess
{
    public class ChessPuzzleJson
    {
        public List<(string name, string position, bool isWhite)> pieces = new List<(string name, string position, bool isWhite)>();
        public List<(string start, string end)> moves = new List<(string start, string end)>();
        public bool playerIsWhite = false;
    }
    public class ChessPuzzle
    {
        public bool PlayerIsWhite { get; }
        // I HATE ternary operators
        public PieceColor PlayerColor
        {
            get
            {
                if (PlayerIsWhite)
                    return PieceColor.White;
                return PieceColor.Black;
            }
        }
        public int MateIn { get; }
        public Move[] Moves { get; }
        public BaseChessPiece[] Pieces { get; }

        public ChessPuzzle(bool playerIsWhite, BaseChessPiece[] pieces, params Move[] moves) 
        {
            this.PlayerIsWhite = playerIsWhite;
            this.MateIn = (moves.Length+1)/2;
            this.Moves = moves;
            this.Pieces = pieces;
        }


        public static ChessPuzzle FromJson(string path)
        {
            ChessPuzzleJson data = JsonConvert.DeserializeObject<ChessPuzzleJson>(File.ReadAllText(path));
            List<Move> moves = new List<Move>();
            List<BaseChessPiece> pieces = new List<BaseChessPiece>();
            foreach (var piece in data.pieces)
            {
                switch (piece.name.ParseToEnum<ChessPieces>())
                {
                    case ChessPieces.Pawn:
                        pieces.Add(new Pawn(piece.isWhite, piece.position));
                        break;
                    case ChessPieces.Queen:
                        pieces.Add(new Queen(piece.isWhite, piece.position));
                        break;
                    case ChessPieces.King:
                        pieces.Add(new King(piece.isWhite, piece.position));
                        break;
                    case ChessPieces.Knight:
                        pieces.Add(new Knight(piece.isWhite, piece.position));
                        break;
                    case ChessPieces.Bishop:
                        pieces.Add(new Bishop(piece.isWhite, piece.position));
                        break;
                    case ChessPieces.Rook:
                        pieces.Add(new Rook(piece.isWhite, piece.position));
                        break;
                    case ChessPieces.None:
                        break;
                    default:
                        break;
                }
            }
            foreach (var move in data.moves)
            {
                moves.Add(new Move(move.start, move.end));
            }
            return new ChessPuzzle(data.playerIsWhite, pieces.ToArray(), moves.ToArray());
        }
    }
}
