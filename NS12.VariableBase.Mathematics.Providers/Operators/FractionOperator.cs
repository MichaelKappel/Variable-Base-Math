using System;

using NS12.VariableBase.Mathematics.Common.Interfaces;

namespace NS12.VariableBase.Mathematics.Providers.Operators
{
    public class FractionOperator : IOperator<Fraction>
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
            IMathEnvironment<Number> environment = aWhole.Denominator.Environment;


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
            IMathEnvironment<Number> environment = aWhole.Denominator.Environment;

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

            IMathEnvironment<Number> environment = aWhole.Denominator.Environment;

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

            Number numeratorA = aWhole.Numerator * bWhole.Denominator;
            Number numeratorB = bWhole.Numerator * aWhole.Denominator;

            return new Fraction(numeratorA, numeratorB);
        }

        public int Compare(Fraction a, Fraction b)
        {
            if (ReferenceEquals(a, default(Fraction)) && ReferenceEquals(b, default(Fraction)))
            {
                return 0;
            }
            else if (ReferenceEquals(a, default(Fraction)))
            {
                return -1;
            }
            else if (ReferenceEquals(b, default(Fraction)))
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



        public bool Equals(Fraction a, Fraction b)
        {
            if (Compare(a, b) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsGreaterThan(Fraction a, Fraction b)
        {
            if (Compare(a, b) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLessThan(Fraction a, Fraction b)
        {
            if (Compare(a, b) < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsGreaterThanOrEqualTo(Fraction a, Fraction b)
        {
            if (Compare(a, b) >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLessThanOrEqualTo(Fraction a, Fraction b)
        {
            if (Compare(a, b) <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsBottom(Fraction number)
        {
            throw new NotImplementedException();
        }

        public bool IsEven(Fraction number)
        {
            throw new NotImplementedException();
        }

        public Fraction Convert(IMathEnvironment<Fraction> environment, Fraction number)
        {
            throw new NotImplementedException();
        }

        public Fraction Square(Fraction number)
        {
            throw new NotImplementedException();
        }

        public Fraction SquareRoot(Fraction number)
        {
            throw new NotImplementedException();
        }

        public Fraction ConvertToBase10(Fraction number)
        {
            throw new NotImplementedException();
        }
    }
}
