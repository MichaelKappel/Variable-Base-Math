using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Math
{
    public class NumberOperator: INumberOperator
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
    }
}
