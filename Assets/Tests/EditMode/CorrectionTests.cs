using System.Collections.Generic;
using NUnit.Framework;
using MisprintDepartment.Core;
using C = MisprintDepartment.Core.PrintCommand;

namespace MisprintDepartment.Tests
{
    public class CorrectionTests
    {
        static readonly HashSet<CorrectionTool> All = new HashSet<CorrectionTool>
        {
            CorrectionTool.Rotate, CorrectionTool.Replace, CorrectionTool.Swap,
            CorrectionTool.CrossOut, CorrectionTool.Shift, CorrectionTool.Restore
        };

        [Test]
        public void Rotate_steps_along_the_wheel()
        {
            var s = CommandStrip.Of(C.Move);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoRotate(0, 1), All);
            Assert.IsTrue(r.ok);
            Assert.AreEqual(C.TurnRight, r.strip[0].command);
        }

        [Test]
        public void Rotate_rejects_non_wheel_command()
        {
            var s = CommandStrip.Of(C.Wait);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoRotate(0, 1), All);
            Assert.IsFalse(r.ok);
        }

        [Test]
        public void Replace_sets_the_command()
        {
            var s = CommandStrip.Of(C.Move, C.Move);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoReplace(1, C.Jump), All);
            Assert.IsTrue(r.ok);
            Assert.AreEqual(C.Jump, r.strip[1].command);
        }

        [Test]
        public void Swap_exchanges_two_slots()
        {
            var s = CommandStrip.Of(C.Move, C.TurnLeft, C.Jump);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoSwap(0, 2), All);
            Assert.IsTrue(r.ok);
            Assert.AreEqual(C.Jump, r.strip[0].command);
            Assert.AreEqual(C.Move, r.strip[2].command);
        }

        [Test]
        public void CrossOut_removes_a_token()
        {
            var s = CommandStrip.Of(C.Move, C.Wait, C.Move);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoCrossOut(1), All);
            Assert.IsTrue(r.ok);
            Assert.AreEqual(2, r.strip.Count);
            Assert.AreEqual(C.Move, r.strip[1].command);
        }

        [Test]
        public void Shift_moves_a_token_one_slot()
        {
            var s = CommandStrip.Of(C.Move, C.TurnLeft, C.Jump);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoShift(0, +1), All);
            Assert.IsTrue(r.ok);
            Assert.AreEqual(C.TurnLeft, r.strip[0].command);
            Assert.AreEqual(C.Move, r.strip[1].command);
        }

        [Test]
        public void Restore_reveals_a_faded_token()
        {
            var s = new CommandStrip();
            s.tokens.Add(new CommandToken(C.Move, InkFlags.Faded));
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoRestore(0, C.Jump), All);
            Assert.IsTrue(r.ok);
            Assert.AreEqual(C.Jump, r.strip[0].command);
            Assert.IsFalse(r.strip[0].Has(InkFlags.Faded));
        }

        [Test]
        public void Restore_rejects_a_clear_token()
        {
            var s = CommandStrip.Of(C.Move);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoRestore(0, C.Jump), All);
            Assert.IsFalse(r.ok);
        }

        [Test]
        public void Restore_clears_a_striped_token_keeping_its_command()
        {
            var s = new CommandStrip();
            s.tokens.Add(new CommandToken(C.Move, InkFlags.Striped));
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoRestore(0, C.Jump), All);
            Assert.IsTrue(r.ok);
            Assert.AreEqual(C.Move, r.strip[0].command, "striped Restore keeps the command");
            Assert.IsFalse(r.strip[0].Has(InkFlags.Striped));
        }

        [Test]
        public void Disallowed_tool_is_rejected()
        {
            var s = CommandStrip.Of(C.Move);
            var only = new HashSet<CorrectionTool> { CorrectionTool.Replace };
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoRotate(0, 1), only);
            Assert.IsFalse(r.ok);
        }

        [Test]
        public void Out_of_range_index_is_rejected()
        {
            var s = CommandStrip.Of(C.Move);
            var r = CorrectionEngine.Apply(s, CorrectionOp.DoReplace(5, C.Jump), All);
            Assert.IsFalse(r.ok);
        }

        [Test]
        public void Applying_a_correction_does_not_mutate_the_input()
        {
            var s = CommandStrip.Of(C.Move, C.Move);
            CorrectionEngine.Apply(s, CorrectionOp.DoReplace(0, C.Jump), All);
            Assert.AreEqual(C.Move, s[0].command, "input strip must be untouched");
        }
    }
}
