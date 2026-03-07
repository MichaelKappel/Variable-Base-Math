using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS12.VariableBase.Mathematics.Providers.Utilities;

namespace NS12.VariableBase.Mathematics.Providers.Tests;

[TestClass]
[TestCategory("TextRadixCipher")]
public class TextRadixCipher_Tests
{
    [TestMethod]
    public void EncryptThenDecrypt_RoundTrips_MessageAndSecret_WithoutExplicitRadix()
    {
        TextRadixCipherResult encrypted = TextRadixCipher.Encrypt("HELLO WORLD", "KEY42");
        TextRadixCipherResult decrypted = TextRadixCipher.Decrypt(encrypted.CipherText, encrypted.SecretText, encrypted.RadixKey);

        Assert.AreEqual("HELLO WORLD", decrypted.MessageText);
        Assert.AreEqual(encrypted.Radix, decrypted.Radix);
    }

    [TestMethod]
    public void Encrypt_ProducesExpectedPayload_For_ThisIsATest_And_Hello()
    {
        TextRadixCipherResult encrypted = TextRadixCipher.Encrypt("This is a test", "Hello");

        Assert.AreEqual("otTaetHslott~las~ ", encrypted.CipherText);
        Assert.AreEqual("~This ateHlo", encrypted.RadixKey);
        Assert.AreEqual(12, encrypted.Radix);
    }

    [TestMethod]
    public void Decrypt_RoundTrips_For_ThisIsATest_And_Hello()
    {
        TextRadixCipherResult decrypted = TextRadixCipher.Decrypt("otTaetHslott~las~ ", "Hello", "~This ateHlo");

        Assert.AreEqual("This is a test", decrypted.MessageText);
    }

    [TestMethod]
    public void Decrypt_Throws_When_ThisIsATest_CipherText_LosesTrailingWhitespace()
    {
        var ex = Assert.ThrowsException<InvalidOperationException>(() =>
            TextRadixCipher.Decrypt("otTaetHslott~las~", "Hello", "~This ateHlo"));

        StringAssert.Contains(ex.Message, "does not divide cleanly");
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
