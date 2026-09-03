using System;

namespace MisprintDepartment.Core
{
    /// <summary>Integer grid coordinate, y-up. Value type, deterministic.</summary>
    [Serializable]
    public struct Cell : IEquatable<Cell>
    {
        public int x;
        public int y;

        public Cell(int x, int y) { this.x = x; this.y = y; }

        public static Cell operator +(Cell a, Cell b) => new Cell(a.x + b.x, a.y + b.y);
        public static Cell operator *(Cell a, int k) => new Cell(a.x * k, a.y * k);
        public static bool operator ==(Cell a, Cell b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Cell a, Cell b) => !(a == b);

        public bool Equals(Cell other) => this == other;
        public override bool Equals(object obj) => obj is Cell c && this == c;
        public override int GetHashCode() => (x * 397) ^ y;
        public override string ToString() => $"({x},{y})";
    }
}
