namespace FakeHttpContext.Tests
{
    using System;
    using System.Web;

    using FluentAssertions;

    using Ploeh.AutoFixture.Xunit2;

    using Xunit;

    public class FakeWorkerRequestTests
    {
        [Theory, AutoData]
        public void Should_return_user_agent(string useraAgent)
        {
            // Arrange
            var worker = new FakeWorkerRequest { UserAgent = useraAgent };

            // Act
            var headerUserAgent = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderUserAgent);

            // Assert
            headerUserAgent.Should().Be(useraAgent);
        }

        [Fact]
        public void Should_have_default_uri()
        {
            // Arrange
            var worker = new FakeWorkerRequest();

            // Act

            // Assert
            worker.Uri.Should().Be(new Uri("http://[::1]:4293/Default"));
        }

        [Theory]
        [InlineData("git://server.com/path", "")]
        [InlineData("git://server.com/path?query=1", "query=1")]
        [InlineData("git://server.com/path?query1=1&query2=2", "query1=1&query2=2")]
        public void Should_read_all_data_from_uri(string uriString, string expectedQuery)
        {
            // Arrange
            var uri = new Uri(uriString);
            var worker = new FakeWorkerRequest { Uri = uri };

            // Act
            var headerHost = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderHost);

            // Assert
            headerHost.Should().Be(uri.Host + ":" + uri.Port);
            worker.GetProtocol().Should().Be(uri.Scheme);
            worker.GetServerName().Should().Be(uri.Host);
            worker.GetLocalPort().Should().Be(uri.Port);
            worker.GetUriPath().Should().Be(uri.LocalPath);
            worker.GetQueryString().Should().Be(expectedQuery);
        }

        [Theory, AutoData]
        public void Should_read_unknown_headers_from_headers_property(string key, string value)
        {
            // Arrange
            var worker = new FakeWorkerRequest();
            worker.Headers.Add(key, value);

            // Act
            var unknownHeaders = worker.GetUnknownRequestHeaders();

            // Assert
            unknownHeaders.ShouldBeEquivalentTo(new[] { new[] { key, value } });
        }
    }
}