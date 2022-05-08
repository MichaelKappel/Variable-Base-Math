
using NS12.VariableBase.Mathematics.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Text;

namespace NS12.VariableBase.Mathematics.Common.Models
{
    public class PrimeModel<T>
    {
        public IMathEnvironment<T> MathEnvironment { get; set; }
        public T Prime { get; set; }
        public DateTime StartDate { get; set; }
    }
}
