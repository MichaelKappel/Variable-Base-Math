using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Math
{
    public class Operator: IOperator
    {
        public Number Add(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Add(a.Segments, b.Segments);

            return new Number(environment, resultSegments, false);
        }

        public Number Subtract(Number a, Number b)
        {

            if (a.Environment != b.Environment)
            {
                throw new Exception("Subtracting differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Subtract(a.Segments, b.Segments);

            return new Number(environment, resultSegments, false);
        }

        public Number Multiply(Number a, Number b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Multipling differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Multiply(a.Segments, b.Segments);

            return new Number(environment, resultSegments, false);
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
                return new Number(environment, resultSegments.Item1, false);
            }
        }
        public int Compare(Number a, Number b)
        {
            if (this.Equals(a, b))
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
            //Check if "a" is 0 check to see if b is also
            if (a.Environment.Algorithm.IsBottom(a.Segments) && a.Fragment == default(Fraction))
            {
                if (Object.ReferenceEquals(b, null))
                {
                    return true;
                }
                else if (b.Environment.Algorithm.IsBottom(b.Segments) && b.Fragment == default(Fraction))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //Check if one is negative and the other is positive
            if (a.IsNegative != b.IsNegative)
            {
                return false;
            }

            //Check if one has a fraction but the other does not
            if (a.Fragment == default(Fraction) && b.Fragment != default(Fraction) 
                || b.Fragment == default(Fraction) && a.Fragment != default(Fraction))
            {
                return false;
            }

            //"a" and "b" are not 0 
            MathEnvironment environment = a.Environment;
            //Check if Environments all match
            if (b.Environment != environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            else if (a.Fragment != default(Fraction) && a.Fragment.Numerator.Environment != environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            else if (b.Fragment != default(Fraction) && b.Fragment.Numerator.Environment != environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }

            if (environment.Algorithm.IsEqual(a.Segments, b.Segments))
            {
                if (a.Fragment == b.Fragment)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
