using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using NS12.VariableBase.Mathematics.Common.Models;

namespace NS12.VariableBase.Mathematics.Common.Interfaces
{
    public interface IBasicMathAlgorithm<T>
    {
        NumberSegments ConvertToBase10(IMathEnvironment<T> base10Environment, IMathEnvironment<T> currentEnvironment, NumberSegments segments);
        NumberSegments AsSegments(IMathEnvironment<T> environment, decimal rawDouble);
        NumberSegments Add(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<T> environment, NumberSegments numerator, NumberSegments denominator, NumberSegments hint = null);
        NumberSegments GetWholeNumberSomewhereBetween(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b, decimal variance = 0);
        int CompareTo(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        bool IsEqual(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        bool IsEven(IMathEnvironment<T> environment, NumberSegments a);
        bool IsGreaterThan(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        bool IsGreaterThanOrEqualTo(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        bool IsLessThan(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        bool IsLessThanOrEqualTo(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        bool IsNotEqual(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        bool IsOdd(IMathEnvironment<T> environment, NumberSegments a);
        NumberSegments Multiply(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        NumberSegments PowerOfBase(IMathEnvironment<T> environment, NumberSegments a, decimal times);
        NumberSegments Square(IMathEnvironment<T> environment, NumberSegments a);
        (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) SquareRoot(IMathEnvironment<T> environment, NumberSegments number);
        NumberSegments Subtract(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
    }
}
