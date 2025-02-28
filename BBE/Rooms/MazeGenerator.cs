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
    public class MazeGenerator : RoomFunction
    {
        protected Cell[] Cells => room.cells.ToArray();
        protected EnvironmentController EC => room.ec;
        private System.Random RNG;
        private Direction LastDirection
        {
            get => last.direction;
            set => last.direction = value;
        }
        private int Steps
        {
            get => last.steps;
            set => last.steps = value;
        }
        private (Direction direction, int steps) last;
        private Stack<Cell> stack;
        private HashSet<Cell> visited;

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
        private void Generate() =>
            Generate(new System.Random(CoreGameManager.Instance.seed));
        private void Generate(System.Random cRng)
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
                    if (Steps > 2)
                        AddItem(current);
                    ResetLast();
                    stack.Pop(); 
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

        private Direction GetDirection(Cell from, Cell to)
        {
            IntVector2 different = to.position - from.position;
            if (!Directions.vectors.Contains(different))
                return Direction.Null;
            return Directions.All().FirstOrDefault(d => d.ToIntVector2() == different);
        }

        private void AddItem(Cell cell)
        {
            ItemMetaData[] list = ItemMetaStorage.Instance.FindAll(x => x.tags.Contains("BBE_RNGLibraryItem"));
            if (list.EmptyOrNull())
                return;
            ItemObject item = list.ChooseRandom(RNG).value;
            Pickup pickup = item.ToPickup(cell.CenterWorldPosition);
            pickup.icon = EC.map.AddIcon(pickup.iconPre, pickup.transform, Color.white);
        }
    }
}
