using System;
using System.Collections.Generic;
using System.Linq;
using NS12.VariableBase.Mathematics.Providers;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Utilities
{
    public sealed class TextRadixCipherResult
    {
        public TextRadixCipherResult(
            string messageText,
            string secretText,
            string cipherText,
            string radixKey,
            int radix,
            string escapedMessageText,
            string escapedSecretText,
            string escapedCipherText,
            string escapedRadixKey,
            double estimatedKeySpaceBits,
            string strengthLabel,
            string strengthSummary)
        {
            MessageText = messageText;
            SecretText = secretText;
            CipherText = cipherText;
            RadixKey = radixKey;
            Radix = radix;
            EscapedMessageText = escapedMessageText;
            EscapedSecretText = escapedSecretText;
            EscapedCipherText = escapedCipherText;
            EscapedRadixKey = escapedRadixKey;
            EstimatedKeySpaceBits = estimatedKeySpaceBits;
            StrengthLabel = strengthLabel;
            StrengthSummary = strengthSummary;
        }

        public string MessageText { get; }
        public string SecretText { get; }
        public string CipherText { get; }
        public string RadixKey { get; }
        public int Radix { get; }
        public string EscapedMessageText { get; }
        public string EscapedSecretText { get; }
        public string EscapedCipherText { get; }
        public string EscapedRadixKey { get; }
        public double EstimatedKeySpaceBits { get; }
        public string StrengthLabel { get; }
        public string StrengthSummary { get; }
    }

    public static class TextRadixCipher
    {
        private const string PreferredZeroSymbols = "~|^_+=:;,.?@#$%&*()[]{}<>!/\\-`'\"";

        public static TextRadixCipherResult Encrypt(string message, string secret)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message is required.", nameof(message));
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Secret is required.", nameof(secret));
            }

            string radixKey = BuildRadixKey(message, secret);
            CharMathEnvironment environment = new CharMathEnvironment(radixKey);

            Number messageNumber = environment.GetNumber(message);
            Number secretNumber = environment.GetNumber(secret);
            Number cipherNumber = Number.Reduce(messageNumber * secretNumber);

            if (cipherNumber.Fragment != null)
            {
                throw new InvalidOperationException("Cipher generation unexpectedly produced a fraction.");
            }

            string cipherText = environment.ConvertToString(cipherNumber.Whole);
            return BuildResult(message, secret, cipherText, radixKey);
        }

        public static TextRadixCipherResult Decrypt(string cipherText, string secret, string radixKey, int? radix = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentException("Encrypted text is required.", nameof(cipherText));
            }

            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Secret is required.", nameof(secret));
            }

            ValidateRadixKey(radixKey, radix);

            CharMathEnvironment environment = new CharMathEnvironment(radixKey);
            Number cipherNumber = environment.GetNumber(cipherText);
            Number secretNumber = environment.GetNumber(secret);

            Number messageNumber = Number.Reduce(cipherNumber / secretNumber);
            if (messageNumber.Fragment != null)
            {
                throw new InvalidOperationException("The encrypted text does not divide cleanly by the supplied secret in this radix. Check the secret, radix key, and base.");
            }

            string messageText = environment.ConvertToString(messageNumber.Whole);
            return BuildResult(messageText, secret, cipherText, radixKey);
        }

        private static TextRadixCipherResult BuildResult(string message, string secret, string cipherText, string radixKey)
        {
            int radix = radixKey.Length;
            double estimatedBits = EstimateKeySpaceBits(secret.Length, radix);
            string strengthLabel = ClassifyStrength(estimatedBits);
            string strengthSummary = string.Format(
                "Approximate secret search space: {0}^{1} possibilities, about {2:F1} bits of search effort before factoring in the known scheme.",
                radix - 1,
                secret.Length,
                estimatedBits);

            return new TextRadixCipherResult(
                message,
                secret,
                cipherText,
                radixKey,
                radix,
                EscapeForDisplay(message),
                EscapeForDisplay(secret),
                EscapeForDisplay(cipherText),
                EscapeForDisplay(radixKey),
                estimatedBits,
                strengthLabel,
                strengthSummary);
        }

        private static void ValidateRadixKey(string radixKey, int? radix)
        {
            if (string.IsNullOrEmpty(radixKey))
            {
                throw new ArgumentException("Radix key is required.", nameof(radixKey));
            }

            if (radixKey.Distinct().Count() != radixKey.Length)
            {
                throw new ArgumentException("Radix key must contain unique characters in a stable order.", nameof(radixKey));
            }

            if (radixKey.Length < 2)
            {
                throw new ArgumentException("Radix key must define at least two symbols.", nameof(radixKey));
            }

            if (radix.HasValue && radix.Value != radixKey.Length)
            {
                throw new ArgumentException(string.Format("Base/radix mismatch: the supplied radix was {0}, but the radix key defines {1} symbols.", radix.Value, radixKey.Length), nameof(radix));
            }
        }

        private static string BuildRadixKey(string message, string secret)
        {
            HashSet<char> seen = new HashSet<char>();
            List<char> ordered = new List<char>();

            foreach (char current in message.Concat(secret))
            {
                if (seen.Add(current))
                {
                    ordered.Add(current);
                }
            }

            char zeroSymbol = FindZeroSymbol(seen);
            ordered.Insert(0, zeroSymbol);
            return new string(ordered.ToArray());
        }

        private static char FindZeroSymbol(HashSet<char> used)
        {
            foreach (char candidate in PreferredZeroSymbols)
            {
                if (!used.Contains(candidate))
                {
                    return candidate;
                }
            }

            for (int codePoint = 33; codePoint <= char.MaxValue; codePoint++)
            {
                char candidate = (char)codePoint;
                if (!char.IsSurrogate(candidate) && !used.Contains(candidate))
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException("Unable to allocate a zero symbol for this radix key.");
        }

        private static double EstimateKeySpaceBits(int secretLength, int radix)
        {
            int availableSecretSymbols = Math.Max(2, radix - 1);
            return secretLength * Math.Log(availableSecretSymbols, 2);
        }

        private static string ClassifyStrength(double estimatedBits)
        {
            if (estimatedBits < 24)
            {
                return "Very weak";
            }

            if (estimatedBits < 48)
            {
                return "Weak";
            }

            if (estimatedBits < 80)
            {
                return "Low";
            }

            if (estimatedBits < 128)
            {
                return "Moderate";
            }

            return "High for obfuscation";
        }

        private static string EscapeForDisplay(string value)
        {
            return string.Concat(value.Select(current => string.Format("\\u{0:X4}", (int)current)));
        }
    }
}
