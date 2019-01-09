using System;
using System.Collections.Generic;
using System.Text;

namespace Math.Interfaces
{
    public interface IWholeNumberOperator
    {
        WholeNumber Add(WholeNumber a, WholeNumber b);

        WholeNumber Subtract(WholeNumber a, WholeNumber b);

        WholeNumber Multiply(WholeNumber a, WholeNumber b);

        Number Divide(WholeNumber a, WholeNumber b);
    }
}
