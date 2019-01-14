using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Math
{
    public class Fraction: IEquatable<Fraction>, IComparable<Fraction>
    {

        internal static IFractionOperator Operator = new FractionOperator();

        internal Fraction(MathEnvironment environment, ReadOnlyCollection<Char> numerator, ReadOnlyCollection<Char> denominator)
        {
            this.Numerator = new Number(environment, numerator, null, null, false);
            this.Denominator = new Number(environment, denominator, null, null, false);
        }

        internal Fraction(Number numerator, Number denominator)
        {
            this.Numerator =  numerator;
            this.Denominator = denominator;
        }

        public Number Numerator { get; protected set; }
        public Number Denominator { get; protected set; }

        #region overrides
        public override String ToString()
        {
            return String.Format("{0}/{1}", this.Numerator, this.Denominator);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                Int64 hashCode1 = this.Numerator.GetHashCode();
                Int64 hashCode2 = this.Denominator.GetHashCode();
                Int64 hashCode = hashCode1 + hashCode2;
                if (hashCode <= Int32.MaxValue)
                {
                    return (Int32)hashCode;
                }
                else
                {
                    return (Int32)(hashCode - Int32.MaxValue);
                }
            }
        }
        public override Boolean Equals(Object other)
        {
            return Operator.Equals(this, (Fraction)other);
        }

        public Boolean Equals(Fraction other)
        {
            return Operator.Equals(this, other);
        }

        public int CompareTo(Fraction other)
        {
            return Operator.Compare(this, other);
        }


        #endregion

        #region operator overrides
        public static bool operator <(Fraction a, Fraction b)
        {
            return Operator.IsLessThan(a, b);
        }

        public static bool operator <=(Fraction a, Fraction b)
        {
            return Operator.IsLessThanOrEqualTo(a, b);
        }
        public static bool operator >(Fraction a, Fraction b)
        {
            return Operator.IsGreaterThan(a, b);
        }

        public static bool operator >=(Fraction a, Fraction b)
        {
            return Operator.IsGreaterThanOrEqualTo(a, b);
        }

        public static bool operator ==(Fraction a, Fraction b)
        {
            return Operator.Equals(a, b);
        }

        public static bool operator !=(Fraction a, Fraction b)
        {
            return !Operator.Equals(a, b);
        }

        #endregion

        #region operator overrides

        public static Fraction operator +(Fraction a, Fraction b)
        {
            return Operator.Add(a, b);
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            return Operator.Subtract(a, b);
        }

        public static Fraction operator *(Fraction a, Fraction b)
        {
            return Operator.Multiply(a, b);
        }


        public static Fraction operator /(Fraction a, Fraction b)
        {
            return Operator.Divide(a, b);
        }


        public static Fraction operator %(Fraction a, Fraction b)
        {
            throw new Exception("% not supported yet");
        }

        #endregion
       
        public Number AsNumber()
        {
            if (Number.Operator.Equals(this.Denominator, this.Denominator.Environment.BottomNumber))
            {
                return Numerator;
            }
            else if (Number.Operator.IsGreaterThan(this.Denominator, this.Numerator))
            {
                return Numerator;
            }
            else
            {
                return Number.Operator.Divide(this.Numerator, this.Denominator);
            }
        }
    }
}
