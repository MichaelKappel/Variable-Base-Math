
using NS12.VariableBase.Mathematics.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace NS12.VariableBase.Mathematics.Common.Models
{
    public class FibonacciModel<T>
    {
        public IMathEnvironment<T> MathEnvironment { get; set; } = default!;
        public T First { get; set; } = default!;
        public T Second { get; set; } = default!;
        public DateTime StartDate { get; set; }
    }
}
