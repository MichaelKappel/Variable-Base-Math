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

        Number PowerOf(Number a, Int64 power);


        WholeNumber Add(WholeNumber a, WholeNumber b);

        WholeNumber Subtract(WholeNumber a, WholeNumber b);

        WholeNumber Multiply(WholeNumber a, WholeNumber b);

        Number Divide(WholeNumber a, WholeNumber b);

        WholeNumber PowerOf(WholeNumber a, UInt64 power);

    }
}
