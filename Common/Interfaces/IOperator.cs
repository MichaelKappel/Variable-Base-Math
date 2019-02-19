using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IOperator<T> : IComparer<T>
    {
        T Square(T number);
        T SquareRoot(T number);
        T Add(T a, T b);
        T Subtract(T a, T b);
        T Multiply(T a, T b);
        T Divide(T a, T b, T hint = default(T));
        Boolean Equals(T a, T b);
        Boolean IsGreaterThan(T a, T b);
        Boolean IsLessThan(T a, T b);
        Boolean IsGreaterThanOrEqualTo(T a, T b);
        Boolean IsLessThanOrEqualTo(T a, T b);
        Boolean IsBottom(T number);
        Boolean IsEven(T number);
        T Convert(IMathEnvironment<T> environment, T number);
        T ConvertToBase10(T number);
    }
}
