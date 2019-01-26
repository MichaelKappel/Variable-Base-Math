//using VariableBase.Mathematics.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using VariableBase.Mathematics.Models;

//namespace VariableBase.Mathematics
//{
//    public class UInt64MathEnvironment: IMathEnvironment
//    {
//        public UInt64MathEnvironment() 
//        {
//            var tempKey = new List<Char>();

//            for (Int32 i = 0; i < 3; i++)
//            {
//                Char currentChar = Convert.ToChar(i);
//                tempKey.Add(currentChar);
//            }

//            this.Key = new ReadOnlyCollection<Char>(tempKey);

//            //Largest even number that  can safely be squared in C# is 281474976710654
//            this.Base = 281474976710654;

//            this.SetupMathEnvironment();
//        }
        
//        public UInt64MathEnvironment(Char size)
//        {
//            var tempKey = new List<Char>();

//            for (UInt64 i = 0; i < size; i++)
//            {
//                Char currentChar = Convert.ToChar(i);
//                tempKey.Add(currentChar);
//            }

//            this.Key = new ReadOnlyCollection<Char>(tempKey);

//            this.Base = size;

//            this.SetupMathEnvironment();
//        }

//        public UInt64MathEnvironment(String rawKey)
//        {
//            var tempKey = new List<Char>();

//            foreach (Char segment in rawKey.ToCharArray())
//            {
//                if (!tempKey.Contains(segment))
//                {
//                    tempKey.Add(segment);
//                }
//            }

//            this.Key = new ReadOnlyCollection<Char>(tempKey);

//            this.Base = (UInt64)this.Key.Count;

//            this.SetupMathEnvironment();
//        }

//        public Number GetNumber(Int32 zeros, Boolean isNegative = false)
//        {
//            var numberSegments = new UInt64[zeros];
//            for (var i = 0; i < zeros - 1; i++)
//            {
//                numberSegments[i] = 0;
//            }
//            numberSegments[numberSegments.Length - 1] = 1;

//            return new Number(this, new NumberSegments(numberSegments), null, isNegative);
//        }

//        public Number GetNumber(String[] wholeNumberSegments, Boolean isNegative = false)
//        {
//            return new Number(this, new NumberSegments(wholeNumberSegments.Select((x) => UInt64.Parse(x)).ToArray()),
//                               null, isNegative);
//        }

//        //public Number GetNumber(String wholeNumber, String fractionNumerator = null, String fractionDenominator = null, Boolean isNegative = false)
//        //{
//        //    List<Char> wholeNumberSegments = wholeNumber.ToCharArray().Reverse().ToList();

//        //    this.ValidateWholeNumber(wholeNumberSegments);

//        //    if (!String.IsNullOrEmpty(fractionNumerator) && !String.IsNullOrEmpty(fractionDenominator))
//        //    {
//        //        List<Char> fractionNumeratorSegments = fractionNumerator.ToCharArray().Reverse().ToList();
//        //        List<Char> fractionDenominatorSegments = fractionDenominator.ToCharArray().Reverse().ToList();

//        //        this.ValidateFraction(fractionNumeratorSegments, fractionDenominatorSegments);
//        //        return new Number(this, 
//        //            new NumberSegments(wholeNumberSegments.Select((x) => this.GetIndex(x)).ToArray()),
//        //            new NumberSegments(fractionNumeratorSegments.Select((x) => this.GetIndex(x)).ToArray()),
//        //            new NumberSegments(fractionDenominatorSegments.Select((x) => this.GetIndex(x)).ToArray()),
//        //            isNegative);
//        //    }
//        //    else
//        //    {
//        //        return new Number(this, new NumberSegments(wholeNumberSegments.Select((x) => this.GetIndex(x)).ToArray()), 
//        //            null, isNegative);
//        //    }
//        //}

//        public void ValidateWholeNumber(List<Char> numberSegments)
//        {
//            while (numberSegments.Count > 1 && numberSegments[numberSegments.Count - 1] == this.Key[0])
//            {
//                numberSegments.RemoveAt(numberSegments.Count - 1);
//            }

//            if (numberSegments[numberSegments.Count - 1] == '\u202c' || numberSegments[numberSegments.Count - 1] == 8237)
//            {
//                numberSegments.RemoveAt(numberSegments.Count - 1);
//            }

//            if (numberSegments[0] == '\u202c' || numberSegments[0] == 8237)
//            {
//                numberSegments.RemoveAt(0);
//            }


