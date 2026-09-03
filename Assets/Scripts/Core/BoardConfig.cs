using System;
using System.Collections.Generic;

namespace MisprintDepartment.Core
{
    /// <summary>Static board definition for a level. Serializable, deterministic.</summary>
    [Serializable]
    public sealed class BoardConfig
    {
        public int width = 5;
        public int height = 5;
        public List<Cell> blocked = new List<Cell>();

        public Cell start = new Cell(0, 0);
        public Direction startFacing = Direction.North;
        public Cell destination = new Cell(0, 1);

        public bool InBounds(Cell c) => c.x >= 0 && c.y >= 0 && c.x < width && c.y < height;

        public bool IsBlocked(Cell c)
        {
            for (int i = 0; i < blocked.Count; i++)
                if (blocked[i] == c) return true;
            return false;
        }

        public bool IsWalkable(Cell c) => InBounds(c) && !IsBlocked(c);

        public bool IsValid(out string error)
        {
            error = null;
            if (width < 1 || height < 1) { error = "Board must be at least 1x1."; return false; }
            if (!InBounds(start)) { error = "Start is off-board."; return false; }
            if (!InBounds(destination)) { error = "Destination is off-board."; return false; }
            if (IsBlocked(start)) { error = "Start is on a blocked cell."; return false; }
            if (IsBlocked(destination)) { error = "Destination is on a blocked cell."; return false; }
            if (start == destination) { error = "Start equals destination."; return false; }
            return true;
        }
    }
}
