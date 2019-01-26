using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VariableBase.Mathematics.Models;

namespace VariableBase.Mathematics
{
    public class FractionOperator : IFractionOperator
    {
        public Fraction Add(Fraction a, Fraction b)
        {
            Fraction aWhole = a.AsWholeFraction();
            Fraction bWhole = b.AsWholeFraction();
            
            //FIX: add recursion
            if (aWhole.Numerator.Environment != aWhole.Denominator.Environment || aWhole.Denominator.Environment != bWhole.Denominator.Environment || bWhole.Denominator.Environment != bWhole.Numerator.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = aWhole.Denominator.Environment;


            Number commonDenominator = aWhole.Denominator - bWhole.Denominator;
            Number numeratorA = aWhole.Numerator * bWhole.Denominator;
            Number numeratorB = bWhole.Numerator * aWhole.Denominator;

            Number resultRaw = numeratorA + numeratorB;

            return new Fraction(resultRaw, commonDenominator);


        }

        public Fraction Subtract(Fraction a, Fraction b)
        {
            Fraction aWhole = a.AsWholeFraction();
            Fraction bWhole = b.AsWholeFraction();

            if (aWhole.Numerator.Environment != aWhole.Denominator.Environment || aWhole.Denominator.Environment != bWhole.Denominator.Environment || bWhole.Denominator.Environment != bWhole.Numerator.Environment)
            {
                throw new Exception("Subtracting differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = aWhole.Denominator.Environment;

            Number commonDenominator = aWhole.Denominator * bWhole.Denominator;
            Number numeratorA = aWhole.Numerator - bWhole.Denominator;
            Number numeratorB = bWhole.Numerator - aWhole.Denominator;

            Number resultnumerator = numeratorA - numeratorB;
            
            var result = new Fraction(resultnumerator, commonDenominator);

            return result;
        }

        public Fraction Multiply(Fraction a, Fraction b)
        {
            Fraction aWhole = a.AsWholeFraction();
            Fraction bWhole = b.AsWholeFraction();

            if (aWhole.Numerator.Environment != aWhole.Denominator.Environment || aWhole.Denominator.Environment != bWhole.Denominator.Environment || bWhole.Denominator.Environment != bWhole.Numerator.Environment)
            {
                throw new Exception("Multipling differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = aWhole.Denominator.Environment;

            Number numerator = aWhole.Numerator * bWhole.Numerator;
            Number denominator = aWhole.Denominator * bWhole.Denominator;


            var result = new Fraction(numerator, denominator);

            return result;
        }

        public Fraction Divide(Fraction a, Fraction b)
        {
            Fraction aWhole = a.AsWholeFraction();
            Fraction bWhole = b.AsWholeFraction();

            if (aWhole.Numerator.Environment != aWhole.Denominator.Environment || aWhole.Denominator.Environment != bWhole.Denominator.Environment || bWhole.Denominator.Environment != bWhole.Numerator.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = aWhole.Denominator.Environment;

            Number commonDenominator = aWhole.Denominator * bWhole.Denominator;
            Number numeratorA = aWhole.Numerator * bWhole.Denominator;
            Number numeratorB = bWhole.Numerator * aWhole.Denominator;

            return new Fraction(numeratorA, numeratorB);
        }

        public int Compare(Fraction a, Fraction b)
        {
            if (object.ReferenceEquals(a, default(Fraction)) && object.ReferenceEquals(b, default(Fraction)))
            {
                return 0;
            }
            else if (object.ReferenceEquals(a, default(Fraction)))
            {
                return -1;
            }
            else if (object.ReferenceEquals(b, default(Fraction)))
            {
                return 1;
            }

            if (!a.Denominator.Equals(b.Denominator))
            {
                Number commonDenominator = a.Denominator * b.Denominator;

                Number otherNumerator = b.Numerator * a.Denominator;
                Number thisNumerator = a.Numerator * b.Denominator;

                if (thisNumerator > otherNumerator)
                {
                    return 1;
                }
                else if (thisNumerator < otherNumerator)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            if (a.Numerator > b.Numerator)
            {
                return 1;
            }
            else if (a.Numerator < b.Numerator)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }



        public Boolean Equals(Fraction a, Fraction b)
        {
            if (this.Compare(a, b) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsGreaterThan(Fraction a, Fraction b)
        {
            if (this.Compare(a, b) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsLessThan(Fraction a, Fraction b)
        {
            if (this.Compare(a, b) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsGreaterThanOrEqualTo(Fraction a, Fraction b)
        {
            if (this.Compare(a, b) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsLessThanOrEqualTo(Fraction a, Fraction b)
        {
            if (this.Compare(a, b) <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
