using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VariableBase.Mathematics.Interfaces
{
    public interface INumberOperator: IComparer<Number>
    {
        Number Add(Number a, Number b);
        Number Subtract(Number a, Number b);
        Number Multiply(Number a, Number b);
        Number Divide(Number a, Number b);
        Boolean Equals(Number a, Number b);
        Boolean IsGreaterThan(Number a, Number b);
        Boolean IsLessThan(Number a, Number b);
        Boolean IsGreaterThanOrEqualTo(Number a, Number b);
        Boolean IsLessThanOrEqualTo(Number a, Number b);
        Boolean IsBottom(Number number);
        bool IsEven(Number number);
        bool IsPrime(Number number);
        Number Convert(IMathEnvironment environment, Number number);
        Number AsBinaryNumber(Number number);
        Tuple<Number, Number> GetComposite(Number number);
    }
}
