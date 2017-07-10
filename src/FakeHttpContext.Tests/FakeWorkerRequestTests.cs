using System;
using System.Web;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace FakeHttpContext.Tests
{
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

        [Theory, AutoData]
        public void Should_return_accept_types(string expectedAcceptTypes)
        {
            // Arrange
            var worker = new FakeWorkerRequest { AcceptTypes = expectedAcceptTypes };

            // Act
            var acceptTypes = worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderAccept);

            // Assert
            acceptTypes.Should().Be(expectedAcceptTypes);
        }

        [Theory, AutoData]
        public void Should_return_post_string_as_array_of_butes(byte[] data)
        {
            // Arrange
            var worker = new FakeWorkerRequest();
            worker.SetPostData(data);

            // Act
            var actualData = worker.GetPreloadedEntityBody();

            // Assert
            actualData.Should().BeSameAs(data);
        }

        [Theory, AutoData]
        public void Should_return_empy_bytes_array_if_post_data_is_not_set()
        {
            // Arrange
            var worker = new FakeWorkerRequest();

            // Act
            var actualData = worker.GetPreloadedEntityBody();

            // Assert
            actualData.Should().BeNull();
        }

        [Theory]
        [InlineAutoData("POST")]
        [InlineData("GET", null)]
        public void Should_swith_request_werb_to_post(string expectedVern, byte[] data)
        {
            // Arrange
            var worker = new FakeWorkerRequest();
            worker.SetPostData(data);

            // Act
            var actualVerb = worker.GetHttpVerbName();

            // Assert
            actualVerb.Should().Be(expectedVern);
        }

        [Theory]
        [InlineAutoData(true)]
        [InlineData(false, null)]
        public void Should_state_that_body_is_compeletely_preloaded(bool expectedResult, byte[] postData)
        {
            // Arrange
            var worker = new FakeWorkerRequest();
            worker.SetPostData(postData);

            // Act
            var isEntireEntityBodyIsPreloaded = worker.IsEntireEntityBodyIsPreloaded();

            // Assert
            isEntireEntityBodyIsPreloaded.Should().Be(expectedResult);
        }
    }
}