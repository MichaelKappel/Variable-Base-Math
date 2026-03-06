using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("NumberOperator")]
[TestCategory("Matrix")]
[TestCategory("Hexadecimal")]
public class NumberOperator_Arithmetic_Hexadecimal_Tests
{
    [DataTestMethod]
    [DynamicData(nameof(GetCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public void ArithmeticOperators_ReturnExpectedResults_ForHexadecimalInputs(string baseName, string key, int caseIndex, string firstRaw, string secondRaw)
    {
        NumberOperator_Arithmetic_TestHelper.AssertArithmeticOperatorsAcrossBase(baseName, key, caseIndex, firstRaw, secondRaw);
    }

    public static IEnumerable<object[]> GetCases()
    {
        return NumberOperator_Arithmetic_TestHelper.GetArithmeticCases("hexadecimal", "0123456789abcdef");
    }

    public static string GetCaseDisplayName(MethodInfo methodInfo, object[] data)
    {
        return NumberOperator_Arithmetic_TestHelper.GetArithmeticCaseDisplayName(methodInfo, data);
    }
}
