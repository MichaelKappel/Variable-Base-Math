using VariableBase.Mathematics.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using static VariableBase.Mathematics.BasicMathAlgorithm;

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

            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting differnt enviorments is not currently supported");
            }
            IMathEnvironment environment = a.Environment;

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

            Tuple<ReadOnlyCollection<Decimal>,ReadOnlyCollection<Decimal>,ReadOnlyCollection<Decimal>> resultSegments = environment.BasicMath.Divide(a.Segments, b.Segments);
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
            if (Object.ReferenceEquals(a.Environment, default(MathEnvironment)) || (a.Environment.BasicMath.IsBottom(a.Segments)) && Object.ReferenceEquals(a.Fragment, default(Fraction)))
            {
                if (Object.ReferenceEquals(b.Environment, default(MathEnvironment)) || (b.Environment.BasicMath.IsBottom(a.Segments) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (Object.ReferenceEquals(b.Environment, default(MathEnvironment)) || (b.Environment.BasicMath.IsBottom(a.Segments) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
            {
                return -1;
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

        public Boolean Equals(Number a, Number b)
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
            Boolean[] binary = this.AsBinary(number);
            return environment.AsNumber(binary);
        }

        public Boolean[] AsBinary(Number number)
        {

            var resultSegments = new List<Boolean>();
            if (this.IsBottom(number))
            {
                resultSegments.Add(false);
            }
            else
            {
                Number tempNumber = number;
                while (tempNumber > number.Environment.KeyNumber[1])
                {
                    if (this.IsOdd(tempNumber))
                    {
                        resultSegments.Add(true);
                    }
                    else
                    {
                        resultSegments.Add(false);
                    }
                    tempNumber = (tempNumber / tempNumber.Environment.SecondNumber).Floor();
                }
                resultSegments.Add(true);
            }
            return resultSegments.ToArray();
        }

        public Number AsBinaryNumber(Number number)
        {
            IMathEnvironment binaryEnvironment = new MathEnvironment("01");
            if (number.Environment == binaryEnvironment)
            {
                return number;
            }

            Boolean[] binary = this.AsBinary(number);
            
            return new Number(binaryEnvironment, new ReadOnlyCollection<Decimal>(binary.Select((x)=> (Decimal)(x ? 1 : 0)).ToArray()), null, number.IsNegative);
        }
    }
}
