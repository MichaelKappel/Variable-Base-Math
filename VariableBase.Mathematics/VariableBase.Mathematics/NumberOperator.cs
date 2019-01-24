using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using static VariableBase.Mathematics.BasicMathAlgorithm;
using System.Diagnostics;

namespace VariableBase.Mathematics
{
    public class NumberOperator : INumberOperator
    {
        public NumberOperator()
        {

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
                ReadOnlyCollection<Decimal> resultSegments = environment.BasicMath.Add(a.Segments, b.Segments);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = a.AsFraction() + b.AsFraction();
                return aResult.AsNumber();
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
                ReadOnlyCollection<Decimal> resultSegments = environment.BasicMath.Subtract(a.Segments, b.Segments);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = a.AsFraction() - b.AsFraction();
                return aResult.AsNumber();
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
                ReadOnlyCollection<Decimal> resultSegments = environment.BasicMath.Multiply(a.Segments, b.Segments);
                return new Number(environment, resultSegments, null, false);
            }
            else
            {
                Fraction aResult = a.AsFraction() * b.AsFraction();
                return aResult.AsNumber();
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
                    ReadOnlyCollection<Decimal> aDividend = numerator.Environment.BasicMath.Add(numerator.Environment.BasicMath.Multiply(numerator.Segments, numerator.Fragment.Denominator.Segments), numerator.Fragment.Numerator.Segments);
                    aFraction = new Fraction(numerator.Environment, aDividend, environment.KeyNumber[1].Segments);
                }
                else
                {
                    aFraction = new Fraction(numerator.Environment, numerator.Segments, environment.KeyNumber[1].Segments);
                }

                var bFraction = default(Fraction);
                if (denominator.Fragment != default(Fraction))
                {
                    ReadOnlyCollection<Decimal> bDividend = denominator.Environment.BasicMath.Add(denominator.Environment.BasicMath.Multiply(denominator.Segments, denominator.Fragment.Denominator.Segments), denominator.Fragment.Numerator.Segments);
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

            Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> resultSegments = environment.BasicMath.Divide(numerator.Segments, denominator.Segments);
            if (resultSegments.Item2 != default(ReadOnlyCollection<Decimal>) && resultSegments.Item3 != default(ReadOnlyCollection<Decimal>))
            {
                return new Number(environment, resultSegments.Item1, resultSegments.Item2, resultSegments.Item3, false);
            }
            else
            {
                return new Number(environment, resultSegments.Item1, null, false);
            }
        }

        public int Compare(Number a, Number b)
        {
            if (Object.ReferenceEquals(a.Environment, default(DecimalMathEnvironment)) || (a.Environment.BasicMath.IsBottom(a.Segments)) && Object.ReferenceEquals(a.Fragment, default(Fraction)))
            {
                if (Object.ReferenceEquals(b.Environment, default(DecimalMathEnvironment)) || (b.Environment.BasicMath.IsBottom(b.Segments) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (Object.ReferenceEquals(b.Environment, default(DecimalMathEnvironment)) || (b.Environment.BasicMath.IsBottom(b.Segments) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
            {
                return 0;
            }

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
            
            IMathEnvironment environment = a.Environment;
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
            if (a.Segments.Count >  b.Segments.Count)
            {
                result = 1;
            }
            else if (a.Segments.Count <  b.Segments.Count)
            {
                result = -1;
            }

            if (result == 0)
            {
                for (var i = a.Segments.Count - 1; i >= 0; i--)
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
            if (number.Environment == default(IMathEnvironment) || number.Segments == default(ReadOnlyCollection<Decimal>) || number.Segments.Count == 0)
            {
                return true;
            }
            else if (number.Segments.Count == 1 && number.Segments[0] == 0)
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
            return number.Environment.BasicMath.IsEven(number.Segments);
        }

        public Boolean IsPrime(Number number)
        {
            if (number.Fragment != default(Fraction))
            {
                return false;
            }
            return number.Environment.PrimeAlgorithm.IsPrime(number.Segments);
        }


        public Tuple<Number, Number> GetComposite(Number number)
        {
            Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> tuple = number.Environment.PrimeAlgorithm.GetComposite(number.Segments);

            if (tuple == default(Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>>))
            {
                return default(Tuple<Number, Number>);
            }

            return new Tuple<Number, Number>(new Number(number.Environment, tuple.Item1, null, false),
                new Number(number.Environment, tuple.Item2, null, false));
        }

        public Number Convert(IMathEnvironment environment, Number number)
        {
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
                Decimal currentNumber = 100000000000000M;
                Number tempNumber = number;
      
                while (currentNumber > 0)
                {
                    ReadOnlyCollection<Decimal> numberDivider = new ReadOnlyCollection<Decimal>(tempNumber.Environment.BasicMath.AsSegments(currentNumber));
                    ReadOnlyCollection<Decimal> resultMultiplier = new ReadOnlyCollection<Decimal>(result.Environment.BasicMath.AsSegments(currentNumber));
                    while (tempNumber.Environment.BasicMath.IsGreaterThan(tempNumber.Segments, numberDivider))
                    {
                        tempNumber -= new Number(tempNumber.Environment, numberDivider, null, false);
                        result += new Number(result.Environment, resultMultiplier, null, false);
                    }
                    currentNumber = Math.Floor(currentNumber / 2M);
                }
                while (tempNumber >= tempNumber.Environment.KeyNumber[1])
                {
                    tempNumber -= tempNumber.Environment.KeyNumber[1];
                    result += result.Environment.KeyNumber[1];
                }
            }
#if DEBUG
            if ((result.Segments.Count == 1 || result.Environment.Base <= Char.MaxValue)
                && (number.Segments.Count == 1 || number.Environment.Base <= Char.MaxValue))
            {
                String originalNumber;
                if (number.Segments.Count == 1)
                {
                    originalNumber = number.Segments[0].ToString();
                }
                else
                {
                    originalNumber = String.Concat(number.Segments.Reverse().Select(x => x.ToString()));
                }

                String resultNumber;
                if (result.Segments.Count == 1)
                {
                    resultNumber = result.Segments[0].ToString();
                }
                else
                {
                    resultNumber = String.Concat(result.Segments.Reverse().Select(x=>x.ToString()));
                }

                if (originalNumber != resultNumber)
                {
                    throw new Exception(String.Format("GetActualValue Error {0} != {1}", originalNumber, resultNumber));
                }
            }
#endif
            return result;
        }
    }
}
