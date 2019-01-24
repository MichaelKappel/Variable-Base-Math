using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VariableBase.Mathematics.Interfaces;

namespace VariableBase.Mathematics
{
    public interface IMathEnvironment: IEquatable<DecimalMathEnvironment>
    {
        IBasicMathAlgorithm BasicMath { get; set; }
        IPrimeAlgorithm PrimeAlgorithm { get; set; }
        Decimal Base { get; }
        ReadOnlyCollection<Char> Key { get; }
        ReadOnlyCollection<Number> KeyNumber { get; }
        Number PowerOfFirstNumber { get; }
        Number SecondNumber { get; }
        Number ConvertToFraction(Decimal numberRaw, Decimal numeratorNumber, Decimal denominatorRaw);
        Boolean Equals(Object other);
        Int32 GetHashCode();
        Decimal GetIndex(Char arg);
        Number GetNumber(Int32 zeros, Boolean isNegative = false);
        Number GetNumber(String wholeNumber, String fractionNumerator = null, String fractionDenominator = null, Boolean isNegative = false);
        Number GetNumber(String[] wholeNumberSegments, Boolean isNegative = false);
        void SetupMathEnvironment();
        String ToString();
        void ValidateFraction(List<Char> numerator, List<Char> denominator);
        void ValidateWholeNumber(List<Char> numberSegments);
    }
}