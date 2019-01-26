using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using static VariableBase.Mathematics.BasicMathAlgorithm;
using System.Diagnostics;
using VariableBase.Mathematics.Models;

namespace VariableBase.Mathematics
{
    public class NumberOperator : INumberOperator
    {
        internal IBasicMathAlgorithm BasicMath { get; set; }
        internal IPrimeAlgorithm PrimeAlgorithm { get; set; }

        internal NumberOperator(IBasicMathAlgorithm basicMath, IPrimeAlgorithm primeAlgorithm)
        {
            this.BasicMath = basicMath;
            this.PrimeAlgorithm = primeAlgorithm;
        }

        public Number AsFraction(Number number, Number numerator, Number denominator)
        {
            if (number.Environment != numerator.Environment || numerator.Environment != denominator.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = number.Environment;

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
            IMathEnvironment environment = a.Environment;

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

            IMathEnvironment environment = a.Environment;

            if (this.IsEqual(a, b))
            {
                return environment.KeyNumber[0];
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

            IMathEnvironment environment = a.Environment;

            if (a.Fragment == default(Fraction) && b.Fragment == default(Fraction))
            {
                NumberSegments resultSegments = this.BasicMath.Multiply(environment, a.Segments, b.Segments);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = this.AsFraction(a) * this.AsFraction(b);
                return new Number(aResult);
            }

        }

        public Number Divide(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = a.Environment;

            Number numerator = a;
            Number denominator = b;

            if (denominator.Fragment != default(Fraction) || denominator.Fragment != default(Fraction))
            {
                var aFraction = default(Fraction);
                if (numerator.Fragment != default(Fraction))
                {
                    NumberSegments aDividend = this.BasicMath.Add(environment, this.BasicMath.Multiply(environment, numerator.Segments, numerator.Fragment.Denominator.Segments), numerator.Fragment.Numerator.Segments);
                    aFraction = new Fraction(numerator.Environment, aDividend, environment.KeyNumber[1].Segments);
                }
                else
                {
                    aFraction = new Fraction(numerator.Environment, numerator.Segments, environment.KeyNumber[1].Segments);
                }

                var bFraction = default(Fraction);
                if (denominator.Fragment != default(Fraction))
                {
                    NumberSegments bDividend =this.BasicMath.Add(environment, this.BasicMath.Multiply(environment, denominator.Segments, denominator.Fragment.Denominator.Segments), denominator.Fragment.Numerator.Segments);
                    bFraction = new Fraction(numerator.Environment, bDividend, environment.KeyNumber[1].Segments);

                }
                else if (aFraction != default(Fraction))
                {
                    bFraction = new Fraction(denominator.Environment, denominator.Segments, environment.KeyNumber[1].Segments);
                }

                Fraction fractionResult = aFraction / bFraction;
                numerator = fractionResult.Numerator;
                denominator = fractionResult.Denominator;
            }

            Tuple<NumberSegments, NumberSegments, NumberSegments> resultSegments = this.BasicMath.Divide(environment, numerator.Segments, denominator.Segments);
            if (resultSegments.Item2 != default(NumberSegments) && resultSegments.Item3 != default(NumberSegments))
            {
                return new Number(environment, resultSegments.Item1, resultSegments.Item2, resultSegments.Item3, false);
            }
            else
            {
                return new Number(environment, resultSegments.Item1, null, false);
            }
        }

        public int Compare(Number a, Object b)
        {
        //    if (typeof(b).IsPrimitive)
        //    {

        //    }

            return 0;
        }

        public int Compare(Number a, Number b)
        {
            if (Object.ReferenceEquals(a.Environment, default(IMathEnvironment)) || (Number.IsBottom(a)) && Object.ReferenceEquals(a.Fragment, default(Fraction)))
            {
                if (Object.ReferenceEquals(b.Environment, default(IMathEnvironment)) || (Number.IsBottom(b) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (Object.ReferenceEquals(b.Environment, default(IMathEnvironment)) || (Number.IsBottom(b) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
            {
                return 0;
            }

            if (a.Environment != b.Environment)
            {
                throw new Exception("Dividing differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = a.Environment;


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
            if (number.Environment == default(IMathEnvironment) || number.Segments == default(NumberSegments) || number.Segments.Size == 0)
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

        public Boolean IsPrime(Number number)
        {
            if (number.Fragment != default(Fraction))
            {
                return false;
            }
            return this.PrimeAlgorithm.IsPrime(number.Environment, this.BasicMath,  number.Segments);
        }


        public Tuple<Number, Number> GetComposite(Number number)
        {
            Tuple<NumberSegments, NumberSegments> tuple = this.PrimeAlgorithm.GetComposite(number.Environment, this.BasicMath, number.Segments);

            if (tuple == default(Tuple<NumberSegments, NumberSegments>))
            {
                return default(Tuple<Number, Number>);
            }

            return new Tuple<Number, Number>(new Number(number.Environment, tuple.Item1, null, false),
                new Number(number.Environment, tuple.Item2, null, false));
        }

        public Number Convert(IMathEnvironment environment, Number number)
        {
            Debug.WriteLine(String.Format("Convert {0} to Base {1}", number, environment.Base));

            Number result = environment.KeyNumber[0];
            if (number == number.Environment.KeyNumber[1])
            {
                return environment.KeyNumber[1];
            }
            else if (number == number.Environment.SecondNumber)
            {
                return environment.SecondNumber;
            }
            else if(!this.IsBottom(number))
            {
                Decimal currentNumber = Decimal.MaxValue;
                Number tempNumber = number;
                
                while (currentNumber > 1)
                {
                    NumberSegments numberDivider = this.BasicMath.AsSegments(number.Environment, currentNumber);
                    NumberSegments resultMultiplier = this.BasicMath.AsSegments(environment, currentNumber);
                    while (this.BasicMath.IsGreaterThan(number.Environment, tempNumber.Segments, numberDivider))
                    {
                        tempNumber -= new Number(tempNumber.Environment, numberDivider, null, false);
                        result += new Number(result.Environment, resultMultiplier, null, false);
                    }
                    currentNumber = (Decimal)Math.Floor(Math.Sqrt((Double)currentNumber));
                }
                
                while (tempNumber >= tempNumber.Environment.KeyNumber[1])
                {
                    tempNumber -= tempNumber.Environment.KeyNumber[1];
                    result += result.Environment.KeyNumber[1];
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

            IMathEnvironment environment = number.Environment;

            Number numerator;
            Number denominator;

            if (number.Fragment == default(Fraction))
            {
                numerator = number;
                denominator = environment.KeyNumber[1];
            }
            else
            {
                numerator = this.Add(this.Multiply(number, number.Fragment.Denominator), number.Fragment.Numerator);
                denominator = number.Fragment.Denominator;

                if (number.Fragment.Numerator.Fragment != default(Fraction) || number.Fragment.Denominator.Fragment != default(Fraction))
                {
                    Fraction numeratorFraction = this.AsFraction(numerator);
                    if (number.Fragment.Numerator.Fragment != default(Fraction))
                    {
                        numeratorFraction = this.AsFraction(numerator) + number.Fragment.Numerator.Fragment;
                    }

                    Fraction denominatorFraction = this.AsFraction(numerator);
                    if (number.Fragment.Numerator.Fragment != default(Fraction))
                    {
                        denominatorFraction = this.AsFraction(denominator) + number.Fragment.Denominator.Fragment;
                    }

                    Fraction fractionResult = numeratorFraction / denominatorFraction;
                    numerator = fractionResult.Numerator;
                    denominator = fractionResult.Denominator;
                }
            }

            return new Fraction(numerator, denominator);
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
    }
}
