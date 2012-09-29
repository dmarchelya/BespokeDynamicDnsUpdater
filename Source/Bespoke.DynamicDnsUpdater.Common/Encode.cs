using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Common
{
	public class Encode
	{
		public Encode()
		{
			Encoding = DefaultEncoding;
		}

		public static Encoding Encoding { get; set; }

		public static Encoding DefaultEncoding
		{
			get { return Encoding.UTF8; }
		}

		/// <summary>
		/// Encodes an UTF8 string as Base64
		/// </summary>
		/// <param name="toEncode">The string to encode.</param>
		/// <returns>The Base64 formatted string.</returns>
		public static string ToBase64String(string toEncode)
		{
			string base64String = ToBase64String(toEncode, Encoding);

			return base64String;
		}

		/// <summary>
		/// Encodes the given byte array as a Base64 string.
		/// </summary>
		/// <param name="bytes">The byte array to encode.</param>
		/// <returns>The Base64 formatted string.</returns>
		public static string ToBase64String(byte[] bytes)
		{
			string base64String = Convert.ToBase64String(bytes);

			return base64String;
		}

		/// <summary>
		/// Encodes the byte array with the given encoding as a Base64 string.
		/// </summary>
		/// <param name="toEncode">The string to encode.</param>
		/// <param name="encoding">The encoding of the byte array.</param>
		/// <returns>The Base64 formatted string.</returns>
		public static string ToBase64String(string toEncode, Encoding encoding)
		{
			var bytes = encoding.GetBytes(toEncode);

			string base64String = ToBase64String(bytes);

			return base64String;
		}

		/// <summary>
		/// Decodes the given Base64 string as an UTF8 string.
		/// </summary>
		/// <param name="base64String">The Base64 string to decode.</param>
		/// <returns>The UTF8 string.</returns>
		public static string FromBase64String(string base64String)
		{
			string output = FromBase64String(base64String, Encoding);

			return output;
		}

		/// <summary>
		/// Decodes the given Base64 stiring to a string with the given encoding. 
		/// </summary>
		/// <param name="base64String">The Base64 string to decode.</param>
		/// <param name="encoding">The encoding type of the string that is decoded.</param>
		/// <returns>The decode string.</returns>
		public static string FromBase64String(string base64String, Encoding encoding)
		{
			byte[] bytes = FromBase64StringToBytes(base64String);

			var output = encoding.GetString(bytes);

			return output;
		}

		public static byte[] FromBase64StringToBytes(string base64String)
		{
			byte[] bytes = Convert.FromBase64String(base64String);

			return bytes;
		}
	}
}
