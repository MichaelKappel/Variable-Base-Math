using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Math
{
    public class NumberOperator : INumberOperator
    {
        public Number Add(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Add(a.Segments, b.Segments);

            return new Number(environment, resultSegments, null, false);
        }

        public Number Subtract(Number a, Number b)
        {

            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Subtract(a.Segments, b.Segments);

            return new Number(environment, resultSegments, null, false);
        }

        public Number Multiply(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Multipling differnt enviorments is not currently supported");
            }

            MathEnvironment environment = a.Environment;

            if (a.Fragment == default(Fraction) && b.Fragment == default(Fraction))
            {
                ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Multiply(a.Segments, b.Segments);
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
            MathEnvironment environment = a.Environment;

            Tuple<ReadOnlyCollection<Char>,ReadOnlyCollection<Char>,ReadOnlyCollection<Char>> resultSegments = environment.Algorithm.Divide(a.Segments, b.Segments);
            if (resultSegments.Item2 != default(ReadOnlyCollection<Char>) && resultSegments.Item3 != default(ReadOnlyCollection<Char>))
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
            if (a.Environment == default(MathEnvironment) || (a.Environment.Algorithm.IsBottom(a.Segments)) && Object.ReferenceEquals(a.Fragment, default(Fraction)))
            {
                if (b.Environment == default(MathEnvironment) || (b.Environment.Algorithm.IsBottom(a.Segments) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (b.Environment == default(MathEnvironment) || (b.Environment.Algorithm.IsBottom(a.Segments) && Object.ReferenceEquals(b.Fragment, default(Fraction))))
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
            
            MathEnvironment environment = a.Environment;
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
            if (number.Environment == default(MathEnvironment) || number.Segments == default(ReadOnlyCollection<Char>) || number.Segments.Count == 0)
            {
                return true;
            }
            else if (number.Segments.Count == 1 && number.Segments[0] == number.Environment.Bottom)
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
