using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FileRepositories.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            FileStorageRepository repository = new FileStorageRepository();
            String expectedResult = "test3";
            repository.Create("test", "test2", "test3");

            var actualResult = repository.Get("test", "test2");

            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}
