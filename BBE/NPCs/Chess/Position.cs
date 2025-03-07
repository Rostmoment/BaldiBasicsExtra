using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BBE.Creators;
using BBE.Extensions;
using UnityEngine;
using UnityEngine.UI;
using MTM101BaldAPI.UI;
using HarmonyLib;
using BBE.Helpers;

namespace BBE.NPCs.Chess
{
    // I copied it from my old chess project, lol
    public class Position
    {
        public char File { get; }
        public int Rank { get; }
        public bool IsGray { get; }
        public int FileAsInt { get; }
        public Vector4 Square { get; }
        public List<Vector2> Pixels { get; }
        public Vector3 PiecePostion { get; }
        public Texture2D Texture2D { get; }
        public Image PointImage { get; private set; }
        public BaseChessPiece PieceAtPosition
        {
            get
            {
                if (chessBoard.chessPieces.Exists(x => x.position == this, out BaseChessPiece result))
                    return result;
                return null;
            }
        }
        public Image image;
        public static ChessBoard chessBoard;
        static Position()
        {
            for (int i = 1; i < 9; i++)
            {
                foreach (char c in "abcdefgh")
                {
                    Position.Create(c, i);
                }
            }
        }
        private Position() : this(1, 1) { }
        private Position(int file, int rank) : this(ToFile(file), rank) { }
        private Position(char file, int rank)
        {
            file = file.ToLower();
            if (file < 'a' || file > 'h')
                throw new InvalidPositionException("File should be between 'a' and 'h'");
            if (rank < 1 || rank > 8)
                throw new InvalidPositionException("Rank should be between 1 and 8");
            File = file;
            Rank = rank;
            FileAsInt = ToFile(File);
            IsGray = grayPositions.Contains(ToInt());
            string name = "ChessTileBrown";
            if (IsGray)
                name = "ChessTileGray";
            Texture2D = BasePlugin.Asset.Get<Texture2D>(name);
            Square = new Vector4(100 + 35 * (FileAsInt - 1), -320 + 35 * (Rank - 1),
                    134 + 35 * (FileAsInt - 1), -286 + 35 * (Rank - 1));
            PiecePostion = new Vector3(-120 + (ToFile(file) - 1) * 35, -115 + (rank - 1) * 35, 0);
            Pixels = new List<Vector2>();
            GetOrAdd(this);
        }
        public static Position Create((char x, int y) xy) => Create(xy.x, xy.y);
        public static Position Create((int x, int y) xy) => Create(xy.x, xy.y);
        public static Position Create(IEnumerable<int> ints)
        {
            if (ints.EmptyOrNull())
                return Create();
            if (ints.Count() != 2)
                return Create();
            return Create(ints.ElementAt(0), ints.ElementAt(1));
        }
        public static Position Create() => Create(1, 1);
        public static Position Create(string pos)
        {
            if (pos.EmptyOrNull())
                return Create();
            if (pos.Length != 2)
                return Create();
            if (char.IsDigit(pos[0]))
                pos = pos.Reverse();
            if (!char.IsLetter(pos[0]) || !char.IsDigit(pos[1]))
                return Create();
            return Create(pos[0], int.Parse(pos[1].ToString()));
        }
        public static Position Create(int file, int rank) => Create(ToFile(file), rank);
        public static Position Create(char file, int rank)
        {
            if (positions.Exists(x => x.Rank == rank && x.File == file)) return positions.Find(x => x.File == file && x.Rank == rank);
            return new Position(file, rank);
        }
        public static bool PositionIsValid(int file, int rank)
        {
            try
            {
                Create(file, rank);
                return true;
            }
            catch (InvalidPositionException) { return false; }
        }
        public static Position[] GetAll()
        {
            if (positions.EmptyOrNull()) Position.Create();
            return positions.ToArray();
        }
        public static Position ToPosition(int value)
        {
            return positions.Find(x => x.ToInt() == value);
        }
        public static void HideAllPoints() => Position.GetAll().Do(x => x.HidePoint());
        public void HidePoint()
        {
            ShowPoint(Color.clear);
        }
        public void ShowPoint(Color color)
        {
            if (chessBoard == null)
                return;
            if (PointImage == null)
                PointImage = UIHelpers.CreateImage(BasePlugin.Asset.Get<Sprite>("ChessWhitePoint"), chessBoard.canvas.transform, PiecePostion, true, 0.7f);
            PointImage.color = color;
            PointImage.transform.localPosition = PiecePostion;
            if (PieceAtPosition != null)
                PointImage.transform.SetSiblingIndex(PieceAtPosition.Image.transform.GetSiblingIndex() + 1);
        }/*
        public bool IDontKnowHowToNameThisMethodButIWillRenameItAsSoonAsPossibleAtLeastIHope(Position position)
        {
            if (PieceAtPosition == null)
                return false;
            if (PieceAtPosition.ValidMoves.EmptyOrNull())
                return false;
            return PieceAtPosition.ValidMoves.Exists(x => x == position);
        }*/
        public int ToInt()
        {
            return ToInt(this);
        }
        public static int ToInt(Position position)
        {
            return (ToFile(position.File) - 1) * 8 + position.Rank;
        }

