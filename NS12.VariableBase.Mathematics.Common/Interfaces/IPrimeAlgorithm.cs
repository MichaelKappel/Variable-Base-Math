using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using NS12.VariableBase.Mathematics.Common.Models;

namespace NS12.VariableBase.Mathematics.Common.Interfaces
{
    public interface IPrimeAlgorithm<T>
    {
        bool IsPrime(T number);
        (T Numerator, T Denominator) GetComposite(T number);
    }
}
