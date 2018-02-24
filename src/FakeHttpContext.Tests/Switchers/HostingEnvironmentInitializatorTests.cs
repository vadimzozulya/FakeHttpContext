using System.Web.Hosting;
using FakeHttpContext.Switchers;
using FluentAssertions;
using Xunit;

namespace FakeHttpContext.Tests.Switchers
{
    public class HostingEnvironmentInitializatorTests
    {
        [Fact]
        public void Should_instantiate_HostingEnvironment()
        {
            using (new HostingEnvironmentInitializator())
            {
                HostingEnvironment.IsHosted.Should().BeTrue();
            }
        }

        [Fact]
        public void Should_clenup_HostingEnvironment_on_Dispose()
        {
            using (new HostingEnvironmentInitializator())
            {
            }

            HostingEnvironment.IsHosted.Should().BeFalse();
        }
    }
}