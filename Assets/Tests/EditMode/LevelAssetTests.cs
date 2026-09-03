using System.Linq;
using NUnit.Framework;
using UnityEditor;
using MisprintDepartment.Core;
using MisprintDepartment.Editor;
using MisprintDepartment.Editor.Build;
using MisprintDepartment.Game;

namespace MisprintDepartment.Tests
{
    /// <summary>
    /// Runs the Editor LevelValidator over every LevelDefinition asset in the
    /// project. As chapters are authored this test guards all of them.
    /// </summary>
    public class LevelAssetTests
    {
        [Test]
        public void All_level_assets_pass_validation()
        {
            var issues = LevelValidator.ValidateAll(out int count);
            var errors = issues.Where(i => i.isError).ToList();
            if (errors.Count > 0)
            {
                var msg = string.Join("\n", errors.Select(e =>
                    $"L{(e.level ? e.level.levelId.ToString() : "?")}: {e.message}"));
                Assert.Fail($"{errors.Count} level validation error(s) across {count} level(s):\n{msg}");
            }
            Assert.Pass($"{count} level(s) validated, no errors.");
        }

        [Test]
        public void Campaign_is_twelve_levels_in_six_batches()
        {
            var real = LevelValidator.LoadAll().Where(l => l.levelId >= 1).OrderBy(l => l.levelId).ToList();
            Assert.AreEqual(12, real.Count, "campaign should be exactly 12 levels");
            CollectionAssert.AreEquivalent(Enumerable.Range(1, 12), real.Select(l => l.levelId));
            for (int i = 0; i < real.Count; i++)
                Assert.AreEqual(i / 2 + 1, real[i].chapter, $"L{real[i].levelId} should be in batch {i / 2 + 1}");
        }

        [Test]
        public void Every_correction_tool_is_used_by_some_level()
        {
            var used = LevelValidator.LoadAll().SelectMany(l => l.allowedTools).Distinct().ToHashSet();
            foreach (CorrectionTool tool in System.Enum.GetValues(typeof(CorrectionTool)))
                Assert.IsTrue(used.Contains(tool), $"no level offers the {tool} tool");
        }

        [Test]
        public void Training_job_exists_and_flows_into_job_one()
        {
            var training = LevelValidator.LoadAll().SingleOrDefault(l => l.levelId == 0 && l.chapter == 0);
            Assert.IsNotNull(training, "expected a Training Job (levelId 0, chapter 0)");

            var outcome = LevelJudge.Evaluate(training, training.referenceSolution);
            Assert.IsTrue(outcome.approved, "training reference solution should pass");

            var db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(MisprintBuild.LevelDbPath);
            Assert.IsNotNull(db, "LevelDatabase asset missing");
            Assert.AreSame(training, db.trainingLevel, "training level not wired into the database");
            Assert.IsFalse(db.levels.Any(l => l.levelId == 0), "training must not be in the main 42 list");
            Assert.AreEqual(1, db.Next(0)?.levelId, "Training should flow into Job 001");
        }
    }
}
