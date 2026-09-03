namespace MisprintDepartment.Game.UI
{
    /// <summary>Names for the six defect-family batches (chapter field 1..6).</summary>
    public static class BatchInfo
    {
        public const int Batches = 6;
        public const int LevelsPerBatch = 2;

        static readonly string[] Names =
        {
            "Registration Error",
            "Mirror Plate",
            "Double Impression",
            "Registration Shift",
            "Faded Ink",
            "Color Pass",
        };

        public static string Name(int batch) =>
            (batch >= 1 && batch <= Names.Length) ? Names[batch - 1] : "Training";

        public static string Label(int batch) =>
            batch <= 0 ? "TRAINING" : $"BATCH {batch:00} · {Name(batch).ToUpperInvariant()}";
    }
}
