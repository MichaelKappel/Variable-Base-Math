using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Common.Models;
using System.IO;
using Common.Interfaces;

namespace VariableBase.Mathematics
{
    public class CharMathEnvironment : IMathEnvironment<Number>
    {
        public CharMathEnvironment()
            : this((Char)65534)
        {

        }

        public CharMathEnvironment(Boolean allowDigit, Boolean allowLetter, Boolean allowLower, Boolean allowNumber, 
            Boolean allowPunctuation, Boolean allowSymbols, Boolean allowSeparators, Boolean allowSurrogates, Boolean allowControl, Boolean allowWhiteSpace) 
        {
            var tempKey = new List<Char>();
            for (UInt64 i = 0; i <= Char.MaxValue; i++)
            {
                Char currentChar = Convert.ToChar(i);
                

                if ((allowDigit || !Char.IsDigit(currentChar))
                    && (allowLetter || !Char.IsLetter(currentChar))
                    && (allowLower || !Char.IsLower(currentChar))
                    && (allowNumber || !Char.IsNumber(currentChar))
                    && (allowPunctuation || !Char.IsPunctuation(currentChar))
                    && (allowSymbols || !Char.IsSymbol(currentChar))
                    && (allowSeparators || !Char.IsSeparator(currentChar))
                    && (allowSurrogates || !Char.IsSurrogate(currentChar))
                    && (allowControl || !Char.IsControl(currentChar))
                    && (allowWhiteSpace || !Char.IsWhiteSpace(currentChar)))
                {
                    tempKey.Add(currentChar);
                }
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.Base = (Decimal)tempKey.Count;

            this.SetupMathEnvironment();
        }
        public CharMathEnvironment(Char size)
        {
            var tempKey = new List<Char>();

            for (UInt64 i = 0; i < size; i++)
            {
                Char currentChar = Convert.ToChar(i);
                tempKey.Add(currentChar);
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.Base = (Decimal)size;

            this.SetupMathEnvironment();
        }

        public CharMathEnvironment(String rawKey)
        {
            var tempKey = new List<Char>();

            foreach (Char segment in rawKey.ToCharArray())
            {
                if (!tempKey.Contains(segment))
                {
                    tempKey.Add(segment);
                }
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.Base = (UInt64)this.Key.Count;

            this.SetupMathEnvironment();
        }
        
        public Number OpenNumberFile(String folderName, String fileName)
        {
            return this.GetNumber(Number.StorageRepository.Get(folderName, fileName));
        }

        public Number GetNumber(String[] wholeNumberSegments, Boolean isNegative = false)
        {
            return new Number(this, new NumberSegments(wholeNumberSegments.Select((x) => UInt64.Parse(x)).ToArray()),
                               null, isNegative);
        }

        public Number GetNumber(NumberSegments segments, Boolean isNegative = false)
        {
            return new Number(this, segments, null, isNegative);
        }

        public Number GetNumber(Decimal rawDecimal)
        {
            //FIX: include fraction
            var resultRaw = new List<Decimal>();
            Boolean isNegative = false;
            if (rawDecimal < 0)
            {
                isNegative = true;
                rawDecimal = Math.Abs(rawDecimal);
            }

            if (rawDecimal < this.Base)
            {
                return this.KeyNumber[(Int32)rawDecimal];
            }

            if (rawDecimal == 0)
            {
                resultRaw.Add(0);
            }
            else
            {

                Decimal carryOver = rawDecimal;
                while (carryOver > 0)
                {
                    if (carryOver >= this.Base)
                    {
                        Decimal columnResultRaw = 0;
                        columnResultRaw = carryOver % this.Base;
                        resultRaw.Add(columnResultRaw);
                        carryOver = ((carryOver - columnResultRaw) / this.Base);
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

        public Number GetNumber(String wholeNumber, String fractionNumerator = null, String fractionDenominator = null, Boolean isNegative = false)
        {
            List<Char> wholeNumberSegments = wholeNumber.ToCharArray().Reverse().ToList();

            this.ValidateWholeNumber(wholeNumberSegments);

            if (!String.IsNullOrEmpty(fractionNumerator) && !String.IsNullOrEmpty(fractionDenominator))
            {
                List<Char> fractionNumeratorSegments = fractionNumerator.ToCharArray().Reverse().ToList();
                List<Char> fractionDenominatorSegments = fractionDenominator.ToCharArray().Reverse().ToList();

                this.ValidateFraction(fractionNumeratorSegments, fractionDenominatorSegments);
                return new Number(this, 
                    new NumberSegments(wholeNumberSegments.Select(x => (UInt16)this.Key.IndexOf(x)).ToArray()),
                    new NumberSegments(fractionNumeratorSegments.Select(x => (UInt16)this.Key.IndexOf(x)).ToArray()),
                    new NumberSegments(fractionDenominatorSegments.Select(x => (UInt16)this.Key.IndexOf(x)).ToArray()),
                    isNegative);
            }
            else
            {
                return new Number(this, new NumberSegments(wholeNumberSegments.Select((x) => (UInt16)this.Key.IndexOf(x)).ToArray()), 
                    null, isNegative);
            }
        }

        public void ValidateWholeNumber(List<Char> numberSegments)
        {
            while (numberSegments.Count > 1 && numberSegments[numberSegments.Count - 1] == this.Key[0])
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


            foreach (Char segment in numberSegments)
            {
                if (this.Base < UInt64.MaxValue - 1 && !this.Key.Contains(segment))
                {
                    throw new Exception(String.Format("Invalid Number {0} not found in {1}", segment, this));
                }
            }
        }

        public void ValidateFraction(List<Char> numerator, List<Char> denominator)
        {
            this.ValidateWholeNumber(numerator);
            this.ValidateWholeNumber(denominator);

            if (numerator == null || numerator.Count == 0 || numerator.Count == 1 && numerator[0] == 0)
            {
                throw new DivideByZeroException("Numerator of nothing not currently supported");
            }

            if (denominator == null || denominator.Count == 0 || denominator.Count == 1 && denominator[0] == 0)
            {
                throw new DivideByZeroException("Denominator of nothing not currently supported");
            }
        }

        public Decimal GetIndex(Char arg)
        {
            return (UInt16)this.Key.IndexOf(arg);
        }

        public void SetupMathEnvironment()
        {
            var tempKeyNumber = new List<Number>();
            for (UInt16 i = 0; i < this.Key.Count; i++)
            {
                tempKeyNumber.Add(new Number(this, new NumberSegments(new UInt16[] { i }), null, false));
            }

            this.KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);
            PowerOfFirstNumber = new Number(this, new NumberSegments(new UInt16[] { 0, 1 }), null, false);
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

        public ReadOnlyCollection<Char> Key
        {
            get;
            protected set;
        }

        public Decimal Base
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

            String result = String.Empty;
            if (this.Base > 500)
            {
                result = String.Format("B {0}", this.Base);
            }
            else
            {
                foreach (UInt64 segment in this.Key)
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
                int hashCode = this.Key.GetHashCode();
                return hashCode;
            }
        }

        public override Boolean Equals(Object other)
        {
            return this.Equals((IMathEnvironment<Number>)other);
        }

        public static bool operator ==(CharMathEnvironment a, IMathEnvironment<Number> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CharMathEnvironment a, IMathEnvironment<Number> b)
        {
            return !a.Equals(b);
        }

        public String GetDefinition()
        {
            return String.Concat(this.Key);
        }

        public NumberSegments ParseNumberSegments(String raw)
        {
           return new NumberSegments(raw.ToCharArray().Select(x2 => this.GetIndex(x2)).Reverse().ToArray());
        }

        public String ConvertToString(NumberSegments segments)
        {
            return String.Concat(segments.Select(x => this.Key[(Int32)x]).Reverse());
        }

        public Boolean Equals(IMathEnvironment<Number> other)
        {
            if (object.ReferenceEquals(other, default(IMathEnvironment<Number>)) || this.Base != other.Base)
            {
                return false;
            }

            if (this.Base != other.Base)
            {
                return false;
            }

            if (this.Base != other.Base)
            {
                return false;
            }
            return true;
        }
    }
}
