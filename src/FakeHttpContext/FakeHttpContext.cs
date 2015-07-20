namespace FakeHttpContext
{
  using System;
  using System.Collections;
  using System.Reflection;
  using System.Web;
  using System.Web.SessionState;

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

      HttpContext.Current.Request.Browser = new HttpBrowserCapabilities { Capabilities = new Hashtable() };

      var sessionContainer = new HttpSessionStateContainer(
           "id",
           new SessionStateItemCollection(),
           new HttpStaticObjectsCollection(),
           10,
           true,
           HttpCookieMode.AutoDetect,
           SessionStateMode.InProc,
           false);

      HttpContext.Current.Items["AspSession"] =
        typeof(HttpSessionState).GetConstructor(
          BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          CallingConventions.Standard,
          new[] { typeof(HttpSessionStateContainer) },
          null).Invoke(new object[] { sessionContainer });
    }

    public string UserAgent
    {
      set
      {
        this.fakeWorkerRequest.UserAgent = value;
        this.Capabilities["browser"] = value;
      }
    }

    public Uri Uri
    {
      set
      {
        this.fakeWorkerRequest.Uri = value;
      }
    }

    public IDictionary Capabilities
    {
      get { return HttpContext.Current.Request.Browser.Capabilities; }
      set { HttpContext.Current.Request.Browser.Capabilities = value; }
    }

    public override void Dispose()
    {
      base.Dispose();
      HttpContext.Current = this.conextBackup;
    }
  }
}