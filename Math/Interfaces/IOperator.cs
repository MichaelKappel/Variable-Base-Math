using System;
using System.Collections.Generic;
using System.Text;

namespace Math.Interfaces
{
    public interface IOperator: IComparer<Number>
    {
        Number Add(Number a, Number b);

        Number Subtract(Number a, Number b);

        Number Multiply(Number a, Number b);

        Number Divide(Number a, Number b);

        Boolean Equals(Number a, Number b);
    }
}
