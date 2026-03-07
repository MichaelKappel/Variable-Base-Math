using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Providers.Utilities;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("Conversion")]
[TestCategory("FromRadix63404")]
[TestCategory("ToOctal")]
public class NumberConversion_Radix63404_To_Octal_Tests
{
    [DataTestMethod]
    [DynamicData(nameof(GetCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public void Convert_ReturnsExpectedRepresentation_FromRadix63404ToOctal(string sourceName, string targetName, int caseIndex, string label, string wholeBase10, bool isNegative, string numeratorBase10, string denominatorBase10)
    {
        NumberConversion_TestHelper.AssertConversion(sourceName, UnicodeRadixKeyProvider.Radix63404Key, targetName, "01234567", caseIndex, label, wholeBase10, isNegative, numeratorBase10, denominatorBase10);
    }

    public static IEnumerable<object[]> GetCases()
    {
        return NumberConversion_TestHelper.GetCases("radix63404", "octal");
    }

    public static string GetCaseDisplayName(MethodInfo methodInfo, object[] data)
    {
        return NumberConversion_TestHelper.GetDisplayName(methodInfo, data);
    }
}
