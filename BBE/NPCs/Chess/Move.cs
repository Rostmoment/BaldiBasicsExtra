using BBE.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.NPCs.Chess
{
    public struct Move
    {
        public Position start;
        public Position end;
        public Move(Position start, Position end)
        {
            this.start = start;
            this.end = end;
        }
        public Move(string start, string end)
        {
            this.start = Position.Create(start);
            this.end = Position.Create(end);
        }
        public override string ToString()
        {
            return start.ToString()+" - "+end.ToString();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Move left, Move right) => Equals(left, right);
        public static bool operator !=(Move left, Move right) => !Equals(left, right);
        public override bool Equals(object obj)
        {
            if (!(obj is Move))
                return false;
            Move move = (Move)obj;
            return move.end == this.end && move.start == this.start;
        }
    }
}
