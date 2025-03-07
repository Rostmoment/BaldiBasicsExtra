using BBE.Extensions;
using BBE.Helpers;
using BBE.NPCs.Chess;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBE.Rooms
{
    public class LibraryMazeGenerator : MazeGenerator
    {
        public IntVector2 RectangleSize => new IntVector2(Mathf.RoundToInt(UniqueX.Length*0.45f), Mathf.RoundToInt(UniqueZ.Length * 0.45f));
        public static IntVector2 MinRoomSizeForRectangle => new IntVector2(10, 10);
        public int[] UniqueX => Cells.Select(cell => cell.position.x).Distinct().ToArray();
        public int[] UniqueZ => Cells.Select(cell => cell.position.z).Distinct().ToArray();
        public bool RoomIsRectangle => UniqueX.Length * UniqueZ.Length == Cells.Length;
        private void AddItem(Cell cell)
        {
            ItemMetaData[] list = ItemMetaStorage.Instance.FindAll(x => x.tags.Contains("BBE_RNGLibraryItem"));
            if (list.EmptyOrNull())
                return;
            ItemObject item = list.ChooseRandom(RNG).value;
            Pickup pickup = item.ToPickup(cell.CenterWorldPosition);
            pickup.icon = EC.map.AddIcon(pickup.iconPre, pickup.transform, Color.white);
        }
        public override void OnDeadEnd(Cell current)
        {
            if (Steps > 2)
                AddItem(current);
            base.OnDeadEnd(current);
        }
        public override void Generate(System.Random cRng)
        {
            base.Generate(cRng);
            foreach (Door door in Room.doors) 
                door.makesNoise = false;/*
            if (UniqueX.Length >= MinRoomSizeForRectangle.x && UniqueZ.Length >= MinRoomSizeForRectangle.z)
                GenerateMiddleRectangle();*/
        }
        private void GenerateMiddleRectangle()
        {
            Cell center = EC.CellFromPosition(new IntVector2((int)UniqueX.Average(), (int)UniqueZ.Average()));
            if (!center.room.Equals(Room))
                return;
            int xSide = RectangleSize.x / 2;
            int zSide = RectangleSize.z / 2;
            List<Cell> cells = new List<Cell>();
            for (int x = 1; x <= xSide; x++)
            {
                for (int z = 1; z <= zSide; z++)
                {
                    foreach (Direction direction in Directions.All())
                    {
                        for (int i = -1; i < 2; i += 2)
                        {
                            IntVector2 intVector = direction.ToIntVector2();
                            intVector.x *= i * x;
                            intVector.z *= i * z;
                            BasePlugin.Logger.LogInfo(intVector.ToString());
                            cells.Add(EC.CellFromPosition(intVector+center.position));
                        }
                    }
                }
            }
            ConnectCells(cells);
        }
        private void ConnectCells(List<Cell> cells)
        {
            cells.RemoveAll(x => x.Null);
            cells.RemoveAll(x => !x.room.Equals(Room));
            foreach (Cell cell in cells)
            {
                foreach (Direction dir in Directions.All()) {
                    Cell toConnect = EC.CellFromPosition(cell.position + dir.ToIntVector2());
                    if (!cells.Contains(toConnect))
                        continue;
                    else
                        EC.ConnectCells(cell.position, dir);
                }
            }
        }
    }
    public class MazeGenerator : RoomFunction
    {
        protected Cell[] Cells => room.cells.ToArray();
        protected EnvironmentController EC => room.ec;
        protected System.Random RNG;
        protected Direction LastDirection
        {
            get => last.direction;
            set => last.direction = value;
        }
        protected int Steps
        {
            get => last.steps;
            set => last.steps = value;
        }
        protected (Direction direction, int steps) last;
        protected Stack<Cell> stack;
        protected HashSet<Cell> visited;

        public override void Initialize(RoomController room)
        {
            base.Initialize(room);
            Generate();
        }
        private void CloseAllCells()
        {
            List<Cell> tmp = new List<Cell>(Cells);
            while (tmp.Count != 0)
            {
                Cell toFindDirections = tmp.FirstOrDefault();
                if (toFindDirections == null)
                    break;
                foreach (Direction direction in Directions.All())
                {
                    if (!toFindDirections.doorDirs.Contains(direction))
                        EC.CloseCell(toFindDirections.position, direction);
                }
                tmp.Remove(toFindDirections);
            }
        }
        private void AddLight(Cell cell)
        {
            room.standardLightCells.Add(cell.position);
            room.lights.Add(cell);
        }
        private void ResetLast(bool zero = true)
        {
            last = (Direction.Null, 0);
            if (!zero)
                Steps++;
        }
        public virtual void OnDeadEnd(Cell current)
        {
            ResetLast();
            stack.Pop();
        }
        public virtual void Generate() =>
            Generate(new System.Random(CoreGameManager.Instance.seed));
        public virtual void Generate(System.Random cRng)
        {
            RNG = cRng;
            stack = new Stack<Cell>();
            visited = new HashSet<Cell>();
            ResetLast();

            Cell startCell = Cells.ChooseRandom(RNG);
            stack.Push(startCell);
            visited.Add(startCell);
            CloseAllCells();
            while (stack.Count > 0)
            {
                Cell current = stack.Peek();
                if (GetUnvisitedNeighbors(current, out Cell[] neighbors))
                {
                    Cell next = neighbors.ChooseRandom(RNG);
                    Direction direction = GetDirection(current, next);
                    if (direction == LastDirection)
                        Steps++;
                    else
                        last = (direction, 1);
                    EC.ConnectCells(current.position, direction);
                    EC.UpdateCell(current.position);
                    stack.Push(next);
                    visited.Add(next);
                }
                else
                {
                    OnDeadEnd(current);
                }
            }
        }
        private Cell[] ConnectAllNeighbors(Cell cell)
        {
            List<Cell> result = new List<Cell>();
            foreach (Direction direction in Directions.All())
            {
                Cell neighbor = EC.CellFromPosition(cell.position + direction.ToIntVector2());
                if (!neighbor.Null && neighbor.room.Equals(room))
                {
                    EC.ConnectCells(cell.position, direction);
                    result.Add(neighbor);
                }
            }
            return result.ToArray();
        }
        private bool GetUnvisitedNeighbors(Cell cell, out Cell[] neighbors)
        {
            List<Cell> result = new List<Cell>();
            foreach (Direction direction in Directions.All())
            {
                Cell neighbor = EC.CellFromPosition(cell.position + direction.ToIntVector2());
                if (!neighbor.Null && neighbor.room.Equals(room) && !visited.Contains(neighbor))
                {
                    result.Add(neighbor);
                }
            }
            neighbors = result.ToArray();
            return result.Count > 0;
        }

        public Direction GetDirection(Cell from, Cell to)
        {
            IntVector2 different = to.position - from.position;
            if (!Directions.vectors.Contains(different))
                return Direction.Null;
            return Directions.All().FirstOrDefault(d => d.ToIntVector2() == different);
        }

    }
}
