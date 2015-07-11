namespace FakeHttpContext
{
  using System;
  using System.Web;

  using global::FakeHttpContext.Switchers;

  public class FakeHttpContext : SwitcherContainer
  {
    private readonly HttpContext conextBackup;

    private readonly FakeWorkerRequest fakeWorkerRequest = new FakeWorkerRequest();

    public FakeHttpContext()
    {
      this.conextBackup = HttpContext.Current;
      this.Switchers.Add(new FakeHostEnvironment());
      HttpContext.Current = new HttpContext(this.fakeWorkerRequest);
    }

    public FakeHttpContext WithUserAgent(string userAgentString)
    {
      this.fakeWorkerRequest.UserAgent = userAgentString;
      return this;
    }

    public FakeHttpContext WithUri(Uri uri)
    {
      this.fakeWorkerRequest.Uri = uri;
      return this;
    }

    public override void Dispose()
    {
      base.Dispose();
      HttpContext.Current = this.conextBackup;
    }
  }
}