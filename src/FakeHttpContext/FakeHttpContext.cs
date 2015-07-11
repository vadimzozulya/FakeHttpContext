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

    public string UserAgent
    {
      set
      {
        this.fakeWorkerRequest.UserAgent = value;
      }
    }

    public Uri Uri
    {
      set
      {
        this.fakeWorkerRequest.Uri = value;
      }
    }

    public override void Dispose()
    {
      base.Dispose();
      HttpContext.Current = this.conextBackup;
    }
  }
}