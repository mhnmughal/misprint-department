using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MisprintDepartment.Core
{
    /// <summary>Ordered list of command tokens. Cloneable for value semantics.</summary>
    [Serializable]
    public sealed class CommandStrip : IEnumerable<CommandToken>
    {
        public List<CommandToken> tokens = new List<CommandToken>();

        public CommandStrip() { }
        public CommandStrip(IEnumerable<CommandToken> source) { tokens = new List<CommandToken>(source); }
        public CommandStrip(IEnumerable<PrintCommand> source)
        {
            tokens = source.Select(c => new CommandToken(c)).ToList();
        }

        public int Count => tokens.Count;
        public CommandToken this[int i] { get => tokens[i]; set => tokens[i] = value; }

        public CommandStrip Clone() => new CommandStrip(tokens);

        public bool ContentEquals(CommandStrip other)
        {
            if (other == null || other.Count != Count) return false;
            for (int i = 0; i < Count; i++)
                if (!tokens[i].Equals(other.tokens[i])) return false;
            return true;
        }

        /// <summary>Compare only commands, ignoring ink flags.</summary>
        public bool CommandsEqual(CommandStrip other)
        {
            if (other == null || other.Count != Count) return false;
            for (int i = 0; i < Count; i++)
                if (tokens[i].command != other.tokens[i].command) return false;
            return true;
        }

        public IEnumerator<CommandToken> GetEnumerator() => tokens.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var sb = new StringBuilder("[ ");
            for (int i = 0; i < tokens.Count; i++)
            {
                sb.Append(tokens[i]);
                if (i < tokens.Count - 1) sb.Append(" | ");
            }
            sb.Append(" ]");
            return sb.ToString();
        }

        public static CommandStrip Of(params PrintCommand[] commands) => new CommandStrip(commands);
    }
}
