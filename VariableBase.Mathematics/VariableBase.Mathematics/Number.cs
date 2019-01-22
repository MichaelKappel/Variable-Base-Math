using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleToAttribute("Math.Tests")]

namespace VariableBase.Mathematics
{
    public struct Number:IEquatable<Number>, IComparable<Number>, IDisposable
    {
        internal static INumberOperator Operator = new NumberOperator();

        public Fraction Fragment { get; set; }

        public UInt16 First { get; set; }


        public Boolean IsNegative { get; set; }

        public MathEnvironment Environment { get; set; }

        public ReadOnlyCollection<UInt16> Segments { get; set; }



        internal Number(MathEnvironment environment, ReadOnlyCollection<UInt16> segments, ReadOnlyCollection<UInt16> numerator, ReadOnlyCollection<UInt16> denominator, Boolean isNegative)
        {
            this.Environment = environment;

            this.IsNegative = isNegative;

            this.Segments = segments;

            if (numerator != default(ReadOnlyCollection<UInt16>) && denominator != default(ReadOnlyCollection<UInt16>))
            { 
                this.Fragment = new Fraction(environment, numerator, denominator);
            }
            else
            {
                this.Fragment = default(Fraction);
            }

            this.First = this.Segments[this.Segments.Count - 1];
            if (this.First == 0 && this.Segments.Count > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }
        }

        internal Number(MathEnvironment environment, ReadOnlyCollection<UInt16> segments, Fraction fragment, Boolean isNegative)
        {
            this.Environment = environment;

            this.IsNegative = isNegative;

            this.Segments = segments;

            this.Fragment = fragment;

            this.First = this.Segments[this.Segments.Count - 1];
            if (this.First == 0 && this.Segments.Count > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }
        }


        #region operator overrides
        public static bool operator <(Number a, Number b)
        {
            return Operator.Compare(a, b) < 0;
        }

        public static bool operator <=(Number a, Number b)
        {
            return Operator.Compare(a, b) <= 0;
        }
        public static bool operator >(Number a, Number b)
        {
            return Operator.Compare(a, b) > 0;
        }

        public static bool operator >=(Number a, Number b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator ==(Number a, Number b)
        {
            return Operator.Equals(a, b);
        }

        public static bool operator !=(Number a, Number b)
        {
            return !Operator.Equals(a, b);
        }


        public static Number operator +(Number a, Number b)
        {
            return Operator.Add(a, b);
        }
        
        public static Number operator -(Number a, Number b)
        {
            return Operator.Subtract(a, b);
        }

        public static Number operator *(Number a, Number b)
        {
            return Operator.Multiply(a, b);
        }


        public static Number operator /(Number a, Number b)
        {
            return Operator.Divide(a, b);
        }

        
        public static Number operator %(Number a, Number b)
        { 
            Number totalResult = Operator.Divide(a, b);
            return new Number(a.Environment, new ReadOnlyCollection<UInt16>(new UInt16[] { 0 }), totalResult.Fragment, totalResult.IsNegative);
        }

        #endregion

        internal Number Copy()
        {
            return new Number(this.Environment, this.Segments, this.Fragment, this.IsNegative);
        }

        internal Fraction AsFraction()
        {
            if (this.Fragment == default(Fraction))
            {
                return new Fraction(this, this.Environment.KeyNumber[1]);
            }
            else
            {
                Number denominator = this.Fragment.Denominator;
                Number numerator = Operator.Add(Operator.Multiply(this.Floor(), denominator), this.Fragment.Numerator);
                return new Fraction(numerator, denominator);
            }
        }

        internal Number AsNegativeNumber()
        {
            return new Number(this.Environment, this.Segments, this.Fragment, true);
        }

        internal Number AsPositiveNumber()
        {
            return new Number(this.Environment, this.Segments, this.Fragment, false);
        }

        internal Number Floor()
        {
            return new Number(this.Environment, this.Segments, null, this.IsNegative);
        }


        public Tuple<Number, Number> GetComposite()
        {
            return Operator.GetComposite(this);
        }

        public Boolean IsPrime()
        {
            return Operator.IsPrime(this);
           
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = this.Segments.GetHashCode();
                return hashCode;
            }
        }

        public override Boolean Equals(Object other)
        {
            return Operator.Equals(this, (Number)other);
        }

        public Boolean Equals(Number other)
        {
            return Operator.Equals(this, other);
        }

        public int CompareTo(Number other)
        {
            return Operator.Compare(this, other);
        }

        public Number Convert(MathEnvironment environment)
        {
            return Operator.Convert(environment, this);
        }

        public Number AsBinary()
        {
            return Operator.AsBinaryNumber(this);
        }

        public String GetActualValue(MathEnvironment environment)
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }

            ReadOnlyCollection<UInt16> resultSegments;
            if (environment != this.Environment)
            {
                resultSegments = this.Convert(environment).Segments;
            }
            else
            {
                resultSegments = this.Segments;
            }

            for (UInt16 i = (UInt16)resultSegments.Count; i > 0; i--)
            {
                result += environment.Key[resultSegments[i - 1]];
            }

            if (this.Fragment != default(Fraction))
            {
                result = String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }
        public String GetDisplayValue()
        {
            String result = String.Empty;
            if (this.IsNegative)
            {
                result = "-" + result;
            }
            if (this.Segments.Count > 200)
            {
                result += String.Format("{0}e{1}", this.Environment.Key[this.First], this.Segments.Count);
                if (this.Fragment != default(Fraction))
                {
                    result += String.Format("{0} {1}", result, this.Fragment);
                }
            }
            else
            {
                for (UInt16 i = (UInt16)this.Segments.Count; i > 0; i--)
                {
                    result += this.Environment.Key[this.Segments[i-1]];
                }
                if (this.Fragment != default(Fraction))
                {
                    result = String.Format("{0} {1}", result, this.Fragment);
                }
            }
            return result;
        }

        public String ToString(MathEnvironment environment)
        {
            return this.GetActualValue(environment);
        }

        public override String ToString()
        {
            return this.GetDisplayValue();
        }

        public void Dispose()
        {
            this.Fragment = default(Fraction);

            this.First = 0;
            
            this.IsNegative = false;

            this.Environment = default(MathEnvironment);

            this.Segments = default(ReadOnlyCollection<UInt16>);
        }
    }
}