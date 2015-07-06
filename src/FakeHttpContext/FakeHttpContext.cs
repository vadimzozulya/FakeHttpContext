namespace FakeHttpContext
{
  using System;
  using System.Reflection;
  using System.Web;
  using System.Web.Hosting;

  public class FakeHttpContext : IDisposable
  {
    private readonly HttpContext conextBackup;

    private readonly FakeWorkerRequest fakeWorkerRequest = new FakeWorkerRequest();

    public FakeHttpContext()
    {
      if (!HostingEnvironment.IsHosted)
      {
        SetAppDomainData();
        InitHostEnvironment();
      }

      this.conextBackup = HttpContext.Current;
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

    public void Dispose()
    {
      HttpContext.Current = this.conextBackup;
    }

    private static void InitHostEnvironment()
    {
      new HostingEnvironment();

      var appDomainAppVPath = GetAppDomainAppVPath();

      var hostringEnvironmentType = typeof(HostingEnvironment);

      var theHostingEnvironment =
        hostringEnvironmentType.GetPrivateStaticFieldValue("_theHostingEnvironment");

      theHostingEnvironment.SetPrivateFieldValue("_appVirtualPath", appDomainAppVPath);
      theHostingEnvironment.SetPrivateFieldValue("_configMapPath", new FakeConfigMapPath());
    }

    private static object GetAppDomainAppVPath()
    {
      var httpRuntimeType = typeof(HttpRuntime);

      var theRuntime = httpRuntimeType.GetPrivateStaticFieldValue("_theRuntime");

      var appDomainAppVPath = theRuntime.GetPrivateFieldValue("_appDomainAppVPath");

      if (appDomainAppVPath == null)
      {
        typeof(HttpRuntime).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance)
          .Invoke(theRuntime, new object[0]);
        appDomainAppVPath = theRuntime.GetPrivateFieldValue("_appDomainAppVPath");
      }

      return appDomainAppVPath;
    }

    private static void SetAppDomainData()
    {
      var dom = System.Threading.Thread.GetDomain();

      var appPath = AppDomain.CurrentDomain.BaseDirectory;

      dom.SetData(".appPath", appPath);
      dom.SetData(".appDomain", "*");
      dom.SetData(".appVPath", "/");
      dom.SetData(".domainId", dom.FriendlyName);
      dom.SetData(".hostingVirtualPath", "/");
      dom.SetData(".hostingInstallDir", HttpRuntime.AspInstallDirectory);
    }
  }
}