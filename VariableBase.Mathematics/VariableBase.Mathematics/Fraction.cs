using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VariableBase.Mathematics.Models;

namespace VariableBase.Mathematics
{
    public class Fraction: IEquatable<Fraction>, IComparable<Fraction>
    {

        public static IFractionOperator Operator = new FractionOperator();

        internal Fraction(IMathEnvironment environment, NumberSegments numerator, NumberSegments denominator)
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
                Decimal hashCode1 = this.Numerator.GetHashCode();
                Decimal hashCode2 = this.Denominator.GetHashCode();
                Decimal hashCode = hashCode1 + hashCode2;
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
       
        public Fraction IncreaseDenominator(Number multiplier)
        {
            Number biggerNumerator = this.Numerator.Fragment.Denominator * multiplier;
            Number biggerDenominator = this.Numerator.Fragment.Denominator * multiplier;

            return new Fraction(biggerNumerator, biggerDenominator);
        }


        public Fraction AsWholeFraction()
        {
            Fraction biggerLookingnumber = this;
            if (this.Numerator.Fragment != default(Fraction))
            {
                Number nWholeNumerator = biggerLookingnumber.Numerator.Fragment.Denominator * biggerLookingnumber.Numerator.Fragment.Numerator;
                Number nBiggerDenominator = biggerLookingnumber.Numerator.Fragment.Denominator * biggerLookingnumber.Denominator;

                biggerLookingnumber = new Fraction(nWholeNumerator, nBiggerDenominator);
            }

            if (biggerLookingnumber.Denominator.Fragment != default(Fraction))
            {
                Number dWholeNumerator = biggerLookingnumber.Denominator.Fragment.Denominator * biggerLookingnumber.Denominator.Fragment.Numerator;
                Number dBiggerDenominator = biggerLookingnumber.Denominator.Fragment.Denominator * biggerLookingnumber.Numerator;

                biggerLookingnumber = new Fraction(dWholeNumerator, dBiggerDenominator);
            }

            return biggerLookingnumber;
        }
    }
}
