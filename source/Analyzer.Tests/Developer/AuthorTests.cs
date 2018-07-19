using System.Collections.Generic;
using Analyzer.Domain.Developer;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Tests.Developer
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

        [Test]
        public void ToString_WhenNameUnknown_ShouldUseFirstEmail()
        {
            // arrange
            var sut = new Author { Name = "unknown", Emails = new List<string> { "abc@def.com" } };
            // act
            var actual = sut.ToString();
            // assert
            actual.Should().Be("abc@def.com");
        }

        [Test]
        public void ToString_WhenNameNotUnknown_ShouldUseName()
        {
            // arrange
            var sut = new Author { Name = "T-rav", Emails = new List<string> { "abc@def.com" } };
            // act
            var actual = sut.ToString();
            // assert
            actual.Should().Be("T-rav");
        }
    }
}
