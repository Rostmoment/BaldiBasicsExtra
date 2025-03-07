using BBE.Extensions;
using BBE.Creators;
using System;
using System.Collections.Generic;
using System.Text;
using BBE.Helpers;

namespace BBE.NPCs.Chess
{
    public class Knight : BaseChessPiece
    {
        public Knight(bool isWhite, string position) : base(isWhite, position) { }
        public Knight(PieceColor color, string position) : base(color, position)
        {
        }

        public override ChessPieces Type => ChessPieces.Knight;

        public override Position[] ValidMoves
        {
            get
            {
                // I hate using var, but (int x, int y)[] more worse
                List<Position> validPositions = new List<Position>();
                var directions = new (int x, int y)[]
                {
                    (2, 1), (2, -1), (-2, 1), (-2, -1), (1, 2), (1, -2), (-1, 2), (-1, -2)
                };
                foreach (var move in directions)
                {
                    try
                    {
                        Position result = position.Add(move);
                        if (result.PieceAtPosition == null || result.PieceAtPosition.Color != Color)
                            validPositions.Add(result);
                    }
                    catch (InvalidPositionException) { }
                }
                return validPositions.ToArray();
            }
        }
    }
    public class King : BaseChessPiece
    {
        public King(bool isWhite, string position) : base(isWhite, position) { }
        public King(PieceColor color, string position) : base(color, position)
        {
        }
        public override ChessPieces Type => ChessPieces.King;

        public override Position[] ValidMoves
        {
            get
            {
                List<Position> validPositions = new List<Position>();
                var directions = new (int x, int y)[]
                {
                    (0, 1), (0, -1), (1, 1), (1, 0), (1, -1), (-1, 1), (-1, -1), (-1, 0)
                };
                foreach ((int x, int y) move in directions)
                {
                    try
                    {
                        Position result = position.Add(move);
                        if (result.PieceAtPosition == null || result.PieceAtPosition.Color != Color)
                            validPositions.Add(result);
                    }
                    catch (InvalidPositionException) { }
                }
                return validPositions.ToArray();
            }
        }
    }
    public class Bishop : BaseChessPiece
    {
        public Bishop(bool isWhite, string position) : base(isWhite, position) { }
        public Bishop(PieceColor color, string position) : base(color, position)
        {
        }

        public override ChessPieces Type => ChessPieces.Bishop;
        public override Position[] ValidMoves
        {
            get
            {
                List<Position> validMoves = new List<Position>();
                var directions = new (int x, int y)[]
                {
                    (1, 1), (-1, -1), (-1, 1), (1, -1)
                };
                foreach (var move in directions)
                {
                    for (int i = 1; i < int.MaxValue; i++)
                    {
                        try
                        {
                            Position result = position.Add(move.x * i, move.y * i);
                            if (result.PieceAtPosition == null)
                                validMoves.Add(result);
                            else if (result.PieceAtPosition.Color != Color)
                            {
                                validMoves.Add(result);
                                break;
                            }
                            else
                                break;
                        }
                        catch (InvalidPositionException)
                        {
                            break;
                        }
                    }
                }
                return validMoves.ToArray();
            }
        }
    }
    public class Queen : BaseChessPiece
    {
        public Queen(bool isWhite, string position) : base(isWhite, position) { }
        public Queen(PieceColor color, string position) : base(color, position)
        {
        }

        public override ChessPieces Type => ChessPieces.Queen;
        public override Position[] ValidMoves
        {
            get
            {
                List<Position> validMoves = new List<Position>();
                var directions = new (int x, int y)[]
                {
                    (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (-1, 1), (1, -1)
                };
                foreach (var move in directions)
                {
                    for (int i = 1; i < int.MaxValue; i++)
                    {
                        try
                        {
                            Position result = position.Add(move.x * i, move.y * i);
                            if (result.PieceAtPosition == null)
                                validMoves.Add(result);
                            else if (result.PieceAtPosition.Color != Color)
                            {
                                validMoves.Add(result);
                                break;
                            }
                            else
                                break;
                        }
                        catch (InvalidPositionException)
                        {
                            break;
                        }
                    }
                }
                return validMoves.ToArray();
            }
        }
    }
    public class Pawn : BaseChessPiece
    {
        public Pawn(bool isWhite, string position) : base(isWhite, position) { }
        public Pawn(PieceColor color, string position) : base(color, position)
        {
        }
        public int StartRank
        {
            get
            {
                if (Color == PieceColor.White)
                    return 2;
                return 7;
            }
        }
        public override ChessPieces Type => ChessPieces.Pawn;
        public override Position[] ValidMoves
        {
            get
            {
                List<Position> validMoves = new List<Position>();
                int modifer = 1;
                if (Color == PieceColor.Black)
                    modifer *= -1;
                Position result = position.AddToRank(modifer);
                if (result.PieceAtPosition == null)
                    validMoves.Add(result);
                if (StartRank == position.Rank && validMoves.Count > 0)
                {
                    result = position.AddToRank(modifer * 2);
                    if (result.PieceAtPosition == null)
                        validMoves.Add(result);
                }
                try
                {
                    result = position.Add(1, modifer);
                    if (result.PieceAtPosition != null)
                        if (result.PieceAtPosition.Color != Color)
                            validMoves.Add(result);
                }
                catch (InvalidPositionException) { }
                try
                {
                    result = position.Add(-1, modifer);
                    if (result.PieceAtPosition != null)
                        if (result.PieceAtPosition.Color != Color)
                            validMoves.Add(result);
                }
                catch (InvalidPositionException) { }
                return validMoves.ToArray();
            }
        }
    }
    public class Rook : BaseChessPiece
    {
        public Rook(bool isWhite, string position) : base(isWhite, position) { }
        public Rook(PieceColor color, string position) : base(color, position)
        {
        }

        public override ChessPieces Type => ChessPieces.Rook;
        public override Position[] ValidMoves
        {
            get
            {
                List<Position> validMoves = new List<Position>();
                var directions = new (int x, int y)[]
                {
                    (1, 0), (-1, 0), (0, 1), (0, -1)
                };
                foreach (var move in directions)
                {
                    for (int i = 1; i < int.MaxValue; i++)
                    {
                        try
                        {
                            Position result = position.Add(move.x * i, move.y * i);
                            if (result.PieceAtPosition == null)
                                validMoves.Add(result);
                            else if (result.PieceAtPosition.Color != Color)
                            {
                                validMoves.Add(result);
                                break;
                            }
                            else
                                break;
                        }
                        catch (InvalidPositionException)
                        {
                            break;
                        }
                    }
                }
                return validMoves.ToArray();
            }
        }
    }
}
