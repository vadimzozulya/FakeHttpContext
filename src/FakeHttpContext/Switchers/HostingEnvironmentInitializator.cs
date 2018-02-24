using System;
using System.Web.Hosting;

namespace FakeHttpContext.Switchers
{
    internal class HostingEnvironmentInitializator : IDisposable
    {
        public HostingEnvironmentInitializator()
        {
            new HostingEnvironment();
        }
        public void Dispose()
        {
            typeof(HostingEnvironment).SetPrivateStaticFieldValue("_theHostingEnvironment", null);
        }
    }
}