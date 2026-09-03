using System.Linq;
using NUnit.Framework;
using MisprintDepartment.Core;
using C = MisprintDepartment.Core.PrintCommand;

namespace MisprintDepartment.Tests
{
    public class RouteSimulatorTests
    {
        static BoardConfig Board5x5()
        {
            return new BoardConfig
            {
                width = 5, height = 5,
                start = new Cell(0, 0), startFacing = Direction.North,
                destination = new Cell(0, 3),
            };
        }

        [Test]
        public void Move_advances_one_cell_in_facing()
        {
            var r = RouteSimulator.Run(Board5x5(), CommandStrip.Of(C.Move, C.Move, C.Move));
            Assert.IsTrue(r.success);
            Assert.AreEqual(new Cell(0, 3), r.finalCell);
        }

        [Test]
        public void TurnLeft_and_TurnRight_are_opposite()
        {
            var b = new BoardConfig { width = 3, height = 3, start = new Cell(1, 1), startFacing = Direction.North, destination = new Cell(2, 1) };
            var left = RouteSimulator.Run(b, CommandStrip.Of(C.TurnLeft, C.Move));   // faces West -> off? (0,1) ok
            var right = RouteSimulator.Run(b, CommandStrip.Of(C.TurnRight, C.Move)); // faces East -> (2,1)
            Assert.AreEqual(new Cell(0, 1), left.finalCell);
            Assert.AreEqual(new Cell(2, 1), right.finalCell);
            Assert.IsTrue(right.success);
        }

        [Test]
        public void Reverse_flips_facing()
        {
            var b = new BoardConfig { width = 3, height = 3, start = new Cell(1, 1), startFacing = Direction.North, destination = new Cell(1, 0) };
            var r = RouteSimulator.Run(b, CommandStrip.Of(C.Reverse, C.Move));
            Assert.IsTrue(r.success);
            Assert.AreEqual(new Cell(1, 0), r.finalCell);
        }

        [Test]
        public void Wait_consumes_a_step_without_moving()
        {
            var r = RouteSimulator.Run(Board5x5(), CommandStrip.Of(C.Wait, C.Move, C.Move, C.Move));
            Assert.IsTrue(r.success);
            Assert.AreEqual(new Cell(0, 3), r.finalCell);
        }

        [Test]
        public void Jump_advances_two_cells()
        {
            var b = new BoardConfig { width = 5, height = 5, start = new Cell(0, 0), startFacing = Direction.North, destination = new Cell(0, 2) };
            var r = RouteSimulator.Run(b, CommandStrip.Of(C.Jump));
            Assert.IsTrue(r.success);
            Assert.AreEqual(new Cell(0, 2), r.finalCell);
        }

        [Test]
        public void Jump_ignores_the_middle_cell_block()
        {
            var b = Board5x5();
            b.blocked.Add(new Cell(0, 1)); // middle
            b.destination = new Cell(0, 2);
            var r = RouteSimulator.Run(b, CommandStrip.Of(C.Jump));
            Assert.IsTrue(r.success, "Jump should clear a blocked middle cell");
        }

        [Test]
        public void Running_off_board_is_RouteInvalid()
        {
            var r = RouteSimulator.Run(Board5x5(), CommandStrip.Of(C.Reverse, C.Move));
            Assert.IsFalse(r.success);
            Assert.AreEqual(FailReason.RouteInvalid, r.failReason);
            Assert.AreEqual(1, r.failedAtIndex);
        }

        [Test]
        public void Moving_into_a_blocked_cell_is_RouteInvalid()
        {
            var b = Board5x5();
            b.blocked.Add(new Cell(0, 1));
            var r = RouteSimulator.Run(b, CommandStrip.Of(C.Move));
            Assert.IsFalse(r.success);
            Assert.AreEqual(FailReason.RouteInvalid, r.failReason);
        }

        [Test]
        public void Ending_off_destination_is_TargetMissed()
        {
            var r = RouteSimulator.Run(Board5x5(), CommandStrip.Of(C.Move));
            Assert.IsFalse(r.success);
            Assert.AreEqual(FailReason.TargetMissed, r.failReason);
        }

        [Test]
        public void Faded_token_is_InstructionError()
        {
            var strip = new CommandStrip();
            strip.tokens.Add(new CommandToken(C.Move, InkFlags.Faded));
            var r = RouteSimulator.Run(Board5x5(), strip);
            Assert.IsFalse(r.success);
            Assert.AreEqual(FailReason.InstructionError, r.failReason);
        }

        [Test]
        public void Striped_token_is_skipped_by_the_machine()
        {
            // clean: Move Move Move -> (0,3); striping the 2nd Move leaves it 1 short.
            var b = new BoardConfig { width = 3, height = 4, start = new Cell(0, 0), startFacing = Direction.North, destination = new Cell(0, 3) };
            var strip = new CommandStrip();
            strip.tokens.Add(new CommandToken(C.Move));
            strip.tokens.Add(new CommandToken(C.Move, InkFlags.Striped));
            strip.tokens.Add(new CommandToken(C.Move));
            var r = RouteSimulator.Run(b, strip);
            Assert.IsFalse(r.success);
            Assert.AreEqual(new Cell(0, 2), r.finalCell);
            Assert.IsTrue(r.trace.Any(t => t.skipped && t.tokenIndex == 1));
        }

        [Test]
        public void Marked_token_runs_twice()
        {
            var b = new BoardConfig { width = 3, height = 4, start = new Cell(0, 0), startFacing = Direction.North, destination = new Cell(0, 3) };
            var strip = new CommandStrip();
            strip.tokens.Add(new CommandToken(C.Move, InkFlags.Marked)); // counts as 2
            strip.tokens.Add(new CommandToken(C.Move));
            var r = RouteSimulator.Run(b, strip);
            Assert.IsTrue(r.success);
            Assert.AreEqual(new Cell(0, 3), r.finalCell);
        }

        [Test]
        public void Simulation_is_deterministic()
        {
            var b = Board5x5();
            var strip = CommandStrip.Of(C.Move, C.TurnRight, C.Move, C.TurnLeft, C.Move, C.Move);
            var a = RouteSimulator.Run(b, strip);
            var c = RouteSimulator.Run(b, strip);
            Assert.AreEqual(a.ToString(), c.ToString());
            Assert.AreEqual(a.finalCell, c.finalCell);
            Assert.AreEqual(a.finalFacing, c.finalFacing);
        }
    }
}
