using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VariableBase.Mathematics
{
    public class DecimalMathEnvironment: IMathEnvironment
    {
        public IBasicMathAlgorithm BasicMath { get; set; }

        public IPrimeAlgorithm PrimeAlgorithm { get; set; }
        
        public DecimalMathEnvironment() 
        {
            this.BasicMath = new BasicMathAlgorithm(this);
            this.PrimeAlgorithm = new SieveOfEratosthenePrimeAlgorithm(this);

            var tempKey = new List<Char>();

            for (Int32 i = 0; i < 3; i++)
            {
                Char currentChar = Convert.ToChar(i);
                tempKey.Add(currentChar);
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.Base = Decimal.MaxValue - 1;

            this.SetupMathEnvironment();
        }


        public DecimalMathEnvironment(UInt16 size)
        {
            this.BasicMath = new BasicMathAlgorithm(this);
            this.PrimeAlgorithm = new SieveOfEratosthenePrimeAlgorithm(this);

            var tempKey = new List<Char>();

            for (Decimal i = 0; i < size; i++)
            {
                Char currentChar = Convert.ToChar(i);
                tempKey.Add(currentChar);
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.Base = size;

            this.SetupMathEnvironment();
        }

        public DecimalMathEnvironment(String rawKey)
        {
            this.BasicMath = new BasicMathAlgorithm(this);
            this.PrimeAlgorithm = new SieveOfEratosthenePrimeAlgorithm(this);

            var tempKey = new List<Char>();

            foreach (Char segment in rawKey.ToCharArray())
            {
                if (!tempKey.Contains(segment))
                {
                    tempKey.Add(segment);
                }
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);

            this.Base = (Decimal)this.Key.Count;

            this.SetupMathEnvironment();
        }

        public Number GetNumber(Int32 zeros, Boolean isNegative = false)
        {
            var numberSegments = new Decimal[zeros];
            for (var i = 0; i < zeros - 1; i++)
            {
                numberSegments[i] = 0;
            }
            numberSegments[numberSegments.Length - 1] = 1;

            return new Number(this, new ReadOnlyCollection<Decimal>(numberSegments), null, isNegative);
        }

        public Number GetNumber(String[] wholeNumberSegments, Boolean isNegative = false)
        {
            return new Number(this, new ReadOnlyCollection<Decimal>(wholeNumberSegments.Select((x) => Decimal.Parse(x)).ToArray()),
                               null, isNegative);
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
                    new ReadOnlyCollection<Decimal>(wholeNumberSegments.Select((x) => this.GetIndex(x)).ToArray()),
                    new ReadOnlyCollection<Decimal>(fractionNumeratorSegments.Select((x) => this.GetIndex(x)).ToArray()),
                    new ReadOnlyCollection<Decimal>(fractionDenominatorSegments.Select((x) => this.GetIndex(x)).ToArray()),
                    isNegative);
            }
            else
            {
                return new Number(this, new ReadOnlyCollection<Decimal>(wholeNumberSegments.Select((x) => this.GetIndex(x)).ToArray()), 
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
                if (this.Base < Decimal.MaxValue- 1 && !this.Key.Contains(segment))
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
            return (Decimal)this.Key.IndexOf(arg);
        }
        
        public void SetupMathEnvironment()
        {
            var tempKeyNumber = new List<Number>();
            for (Decimal i = 0; i < this.Key.Count; i++)
            {
                tempKeyNumber.Add(new Number(this, new ReadOnlyCollection<Decimal>(new Decimal[] { i }), null, false));
            }

            this.KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);
    
            if (this.Base > 2)
            {
                this.SecondNumber = this.KeyNumber[2];
            }
            else
            {
                this.SecondNumber = this.PowerOfFirstNumber;
            }
        }

        public Number ConvertToFraction(Double numberRaw, Double numeratorNumber, Double denominatorRaw)
        {
            Number result;
            IList<Decimal> number = this.BasicMath.AsSegments(numberRaw);
            if (numeratorNumber > 0)
            {
                IList<Decimal> numerator = this.BasicMath.AsSegments(numeratorNumber);
                IList<Decimal> denominator = this.BasicMath.AsSegments(denominatorRaw);

                result = new Number(this, new ReadOnlyCollection<Decimal>(number), new ReadOnlyCollection<Decimal>(numerator), new ReadOnlyCollection<Decimal>(denominator), false);
            }
            else
            {
                result = new Number(this, new ReadOnlyCollection<Decimal>(number), default(Fraction), false);
            }
            return result;
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
                foreach (Decimal segment in this.Key)
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
            return this.Equals((DecimalMathEnvironment)other);
        }

        public static bool operator ==(DecimalMathEnvironment a, DecimalMathEnvironment b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DecimalMathEnvironment a, DecimalMathEnvironment b)
        {
            return !a.Equals(b);
        }

        public Number AsNumber(Boolean[] binary, Boolean isNegative = false)
        {
            ReadOnlyCollection<Decimal> result = this.KeyNumber[0].Segments;
            for (Double i = 0; i < (Double)binary.Length; i++)
            {
                if (binary[(Int32)i])
                {
                    ReadOnlyCollection<Decimal> currentResult = new ReadOnlyCollection<Decimal>(new Decimal[] { 1 });
                    for (Double iSq = 0; iSq < i; iSq++)
                    {
                        currentResult = this.BasicMath.Multiply(currentResult,
                            this.SecondNumber.Segments);
                    }
                    result = this.BasicMath.Add(result, currentResult);
                }
            }

            return new Number(this, result, null, isNegative);
        }

        public Boolean Equals(DecimalMathEnvironment other)
        {
            if (object.ReferenceEquals(other, default(DecimalMathEnvironment)) || this.Base != other.Base)
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
