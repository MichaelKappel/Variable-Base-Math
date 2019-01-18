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
    public struct Number:IEquatable<Number>, IComparable<Number>
    {
        internal static INumberOperator Operator = new NumberOperator();

        public Fraction Fragment { get; set; }

        public Char FirstChar { get; set; }


        public Boolean IsNegative { get; set; }

        public MathEnvironment Environment { get; set; }

        public ReadOnlyCollection<Char> Segments { get; set; }



        internal Number(MathEnvironment environment, ReadOnlyCollection<Char> segments, ReadOnlyCollection<Char> numerator, ReadOnlyCollection<Char> denominator, Boolean isNegative)
        {
            this.Environment = environment;

            this.IsNegative = isNegative;

            this.Segments = segments;

            if (numerator != default(ReadOnlyCollection<Char>) && denominator != default(ReadOnlyCollection<Char>))
            { 
                this.Fragment = new Fraction(environment, numerator, denominator);
            }else{
                this.Fragment = default(Fraction);
            }

            this.FirstChar = this.Segments[this.Segments.Count - 1];
            if (this.FirstChar == this.Environment.Bottom && this.Segments.Count > 1)
            {
                throw new Exception("Numbers longer then a power can not start with bottom number char");
            }
        }

        internal Number(MathEnvironment environment, ReadOnlyCollection<Char> segments, Fraction fragment, Boolean isNegative)
        {
            this.Environment = environment;

            this.IsNegative = isNegative;

            this.Segments = segments;

            this.Fragment = fragment;

            this.FirstChar = this.Segments[this.Segments.Count - 1];
            if (this.FirstChar == this.Environment.Bottom && this.Segments.Count > 1)
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
            return new Number(a.Environment, new ReadOnlyCollection<Char>(new Char[] { a.Environment.Bottom }), totalResult.Fragment, totalResult.IsNegative);
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
                return new Fraction(this, this.Environment.FirstNumber);
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
            return Operator.AsBinary(this);
        }

        

        public override String ToString()
        {
            String result = null;
            foreach (Char segment in this.Segments.Reverse())
            {
                result += segment;
            }
            if (this.IsNegative)
            {
                result = "-" + result;
            }
            if (this.Fragment != default(Fraction))
            {
                result = String.Format("{0} {1}", result, this.Fragment);
            }

            return result;
        }
    }
}