# FakeHttpContext

[![#](https://img.shields.io/nuget/v/FakeHttpContext.svg)](https://www.nuget.org/packages/FakeHttpContext/)
[![Build status](https://ci.appveyor.com/api/projects/status/l2jnlqj0vtkbr6ob?svg=true)](https://ci.appveyor.com/project/vadimzozulya/fakehttpcontext)

This util allows to initialize `HttpContex.Current` with fake context.

## Install with NuGet
You can install the utility with [NuGet](https://www.nuget.org/packages/FakeHttpContext/)

## Examples

```csharp
[Fact]
public void Should_initialize_HttpContext_Current()
{
  // Arrange
  using (new FakeHttpContext())
  {
    // Assert
    HttpContext.Current.Should().NotBeNull();
  }
}

[Fact]
public void Should_fake_user_agent()
{
  // Arrange
  const string ExpectedUserAgentString = "user agent string";
  using (new FakeHttpContext {UserAgent = ExpectedUserAgentString})
  {
    // Assert
    HttpContext.Current.Request.UserAgent.Should().Be(ExpectedUserAgentString);
  }
}

[Fact]
public void Should_allow_to_use_map_path()
{
  // Arrange
  var expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "myPath");

  // Act
  using (new FakeHttpContext())
  {
  // Assert
    HttpContext.Current.Server.MapPath("myPath").Should().Be(expectedPath);
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
```

For more examples please see `FakeHttpContext.Tests` project.
