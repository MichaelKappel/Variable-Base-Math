using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("NumberOperator")]
[TestCategory("Matrix")]
[TestCategory("Quinary")]
public class NumberOperator_Arithmetic_Quinary_Tests
{
    [DataTestMethod]
    [DynamicData(nameof(GetCases), DynamicDataSourceType.Method, DynamicDataDisplayName = nameof(GetCaseDisplayName))]
    public void ArithmeticOperators_ReturnExpectedResults_ForQuinaryInputs(string baseName, string key, int caseIndex, string firstRaw, string secondRaw)
    {
        NumberOperator_Arithmetic_TestHelper.AssertArithmeticOperatorsAcrossBase(baseName, key, caseIndex, firstRaw, secondRaw);
    }

    public static IEnumerable<object[]> GetCases()
    {
        return NumberOperator_Arithmetic_TestHelper.GetArithmeticCases("quinary", "01234");
    }

    public static string GetCaseDisplayName(MethodInfo methodInfo, object[] data)
    {
        return NumberOperator_Arithmetic_TestHelper.GetArithmeticCaseDisplayName(methodInfo, data);
    }
}
