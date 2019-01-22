using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics
{
    public class MathEnvironment: IEquatable<MathEnvironment>, IMathEnvironment
    {
        public MathAlgorithm Algorithm;
        
        public MathEnvironment() 
        {
            this.Algorithm = new MathAlgorithm(this);

            var tempKey = new List<Char>();

            for (UInt16 i = 0; i < UInt16.MaxValue - 1; i++)
            {
                Char currentChar = Convert.ToChar(i);
                tempKey.Add(currentChar);
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.SetupMathEnvironment();
        }

        public MathEnvironment(String rawKey)
        {
            this.Algorithm = new MathAlgorithm(this);

            var tempKey = new List<Char>();

            foreach (Char segment in rawKey.ToCharArray())
            {
                if (!tempKey.Contains(segment))
                {
                    tempKey.Add(segment);
                }
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.SetupMathEnvironment();
        }

        public Number GetNumber(Int64 zeros, Boolean isNegative = false)
        {
            var numberSegments = new UInt16[zeros];
            for (var i = 0; i < zeros - 1; i++)
            {
                numberSegments[i] = 0;
            }
            numberSegments[numberSegments.Length - 1] = 1;

            return new Number(this, new ReadOnlyCollection<UInt16>(numberSegments), null, isNegative);
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
                    new ReadOnlyCollection<UInt16>(wholeNumberSegments.Select((x) => this.GetIndex(x)).ToArray()),
                    new ReadOnlyCollection<UInt16>(fractionNumeratorSegments.Select((x) => this.GetIndex(x)).ToArray()),
                    new ReadOnlyCollection<UInt16>(fractionDenominatorSegments.Select((x) => this.GetIndex(x)).ToArray()),
                    isNegative);
            }
            else
            {
                return new Number(this, new ReadOnlyCollection<UInt16>(wholeNumberSegments.Select((x) => this.GetIndex(x)).ToArray()), 
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
                if (!this.Key.Contains(segment))
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

        public UInt16 GetIndex(Char arg)
        {
            return (UInt16)this.Key.IndexOf(arg);
        }
        
        public void SetupMathEnvironment()
        {
            var tempKeyNumber = new List<Number>();
            for (UInt16 i = 0; i < this.Key.Count; i++)
            {
                tempKeyNumber.Add(new Number(this, new ReadOnlyCollection<UInt16>(new UInt16[] { i }), null, false));
            }

            this.KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);
    
            if (this.Key.Count > 2)
            {
                this.SecondNumber = this.KeyNumber[2];
            }
            else
            {
                this.SecondNumber = this.PowerOfFirstNumber;
            }
        }

        public Number ConvertToFraction(UInt16 numberRaw, UInt16 numeratorNumber, UInt16 denominatorRaw)
        {
            Number result;
            List <UInt16> number = this.AsSegments(numberRaw);
            if (numeratorNumber > 0)
            {
                List<UInt16> numerator = this.AsSegments(numeratorNumber);
                List<UInt16> denominator = this.AsSegments(denominatorRaw);

                result = new Number(this, new ReadOnlyCollection<UInt16>(number), new ReadOnlyCollection<UInt16>(numerator), new ReadOnlyCollection<UInt16>(denominator), false);
            }
            else
            {
                result = new Number(this, new ReadOnlyCollection<UInt16>(number), default(Fraction), false);
            }
            return result;
        }

        public List<UInt16> AsSegments(UInt64 rawUInt64)
        {
            return this.Algorithm.AsSegments(rawUInt64);
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
        
        public ReadOnlyCollection<Number> KeyNumber
        {
            get;
            protected set;
        }

        public override string ToString()
        {

            String result = String.Empty;
            if (this.Key.Count > 500)
            {
                result = String.Format("B {0}", this.Key.Count);
            }
            else
            {
                foreach (UInt16 segment in this.Key)
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
            return this.Equals((MathEnvironment)other);
        }

        public static bool operator ==(MathEnvironment a, MathEnvironment b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MathEnvironment a, MathEnvironment b)
        {
            return !a.Equals(b);
        }

        public Number AsNumber(Boolean[] binary, Boolean isNegative = false)
        {
            ReadOnlyCollection<UInt16> result = this.KeyNumber[0].Segments;
            for (UInt64 i = 0; i < (UInt64)binary.Length; i++)
            {
                if (binary[i])
                {
                    ReadOnlyCollection<UInt16> currentResult = new ReadOnlyCollection<UInt16>(new UInt16[] { 1 });
                    for (UInt64 iSq = 0; iSq < i; iSq++)
                    {
                        currentResult = this.Algorithm.Multiply(currentResult,
                            this.SecondNumber.Segments);
                    }
                    result = this.Algorithm.Add(result, currentResult);
                }
            }

            return new Number(this, result, null, isNegative);
        }

        public Boolean Equals(MathEnvironment other)
        {
            if (object.ReferenceEquals(other, default(MathEnvironment)) || this.Key.Count != other.Key.Count)
            {
                return false;
            }

            if (this.Key.Count != other.Key.Count)
            {
                return false;
            }

            for (var i=0; i < this.Key.Count; i++)
            {
                if (this.Key[i] != other.Key[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
