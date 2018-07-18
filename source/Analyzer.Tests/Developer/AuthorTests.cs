using Analyzer.Domain;
using Analyzer.Domain.Developer;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class AuthorTests
    {
        [Test]
        public void WhenConstructing_ShouldInitalizeEmailsList()
        {
            // arrange
            // act
            var sut = new Author();
            // assert
            sut.Emails.Should().NotBeNull();
        }
    }
}
