using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Diagnostics;
using Common.Models;
using Common.Interfaces;

namespace VariableBase.Mathematics.Operators
{
    public class NumberOperator : IOperator<Number>
    {
        public IBasicMathAlgorithm<Number> BasicMath { get; set; }

        public NumberOperator(IBasicMathAlgorithm<Number> basicMath)
        {
            this.BasicMath = basicMath;
        }

        public Number Square(Number number)
        {
            return new Number(number.Environment, this.BasicMath.Square(number.Environment, number.Segments), null, number.IsNegative);
        }

        public Number SquareRoot(Number number)
        {
            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) rawResult = this.BasicMath.SquareRoot(number.Environment, number.Segments);

            return new Number(number.Environment, rawResult.Whole, rawResult.Numerator, rawResult.Denominator, number.IsNegative);
        }

        public Number ConvertToBase10(Number number)
        {
            var base10Environment = new CharMathEnvironment("0123456789");
            if (number.Fragment == default(Fraction))
            {
                return new Number(base10Environment, this.BasicMath.ConvertToBase10(base10Environment, number.Environment, number.Segments), null, number.IsNegative);
            }
            else
            {
                Number numerator = number.Fragment.Numerator.ConvertToBase10();
                Number denominator = number.Fragment.Denominator.ConvertToBase10();
                return new Number(base10Environment, this.BasicMath.ConvertToBase10(base10Environment, number.Environment, number.Segments), new Fraction(numerator, denominator), number.IsNegative);
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
            NumberSegments numberSegments = number.Segments;
            if (numerator != 0)
            {
                var fraction = new Fraction(numerator, denominator);
                result = new Number(environment, number.Segments, fraction, false);
            }
            else
            {
                result = new Number(environment, number.Segments,  default(Fraction), false);
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

            if (a.Fragment == default(Fraction) && b.Fragment == default(Fraction))
            {
                NumberSegments resultSegments = this.BasicMath.Add(environment, a.Segments, b.Segments);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = this.AsFraction(a) + this.AsFraction(a);
                return new Number(aResult);
            }
        }

        public Number Subtract(Number a, Number b)
        {
            if (this.IsBottom(b))
            {
                return b;
            }

            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting differnt enviorments is not currently supported");
            }

            IMathEnvironment<Number> environment = a.Environment;

            if (this.IsEqual(a, b))
            {
                return environment.GetNumber(0);
            }


            if (a.Fragment == default(Fraction) && b.Fragment == default(Fraction))
            {
                NumberSegments resultSegments = this.BasicMath.Subtract(environment, a.Segments, b.Segments);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = this.AsFraction(a) - this.AsFraction(b);
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

            if (a.Fragment == default(Fraction) && b.Fragment == default(Fraction))
            {
                NumberSegments resultSegments = this.BasicMath.Multiply(environment, a.Segments, b.Segments);
                return new Number(environment, resultSegments, default(Fraction), false);
            }
            else
            {
                Fraction aResult = this.AsFraction(a) * this.AsFraction(b);
                return new Number(aResult);
            }

        }

        public Number Divide(Number a, Number b, Number hint = default(Number))
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = a.Environment;

            Number numerator = a;
            Number denominator = b;

            if (denominator.Fragment != default(Fraction) || denominator.Fragment != default(Fraction))
            {
                var aFraction = default(Fraction);
                if (numerator.Fragment != default(Fraction))
                {
                    NumberSegments aDividend = this.BasicMath.Add(environment, this.BasicMath.Multiply(environment, numerator.Segments, numerator.Fragment.Denominator.Segments), numerator.Fragment.Numerator.Segments);
                    aFraction = new Fraction(numerator.Environment, aDividend, numerator.Fragment.Denominator.Segments);
                }
                else
                {
                    aFraction = new Fraction(numerator.Environment, numerator.Segments, environment.GetNumber(1).Segments);
                }

                var bFraction = default(Fraction);
                if (denominator.Fragment != default(Fraction))
                {
                    NumberSegments bDividend =this.BasicMath.Add(environment, this.BasicMath.Multiply(environment, denominator.Segments, denominator.Fragment.Denominator.Segments), denominator.Fragment.Numerator.Segments);
                    bFraction = new Fraction(denominator.Environment, bDividend, denominator.Fragment.Denominator.Segments);
                }
                else if (aFraction != default(Fraction))
                {
                    bFraction = new Fraction(denominator.Environment, denominator.Segments, environment.GetNumber(1).Segments);
                }

                numerator = aFraction.Numerator * bFraction.Denominator;
                denominator = bFraction.Numerator * aFraction.Denominator;
            }

            (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) resultSegments;
            if (hint == default(Number))
            {
                resultSegments = this.BasicMath.Divide(environment, numerator.Segments, denominator.Segments);
            }
            else
            {
                resultSegments = this.BasicMath.Divide(environment, numerator.Segments, denominator.Segments, hint.Segments);
            }

            if (resultSegments.Numerator != default(NumberSegments) && resultSegments.Denominator != default(NumberSegments))
            {
                return new Number(environment, resultSegments.Whole, resultSegments.Numerator, resultSegments.Denominator, false);
            }
            else
            {
                return new Number(environment, resultSegments.Whole, default(Fraction), false);
            }
        }

        public int Compare(Number a, Object b)
        {
            throw new NotImplementedException();
        }

        public int Compare(Number a, Number b)
        {
            if (Object.ReferenceEquals(a.Environment, default(IMathEnvironment<Number>)) || (Number.IsBottom(a)) && Object.ReferenceEquals(a.Fragment, default(Fraction)))
            {
                if (Object.ReferenceEquals(b.Environment, default(IMathEnvironment<Number>)) || (Number.IsBottom(b) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (Object.ReferenceEquals(b.Environment, default(IMathEnvironment<Number>)) || (Number.IsBottom(b) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
            {
                return 1;
            }

            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            IMathEnvironment<Number> environment = a.Environment;


            Boolean reverse = false;
            if (!a.IsNegative &&  b.IsNegative)
            {
                return 1;
            }
            else if (a.IsNegative && ! b.IsNegative)
            {
                return -1;
            }
            else if (a.IsNegative &&  b.IsNegative)
            {
                reverse = true;
            }
            
            //Check if Environments all match
            if (b.Environment != environment)
            {
                throw new Exception("Compare differnt enviorments is not currently supported");
            }
            else if (!Object.ReferenceEquals(a.Fragment, default(Fraction)) && a.Fragment.Numerator.Environment != environment)
            {
                throw new Exception("Compare differnt enviorments is not currently supported");
            }
            else if (!Object.ReferenceEquals(b.Fragment, default(Fraction)) && b.Fragment.Numerator.Environment != environment)
            {
                throw new Exception("Compare differnt enviorments is not currently supported");
            }
            
            Int32 result = 0;
            if (a.Segments.Size >  b.Segments.Size)
            {
                result = 1;
            }
            else if (a.Segments.Size <  b.Segments.Size)
            {
                result = -1;
            }

            if (result == 0)
            {
                for (var i = a.Segments.Size - 1; i >= 0; i--)
                {
                    if (a.Segments[i] !=  b.Segments[i])
                    {
                        if (a.Segments[i] >  b.Segments[i])
                        {
                            result = 1;
                            break;
                        }
                        else if (a.Segments[i] <  b.Segments[i])
                        {
                            result = -1;
                            break;
                        }
                    }
                }
            }

            if (result == 0)
            {
                if (a.Fragment != default(Fraction))
                {
                    result = a.Fragment.CompareTo( b.Fragment);
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

        public Boolean IsEqual(Number a, Number b)
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

        public Boolean IsGreaterThan(Number a, Number b)
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

        public Boolean IsLessThan(Number a, Number b)
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

        public Boolean IsGreaterThanOrEqualTo(Number a, Number b)
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

        public Boolean IsLessThanOrEqualTo(Number a, Number b)
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

        public Boolean IsBottom(Number number)
        {
            if (number.Environment == default(IMathEnvironment<Number>) || number.Segments == default(NumberSegments) || number.Segments.Size == 0)
            {
                return true;
            }
            else if (number.Segments.Size == 1 && number.Segments[0] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean IsOdd(Number number)
        {
            return !this.IsEven(number);
        }
        public Boolean IsEven(Number number)
        {
            return this.BasicMath.IsEven(number.Environment, number.Segments);
        }

        public Number Convert(IMathEnvironment<Number> environment, Number number)
        {
            Debug.WriteLine(String.Format("Convert {0} to Base {1}", number, environment.Base));

            Number result = environment.GetNumber(0);
            if (number == number.Environment.GetNumber(1))
            {
                return environment.GetNumber(1);
            }
            else if (number == number.Environment.GetNumber(2))
            {
                return environment.GetNumber(2);
            }
            else if(!this.IsBottom(number))
            {
                Number tempNumber = number;

                Number numberDivider = new Number(number.Environment, this.BasicMath.AsSegments(number.Environment, Decimal.MaxValue), null, false);
                Number resultMultiplier = new Number(environment, this.BasicMath.AsSegments(environment, Decimal.MaxValue), null, false);
                
                while(numberDivider < number)
                {
                    numberDivider = this.Square(numberDivider);
                    resultMultiplier = this.Square(resultMultiplier);
                }

                while (this.IsGreaterThan(numberDivider, number.Environment.GetNumber(1)))
                {
                    while (tempNumber > numberDivider)
                    {
                        tempNumber -= numberDivider;
                        result += resultMultiplier;
                    }
                    numberDivider = this.SquareRoot(numberDivider);
                    resultMultiplier = this.SquareRoot(resultMultiplier);
                }
                
                while (tempNumber >= tempNumber.Environment.GetNumber(1))
                {
                    tempNumber -= tempNumber.Environment.GetNumber(1);
                    result += result.Environment.GetNumber(1);
                }
            }
#if DEBUG
            Debug.WriteLine(String.Format("Converted {0} to {1}", number, result));
            if (number.Environment.Base == 10)
            {
                Debug.WriteLine(String.Format("Reverse check {0} back to Base {1}", result, number.Environment.Base));
                Number reverseBack = this.Convert(number.Environment, result);
                if (number != reverseBack)
                {
                    throw new Exception(String.Format("GetActualValue Error {0} != {1}", number, reverseBack));
                }
                Debug.WriteLine(String.Format("Converted {0} back to {1}", result, reverseBack));
            }
#endif
            return result;
        }


        public Fraction AsFraction(Number number)
        {
            if (number.Fragment != default(Fraction) && (number.Environment != number.Fragment.Denominator.Environment || number.Fragment.Denominator.Environment != number.Fragment.Denominator.Environment))
            {
                throw new Exception("Fractions in differnt enviorments is not currently supported");
            }

            IMathEnvironment<Number> environment = number.Environment;

            if (number.Fragment == default(Fraction))
            {
                Number numerator = number;
                Number denominator = environment.GetNumber(1);

                return new Fraction(numerator, denominator);
            }
            else if (number.Fragment.Numerator.Fragment != default(Fraction) || number.Fragment.Denominator.Fragment != default(Fraction))
            {
                Fraction numeratorFraction = this.AsFraction(number.Fragment.Numerator);
                Fraction denominatorFraction = this.AsFraction(number.Fragment.Denominator);

                return new Fraction(new Number(numeratorFraction), new Number(denominatorFraction));
            }
            else
            {
                Number numerator = new Number(environment, 
                    this.BasicMath.Add(environment, this.BasicMath.Multiply(environment, number.Segments, number.Fragment.Denominator.Segments), number.Fragment.Numerator.Segments)
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
