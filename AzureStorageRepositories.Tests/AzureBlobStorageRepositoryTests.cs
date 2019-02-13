using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VariableBase.Mathematics;

namespace AzureStorageRepositories.Tests
{
    [TestClass]
    public class AzureBlobStorageRepositoryTests
    {
        [TestMethod]
        public void AzureBlobStorageTest_1()
        {
            AzureBlobStorageRepository repository = new AzureBlobStorageRepository();
            String expectedResult = "test3";
            repository.Create("test", "test2", "test3");

            var actualResult = repository.Get("test", "test2");

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
