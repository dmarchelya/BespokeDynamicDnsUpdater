using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bespoke.DynamicDnsUpdater.Common.Tests
{
	[TestFixture]
	public class EncryptionServiceTests
	{
		[Test]
		public void CanInitEncryptionServiceWithoutKeyAndIV()
		{
			var service = new EncryptionService();

			Assert.IsNotNull(service.Key);
			Assert.IsNotNull(service.InitializationVector);
		}

		[Test]
		public void CanEncryptToBase64()
		{
			var service = new EncryptionService();

			var encrytedString = service.EncryptToBase64String("test");

			Assert.IsNotNull(encrytedString);
		}

		[Test]
		public void CanDecryptBase64StringToString()
		{
			var service = new EncryptionService();
			var encrytedString = service.EncryptToBase64String("test");

			var decryptionService = new EncryptionService(service.Key, service.InitializationVector);
			var decryptedString = decryptionService.DecryptBase64StringToString(encrytedString);

			Assert.AreEqual("test", decryptedString);
		}

		[Test]
		public void CanDecryptToString()
		{
			var service = new EncryptionService();
			Encode.Encoding = Encoding.ASCII;
			service.Encoding = Encoding.ASCII;

			var encryptedBytes = service.EncryptToBytes("test");

			var decryptedString = service.DecryptToString(encryptedBytes);

			Assert.AreEqual("test", decryptedString);
		}

		[Test]
		public void CanGenerateNewKey()
		{
			var service = new EncryptionService();
			service.GenerateKeyToString();

			var encrytedString = service.EncryptToBase64String("test");

			var decryptionService = new EncryptionService(service.Key, service.InitializationVector);
			var decryptedString = decryptionService.DecryptBase64StringToString(encrytedString);

			Assert.AreEqual("test", decryptedString);
		}
	}
}
