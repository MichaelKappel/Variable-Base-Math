using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("NumberOperator")]
[TestCategory("Matrix")]
[TestCategory("Binary")]
public class NumberOperator_Arithmetic_Binary_Tests
{
    [DataTestMethod]
    [DynamicData(nameof(GetCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public void ArithmeticOperators_ReturnExpectedResults_ForBinaryInputs(string baseName, string key, int caseIndex, string firstRaw, string secondRaw)
    {
        NumberOperator_Arithmetic_TestHelper.AssertArithmeticOperatorsAcrossBase(baseName, key, caseIndex, firstRaw, secondRaw);
    }

    public static IEnumerable<object[]> GetCases()
    {
        return NumberOperator_Arithmetic_TestHelper.GetArithmeticCases("binary", "01");
    }

    public static string GetCaseDisplayName(MethodInfo methodInfo, object[] data)
    {
        return NumberOperator_Arithmetic_TestHelper.GetArithmeticCaseDisplayName(methodInfo, data);
    }
}
