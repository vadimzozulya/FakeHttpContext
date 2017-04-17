using System;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace FakeHttpContext.Switchers
{
    internal class FakeHostEnvironment : SwitcherContainer
    {
        private readonly FakeConfigMapPath _configMapPath = new FakeConfigMapPath();

        public FakeHostEnvironment()
        {
            if (HostingEnvironment.IsHosted)
            {
                return;
            }

            new HostingEnvironment();
            var hostringEnvironmentType = typeof(HostingEnvironment);
            var theHostingEnvironment = hostringEnvironmentType.GetPrivateStaticFieldValue("_theHostingEnvironment");

            Switchers.Add(
                new AppDomainDataSwitcher
                    {
                        { ".appPath", AppDomain.CurrentDomain.BaseDirectory },
                        { ".appDomain", "*" },
                        { ".appVPath", "/" }
                    });
            Switchers.Add(new PrivateFieldSwitcher(theHostingEnvironment, "_appVirtualPath", GetVirtualPath()));
            Switchers.Add(new PrivateFieldSwitcher(theHostingEnvironment, "_configMapPath", _configMapPath));
            Switchers.Add(new PrivateFieldSwitcher(theHostingEnvironment, "_appPhysicalPath", AppDomain.CurrentDomain.BaseDirectory));
        }

        public override void Dispose()
        {
            base.Dispose();
            typeof(HostingEnvironment).SetPrivateStaticFieldValue("_theHostingEnvironment", null);
        }

        public string BasePath
        {
            set { _configMapPath.BasePath = value; }
        }

        private static object GetVirtualPath()
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
    }
}