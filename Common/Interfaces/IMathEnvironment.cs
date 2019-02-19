using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common.Models;

namespace Common.Interfaces
{
    public interface IMathEnvironment<T>: IEquatable<IMathEnvironment<T>>
    {
        String ConvertToString(NumberSegments segments);
        String GetDefinition();
        Decimal Base { get; }
        ReadOnlyCollection<Char> Key { get; }
        T PowerOfFirstNumber { get; }
        T SecondNumber { get; }
        Boolean Equals(Object other);
        Int32 GetHashCode();
        T GetNumber(String wholeNumber, String fractionNumerator = null, String fractionDenominator = null, Boolean isNegative = false);
        T GetNumber(String[] wholeNumberSegments, Boolean isNegative = false);
        T GetNumber(NumberSegments segments, Boolean isNegative = false);
        T GetNumber(Decimal number);
        void SetupMathEnvironment();
        String ToString();
        void ValidateFraction(List<Char> numerator, List<Char> denominator);
        void ValidateWholeNumber(List<Char> numberSegments);
        Decimal GetIndex(Char arg);
        NumberSegments ParseNumberSegments(String raw);
        T OpenNumberFile(String folderName, String fileName);
    }
}