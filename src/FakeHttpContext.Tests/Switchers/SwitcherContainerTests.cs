namespace FakeHttpContext.Tests.Switchers
{
  using System;

  using global::FakeHttpContext.Switchers;

  using NSubstitute;

  using Xunit;

  public class SwitcherContainerTests
  {
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

    private class SwitcherTest : SwitcherContainer
    {
    }
  }
}