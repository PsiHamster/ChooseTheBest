using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ChooseTheBest.Api.Utils
{
	public interface IPasswordHasher
	{
		public (string hash, string salt) HashPassword(string password);
		public string HashPassword(string password, string base64Salt);
	}

	public class PasswordHasher : IPasswordHasher
	{
		public (string hash, string salt) HashPassword(string password)
		{
			byte[] salt = new byte[128 / 8];
			RandomNumberGenerator.Fill(salt);

			return (HashPassword(password, salt), Convert.ToBase64String(salt));
		}

		public string HashPassword(string password, string base64Salt)
		{
			byte[] salt = Convert.FromBase64String(base64Salt);
			return HashPassword(password, salt);
		}

		private string HashPassword(string password, byte[] salt)
		{
			return Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 100000,
				numBytesRequested: 256 / 8));
		}
	}
}
