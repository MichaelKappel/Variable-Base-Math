using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VariableBase.Mathematics.Interfaces;
using VariableBase.Mathematics.Models;

namespace VariableBase.Mathematics.Interfaces
{
    public interface IPrimeAlgorithm
    {
        Boolean IsPrime(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments number);
        Tuple<NumberSegments, NumberSegments> GetComposite(IMathEnvironment environment, IBasicMathAlgorithm basicMath, NumberSegments number);
    }
}