        public Position AddToRank(int rank)
        {
            return Position.Create(File, Rank + rank);
        }
        public Position Add((char file, int rank) toAdd) => Add(toAdd.file, toAdd.rank);
        public Position Add((int file, int rank) toAdd) => Add(toAdd.file, toAdd.rank);
        public Position Add(IEnumerable<int> toAdd)
        {
            if (toAdd.EmptyOrNull())
                return this;
            if (toAdd.Count() != 2) throw new ArgumentException("Data to add should contains 2 elements");
            return Add(toAdd.ElementAt(0), toAdd.ElementAt(1));
        }
        public Position Add(int file, int rank)
        {
            return GetOrAdd(AddToRank(rank).AddToFile(file));
        }
        public Position Add(char file, int rank) => Add(ToFile(file), rank);
        public Position AddToFile(char file) => AddToFile(ToFile(file));
        public Position AddToFile(int file)
        {
            char c = ToFile(ToFile(File) + file);
            return Position.Create(c, Rank);
        }
        public override bool Equals(object obj)
        {
            if (obj is Position other)
                return File == other.File && Rank == other.Rank;
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $"{File}{Rank}";
        }
        public static implicit operator string(Position pos) => pos.ToString();
        public static implicit operator (char x, int y)(Position pos) => (pos.File, pos.Rank);
        public static implicit operator (int x, int y)(Position pos) => (pos.FileAsInt, pos.Rank);
        public static implicit operator int[](Position pos) => new int[] { pos.FileAsInt, pos.Rank };
        public static implicit operator List<int>(Position pos) => new List<int> { pos.FileAsInt,  pos.Rank };
        public static implicit operator Position(string position) => Position.Create(position);
        public static implicit operator Position((char x, int y) xy) => Position.Create(xy);
        public static implicit operator Position((int x, int y) xy) => Position.Create(xy);
        public static implicit operator Position(int[] ints) => Position.Create(ints);
        public static implicit operator Position(List<int> ints) => Position.Create(ints);
        public static bool operator ==(Position pos1, Position pos2) => Equals(pos1, pos2);
        public static bool operator !=(Position pos1, Position pos2) => !Equals(pos1, pos2);
        public static Position operator +(Position pos1, Position pos2) => pos1.AddToFile(pos2.FileAsInt).AddToRank(pos2.Rank);
        public static Position operator -(Position pos1, Position pos2) => pos1.AddToFile(-pos2.FileAsInt).AddToRank(-pos2.Rank);
        public static Position operator *(Position pos1, Position pos2) => Position.Create(pos1.FileAsInt * pos2.FileAsInt, pos1.Rank * pos2.Rank);
        public static Position operator *(Position pos, int multiplier) => Position.Create(pos.FileAsInt * multiplier, pos.Rank * multiplier);
        public static Position operator /(Position pos1, Position pos2) => Position.Create(pos1.FileAsInt / pos2.FileAsInt, pos1.Rank / pos2.Rank);
        public static Position operator /(Position pos, int divider) => Position.Create(pos.FileAsInt / divider, pos.Rank / divider);

        public static int ToFile(char c)
        {
            c = c.ToLower();
            if (files.Count(x => x.Value == c) == 0)
            {
                throw new InvalidPositionException("File should be between 'a' and 'h'");
            }
            return files.FindKey(c);
        }
        public static char ToFile(int i)
        {
            if (!files.ContainsKey(i))
            {
                throw new InvalidPositionException("File should be between 1 and 8");
            }
            return files[i];
        }
        private static Position GetOrAdd(Position position)
        {
            if (!positions.Exists(x => x == position))
            {
                positions.Add(position);
            }
            return positions.Find(x => x == position);
        }
        private static readonly int[] grayPositions = new int[]
        {
            2, 4, 6, 8, 9, 11, 13, 15, 18, 20, 22, 24, 25, 27, 29, 31, 34,
            36, 38, 40, 41, 43, 45, 47, 50, 52, 54, 56, 57, 59, 61, 63
        };
        private static readonly List<Position> positions = new List<Position>();
        private static readonly Dictionary<int, char> files = new Dictionary<int, char>()
        {
            {1, 'a'},
            {2, 'b'},
            {3, 'c'},
            {4, 'd'},
            {5, 'e'},
            {6, 'f'},
            {7, 'g'},
            {8, 'h'}
        };
    }
}
