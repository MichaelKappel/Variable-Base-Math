using System;
using System.Text;

namespace NS12.VariableBase.Mathematics.Providers.Utilities
{
    public static class UnicodeRadixKeyProvider
    {
        public const int Radix63404 = 63404;

        private static readonly Lazy<string> _radix63404Key = new Lazy<string>(BuildRadix63404Key);

        public static string Radix63404Key => _radix63404Key.Value;

        public static bool IsRadix63404Candidate(char value)
        {
            return !char.IsWhiteSpace(value)
                && !char.IsControl(value)
                && !char.IsSurrogate(value);
        }

        private static string BuildRadix63404Key()
        {
            var builder = new StringBuilder(Radix63404);
            for (int codePoint = 0; codePoint <= char.MaxValue; codePoint++)
            {
                char current = (char)codePoint;
                if (IsRadix63404Candidate(current))
                {
                    builder.Append(current);
                }
            }

            if (builder.Length != Radix63404)
            {
                throw new InvalidOperationException($"Expected {Radix63404} radix-63404 digits but found {builder.Length}.");
            }

            return builder.ToString();
        }
    }
}
