using System;

using System.Diagnostics;
using NS12.VariableBase.Mathematics.Common.Models;
using NS12.VariableBase.Mathematics.Common.Interfaces;
using NS12.VariableBase.Mathematics.Providers.MathEnvironments;

namespace NS12.VariableBase.Mathematics.Providers.Operators
{
    public class NumberOperator : IOperator<Number>
    {
        public IBasicMathAlgorithm<Number> BasicMath { get; set; }

        public NumberOperator(IBasicMathAlgorithm<Number> basicMath)
        {
            BasicMath = basicMath;
        }

        public Number Square(Number number)
        {
            return new Number(number.Environment, BasicMath.Square(number.Environment, number.Whole), null, number.IsNegative);
        }

        public Number SquareRoot(Number number)
        {
            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) rawResult = BasicMath.SquareRoot(number.Environment, number.Whole);

            return new Number(number.Environment, rawResult.Whole, rawResult.Numerator, rawResult.Denominator, number.IsNegative);
        }

        public Number ConvertToBase10(Number number)
        {
            var base10Environment = new CharMathEnvironment("0123456789");
            if (number.Fragment == default)
            {
                return new Number(base10Environment, BasicMath.ConvertToBase10(base10Environment, number.Environment, number.Whole), null, number.IsNegative);
            }
            else
            {
                Number numerator = number.Fragment.Numerator.ConvertToBase10();
                Number denominator = number.Fragment.Denominator.ConvertToBase10();
                return new Number(base10Environment, BasicMath.ConvertToBase10(base10Environment, number.Environment, number.Whole), new Fraction(numerator, denominator), number.IsNegative);
            }
        }

        public Number AsFraction(Number number, Number numerator, Number denominator)
        {
            if (number.Environment != numerator.Environment || numerator.Environment != denominator.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = number.Environment;

            Number result;
            NumberSegments numberSegments = number.Whole;
            if (numerator != 0)
            {
                var fraction = new Fraction(numerator, denominator);
                result = new Number(environment, number.Whole, fraction, false);
            }
            else
            {
                result = new Number(environment, number.Whole, default(Fraction), false);
            }
            return result;
        }

        public Number Add(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = a.Environment;

            if (a.Fragment == default && b.Fragment == default)
            {
                NumberSegments resultSegments = BasicMath.Add(environment, a.Whole, b.Whole);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = AsFraction(a) + AsFraction(a);
                return new Number(aResult);
            }
        }

        public Number Subtract(Number a, Number b)
        {
            if (IsBottom(b))
            {
                return b;
            }

            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting differnt enviorments is not currently supported");
            }

            IMathEnvironment<Number> environment = a.Environment;

            if (IsEqual(a, b))
            {
                return environment.GetNumber(0);
            }


            if (a.Fragment == default && b.Fragment == default)
            {
                NumberSegments resultSegments = BasicMath.Subtract(environment, a.Whole, b.Whole);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = AsFraction(a) - AsFraction(b);
                return new Number(aResult);
            }
        }

        public Number Multiply(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Multipling differnt enviorments is not currently supported");
            }

            IMathEnvironment<Number> environment = a.Environment;

