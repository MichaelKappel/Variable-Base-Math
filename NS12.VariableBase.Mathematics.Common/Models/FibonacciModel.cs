
using NS12.VariableBase.Mathematics.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace NS12.VariableBase.Mathematics.Common.Models
{
    public class FibonacciModel<T>
    {
        public IMathEnvironment<T> MathEnvironment { get; set; }
        public T First { get; set; }
        public T Second { get; set; }
        public DateTime StartDate { get; set; }
    }
}
