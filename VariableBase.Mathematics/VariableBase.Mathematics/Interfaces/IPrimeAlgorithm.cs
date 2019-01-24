using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VariableBase.Mathematics.Interfaces
{
    public interface IPrimeAlgorithm
    {
        Boolean IsPrime(ReadOnlyCollection<Double> number);
        Tuple<ReadOnlyCollection<Double>, ReadOnlyCollection<Double>> GetComposite(ReadOnlyCollection<Double> number);
    }
}
