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
            Algorithm = new MathAlgorithm(this);

            var tempKey = new List<Char>();
            var tempKeyNumber = new List<Number>();

            for (var i = 0; i < Char.MaxValue; i++)
            {
                Char currentChar = Convert.ToChar(i);
                tempKey.Add(currentChar);
                tempKeyNumber.Add(new Number(this, new ReadOnlyCollection<Char>(new Char[] { currentChar }), null, false));
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);
            this.KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);

            this.SetupMathEnvironment();
        }

        public MathEnvironment(String rawKey)
        {
            Algorithm = new MathAlgorithm(this);
            this.SetKey(rawKey);
        }
        public Number GetNumber(List<Char> wholeNumberSegments)
        {
            this.ValidateWholeNumber(wholeNumberSegments);
            return new Number(this, new ReadOnlyCollection<Char>(wholeNumberSegments), null, false);
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
                return new Number(this, new ReadOnlyCollection<Char>(wholeNumberSegments), new ReadOnlyCollection<Char>(fractionNumeratorSegments), new ReadOnlyCollection<Char>(fractionDenominatorSegments), isNegative);
            }
            else
            {
                return new Number(this, new ReadOnlyCollection<Char>(wholeNumberSegments), null, isNegative);
            }
        }

        public void ValidateWholeNumber(List<Char> numberSegments)
        {
            while (numberSegments.Count > 1 && numberSegments[numberSegments.Count - 1] == this.Bottom)
            {
                numberSegments.RemoveAt(numberSegments.Count - 1);
            }

            if (numberSegments[0] == '\u202c' || numberSegments[0] == 8237)
            {
                numberSegments.RemoveAt(0);
            }

            if (numberSegments[numberSegments.Count - 1] == '\u202c' || numberSegments[numberSegments.Count - 1] == 8237)
            {
                numberSegments.RemoveAt(numberSegments.Count - 1);
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

            if (numerator == null || numerator.Count == 0 || numerator.Count == 1 && numerator[0] == this.Bottom)
            {
                throw new DivideByZeroException("Numerator of nothing not currently supported");
            }

            if (denominator == null || denominator.Count == 0 || denominator.Count == 1 && denominator[0] == this.Bottom)
            {
                throw new DivideByZeroException("Denominator of nothing not currently supported");
            }
          

        }

        public UInt16 GetIndex(Char arg)
        {
            return (UInt16)this.Key.IndexOf(arg);
        }

        public void SetKey(String rawKey)
        {
           var tempKey = new List<Char>();
            var tempKeyNumber = new List<Number>();

            foreach (Char segment in rawKey.ToCharArray())
            {
                if (!tempKey.Contains(segment))
                {
                    tempKey.Add(segment);
                    tempKeyNumber.Add(new Number(this, new ReadOnlyCollection<Char>(new Char[] { segment }), null, false));
                }
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);
            this.KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);

            this.SetupMathEnvironment();
        }


        public void SetupMathEnvironment()
        {
            this.Bottom = this.Key[0];
            this.First = this.Key[1];
            this.Top = this.Key[this.Key.Count - 1];
            this.Base = (UInt16)this.Key.Count;

            this.BottomNumber = new Number(this, new ReadOnlyCollection<Char>(new Char[] { this.Key[0] }), null, false);
            this.FirstNumber = new Number(this, new ReadOnlyCollection<Char>(new Char[] { this.Key[1] }), null, false);
            this.TopNumber = new Number(this, new ReadOnlyCollection<Char>(new Char[] { this.Top }), null, false);
            this.PowerOfFirstNumber = new Number(this, new ReadOnlyCollection<Char>(new Char[] { this.Bottom, this.First }), null, false);


            if (this.Base > 2)
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
            List<Char> number = this.ConvertToChars(numberRaw);
            List<Char> numerator = this.ConvertToChars(numeratorNumber);
            List<Char> denominator = this.ConvertToChars(denominatorRaw);

            var result = new Number(this, new ReadOnlyCollection<Char>(number), new ReadOnlyCollection<Char>(numerator), new ReadOnlyCollection<Char>(denominator), false);

            return result;
        }

        public List<Char> ConvertToChars(UInt16 number)
        {
            var resultRaw = new List<Char>();

            Int32 carryOver = number;
            while (carryOver > 0)
            {
                if (carryOver >= this.Base)
                {
                    Int32 columnResultRaw = 0;
                    columnResultRaw = (carryOver % this.Base);
                    resultRaw.Add(this.Key[columnResultRaw]);
                    carryOver = (Int32)(((Decimal)carryOver - (Decimal)columnResultRaw) / (Decimal)this.Base);
                }
                else
                {
                    resultRaw.Add(this.Key[carryOver]);
                    carryOver = 0;
                }
            }
            return resultRaw;
        }

        public Char Bottom
        {
            get;
            protected set;
        }

        public Number BottomNumber
        {
            get;
            protected set;
        }
        
        public Number PowerOfFirstNumber
        {
            get;
            protected set;
        }

        public Char First
        {
            get;
            protected set;
        }

        public Number FirstNumber
        {
            get;
            protected set;
        }

        public Number SecondNumber
        {
            get;
            protected set;
        }


        public Char Top
        {
            get;
            protected set;
        }

        public Number TopNumber
        {
            get;
            protected set;
        }

        public UInt16 Base {
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
            String result = null;
            foreach (Char segment in this.Key)
            {
                if (result != null)
                {
                    result += '|';
                }
                result += segment;
            }
            return String.Format("[BASE:\"{0}\"|Bottom:\"{1}\"|Top:\"{2}]\"", this.Base, this.Bottom, this.Top) + result;
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
        
        public Boolean Equals(MathEnvironment other)
        {
            if (object.ReferenceEquals(other, default(MathEnvironment)) || this.Base != other.Base)
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
