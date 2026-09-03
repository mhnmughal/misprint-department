using System.IO;
using NUnit.Framework;
using MisprintDepartment.Core;
using MisprintDepartment.Game;

namespace MisprintDepartment.Tests
{
    public class SaveTests
    {
        string _dir;
        string _path;

        [SetUp]
        public void SetUp()
        {
            _dir = Path.Combine(Path.GetTempPath(), "misprint_save_tests", System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            _path = Path.Combine(_dir, "save.json");
        }

        [TearDown]
        public void TearDown()
        {
            try { Directory.Delete(_dir, true); } catch { }
        }

        [Test]
        public void First_launch_gives_default_data()
        {
            var s = new SaveService(_path);
            s.Load();
            Assert.AreEqual(1, s.Data.highestUnlockedLevel);
            Assert.AreEqual(0, s.Data.levels.Count);
        }

        [Test]
        public void Save_then_reload_round_trips()
        {
            var s = new SaveService(_path);
            s.Load();
            s.Data.RecordResult(1, true, ProofRating.CleanProof, 2, SaveService.TotalLevels);
            s.Save();

            var s2 = new SaveService(_path);
            s2.Load();
            Assert.AreEqual(2, s2.Data.highestUnlockedLevel);
            var rec = s2.Data.Find(1);
            Assert.IsNotNull(rec);
            Assert.IsTrue(rec.completed);
            Assert.AreEqual((int)ProofRating.CleanProof, rec.rating);
            Assert.AreEqual(2, rec.bestCorrections);
        }

        [Test]
        public void RecordResult_only_improves_rating_and_best()
        {
            var d = new SaveData();
            d.RecordResult(1, true, ProofRating.Verified, 5, 42);
            d.RecordResult(1, true, ProofRating.MasterProof, 2, 42);
            d.RecordResult(1, true, ProofRating.CleanProof, 8, 42); // worse — ignored
            var rec = d.Find(1);
            Assert.AreEqual((int)ProofRating.MasterProof, rec.rating);
            Assert.AreEqual(2, rec.bestCorrections);
        }

        [Test]
        public void Unlock_never_exceeds_total_levels()
        {
            var d = new SaveData();
            d.RecordResult(42, true, ProofRating.Verified, 1, 42);
            Assert.AreEqual(42, d.highestUnlockedLevel);
        }

        [Test]
        public void Corrupt_json_falls_back_to_default_and_backs_up()
        {
            File.WriteAllText(_path, "{ this is not json ]");
            var s = new SaveService(_path);
            s.Load();
            Assert.AreEqual(1, s.Data.highestUnlockedLevel);
            Assert.IsTrue(File.Exists(_path + ".bak"), "bad file should be backed up");
        }

        [Test]
        public void Empty_file_falls_back_to_default()
        {
            File.WriteAllText(_path, "");
            var s = new SaveService(_path);
            s.Load();
            Assert.AreEqual(1, s.Data.highestUnlockedLevel);
        }

        [Test]
        public void Unknown_future_version_is_rejected()
        {
            var data = SaveService.Deserialize("{\"version\":999,\"highestUnlockedLevel\":40}", out string problem);
            Assert.IsNotNull(problem);
            Assert.AreEqual(1, data.highestUnlockedLevel);
        }

        [Test]
        public void Hand_edited_out_of_range_values_are_clamped()
        {
            var data = SaveService.Deserialize(
                "{\"version\":1,\"highestUnlockedLevel\":9999,\"musicVolume\":5.0}", out string problem);
            Assert.IsNull(problem);
            Assert.AreEqual(SaveService.TotalLevels, data.highestUnlockedLevel);
            Assert.AreEqual(1f, data.musicVolume);
        }

        [Test]
        public void ResetProgress_clears_levels_but_keeps_settings()
        {
            var s = new SaveService(_path);
            s.Load();
            s.Data.musicVolume = 0.25f;
            s.Data.RecordResult(1, true, ProofRating.Verified, 1, 42);
            s.Save();

            s.ResetProgress();
            Assert.AreEqual(1, s.Data.highestUnlockedLevel);
            Assert.AreEqual(0, s.Data.levels.Count);
            Assert.AreEqual(0.25f, s.Data.musicVolume);
        }
    }
}
