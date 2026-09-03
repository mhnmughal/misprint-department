using System;

namespace MisprintDepartment.Core
{
    /// <summary>Ink-pass / print modifiers a token can carry (Chapters 5-6).</summary>
    [Flags]
    public enum InkFlags
    {
        None = 0,
        Faded = 1 << 0,     // unreadable — needs Restore
        Marked = 1 << 1,    // marked ink — per-level color-pass rule
        Outlined = 1 << 2,  // outlined command
        Striped = 1 << 3,   // striped command
    }

    /// <summary>One slot on the command strip: a command plus print modifiers.</summary>
    [Serializable]
    public struct CommandToken : IEquatable<CommandToken>
    {
        public PrintCommand command;
        public InkFlags flags;

        public CommandToken(PrintCommand command, InkFlags flags = InkFlags.None)
        {
            this.command = command;
            this.flags = flags;
        }

        public bool Has(InkFlags f) => (flags & f) == f;
        public CommandToken With(InkFlags f) => new CommandToken(command, flags | f);
        public CommandToken Without(InkFlags f) => new CommandToken(command, flags & ~f);
        public CommandToken AsCommand(PrintCommand c) => new CommandToken(c, flags);

        public bool Equals(CommandToken other) => command == other.command && flags == other.flags;
        public override bool Equals(object obj) => obj is CommandToken t && Equals(t);
        public override int GetHashCode() => ((int)command * 397) ^ (int)flags;
        public override string ToString() =>
            flags == InkFlags.None ? command.ToString() : $"{command}[{flags}]";
    }
}
