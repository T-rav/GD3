using Analyzer.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests
{
    [TestFixture]
    public class AliasTests
    {
        [Test]
        public void WhenInitalizing_ShouldAssignIdGuidValue()
        {
            // arrange
            // act
            var sut = new Alias();
            // assert
            sut.Id.Should().NotBeEmpty();
        }
    }
}