            if (a.Fragment == default && b.Fragment == default)
            {
                NumberSegments resultSegments = BasicMath.Multiply(environment, a.Whole, b.Whole);
                return new Number(environment, resultSegments, default(Fraction), false);
            }
            else
            {
                Fraction aResult = AsFraction(a) * AsFraction(b);
                return new Number(aResult);
            }

        }

        public Number Divide(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = a.Environment;

            Number numerator = a;
            Number denominator = b;

            if (denominator.Fragment != default || denominator.Fragment != default)
            {
                if (a.Fragment != default && a.Fragment.Numerator != default && a.Fragment.Numerator.Fragment != default)
                {
                    throw new Exception("Dividing multilevel fractions is not currently supported in 'a' Numerator");
                }

                if (a.Fragment != default && a.Fragment.Denominator != default && a.Fragment.Denominator.Fragment != default)
                {
                    throw new Exception("Dividing multilevel fractions is not currently supported in 'a' Denominator");
                }

                if (b.Fragment != default && b.Fragment.Numerator != default && b.Fragment.Numerator.Fragment != default)
                {
                    throw new Exception("Dividing multilevel fractions is not currently supported in 'b' Numerator");
                }

                if (b.Fragment != default && b.Fragment.Denominator != default && b.Fragment.Denominator.Fragment != default)
                {
                    throw new Exception("Dividing multilevel fractions is not currently supported in 'b' Denominator");
                }


                Fraction aFraction = default(Fraction);
                if (numerator.Fragment != default)
                {
                    NumberSegments aDividend = BasicMath.Add(environment, BasicMath.Multiply(environment, numerator.Whole, numerator.Fragment.Denominator.Whole), numerator.Fragment.Numerator.Whole);
                    aFraction = new Fraction(numerator.Environment, aDividend, numerator.Fragment.Denominator.Whole);
                }
                else
                {
                    aFraction = new Fraction(numerator.Environment, numerator.Whole, environment.GetNumber(1).Whole);
                }

                var bFraction = default(Fraction);
                if (denominator.Fragment != default)
                {
                    NumberSegments bDividend = BasicMath.Add(environment, BasicMath.Multiply(environment, denominator.Whole, denominator.Fragment.Denominator.Whole), denominator.Fragment.Numerator.Whole);
                    bFraction = new Fraction(denominator.Environment, bDividend, denominator.Fragment.Denominator.Whole);
                }
                else if (aFraction != default)
                {
                    bFraction = new Fraction(denominator.Environment, denominator.Whole, environment.GetNumber(1).Whole);
                }

                numerator = aFraction.Numerator * bFraction.Denominator;
                denominator = bFraction.Numerator * aFraction.Denominator;
            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) resultSegments = BasicMath.Divide(environment, numerator.Whole, denominator.Whole);

            if (resultSegments.Numerator != default(NumberSegments) && resultSegments.Denominator != default(NumberSegments))
            {
                return new Number(environment, resultSegments.Whole, resultSegments.Numerator, resultSegments.Denominator, false);
            }
            else
            {
                return new Number(environment, resultSegments.Whole, default(Fraction), false);
            }
        }

        public int Compare(Number a, object b)
        {
            throw new NotImplementedException();
        }

        public int Compare(Number a, Number b)
        {
            if (object.ReferenceEquals(a.Environment, default(IMathEnvironment<Number>)) || Number.IsBottom(a) && ReferenceEquals(a.Fragment, default(Fraction)))
            {
                if (object.ReferenceEquals(b.Environment, default(IMathEnvironment<Number>)) || Number.IsBottom(b) && ReferenceEquals(b.Fragment, default(Fraction)))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (object.ReferenceEquals(b.Environment, default(IMathEnvironment<Number>)) || Number.IsBottom(b) && ReferenceEquals(b.Fragment, default(Fraction)))
            {
                return 1;
            }

            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = a.Environment;


            bool reverse = false;
            if (!a.IsNegative && b.IsNegative)
            {
                return 1;
            }
            else if (a.IsNegative && !b.IsNegative)
            {
                return -1;
            }
            else if (a.IsNegative && b.IsNegative)
            {
                reverse = true;
            }

            //Check if Environments all match
            if (b.Environment != environment)
            {
                throw new Exception("Compare differnt enviorments is not currently supported");
            }
            else if (!ReferenceEquals(a.Fragment, default(Fraction)) && a.Fragment.Numerator.Environment != environment)
            {
                throw new Exception("Compare differnt enviorments is not currently supported");
            }
            else if (!ReferenceEquals(b.Fragment, default(Fraction)) && b.Fragment.Numerator.Environment != environment)
            {
                throw new Exception("Compare differnt enviorments is not currently supported");
            }

            int result = 0;
            if (a.Whole.Size > b.Whole.Size)
            {
                result = 1;
            }
            else if (a.Whole.Size < b.Whole.Size)
            {
                result = -1;
            }

            if (result == 0)
            {
                for (var i = a.Whole.Size - 1; i >= 0; i--)
                {
                    if (a.Whole[i] != b.Whole[i])
                    {
                        if (a.Whole[i] > b.Whole[i])
                        {
                            result = 1;
                            break;
                        }
                        else if (a.Whole[i] < b.Whole[i])
                        {
                            result = -1;
                            break;
                        }
                    }
                }
            }

            if (result == 0)
            {
                if (a.Fragment != default)
                {
                    result = a.Fragment.CompareTo(b.Fragment);
                }
            }

            if (reverse && result == 1)
            {
                return -1;
            }
            else if (reverse && result == -1)
            {
                return 1;
            }
            else
            {
                return result;
            }
        }

        public bool IsEqual(Number a, Number b)
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

        public bool IsGreaterThan(Number a, Number b)
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

        public bool IsLessThan(Number a, Number b)
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

        public bool IsGreaterThanOrEqualTo(Number a, Number b)
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

        public bool IsLessThanOrEqualTo(Number a, Number b)
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

        public bool IsBottom(Number number)
        {
            if (number.Environment == default(IMathEnvironment<Number>) || number.Whole == default(NumberSegments) || number.Whole.Size == 0)
            {
                return true;
            }
            else if (number.Whole.Size == 1 && number.Whole[0] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsOdd(Number number)
        {
            return !IsEven(number);
        }
        public bool IsEven(Number number)
        {
            return BasicMath.IsEven(number.Environment, number.Whole);
        }

        public Number Convert(IMathEnvironment<Number> environment, Number number)
        {
            Debug.WriteLine(string.Format("Convert {0} to Base {1}", number, environment.Base));

            Number result = environment.GetNumber(0);
            if (number == number.Environment.GetNumber(1))
            {
                return environment.GetNumber(1);
            }
            else if (number == number.Environment.GetNumber(2))
            {
                return environment.GetNumber(2);
            }
            else if (!IsBottom(number))
            {
                Number tempNumber = number;

                Number numberDivider = new Number(number.Environment, BasicMath.AsSegments(number.Environment, decimal.MaxValue), null, false);
                Number resultMultiplier = new Number(environment, BasicMath.AsSegments(environment, decimal.MaxValue), null, false);

                while (numberDivider < number)
                {
                    numberDivider = Square(numberDivider);
                    resultMultiplier = Square(resultMultiplier);
                }

                while (IsGreaterThan(numberDivider, number.Environment.GetNumber(1)))
                {
                    while (tempNumber > numberDivider)
                    {
                        tempNumber -= numberDivider;
                        result += resultMultiplier;
                    }
                    numberDivider = SquareRoot(numberDivider);
                    resultMultiplier = SquareRoot(resultMultiplier);
                }

                while (tempNumber >= tempNumber.Environment.GetNumber(1))
                {
                    tempNumber -= tempNumber.Environment.GetNumber(1);
                    result += result.Environment.GetNumber(1);
                }
            }
#if DEBUG
            Debug.WriteLine(string.Format("Converted {0} to {1}", number, result));
            if (number.Environment.Base == 10)
            {
                Debug.WriteLine(string.Format("Reverse check {0} back to Base {1}", result, number.Environment.Base));
                Number reverseBack = this.Convert(number.Environment, result);
                if (number != reverseBack)
                {
                    throw new Exception(string.Format("GetActualValue Error {0} != {1}", number, reverseBack));
                }
                Debug.WriteLine(string.Format("Converted {0} back to {1}", result, reverseBack));
            }
#endif
            return result;
        }


        public Fraction AsFraction(Number number)
        {
            if (number.Fragment != default && (number.Environment != number.Fragment.Denominator.Environment || number.Fragment.Denominator.Environment != number.Fragment.Denominator.Environment))
            {
                throw new Exception("Fractions in differnt enviorments is not currently supported");
            }

            IMathEnvironment<Number> environment = number.Environment;

            if (number.Fragment == default)
            {
                Number numerator = number;
                Number denominator = environment.GetNumber(1);

                return new Fraction(numerator, denominator);
            }
            else if (number.Fragment.Numerator.Fragment != default || number.Fragment.Denominator.Fragment != default)
            {
                Fraction numeratorFraction = AsFraction(number.Fragment.Numerator);
                Fraction denominatorFraction = AsFraction(number.Fragment.Denominator);

                return new Fraction(new Number(numeratorFraction), new Number(denominatorFraction));
            }
            else
            {
                Number numerator = new Number(environment,
                    BasicMath.Add(environment, BasicMath.Multiply(environment, number.Whole, number.Fragment.Denominator.Whole), number.Fragment.Numerator.Whole)
                    , default(Fraction)
                    , number.IsNegative);

                Number denominator = number.Fragment.Denominator;

                return new Fraction(numerator, denominator);
            }
        }

        public int CompareTo(Number other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Number other)
        {
            throw new NotImplementedException();
        }

        public int Compare(decimal x, decimal y)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(decimal other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(decimal other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Number a, Number b)
        {
            throw new NotImplementedException();
        }
    }
}