//            foreach (Char segment in numberSegments)
//            {
//                if (this.Base < UInt64.MaxValue- 1 && !this.Key.Contains(segment))
//                {
//                    throw new Exception(String.Format("Invalid Number {0} not found in {1}", segment, this));
//                }
//            }
//        }

//        public void ValidateFraction(List<Char> numerator, List<Char> denominator)
//        {
//            this.ValidateWholeNumber(numerator);
//            this.ValidateWholeNumber(denominator);

//            if (numerator == null || numerator.Count == 0 || numerator.Count == 1 && numerator[0] == 0)
//            {
//                throw new DivideByZeroException("Numerator of nothing not currently supported");
//            }

//            if (denominator == null || denominator.Count == 0 || denominator.Count == 1 && denominator[0] == 0)
//            {
//                throw new DivideByZeroException("Denominator of nothing not currently supported");
//            }
//        }

//        public UInt16 GetChar(Char arg)
//        {
//            return (UInt16)this.Key.IndexOf(arg);
//        }
        
//        public void SetupMathEnvironment()
//        {
//            var tempKeyNumber = new List<Number>();
//            for (UInt64 i = 0; i < this.Key.Count; i++)
//            {
//                tempKeyNumber.Add(new Number(this, new NumberSegments(new UInt64[] { i }), null, false));
//            }

//            this.KeyNumber = new ReadOnlyCollection<Number>(tempKeyNumber);
    
//            if (this.Base > 2)
//            {
//                this.SecondNumber = this.KeyNumber[2];
//            }
//            else
//            {
//                this.SecondNumber = this.PowerOfFirstNumber;
//            }
//        }
        
//        public Number PowerOfFirstNumber
//        {
//            get;
//            protected set;
//        }

//        public Number SecondNumber
//        {
//            get;
//            protected set;
//        }

//        public ReadOnlyCollection<Char> Key
//        {
//            get;
//            protected set;
//        }

//        public UInt64 Base
//        {
//            get;
//            private set;
//        }

//        public ReadOnlyCollection<Number> KeyNumber
//        {
//            get;
//            protected set;
//        }
//        public IBasicMathAlgorithm BasicMath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
//        public IPrimeAlgorithm PrimeAlgorithm { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//        decimal IMathEnvironment.Base => throw new NotImplementedException();

//        public override string ToString()
//        {

//            String result = String.Empty;
//            if (this.Base > 500)
//            {
//                result = String.Format("B {0}", this.Base);
//            }
//            else
//            {
//                foreach (UInt64 segment in this.Key)
//                {
//                    if (result != null)
//                    {
//                        result += '|';
//                    }
//                    result += segment;
//                }
//            }
//            return result;
//        }
        
//        public override int GetHashCode()
//        {
//            unchecked
//            {
//                int hashCode = this.Key.GetHashCode();
//                return hashCode;
//            }
//        }

//        public override Boolean Equals(Object other)
//        {
//            return this.Equals((IMathEnvironment)other);
//        }

//        public static bool operator ==(IMathEnvironment a, IMathEnvironment b)
//        {
//            return a.Equals(b);
//        }

//        public static bool operator !=(IMathEnvironment a, IMathEnvironment b)
//        {
//            return !a.Equals(b);
//        }

//        public Number AsNumber(Boolean[] binary, Boolean isNegative = false)
//        {
//            NumberSegments result = this.KeyNumber[0].Segments;
//            for (UInt64 i = 0; i < binary.Length; i++)
//            {
//                if (binary[(Int32)i])
//                {
//                    NumberSegments currentResult = new NumberSegments(new UInt64[] { 1 });
//                    for (UInt64 iSq = 0; iSq < i; iSq++)
//                    {
//                        currentResult = this.BasicMath.Multiply(currentResult,
//                            this.SecondNumber.Segments);
//                    }
//                    result = this.BasicMath.Add(result, currentResult);
//                }
//            }

//            return new Number(this, result, null, isNegative);
//        }

//        public Boolean Equals(IMathEnvironment other)
//        {
//            if (object.ReferenceEquals(other, default(IMathEnvironment)) || this.Base != other.Base)
//            {
//                return false;
//            }

//            if (this.Base != other.Base)
//            {
//                return false;
//            }

//            if (this.Base != other.Base)
//            {
//                return false;
//            }
//            return true;
//        }
//    }
//}
