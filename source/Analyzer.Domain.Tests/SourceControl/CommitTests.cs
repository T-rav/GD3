using System;
using System.Collections.Generic;
using System.Text;
using Analyzer.Domain.SourceControl;
using FluentAssertions;
using NUnit.Framework;

namespace Analyzer.Domain.Tests.SourceControl
{
    [TestFixture]
    public class CommitTests
    {
        [Test]
        public void Ctor_ShouldInitializePatchList()
        {
            //---------------Arrange------------------
            //---------------Act----------------------
            var sut = new Commit();
            //---------------Assert-------------------
            sut.Patch.Should().NotBeNull();
        }
    }
}
