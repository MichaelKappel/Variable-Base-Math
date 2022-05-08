using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NS12.VariableBase.Mathematics.Common.Models;
using System.IO;
using NS12.VariableBase.Mathematics.Providers;
using NS12.VariableBase.Mathematics.Common.Interfaces;

namespace NS12.VariableBase.Mathematics.Providers.MathEnvironments
{
    public class CharMathEnvironment : IMathEnvironment<Number>
    {
        public CharMathEnvironment()
            : this((char)65534)
        {

        }

        public CharMathEnvironment(bool allowDigit, bool allowLetter, bool allowLower, bool allowNumber,
            bool allowPunctuation, bool allowSymbols, bool allowSeparators, bool allowSurrogates, bool allowControl, bool allowWhiteSpace)
        {
            var tempKey = new List<char>();
            for (ulong i = 0; i <= char.MaxValue; i++)
            {
                char currentChar = Convert.ToChar(i);


                if ((allowDigit || !char.IsDigit(currentChar))
                    && (allowLetter || !char.IsLetter(currentChar))
                    && (allowLower || !char.IsLower(currentChar))
                    && (allowNumber || !char.IsNumber(currentChar))
                    && (allowPunctuation || !char.IsPunctuation(currentChar))
                    && (allowSymbols || !char.IsSymbol(currentChar))
                    && (allowSeparators || !char.IsSeparator(currentChar))
                    && (allowSurrogates || !char.IsSurrogate(currentChar))
                    && (allowControl || !char.IsControl(currentChar))
                    && (allowWhiteSpace || !char.IsWhiteSpace(currentChar)))
                {
                    tempKey.Add(currentChar);
                }
            }

            Key = new ReadOnlyCollection<char>(tempKey);

            Base = tempKey.Count;

            SetupMathEnvironment();
        }
        public CharMathEnvironment(char size)
        {
            var tempKey = new List<char>();

            for (ulong i = 0; i < size; i++)
            {
                char currentChar = Convert.ToChar(i);
                tempKey.Add(currentChar);
            }

            Key = new ReadOnlyCollection<char>(tempKey);

            Base = size;

            SetupMathEnvironment();
        }

        public CharMathEnvironment(string rawKey)
        {
            var tempKey = new List<char>();

            foreach (char segment in rawKey.ToCharArray())
            {
                if (!tempKey.Contains(segment))
                {
                    tempKey.Add(segment);
                }
            }

            Key = new ReadOnlyCollection<char>(tempKey);

            Base = (ulong)Key.Count;

            SetupMathEnvironment();
        }

        //public Number OpenNumberFile(string folderName, string fileName)
        //{
        //    return this.GetNumber(Number.StorageRepository.Get(folderName, fileName));
        //}

        public Number GetNumber(string[] wholeNumberSegments, bool isNegative = false)
        {
            return new Number(this, new NumberSegments(wholeNumberSegments.Select((x) => ulong.Parse(x)).ToArray()),
                               null, isNegative);
        }

        public Number GetNumber(NumberSegments segments, bool isNegative = false)
        {
            return new Number(this, segments, null, isNegative);
        }

        public Number GetNumber(decimal rawDecimal)
        {
            //FIX: include fraction
            var resultRaw = new List<decimal>();
            bool isNegative = false;
            if (rawDecimal < 0)
            {
                isNegative = true;
                rawDecimal = Math.Abs(rawDecimal);
            }

            if (rawDecimal < Base)
            {
                return KeyNumber[(int)rawDecimal];
            }

            if (rawDecimal == 0)
            {
                resultRaw.Add(0);
            }
            else
            {

                decimal carryOver = rawDecimal;
                while (carryOver > 0)
                {
                    if (carryOver >= Base)
                    {
                        decimal columnResultRaw = 0;
                        columnResultRaw = carryOver % Base;
                        resultRaw.Add(columnResultRaw);
                        carryOver = (carryOver - columnResultRaw) / Base;
                    }
                    else
                    {
                        resultRaw.Add(carryOver);
                        carryOver = 0;
                    }
                }
            }

            return new Number(this, new NumberSegments(resultRaw), null, isNegative);
        }

