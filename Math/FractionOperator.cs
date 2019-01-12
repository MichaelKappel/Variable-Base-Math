using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Math
{
    public class FractionOperator : IFractionOperator
    {
        public Fraction Add(Fraction a, Fraction b)
        {
            //FIX: add recursion
            if (a.Numerator.Environment != a.Denominator.Environment || a.Denominator.Environment != b.Denominator.Environment || b.Denominator.Environment != b.Numerator.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Denominator.Environment;

            ReadOnlyCollection<Char> commonDenominator = environment.Algorithm.Multiply(a.Denominator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorA = environment.Algorithm.Multiply(a.Numerator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorB = environment.Algorithm.Multiply(b.Numerator.Segments, a.Denominator.Segments);

            ReadOnlyCollection<Char> resultRaw = environment.Algorithm.Add(numeratorA, numeratorB);

            var result = new Fraction(environment, resultRaw, commonDenominator);

            return result;
        }

        public Fraction Subtract(Fraction a, Fraction b)
        {
            //FIX: add recursion
            if (a.Numerator.Environment != a.Denominator.Environment || a.Denominator.Environment != b.Denominator.Environment || b.Denominator.Environment != b.Numerator.Environment)
            {
                throw new Exception("Subtracting differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Denominator.Environment;

            ReadOnlyCollection<Char> commonDenominator = environment.Algorithm.Multiply(a.Denominator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorA = environment.Algorithm.Multiply(a.Numerator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorB = environment.Algorithm.Multiply(b.Numerator.Segments, a.Denominator.Segments);

            ReadOnlyCollection<Char> resultRaw = environment.Algorithm.Subtract(numeratorA, numeratorB);

            var result = new Fraction(environment, resultRaw, commonDenominator);

            return result;
        }

        public Fraction Multiply(Fraction a, Fraction b)
        {
            //FIX: add recursion
            if (a.Numerator.Environment != a.Denominator.Environment || a.Denominator.Environment != b.Denominator.Environment || b.Denominator.Environment != b.Numerator.Environment)
            {
                throw new Exception("Multipling differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Denominator.Environment;

            ReadOnlyCollection<Char> commonDenominator = environment.Algorithm.Multiply(a.Denominator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorA = environment.Algorithm.Multiply(a.Numerator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorB = environment.Algorithm.Multiply(b.Numerator.Segments, a.Denominator.Segments);

            ReadOnlyCollection<Char> rawResult = environment.Algorithm.Multiply(numeratorA, numeratorB);

            var result = new Fraction(environment, rawResult, commonDenominator);

            return result;
        }

        public Fraction Divide(Fraction a, Fraction b)
        {
            //FIX: add recursion
            if (a.Numerator.Environment != a.Denominator.Environment || a.Denominator.Environment != b.Denominator.Environment || b.Denominator.Environment != b.Numerator.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Denominator.Environment;

            ReadOnlyCollection<Char> commonDenominator = environment.Algorithm.Multiply(a.Denominator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorA = environment.Algorithm.Multiply(a.Numerator.Segments, b.Denominator.Segments);
            ReadOnlyCollection<Char> numeratorB = environment.Algorithm.Multiply(b.Numerator.Segments, a.Denominator.Segments);

            Tuple<ReadOnlyCollection<Char>, ReadOnlyCollection<Char>,ReadOnlyCollection<Char>> rawResult = environment.Algorithm.Divide(numeratorA, numeratorB);

            var result = new Fraction(environment, rawResult.Item1, commonDenominator);

            return result;
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
