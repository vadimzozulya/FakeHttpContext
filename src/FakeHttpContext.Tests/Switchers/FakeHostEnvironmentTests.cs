namespace FakeHttpContext.Tests.Switchers
{
  using System;
  using System.Linq;
  using System.Web.Hosting;

  using global::FakeHttpContext.Switchers;

  using FluentAssertions;

  using Xunit;

  public class FakeHostEnvironmentTests
  {
    [Fact]
    public void Should_instantiate_hosting_environment()
    {
      // Act
      using (new FakeHostEnvironment())
      {
        // Assert
        HostingEnvironment.IsHosted.Should().BeTrue();
      }

      HostingEnvironment.IsHosted.Should().BeFalse();
    }

    [Fact]
    public void Should_initialize_application_virtual_path()
    {
      // Act
      using (new FakeHostEnvironment())
      {
        // Assert
        HostingEnvironment.ApplicationVirtualPath.Should().Be("/");
      }
    }

    [Fact]
    public void Should_initialize_config_map_path()
    {
      // Act
      using (var fakeEnvironment = new FakeHostEnvironment())
      {
        // Assert
        fakeEnvironment.Switchers.Should().Contain(
            x =>
              x is PrivateFieldSwitcher &&
            ((PrivateFieldSwitcher)x).FieldName == "_configMapPath"
            && ((PrivateFieldSwitcher)x).NewValue is FakeConfigMapPath);
      }
    }

    [Fact]
    public void Should_initialize_app_domain_data()
    {
      // Act
      using (var fakeEnvironment = new FakeHostEnvironment())
      {
        // Assert
        fakeEnvironment.Switchers.Should()
          .Contain(
            x =>
            x is AppDomainDataSwitcher
            && (string)((AppDomainDataSwitcher)x).FakeLocalStore[".appPath"][0] == AppDomain.CurrentDomain.BaseDirectory
            && (string)((AppDomainDataSwitcher)x).FakeLocalStore[".appDomain"][0] == "*"
            && (string)((AppDomainDataSwitcher)x).FakeLocalStore[".appVPath"][0] == "/");
      }
    }

    [Fact]
    public void Should_do_nothing_if_is_already_hosted()
    {
      // Arrange
      using (new FakeHostEnvironment())
      {
        // Act
        Action action = () =>
          {
            using (new FakeHostEnvironment())
            {
            }
          };

        // Assert
        action.ShouldNotThrow();
      }
    }

    [Fact]
    public void Should_set_appPhisicalPath()
    {
      // Act
      using (var fakeEnvironment = new FakeHostEnvironment())
      {
        // Assert
        fakeEnvironment.Switchers.Should()
          .Contain(
            x =>
            x is PrivateFieldSwitcher
            && ((PrivateFieldSwitcher)x).FieldName == "_appPhysicalPath"
            && (string)((PrivateFieldSwitcher)x).NewValue == AppDomain.CurrentDomain.BaseDirectory);
      }
    }
  }
}