        public Number GetNumber(string wholeNumber, string fractionNumerator = null, string fractionDenominator = null, bool isNegative = false)
        {
            List<char> wholeNumberSegments = wholeNumber.ToCharArray().Reverse().ToList();

            ValidateWholeNumber(wholeNumberSegments);

            if (!string.IsNullOrEmpty(fractionNumerator) && !string.IsNullOrEmpty(fractionDenominator))
            {
                List<char> fractionNumeratorSegments = fractionNumerator.ToCharArray().Reverse().ToList();
                List<char> fractionDenominatorSegments = fractionDenominator.ToCharArray().Reverse().ToList();

                ValidateFraction(fractionNumeratorSegments, fractionDenominatorSegments);
                return new Number(this,
                    new NumberSegments(wholeNumberSegments.Select(x => (ushort)Key.IndexOf(x)).ToArray()),
                    new NumberSegments(fractionNumeratorSegments.Select(x => (ushort)Key.IndexOf(x)).ToArray()),
                    new NumberSegments(fractionDenominatorSegments.Select(x => (ushort)Key.IndexOf(x)).ToArray()),
                    isNegative);
            }
            else
            {
                return new Number(this, new NumberSegments(wholeNumberSegments.Select((x) => (ushort)Key.IndexOf(x)).ToArray()),
                    default, isNegative);
            }
        }

        public void ValidateWholeNumber(List<char> numberSegments)
        {
            while (numberSegments.Count > 1 && numberSegments[numberSegments.Count - 1] == Key[0])
            {
                numberSegments.RemoveAt(numberSegments.Count - 1);
            }

            if (numberSegments[numberSegments.Count - 1] == '\u202c' || numberSegments[numberSegments.Count - 1] == 8237)
            {
                numberSegments.RemoveAt(numberSegments.Count - 1);
            }

            if (numberSegments[0] == '\u202c' || numberSegments[0] == 8237)
            {
                numberSegments.RemoveAt(0);
            }


            foreach (char segment in numberSegments)
            {
                if (Base < ulong.MaxValue - 1 && !Key.Contains(segment))
                {
                    throw new Exception(string.Format("Invalid Number {0} not found in {1}", segment, this));
                }
            }
        }

        public void ValidateFraction(List<char> numerator, List<char> denominator)
        {
            ValidateWholeNumber(numerator);
            ValidateWholeNumber(denominator);

            if (numerator == null || numerator.Count == 0 || numerator.Count == 1 && numerator[0] == 0)
            {
                throw new DivideByZeroException("Numerator of nothing not currently supported");
            }

            if (denominator == null || denominator.Count == 0 || denominator.Count == 1 && denominator[0] == 0)
            {
                throw new DivideByZeroException("Denominator of nothing not currently supported");
            }
        }

        public decimal GetIndex(char arg)
        {
            return (ushort)Key.IndexOf(arg);
        }

        public void SetupMathEnvironment()
        {
            var tempKeyNumber = new List<Number>();
            for (ushort i = 0; i < Key.Count; i++)
            {
                tempKeyNumber.Add(new Number(this, new NumberSegments(new ushort[] { i }), null, false));
            }

            KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);
            PowerOfFirstNumber = new Number(this, new NumberSegments(new ushort[] { 0, 1 }), null, false);
        }

        public Number PowerOfFirstNumber
        {
            get;
            protected set;
        }

        public Number SecondNumber
        {
            get;
            protected set;
        }

        public ReadOnlyCollection<char> Key
        {
            get;
            protected set;
        }

        public decimal Base
        {
            get;
            private set;
        }

        public ReadOnlyCollection<Number> KeyNumber
        {
            get;
            protected set;
        }

        public override string ToString()
        {

            string result = string.Empty;
            if (Base > 500)
            {
                result = string.Format("B {0}", Base);
            }
            else
            {
                foreach (ulong segment in Key)
                {
                    if (result != null)
                    {
                        result += '|';
                    }
                    result += segment;
                }
            }
            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Key.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object other)
        {
            return Equals((IMathEnvironment<Number>)other);
        }

        public static bool operator ==(CharMathEnvironment a, IMathEnvironment<Number> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CharMathEnvironment a, IMathEnvironment<Number> b)
        {
            return !a.Equals(b);
        }

        public string GetDefinition()
        {
            return string.Concat(Key);
        }

        public NumberSegments ParseNumberSegments(string raw)
        {
            return new NumberSegments(raw.ToCharArray().Select(x2 => GetIndex(x2)).Reverse().ToArray());
        }

        public string ConvertToString(NumberSegments segments)
        {
            return string.Concat(segments.Select(x => Key[(int)x]).Reverse());
        }

        public bool Equals(IMathEnvironment<Number> other)
        {
            if (ReferenceEquals(other, default(IMathEnvironment<Number>)) || Base != other.Base)
            {
                return false;
            }

            if (Base != other.Base)
            {
                return false;
            }

            if (Base != other.Base)
            {
                return false;
            }
            return true;
        }
    }
}
