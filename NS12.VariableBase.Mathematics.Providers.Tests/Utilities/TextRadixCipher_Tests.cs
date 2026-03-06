using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Providers.Utilities;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("TextRadixCipher")]
public class TextRadixCipher_Tests
{
    [TestMethod]
    public void EncryptThenDecrypt_RoundTrips_MessageAndSecret()
    {
        TextRadixCipherResult encrypted = TextRadixCipher.Encrypt("HELLO WORLD", "KEY42");
        TextRadixCipherResult decrypted = TextRadixCipher.Decrypt(encrypted.CipherText, encrypted.SecretText, encrypted.RadixKey, encrypted.Radix);

        Assert.AreEqual("HELLO WORLD", decrypted.MessageText);
    }

    [TestMethod]
    public void Encrypt_UsesReservedZeroSymbol_NotPresentInInputs()
    {
        TextRadixCipherResult encrypted = TextRadixCipher.Encrypt("mathematics", "secret");
        char zeroSymbol = encrypted.RadixKey[0];

        Assert.IsFalse((encrypted.MessageText + encrypted.SecretText).Contains(zeroSymbol));
    }

    [TestMethod]
    public void Decrypt_Throws_WhenSuppliedRadixDoesNotMatchRadixKey()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() => TextRadixCipher.Decrypt("cipher", "secret", "~cipherst", 5));
        StringAssert.Contains(ex.Message, "Base/radix mismatch");
    }
}
