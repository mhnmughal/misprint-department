using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MisprintDepartment.Core;
using MisprintDepartment.Game;
using MisprintDepartment.Editor;

namespace MisprintDepartment.Tests.PlayMode
{
    /// <summary>
    /// Drives the real LevelController through each Chapter 1 job with its
    /// reference solution and asserts the proof is approved at Master Proof.
    /// This is the automated stand-in for the manual Chapter 1 QA pass.
    /// </summary>
    public class Chapter1FlowTests
    {
        static LevelDefinition[] Chapter1() =>
            LevelValidator.LoadAll().Where(l => l.chapter == 1).OrderBy(l => l.levelId).ToArray();

        [UnityTest]
        public IEnumerator Every_chapter1_job_is_approved_by_its_reference_solution()
        {
            foreach (var level in Chapter1())
            {
                GameSession.PendingLevel = level;
                var go = new GameObject("LevelController_" + level.levelId);
                var lc = go.AddComponent<LevelController>();
                yield return null; // let Start() run Load()
                Assert.AreEqual(level.levelId, lc.Level.levelId);

                foreach (var op in level.referenceSolution)
                {
                    bool ok = lc.TryApply(op, out string err);
                    Assert.IsTrue(ok, $"L{level.levelId}: correction {op} rejected: {err}");
                }

                lc.RunProof();
                float timeout = Time.realtimeSinceStartup + 15f;
                while (lc.ProofRunning && Time.realtimeSinceStartup < timeout)
                    yield return null;

                Assert.IsFalse(lc.ProofRunning, $"L{level.levelId}: proof never finished");
                Assert.IsNotNull(lc.LastOutcome, $"L{level.levelId}: no outcome");
                Assert.IsTrue(lc.LastOutcome.approved,
                    $"L{level.levelId} '{level.displayTitle}': {lc.LastOutcome.message}");
                Assert.AreEqual(ProofRating.MasterProof, lc.LastOutcome.rating,
                    $"L{level.levelId}: expected Master Proof, got {lc.LastOutcome.rating}");

                Object.Destroy(go);
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Uncorrected_job_is_rejected()
        {
            var level = Chapter1().First();
            GameSession.PendingLevel = level;
            var go = new GameObject("LC");
            var lc = go.AddComponent<LevelController>();
            yield return null;

            lc.RunProof();
            float timeout = Time.realtimeSinceStartup + 15f;
            while (lc.ProofRunning && Time.realtimeSinceStartup < timeout)
                yield return null;

            Assert.IsFalse(lc.LastOutcome.approved, "printed strip should not pass uncorrected");
            Object.Destroy(go);
        }
    }
}
