using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VariableBase.Mathematics.Models;

namespace VariableBase.Mathematics.Interfaces
{
    public interface IBasicMathAlgorithm
    {
        NumberSegments AsSegments(IMathEnvironment environment, Decimal rawDouble);
        NumberSegments Add(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        Tuple<NumberSegments, NumberSegments, NumberSegments> Divide(IMathEnvironment environment,NumberSegments numerator, NumberSegments denominator, NumberSegments hint = null);
        NumberSegments GetWholeNumberSomewhereBetween(IMathEnvironment environment,NumberSegments a, NumberSegments b, Decimal variance = 0);
        Boolean IsEqual(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        Boolean IsEven(IMathEnvironment environment,NumberSegments a);
        Boolean IsGreaterThan(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        Boolean IsGreaterThanOrEqualTo(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        Boolean IsLessThan(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        Boolean IsLessThanOrEqualTo(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        Boolean IsNotEqual(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        Boolean IsOdd(IMathEnvironment environment,NumberSegments a);
        NumberSegments Multiply(IMathEnvironment environment,NumberSegments a, NumberSegments b);
        NumberSegments PowerOfBase(IMathEnvironment environment,NumberSegments a, Decimal times);
        NumberSegments Square(IMathEnvironment environment,NumberSegments a);
        Tuple<NumberSegments, NumberSegments, NumberSegments> SquareRoot(IMathEnvironment environment, NumberSegments number);
        NumberSegments Subtract(IMathEnvironment environment,NumberSegments a, NumberSegments b);
    }
}
