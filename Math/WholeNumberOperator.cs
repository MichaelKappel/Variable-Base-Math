using Math.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Math
{
    public class WholeNumberOperator: IWholeNumberOperator
    {
        public WholeNumber Add(WholeNumber a, WholeNumber b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Add(a.Segments, b.Segments);

            return new WholeNumber(environment, resultSegments, false);
        }

        public WholeNumber Subtract(WholeNumber a, WholeNumber b)
        {

            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Subtract(a.Segments, b.Segments);

            return new WholeNumber(environment, resultSegments, false);
        }

        public WholeNumber Multiply(WholeNumber a, WholeNumber b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
            }
            MathEnvironment environment = a.Environment;

            ReadOnlyCollection<Char> resultSegments = environment.Algorithm.Multiply(a.Segments, b.Segments);

            return new WholeNumber(environment, resultSegments, false);
        }

        public Number Divide(WholeNumber a, WholeNumber b)
        {
            if (a.Environment != b.Environment)
            {
                throw new Exception("Adding differnt enviorments is not currently supported");
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
