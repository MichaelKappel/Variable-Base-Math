using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("Conversion")]
[TestCategory("FromBase36")]
[TestCategory("ToBinary")]
public class NumberConversion_Base36_To_Binary_Tests
{
    [DataTestMethod]
    [DynamicData(nameof(GetCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public void Convert_ReturnsExpectedRepresentation_FromBase36ToBinary(string sourceName, string targetName, int caseIndex, string label, string wholeBase10, bool isNegative, string numeratorBase10, string denominatorBase10)
    {
        NumberConversion_TestHelper.AssertConversion(sourceName, "0123456789abcdefghijklmnopqrstuvwxyz", targetName, "01", caseIndex, label, wholeBase10, isNegative, numeratorBase10, denominatorBase10);
    }

    public static IEnumerable<object[]> GetCases()
    {
        return NumberConversion_TestHelper.GetCases("base36", "binary");
    }

    public static string GetCaseDisplayName(MethodInfo methodInfo, object[] data)
    {
        return NumberConversion_TestHelper.GetDisplayName(methodInfo, data);
    }
}
