using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VariableBase.Mathematics.Interfaces
{
    public interface IBasicMathAlgorithm
    {
        IList<Double> AsSegments(Double rawDouble);
        ReadOnlyCollection<Double> Add(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> Divide(ReadOnlyCollection<Double> numerator, ReadOnlyCollection<Double> denominator, ReadOnlyCollection<Double> hint = null);
        ReadOnlyCollection<Double> GetWholeNumberSomewhereBetween(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b, Double variance = 0);
        Boolean IsBottom(ReadOnlyCollection<Double> number);
        Boolean IsEqual(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        Boolean IsEven(ReadOnlyCollection<Double> a);
        Boolean IsFirst(ReadOnlyCollection<Double> number);
        Boolean IsGreaterThan(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        Boolean IsGreaterThanOrEqualTo(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        Boolean IsLessThan(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        Boolean IsLessThanOrEqualTo(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        Boolean IsNotEqual(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        Boolean IsOdd(ReadOnlyCollection<Double> a);
        ReadOnlyCollection<Double> Multiply(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
        ReadOnlyCollection<Double> PowerOfBase(ReadOnlyCollection<Double> a, Int32 times);
        ReadOnlyCollection<Double> Square(ReadOnlyCollection<Double> a);
        Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> SquareRoot(ReadOnlyCollection<Double> number);
        ReadOnlyCollection<Double> Subtract(ReadOnlyCollection<Double> a, ReadOnlyCollection<Double> b);
    }
}
