namespace FakeHttpContext.Tests.Switchers
{
    using System;

    using global::FakeHttpContext.Switchers;

    using FluentAssertions;

    using Ploeh.AutoFixture.Xunit2;

    using Xunit;

    public class AppDomainDataSwitcherTests
    {
        [Theory, AutoData]
        public void Should_set_data(string name, string oldValue, string newValue)
        {
            // Arrange
            AppDomain.CurrentDomain.SetData(name, oldValue);

            // Act
            using (new AppDomainDataSwitcher { { name, newValue } })
            {
                // Assert
                AppDomain.CurrentDomain.GetData(name).Should().Be(newValue);
            }

            AppDomain.CurrentDomain.GetData(name).Should().Be(oldValue);
        }
    }
}