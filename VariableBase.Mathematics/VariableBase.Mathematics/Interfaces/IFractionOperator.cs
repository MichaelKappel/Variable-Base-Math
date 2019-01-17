using System;
using System.Collections.Generic;
using System.Text;

namespace VariableBase.Mathematics.Interfaces
{
    public interface IFractionOperator : IComparer<Fraction>
    {
        Fraction Add(Fraction a, Fraction b);
        Fraction Subtract(Fraction a, Fraction b);
        Fraction Multiply(Fraction a, Fraction b);
        Fraction Divide(Fraction a, Fraction b);
        Boolean Equals(Fraction a, Fraction b);
        Boolean IsGreaterThan(Fraction a, Fraction b);
        Boolean IsLessThan(Fraction a, Fraction b);
        Boolean IsGreaterThanOrEqualTo(Fraction a, Fraction b);
        Boolean IsLessThanOrEqualTo(Fraction a, Fraction b);
    }
}
