namespace MisprintDepartment.Core
{
    /// <summary>The six base printed route instructions.</summary>
    public enum PrintCommand
    {
        Move = 0,
        TurnLeft = 1,
        TurnRight = 2,
        Wait = 3,
        Reverse = 4,
        Jump = 5,
    }

    /// <summary>Grid facing. North = +Y. Clockwise: N -> E -> S -> W.</summary>
    public enum Direction
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
    }

    public static class DirectionExtensions
    {
        public static Direction TurnRight(this Direction d) => (Direction)(((int)d + 1) & 3);
        public static Direction TurnLeft(this Direction d) => (Direction)(((int)d + 3) & 3);
        public static Direction Reverse(this Direction d) => (Direction)(((int)d + 2) & 3);

        /// <summary>Unit step for this direction, y-up.</summary>
        public static Cell Step(this Direction d)
        {
            switch (d)
            {
                case Direction.North: return new Cell(0, 1);
                case Direction.East: return new Cell(1, 0);
                case Direction.South: return new Cell(0, -1);
                default: return new Cell(-1, 0);
            }
        }
    }

    public static class PrintCommandExtensions
    {
        /// <summary>
        /// The "command wheel" used by RotateDefect / Rotate correction.
        /// Move -> TurnRight -> Reverse -> TurnLeft -> (Move). Wait and Jump
        /// are off the wheel and rotate to themselves.
        /// </summary>
        static readonly PrintCommand[] Wheel =
        {
            PrintCommand.Move, PrintCommand.TurnRight, PrintCommand.Reverse, PrintCommand.TurnLeft
        };

        public static bool OnWheel(this PrintCommand c) =>
            c == PrintCommand.Move || c == PrintCommand.TurnRight ||
            c == PrintCommand.Reverse || c == PrintCommand.TurnLeft;

        public static PrintCommand Rotate(this PrintCommand c, int steps)
        {
            if (!c.OnWheel()) return c;
            int i = System.Array.IndexOf(Wheel, c);
            int len = Wheel.Length;
            int j = ((i + steps) % len + len) % len;
            return Wheel[j];
        }
    }
}
