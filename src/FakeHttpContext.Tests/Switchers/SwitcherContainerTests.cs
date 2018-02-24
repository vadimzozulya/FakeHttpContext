using System;
using FakeHttpContext.Switchers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FakeHttpContext.Tests.Switchers
{
    public class SwitcherContainerTests
    {
        private class SwitcherTest : SwitcherContainer
        {
        }

        [Fact]
        public void Should_dispose_all_even_if_some_throws()
        {
            // Arrange
            var switcherTest = new SwitcherTest();
            var throwingSwitcher = Substitute.For<IDisposable>();
            var exc = new Exception("exception");
            throwingSwitcher.When(x => x.Dispose()).Throw(exc);

            switcherTest.Switchers.Add(throwingSwitcher);
            switcherTest.Switchers.Add(Substitute.For<IDisposable>());

            // Act
            Action action = () => switcherTest.Dispose();

            // Assert
            action.ShouldThrow<AggregateException>()
                .And.InnerExceptions.Should().BeEquivalentTo(exc);

            switcherTest.Switchers.ForEach(x => x.Received().Dispose());
        }

        [Fact]
        public void Should_dispose_all_switchers()
        {
            // Arrange
            var switcherTest = new SwitcherTest();
            switcherTest.Switchers.Add(Substitute.For<IDisposable>());
            switcherTest.Switchers.Add(Substitute.For<IDisposable>());

            // Act
            switcherTest.Dispose();

            // Assert
            switcherTest.Switchers.ForEach(x => x.Received().Dispose());
        }
    }
}