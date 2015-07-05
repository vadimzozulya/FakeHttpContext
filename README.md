This util allow to initialize HttpContex.Current with fake context.

Examples
---
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
  using (new FakeHttpContext().WithUserAgent(ExpectedUserAgentString))
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

  using (new FakeHttpContext())
  {
	// Act && Assert
	HttpContext.Current.Server.MapPath("myPath").Should().Be(expectedPath);
  }
}
```

For more examples please see FakeHttpContext.Tests project