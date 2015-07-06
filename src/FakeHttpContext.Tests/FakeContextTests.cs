namespace FakeHttpContext.Tests
{
  using System;
  using System.IO;
  using System.Web;

  using FluentAssertions;

  using Ploeh.AutoFixture.Xunit2;

  using Xunit;

  /// <summary>The fake context tests.</summary>
  public class FakeContextTests
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

    /// <summary>The should_ fact method name.</summary>
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
      using (new FakeHttpContext().WithUserAgent(userAgentString))
      {
        // Assert
        HttpContext.Current.Request.UserAgent.Should().Be(userAgentString);
      }
    }

    [Theory, AutoData]
    public void Should_fake_url(Uri uri)
    {
      // Act
      using (new FakeHttpContext().WithUri(uri))
      {
        // Assert
        HttpContext.Current.Request.Url.Should().Be(uri);
      }
    }
  }
}
