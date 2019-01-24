using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VariableBase.Mathematics.Interfaces
{
    public interface IPrimeAlgorithm
    {
        Boolean IsPrime(ReadOnlyCollection<Decimal> number);
        Tuple<ReadOnlyCollection<Decimal>, ReadOnlyCollection<Decimal>> GetComposite(ReadOnlyCollection<Decimal> number);
    }
}
