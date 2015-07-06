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
    [InlineData("git://server.com/path?query=1")]
    public void Should_read_all_data_from_uri(string uriString)
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
      worker.GetQueryString().Should().Be(uri.Query);
    }
  }
}