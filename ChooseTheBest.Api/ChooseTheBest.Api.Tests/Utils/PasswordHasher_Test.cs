using AutoFixture;
using ChooseTheBest.Api.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace ChooseTheBest.Api.Tests.Utils
{
	[TestFixture]
	public class PasswordHasher_Test
	{
		[SetUp]
		public void SetUp()
		{
			_passwordHasher = new PasswordHasher();
		}

		[Test]
		public void ShouldHashPassword()
		{
			var fixture = new Fixture();
			var password = fixture.Create<string>();
			var (hashedPassword, salt) = _passwordHasher!.HashPassword(password);

			_passwordHasher.HashPassword(password).hash.Should().NotBe(hashedPassword);
			_passwordHasher.HashPassword(password, salt).Should().Be(hashedPassword);
		}


		IPasswordHasher? _passwordHasher;
	}
}
