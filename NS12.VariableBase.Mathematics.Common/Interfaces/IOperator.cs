
using System;
using System.Collections.Generic;
using System.Text;

namespace NS12.VariableBase.Mathematics.Common.Interfaces
{
    public interface IOperator<T> : IComparer<T>
    {
        T Square(T number);
        T SquareRoot(T number);
        T Add(T a, T b);
        T Subtract(T a, T b);
        T Multiply(T a, T b);
        T Divide(T a, T b, T hint = default);
        bool Equals(T a, T b);
        bool IsGreaterThan(T a, T b);
        bool IsLessThan(T a, T b);
        bool IsGreaterThanOrEqualTo(T a, T b);
        bool IsLessThanOrEqualTo(T a, T b);
        bool IsBottom(T number);
        bool IsEven(T number);
        T Convert(IMathEnvironment<T> environment, T number);
        T ConvertToBase10(T number);
    }
}
