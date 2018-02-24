using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Hosting;
using FakeHttpContext.Switchers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FakeHttpContext.Tests.Switchers
{
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
                        x is PrivateFieldSwitcher
                        && ((PrivateFieldSwitcher)x).FieldName == "_configMapPath"
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
        public void Should_throw_InvlidOperationException_if_alderdy_intantiated_in_the_same_thread()
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
                action.ShouldThrow<InvalidOperationException>().WithMessage("Only one instance of FakeHostedEnvironment is allowed in a thread.");
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

        [Fact]
        public void Should_be_possible_to_intantiate_environment_in_parallel()
        {
            // Arrange
            var tasks = new List<Task>();
            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    using (new FakeHostEnvironment())
                    {
                    }
                }));
            }

            // Act
            Action action = () => Task.WaitAll(tasks.ToArray());

            // Assert
            action.ShouldNotThrow();
        }

        [Fact]
        public void Should_not_stack_if_initialization_is_failed()
        {
            // Arrange
            using (new HostingEnvironmentInitializator())
            {
                var tasks = new List<Task>();
                for (var i = 0; i < 2; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            using (new FakeHostEnvironment())
                            {
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }));
                }

                // Act && Assert
                Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(2)).Should().BeTrue();
            }
        }

        [Fact]
        public void Should_not_swallow_initialization_exception()
        {
            // Arrange
            using (new HostingEnvironmentInitializator())
            {
                Action action = () =>
                {
                    // Act
                    using (new FakeHostEnvironment())
                    {
                    }
                };

                // Assert
                action.ShouldThrow<InvalidOperationException>();
            }
        }

        [Fact]
        public void Should_not_stack_if_switchers_throw_on_dispose()
        {
            var tasks = new List<Task>();
            for (var i = 0; i < 2; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        using (var env = new FakeHostEnvironment())
                        {
                            var disposable = Substitute.For<IDisposable>();
                            disposable.When(x => x.Dispose()).Throw<Exception>();
                            env.Switchers.Add(disposable);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }));
            }

            // Act && Assert
            Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(2)).Should().BeTrue();
        }

        [Fact]
        public void Should_not_swallow_disposal_exception()
        {
            // Arrange
            Action action = () =>
            {
                // Act
                using (var env = new FakeHostEnvironment())
                {
                    var disposable = Substitute.For<IDisposable>();
                    disposable.When(x => x.Dispose()).Throw<Exception>();
                    env.Switchers.Add(disposable);
                }
            };

            // Assert
            action.ShouldThrow<Exception>();
        }
    }
}