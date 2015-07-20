﻿namespace FakeHttpContext.Tests
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
  }
}
