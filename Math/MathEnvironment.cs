using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Math
{
    public class MathEnvironment: IEquatable<MathEnvironment>
    {
        public MathEnvironment(String rawKey)
        {
            this.SetKey(rawKey);
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
                return new Number(this, wholeNumberSegments, isNegative, new Fraction(this, fractionNumeratorSegments, fractionDenominatorSegments));
            }
            else
            {
                return new Number(this, wholeNumberSegments, isNegative);
            }

        }

        public WholeNumber GetWholeNumber(String wholeNumber, Boolean isNegative)
        {

            List<Char> wholeNumberSegments = wholeNumber.ToCharArray().Reverse().ToList();

            this.ValidateWholeNumber(wholeNumberSegments);

            return new WholeNumber(this, wholeNumberSegments, isNegative);


        }

        public void ValidateWholeNumber(List<Char> numberSegments)
        {
            while (numberSegments.Count > 1 && numberSegments[numberSegments.Count - 1] == this.Bottom)
            {
                numberSegments.RemoveAt(numberSegments.Count - 1);
            }

            if (numberSegments[0] == ('\u202c'))
            {
                numberSegments.RemoveAt(0);
            }

            if (numberSegments[numberSegments.Count - 1] == ('\u202c'))
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

        public UInt64 GetIndex(Char arg)
        {
            return (UInt64)this.Key.IndexOf(arg);
        }

        public void SetKey(String rawKey)
        {
           var tempKey = new List<Char>();
            var tempKeyNumber = new List<Number>();
            var tempKeyWholeNumber = new List<WholeNumber>();

            foreach (Char segment in rawKey.ToCharArray())
            {
                if (!tempKey.Contains(segment))
                {
                    tempKey.Add(segment);
                    tempKeyNumber.Add(new Number(this, segment, false));
                    tempKeyWholeNumber.Add(new WholeNumber(this, segment, false));
                }
            }

            this.Key = new ReadOnlyCollection<Char>(tempKey);
            this.KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);
            this.KeyWholeNumber = new ReadOnlyCollection<WholeNumber>(tempKeyWholeNumber);

            this.Bottom = this.Key[0];
            this.First = this.Key[1];
            this.Top = this.Key[this.Key.Count - 1];
            this.Base = (UInt64)this.Key.Count;

            this.BottomNumber = new Number(this, this.Key[0], false);
            this.FirstNumber = new Number(this, this.Key[1], false); 
            this.TopNumber = new Number(this, this.Top, false);
            this.PowerOfFirstNumber =  new Number(this, new Char[] { this.Bottom, this.First }, false); 

            this.BottomWholeNumber = new WholeNumber(this, this.Key[0], false);
            this.FirstWholeNumber = new WholeNumber(this, this.Key[1], false);
            this.TopWholeNumber = new WholeNumber(this, this.Top, false);
            this.PowerOfFirstWholeNumber = new WholeNumber(this, new Char[] { this.Bottom, this.First  }, false);
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

        public WholeNumber BottomWholeNumber
        {
            get;
            protected set;
        }


        public Number PowerOfFirstNumber
        {
            get;
            protected set;
        }

        public WholeNumber PowerOfFirstWholeNumber
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

        public WholeNumber FirstWholeNumber
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

        public WholeNumber TopWholeNumber
        {
            get;
            protected set;
        }

        public UInt64 Base {
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

        public ReadOnlyCollection<WholeNumber> KeyWholeNumber
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

        public Boolean Equals(MathEnvironment other)
        {
            if (this.Base != other.Base)
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
