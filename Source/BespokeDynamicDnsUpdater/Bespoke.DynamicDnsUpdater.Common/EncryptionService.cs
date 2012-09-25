using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Common
{
	public class EncryptionService
	{
		#region Constructors

		public EncryptionService()
		{
			var encryptionAlgorithm = DefaultEncyrptionAlgorithm;
			
			encryptionAlgorithm.GenerateKey();
			encryptionAlgorithm.GenerateIV();

			InitializeEncryptionService(encryptionAlgorithm, encryptionAlgorithm.Key, encryptionAlgorithm.IV, DefaultEncoding);
		}

		public EncryptionService(SymmetricAlgorithm encryptionAlgorithm, byte[] key, byte[] initializationVector, Encoding encoding)
		{
			InitializeEncryptionService(encryptionAlgorithm, key, initializationVector, encoding);
		}

		public EncryptionService(byte[] key, byte[] initializationVector)
			: this(DefaultEncyrptionAlgorithm, key, initializationVector, DefaultEncoding)
		{
		}

		private void InitializeEncryptionService(SymmetricAlgorithm encryptionAlgorithm, byte[] key, byte[] initializationVector, Encoding encoding)
		{
			EncryptionAlgorithm = encryptionAlgorithm;
			Encoding = encoding;

			if (key != null && initializationVector != null)
			{
				InitializeEncryptor(key, initializationVector);
				InitializeDecryptor(key, initializationVector);
			}
		}

		#endregion Constructors

		#region Private Properties

		private ICryptoTransform Encryptor { get; set; }

		private ICryptoTransform Decryptor { get; set; }

		#endregion Private Properties

		#region Public Properties

		public SymmetricAlgorithm EncryptionAlgorithm { get; set; }

		public static SymmetricAlgorithm DefaultEncyrptionAlgorithm
		{
			//http://msdn.microsoft.com/en-us/library/system.security.cryptography.aescryptoserviceprovider.aspx
			//Performs symmetric encryption and decryption using the Cryptographic Application Programming Interfaces (CAPI) implementation of the Advanced Encryption Standard (AES) algorithm. 
			get { return new AesCryptoServiceProvider(); }
		}

		public Encoding Encoding { get; set; }

		public static Encoding DefaultEncoding { get { return Encode.DefaultEncoding; } }

		public byte[] Key
		{
			get { return EncryptionAlgorithm.Key;}
			set { EncryptionAlgorithm.Key = value; }
		}

		public byte[] InitializationVector
		{
			get { return EncryptionAlgorithm.IV; }
			set { EncryptionAlgorithm.IV = value; }
		}

		#endregion Public Properties

		public string GenerateKeyToString()
		{
			EncryptionAlgorithm.GenerateKey();

			var bytes = EncryptionAlgorithm.Key;
			var key = Encoding.GetString(bytes);

			//Need to reinitialize *cryptors
			InitializeEncryptor(bytes, InitializationVector);
			InitializeDecryptor(bytes, InitializationVector);

			return key;
		}

		private void InitializeEncryptor(byte[] key, byte[] initializationVector)
		{
			Encryptor = EncryptionAlgorithm.CreateEncryptor(key, initializationVector);
			EncryptionAlgorithm.Padding = PaddingMode.PKCS7;
		}

		private void InitializeDecryptor(byte[] key, byte[] initializationVector)
		{
			Decryptor = EncryptionAlgorithm.CreateDecryptor(key, initializationVector);				
		}

		public byte[] EncryptToBytes(string stringToEncrypt)
		{
			var bytes = EncryptToBytes(Encoding.GetBytes(stringToEncrypt));

			return bytes;
		}

		public byte[] EncryptToBytes(byte[] bytes)
		{
			if(Encryptor == null)
				InitializeEncryptor(EncryptionAlgorithm.Key, EncryptionAlgorithm.IV);

			var encrytedBytes = TransformBytes(bytes, Encryptor);

			return encrytedBytes;
		}

		public string EncryptToBase64String(byte[] bytes)
		{
			var encryptedBytes = EncryptToBytes(bytes);

			var encryptedString = Encode.ToBase64String(encryptedBytes);

			return encryptedString;
		}

		public string EncryptToBase64String(string stringToEncrypt)
		{
			var encryptedString = EncryptToBase64String(Encoding.GetBytes(stringToEncrypt));

			return encryptedString;
		}

		public byte[] DecryptToBytes(byte[] bytes)
		{
			if(Decryptor == null)
				InitializeDecryptor(EncryptionAlgorithm.Key, EncryptionAlgorithm.IV);

			var decryptedBytes = TransformBytes(bytes, Decryptor);

			return decryptedBytes;
		}

		public byte[] DecryptToBytes(string encryptedString)
		{
			var bytes = DecryptToBytes(Encoding.GetBytes(encryptedString));

			return bytes;	
		}

		public string DecryptToString(byte[] bytes)
		{
			var decryptedBytes = DecryptToBytes(bytes);

			var decryptedString = Encoding.GetString(decryptedBytes);

			return decryptedString;
		}

		public string DecryptToString(string encryptedString)
		{
			var bytes = Encoding.GetBytes(encryptedString);

			var decryptedString = DecryptToString(bytes);

			return decryptedString;
		}

		public byte[] DecryptBase64StringToBytes(string encryptedString)
		{
			var bytes = Encode.FromBase64StringToBytes(encryptedString);

			var decryptedBytes = DecryptToBytes(bytes);

			return decryptedBytes;
		}

		public string DecryptBase64StringToString(string encryptedString)
		{
			var bytes = DecryptBase64StringToBytes(encryptedString);

			var decryptedString = Encoding.GetString(bytes);

			return decryptedString;
		}

		protected byte[] TransformBytes(byte[] bytes, ICryptoTransform transform)
		{
			using(var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					cryptoStream.Write(bytes, 0, bytes.Length);
					//cryptoStream.FlushFinalBlock();
				}

				return memoryStream.ToArray();
			}
		}
	}
}
