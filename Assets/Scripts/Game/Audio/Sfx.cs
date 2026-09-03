namespace MisprintDepartment.Game
{
    /// <summary>Identifiers for every sound effect the game plays.</summary>
    public enum Sfx
    {
        None = 0,
        UiSelect,
        MechButton,
        CorrectionRotate,
        CorrectionReplace,
        PrinterStart,
        RollerFeed,
        ProofLever,
        RouteStep,
        RouteTurn,
        ProofApproved,
        ProofRejected,
        TicketMove,
        StampMark,
    }

    public enum SfxBus { Sfx, Ui }
}
