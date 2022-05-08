using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Common.Models;

namespace Common.Interfaces
{
    public interface IBasicMathAlgorithm<T>
    {
        NumberSegments ConvertToBase10(IMathEnvironment<T> base10Environment, IMathEnvironment<T> currentEnvironment, NumberSegments segments);
        NumberSegments AsSegments(IMathEnvironment<T> environment, Decimal rawDouble);
        NumberSegments Add(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) Divide(IMathEnvironment<T> environment,NumberSegments numerator, NumberSegments denominator, NumberSegments hint = null);
        NumberSegments GetWholeNumberSomewhereBetween(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b, Decimal variance = 0);
        Int32 CompareTo(IMathEnvironment<T> environment, NumberSegments a, NumberSegments b);
        Boolean IsEqual(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        Boolean IsEven(IMathEnvironment<T> environment,NumberSegments a);
        Boolean IsGreaterThan(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        Boolean IsGreaterThanOrEqualTo(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        Boolean IsLessThan(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        Boolean IsLessThanOrEqualTo(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        Boolean IsNotEqual(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        Boolean IsOdd(IMathEnvironment<T> environment,NumberSegments a);
        NumberSegments Multiply(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
        NumberSegments PowerOfBase(IMathEnvironment<T> environment,NumberSegments a, Decimal times);
        NumberSegments Square(IMathEnvironment<T> environment,NumberSegments a);
        (NumberSegments Whole, NumberSegments Numerator, NumberSegments Denominator) SquareRoot(IMathEnvironment<T> environment, NumberSegments number);
        NumberSegments Subtract(IMathEnvironment<T> environment,NumberSegments a, NumberSegments b);
    }
}
