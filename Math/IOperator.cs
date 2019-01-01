using System;
using System.Collections.Generic;
using System.Text;

namespace Math
{
    public interface IOperator
    {

        Number Add(Number a, Number b);

        Number Subtract(Number a, Number b);

        Number Multiply(Number a, Number b);

        Number Divide(Number a, Number b);

        Number PowerOf(Number a, Int32 power);
 
    }
}
