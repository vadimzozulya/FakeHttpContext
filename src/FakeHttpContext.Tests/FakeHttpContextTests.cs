using System;
using System.Collections;
using System.IO;
using System.Web;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace FakeHttpContext.Tests
{
    public class FakeHttpContextTests
    {
        [Fact]
        public void Should_initialize_httpContext_current()
        {
            // Act
            using (new FakeHttpContext())
            {
                // Assert
                HttpContext.Current.Should().NotBeNull();
            }
        }

        [Fact]
        public void Should_restore_previous_context()
        {
            // Arrange
            var httpContext = new HttpContext(
                new HttpRequest(string.Empty, "http://server", string.Empty),
                new HttpResponse(new StringWriter()));
            HttpContext.Current = httpContext;

            // Act
            using (new FakeHttpContext())
            {
                // Assert
                HttpContext.Current.Should().NotBeSameAs(httpContext);
            }

            HttpContext.Current.Should().BeSameAs(httpContext);
        }

        [Theory, AutoData]
        public void Should_allow_to_use_map_path(string path)
        {
            // Arrange
            var expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

            // Act
            using (new FakeHttpContext())
            {
                // Assert
                HttpContext.Current.Server.MapPath(path).Should().Be(expectedPath);
            }
        }

        [Theory, AutoData]
        public void Should_fake_user_agent(string userAgentString)
        {
            // Arrange && Act
            using (new FakeHttpContext { UserAgent = userAgentString })
            {
                // Assert
                HttpContext.Current.Request.UserAgent.Should().Be(userAgentString);
            }
        }

        [Theory, AutoData]
        public void Should_fake_url(Uri uri)
        {
            // Act
            using (new FakeHttpContext { Uri = uri })
            {
                // Assert
                HttpContext.Current.Request.Url.Should().Be(uri);
            }
        }

        [Theory, AutoData]
        public void Should_fake_app_domain_path()
        {
            // Arrange
            // this is necessary to call HttpRuntime static constructor
            HttpRuntime.AppDomainId.Should().BeNullOrEmpty();

            // Act
            using (new FakeHttpContext())
            {
                // Assert
                HttpRuntime.AppDomainAppPath.Should().Be(AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        [Theory]
        [InlineData("http://test?", "query1", null)]
        [InlineData("http://test?query1=1&query2=2", "query1", "1")]
        [InlineData("http://test?query1=1&query2=2", "query2", "2")]
        public void Should_fake_query_string(string uriString, string queryKey, string expectedValue)
        {
            // Arrange
            // Act
            using (new FakeHttpContext { Uri = new Uri(uriString) })
            {
                // Assert
                HttpContext.Current.Request.QueryString[queryKey].Should().Be(expectedValue);
            }
        }

        [Theory, AutoData]
        public void Should_fake_session(string key, object value)
        {
            // Act
            using (new FakeHttpContext())
            {
                HttpContext.Current.Session[key] = value;

                // Assert
                HttpContext.Current.Session.Should().NotBeNull();
                HttpContext.Current.Session[key].Should().Be(value);
            }
        }

        [Fact]
        public void Should_fake_browser_capabilities()
        {
            // Act
            using (new FakeHttpContext())
            {
                // Assert
                HttpContext.Current.Request.Browser.Should().NotBeNull();
            }
        }

        [Theory, AutoData]
        public void Should_fake_capabilities_properties(string version, object key, object value)
        {
            // Act
            using (var context = new FakeHttpContext { Capabilities = new Hashtable { { "version", version } } })
            {
                HttpContext.Current.Request.Browser.Capabilities[key] = value;

                // Assert
                HttpContext.Current.Request.Browser.Version.Should().Be(version);
                context.Capabilities[key].Should().Be(value);
            }
        }

        [Theory, AutoData]
        public void Should_fake_browser_user_agent(string userAgent)
        {
            // Act
            using (new FakeHttpContext { UserAgent = userAgent })
            {
                // Assert
                HttpContext.Current.Request.Browser.Browser.Should().Be(userAgent);
            }
        }

        [Fact]
        public void Should_be_possible_to_map_root_path()
        {
            // Arrange
            // Act
            using (new FakeHttpContext())
            {
                // Assert
                HttpContext.Current.Server.MapPath("/").Should().Be(AppDomain.CurrentDomain.BaseDirectory + "\\");
            }
        }

        [Theory, AutoData]
        public void Should_be_possible_to_fake_http_headers(string headerKey, string headerValue)
        {
            // Act
            using (new FakeHttpContext { Request = { { headerKey, headerValue } } })
            {
                // Assert
                HttpContext.Current.Request.Headers[headerKey].Should().Be(headerValue);
            }
        }

        [Theory, AutoData]
        public void Should_be_possible_to_fake_http_header_if_the_property_header_is_already_initialized(string headerKey, string headerValue)
        {
            // Arrange
            using (var fakeContext = new FakeHttpContext())
            {
                // ReSharper disable once UnusedVariable
                var touchHeaders = HttpContext.Current.Request.Headers;

                // Act
                fakeContext.Request.Add(headerKey, headerValue);

                // Assert
                HttpContext.Current.Request.Headers[headerKey].Should().Be(headerValue);
            }
        }

        [Fact]
        public void Should_be_possible_to_fake_accept_types()
        {
            // Arrange
            using (
                new FakeHttpContext
                {
                    // Act
                    Request = { AcceptTypes = new[] { "application/json", "application/javascript" } }
                })
            {
                // Assert
                HttpContext.Current.Request.AcceptTypes.Should()
                    .BeEquivalentTo("application/json", "application/javascript");
            }
        }

        [Theory]
        [InlineAutoData("a:/pase_path")]
        [InlineAutoData("b:\\pase_path")]
        public void Should_be_possible_to_set_base_path_for_MapPath(string basePath, string path)
        {
            var expectedPath = Path.Combine(basePath, path);
            using(new FakeHttpContext { BasePath = basePath })
            {
                HttpContext.Current.Server.MapPath(path).Should().Be(expectedPath);
            }
        }

        [Theory]
        [InlineAutoData("/pase_path")]
        [InlineAutoData("\\pase_path")]
        [InlineAutoData("pase_path")]
        [InlineAutoData("pase_pathC:\\")]
        public void Should_throw_if_BasePath_Is_Not_Absolute(string basePath, string path)
        {
            using (var context = new FakeHttpContext())
            {
                Action action = () => context.BasePath = basePath;

                action.ShouldThrow<ArgumentException>()
                    .Where(x => x.Message.StartsWith("BasePath must be an absolute path.")
                                && x.ParamName == "value");
            }
        }
    }
}
