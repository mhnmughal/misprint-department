using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using MisprintDepartment.Core;
using C = MisprintDepartment.Core.PrintCommand;

namespace MisprintDepartment.Tests
{
    public class DefectAndJudgeTests
    {
        [Test]
        public void MirrorDefect_swaps_turns()
        {
            var clean = CommandStrip.Of(C.TurnLeft, C.Move, C.TurnRight);
            var d = new MirrorDefect { selection = SelectionMode.All };
            var printed = d.Apply(clean);
            Assert.AreEqual(C.TurnRight, printed[0].command);
            Assert.AreEqual(C.Move, printed[1].command);
            Assert.AreEqual(C.TurnLeft, printed[2].command);
            Assert.AreEqual(C.TurnLeft, clean[0].command, "defect must not mutate the clean strip");
        }

        [Test]
        public void DuplicateDefect_every_nth_lengthens_strip()
        {
            var clean = CommandStrip.Of(C.Move, C.Move, C.Move, C.Move, C.Move, C.Move);
            var d = new DuplicateDefect { trigger = DuplicateTrigger.EveryNth, nth = 3 };
            var printed = d.Apply(clean);
            Assert.AreEqual(8, printed.Count); // slots 3 and 6 duplicated
        }

        [Test]
        public void ShiftDefect_drifts_a_slot_right()
        {
            var clean = CommandStrip.Of(C.Move, C.Move, C.Move, C.TurnLeft, C.Move);
            var d = new ShiftDefect { selection = SelectionMode.EveryNth, nth = 4, direction = 1 };
            var printed = d.Apply(clean);
            // slot index 3 (the TurnLeft) swaps with index 4
            Assert.AreEqual(C.Move, printed[3].command);
            Assert.AreEqual(C.TurnLeft, printed[4].command);
        }

        [Test]
        public void FadeDefect_marks_faded()
        {
            var clean = CommandStrip.Of(C.Move, C.Jump, C.Move);
            var d = new FadeDefect { indices = new List<int> { 1 } };
            var printed = d.Apply(clean);
            Assert.IsTrue(printed[1].Has(InkFlags.Faded));
        }

        [Test]
        public void RotateDefect_is_reversible_by_Rotate_correction()
        {
            var clean = CommandStrip.Of(C.Move, C.Move, C.TurnRight);
            var d = new RotateDefect { selection = SelectionMode.ExplicitIndices, indices = new List<int> { 2 }, steps = 1 };
            var printed = d.Apply(clean);
            Assert.AreNotEqual(C.TurnRight, printed[2].command);
            var fixedBack = CorrectionEngine.Apply(printed, CorrectionOp.DoRotate(2, -1), null);
            Assert.IsTrue(fixedBack.ok);
            Assert.AreEqual(C.TurnRight, fixedBack.strip[2].command);
        }

        [Test]
        public void CompositeDefect_applies_rules_in_order()
        {
            var clean = CommandStrip.Of(C.TurnLeft, C.Move, C.Move);
            var d = new CompositeDefect
            {
                rules = new List<DefectRule>
                {
                    new MirrorDefect { selection = SelectionMode.All },
                    new SwapDefect { a = 0, b = 2 },
                }
            };
            var printed = d.Apply(clean);
            // mirror -> [TurnRight, Move, Move] ; swap 0<->2 -> [Move, Move, TurnRight]
            Assert.AreEqual(C.Move, printed[0].command);
            Assert.AreEqual(C.TurnRight, printed[2].command);
        }

        // ---- LevelJudge integration ----

        static LevelDefinition MakeMirrorLevel()
        {
            var lvl = ScriptableObject.CreateInstance<LevelDefinition>();
            lvl.levelId = 7;
            lvl.chapter = 2;
            lvl.board = new BoardConfig
            {
                width = 4, height = 4,
                start = new Cell(0, 0), startFacing = Direction.North,
                destination = new Cell(2, 2),
            };
            // clean route: Move Move TurnRight Move Move  -> (0,0)->(0,2) face E ->(2,2)
            lvl.cleanStrip = new List<SerializableToken>
            {
                Tok(C.Move), Tok(C.Move), Tok(C.TurnRight), Tok(C.Move), Tok(C.Move),
            };
            lvl.defect = new MirrorDefect { selection = SelectionMode.All }; // TurnRight -> TurnLeft
            lvl.allowedTools = new List<CorrectionTool> { CorrectionTool.Rotate, CorrectionTool.Replace };
            lvl.correctionBudget = 2;
            lvl.par = 1;
            lvl.referenceSolution = new List<CorrectionOp> { CorrectionOp.DoReplace(2, C.TurnRight) };
            return lvl;
        }

        static SerializableToken Tok(C c) => new SerializableToken { command = c, flags = InkFlags.None };

        [Test]
        public void Judge_approves_the_reference_solution_with_MasterProof()
        {
            var lvl = MakeMirrorLevel();
            var outcome = LevelJudge.Evaluate(lvl, lvl.referenceSolution);
            Assert.IsTrue(outcome.approved, outcome.message);
            Assert.AreEqual(ProofRating.MasterProof, outcome.rating);
        }

        [Test]
        public void Judge_rejects_the_uncorrected_printed_strip()
        {
            var lvl = MakeMirrorLevel();
            var outcome = LevelJudge.Evaluate(lvl, new List<CorrectionOp>());
            Assert.IsFalse(outcome.approved);
        }

        [Test]
        public void Judge_rejects_over_budget()
        {
            var lvl = MakeMirrorLevel();
            var ops = new List<CorrectionOp>
            {
                CorrectionOp.DoReplace(2, C.TurnRight),
                CorrectionOp.DoReplace(2, C.TurnRight),
                CorrectionOp.DoReplace(2, C.TurnRight),
            };
            var outcome = LevelJudge.Evaluate(lvl, ops);
            Assert.IsFalse(outcome.approved);
            StringAssert.Contains("limit", outcome.message.ToLower());
        }

        [Test]
        public void Judge_is_deterministic()
        {
            var lvl = MakeMirrorLevel();
            var a = LevelJudge.Evaluate(lvl, lvl.referenceSolution);
            var b = LevelJudge.Evaluate(lvl, lvl.referenceSolution);
            Assert.AreEqual(a.approved, b.approved);
            Assert.AreEqual(a.rating, b.rating);
            Assert.AreEqual(a.route.finalCell, b.route.finalCell);
        }
    }
}
