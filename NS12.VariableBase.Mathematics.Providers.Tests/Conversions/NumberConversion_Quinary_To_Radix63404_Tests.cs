using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Providers.Utilities;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("Conversion")]
[TestCategory("FromQuinary")]
[TestCategory("ToRadix63404")]
public class NumberConversion_Quinary_To_Radix63404_Tests
{
    [DataTestMethod]
    [DynamicData(nameof(GetCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public void Convert_ReturnsExpectedRepresentation_FromQuinaryToRadix63404(string sourceName, string targetName, int caseIndex, string label, string wholeBase10, bool isNegative, string numeratorBase10, string denominatorBase10)
    {
        NumberConversion_TestHelper.AssertConversion(sourceName, "01234", targetName, UnicodeRadixKeyProvider.Radix63404Key, caseIndex, label, wholeBase10, isNegative, numeratorBase10, denominatorBase10);
    }

    public static IEnumerable<object[]> GetCases()
    {
        return NumberConversion_TestHelper.GetCases("quinary", "radix63404");
    }

    public static string GetCaseDisplayName(MethodInfo methodInfo, object[] data)
    {
        return NumberConversion_TestHelper.GetDisplayName(methodInfo, data);
    }
}
