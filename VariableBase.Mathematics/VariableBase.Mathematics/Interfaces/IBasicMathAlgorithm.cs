using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VariableBase.Mathematics.Interfaces
{
    public interface IBasicMathAlgorithm
    {
        IList<Decimal> AsSegments(Double rawDouble);
        ReadOnlyCollection<Decimal> Add(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> Divide(ReadOnlyCollection<Decimal> numerator, ReadOnlyCollection<Decimal> denominator, ReadOnlyCollection<Decimal> hint = null);
        ReadOnlyCollection<Decimal> GetWholeNumberSomewhereBetween(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b, Double variance = 0);
        Boolean IsBottom(ReadOnlyCollection<Decimal> number);
        Boolean IsEqual(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        Boolean IsEven(ReadOnlyCollection<Decimal> a);
        Boolean IsFirst(ReadOnlyCollection<Decimal> number);
        Boolean IsGreaterThan(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        Boolean IsGreaterThanOrEqualTo(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        Boolean IsLessThan(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        Boolean IsLessThanOrEqualTo(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        Boolean IsNotEqual(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        Boolean IsOdd(ReadOnlyCollection<Decimal> a);
        ReadOnlyCollection<Decimal> Multiply(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
        ReadOnlyCollection<Decimal> PowerOfBase(ReadOnlyCollection<Decimal> a, Int32 times);
        ReadOnlyCollection<Decimal> Square(ReadOnlyCollection<Decimal> a);
        Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> SquareRoot(ReadOnlyCollection<Decimal> number);
        ReadOnlyCollection<Decimal> Subtract(ReadOnlyCollection<Decimal> a, ReadOnlyCollection<Decimal> b);
    }
}
