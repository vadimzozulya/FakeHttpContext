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
      // Arrange
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

      using (new FakeHttpContext())
      {
        HttpContext.Current = null;
      }

      HttpContext.Current.Should().BeSameAs(httpContext);
    }

    [Theory, AutoData]
    public void Should_allow_to_use_map_path(string path)
    {
      // Arrange
      var expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

      using (new FakeHttpContext())
      {
        // Act && Assert
        HttpContext.Current.Server.MapPath(path).Should().Be(expectedPath);
      }
    }

    [Theory, AutoData]
    public void Should_fake_user_agent(string userAgentString)
    {
      // Arrange
      using (new FakeHttpContext().WithUserAgent(userAgentString))
      {
        // Act
        // Assert
        HttpContext.Current.Request.UserAgent.Should().Be(userAgentString);
      }
    }

    public void Should_fake_user_agent1()
    {
      // Arrange
      const string ExpectedUserAgentString = "user agent string";
      using (new FakeHttpContext().WithUserAgent(ExpectedUserAgentString))
      {
        // Assert
        HttpContext.Current.Request.UserAgent.Should().Be(ExpectedUserAgentString);
      }
    }

  }
}
