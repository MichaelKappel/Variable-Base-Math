using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NS12.VariableBase.Mathematics.Common.Models;

namespace NS12.VariableBase.Mathematics.Common.Interfaces
{
    public interface IMathEnvironment<T> : IEquatable<IMathEnvironment<T>>
    {
        string ConvertToString(NumberSegments segments);
        string GetDefinition();
        decimal Base { get; }
        ReadOnlyCollection<char> Key { get; }
        T PowerOfFirstNumber { get; }
        T SecondNumber { get; }
        bool Equals(object other);
        int GetHashCode();
        T GetNumber(string wholeNumber, string fractionNumerator = null, string fractionDenominator = null, bool isNegative = false);
        T GetNumber(string[] wholeNumberSegments, bool isNegative = false);
        T GetNumber(NumberSegments segments, bool isNegative = false);
        T GetNumber(decimal number);
        void SetupMathEnvironment();
        string ToString();
        void ValidateFraction(List<char> numerator, List<char> denominator);
        void ValidateWholeNumber(List<char> numberSegments);
        decimal GetIndex(char arg);
        NumberSegments ParseNumberSegments(string raw);
        T OpenNumberFile(string folderName, string fileName);
    }
}