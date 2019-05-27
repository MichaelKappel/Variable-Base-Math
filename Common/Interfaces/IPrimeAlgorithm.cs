using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Common.Models;

namespace Common.Interfaces
{
    public interface IPrimeAlgorithm<T>
    {
        Boolean IsPrime(T number);
         (T Numerator, T Denominator) GetComposite(T number);
    }
}
