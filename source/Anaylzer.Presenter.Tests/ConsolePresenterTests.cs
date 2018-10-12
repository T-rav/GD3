using System;
using System.IO;
using System.Text;
using Analyzer.Presenter;
using FluentAssertions;
using NUnit.Framework;
using TddBuddy.CleanArchitecture.Domain.Messages;

namespace Anaylzer.Presenter.Tests
{
    [TestFixture]
    public class ConsolePresenterTests
    {
        [Test]
        public void Render_WhenErrorsPresent_ShouldRenderErrors()
        {
            //---------------Arrange------------------
            var errors = new ErrorOutputMessage {Errors = {"error 1", "error 2", "error 3"}};

            var fakeoutput = new StringBuilder();
            Console.SetOut(new StringWriter(fakeoutput));

            var sut = new ConsolePresenter();
            //---------------Act----------------------
            sut.Respond(errors);
            sut.Render();
            //---------------Assert-------------------
            var expected = $"The following errors occured:{Environment.NewLine}" +
                           $"error 1{Environment.NewLine}" +
                           $"error 2{Environment.NewLine}" +
                           $"error 3{Environment.NewLine}";
            fakeoutput.ToString().Should().Be(expected);
        }
    }
}

