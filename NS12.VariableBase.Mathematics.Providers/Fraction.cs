using System;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Providers.Operators;
using NS12.VariableBase.Mathematics.Common.Interfaces;

namespace NS12.VariableBase.Mathematics.Providers
{
    public class Fraction : IEquatable<Fraction?>, IComparable<Fraction?>
    {

        public static IOperator<Fraction?> Operator = new FractionOperator();

        internal Fraction(IMathEnvironment<Number> environment, NumberSegments numerator, NumberSegments denominator)
        {
            Numerator = new Number(environment, numerator, false);
            Denominator = new Number(environment, denominator, false);
        }

        internal Fraction(Number numerator, Number denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public Number Numerator { get; protected set; }
        public Number Denominator { get; protected set; }

        #region overrides
        public override string ToString()
        {
            return string.Format("{0}/{1}", Numerator, Denominator);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                decimal hashCode1 = Numerator.GetHashCode();
                decimal hashCode2 = Denominator.GetHashCode();
                decimal hashCode = hashCode1 + hashCode2;
                if (hashCode <= int.MaxValue)
                {
                    return (int)hashCode;
                }
                else
                {
                    return (int)(hashCode - int.MaxValue);
                }
            }
        }
        public override bool Equals(object other)
        {
            return Operator.Equals(this, (Fraction?)other);
        }

        public bool Equals(Fraction? other)
        {
            return Operator.Equals(this, other);
        }

        public int CompareTo(Fraction? other)
        {
            return Operator.Compare(this, other);
        }


        #endregion

        #region operator overrides
        public static bool operator <(Fraction? a, Fraction? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            return Operator.IsLessThan(a, b);
        }

        public static bool operator <=(Fraction? a, Fraction? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            return Operator.IsLessThanOrEqualTo(a, b);
        }
        public static bool operator >(Fraction? a, Fraction? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            return Operator.IsGreaterThan(a, b);
        }

        public static bool operator >=(Fraction? a, Fraction? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            return Operator.IsGreaterThanOrEqualTo(a, b);
        }

        public static bool operator ==(Fraction? a, Fraction? b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            return Operator.Equals(a, b);
        }

        public static bool operator !=(Fraction? a, Fraction? b)
        {
            if (a is null && b is null)
            {
                return false;
            }
            return !Operator.Equals(a, b);
        }

        #endregion

        #region operator overrides

        public static Fraction? operator +(Fraction? a, Fraction? b)
        {
#if DEBUG
            Console.WriteLine("Add Fraction");
#endif
            return Operator.Add(a, b);
        }

        public static Fraction? operator -(Fraction? a, Fraction? b)
        {
#if DEBUG
            Console.WriteLine("Subtract Fraction");
#endif
            return Operator.Subtract(a, b);
        }

        public static Fraction? operator *(Fraction? a, Fraction? b)
        {
#if DEBUG
            Console.WriteLine("Multiply Fraction");
#endif
            return Operator.Multiply(a, b);
        }


        public static Fraction? operator /(Fraction? a, Fraction? b)
        {
#if DEBUG
            Console.WriteLine("Divide Fraction");
#endif
            return Operator.Divide(a, b);
        }


        public static Fraction operator %(Fraction? a, Fraction? b)
        {
            throw new Exception("% not supported yet");
        }

        #endregion

        public Fraction IncreaseDenominator(Number multiplier)
        {
            Number biggerNumerator = Numerator.Fragment.Denominator * multiplier;
            Number biggerDenominator = Numerator.Fragment.Denominator * multiplier;

            return new Fraction(biggerNumerator, biggerDenominator);
        }


        public Fraction AsWholeFraction()
        {
            Fraction biggerLookingnumber;
            if (Numerator.Fragment != null)
            {
                Number nWholeNumerator = this.Numerator.Fragment.Denominator * this.Numerator.Fragment.Numerator;
                Number nBiggerDenominator = this.Numerator.Fragment.Denominator * this.Denominator;

                biggerLookingnumber = new Fraction(nWholeNumerator, nBiggerDenominator);
            }
            else
            {
                biggerLookingnumber = this;
            }

            if (biggerLookingnumber.Denominator.Fragment != null)
            {
                Number dWholeNumerator = biggerLookingnumber.Denominator.Fragment.Denominator * biggerLookingnumber.Denominator.Fragment.Numerator;
                Number dBiggerDenominator = biggerLookingnumber.Denominator.Fragment.Denominator * biggerLookingnumber.Numerator;

                biggerLookingnumber = new Fraction(dWholeNumerator, dBiggerDenominator);
            }

            return biggerLookingnumber;
        }
    }
}
