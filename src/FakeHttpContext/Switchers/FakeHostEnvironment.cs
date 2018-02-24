using System;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace FakeHttpContext.Switchers
{
    internal class FakeHostEnvironment : SwitcherContainer
    {
        private const string AlreadyInstantiatedMessage =
            "Only one instance of FakeHostedEnvironment is allowed in a thread.";

        private static readonly object SyncObject = new object();

        [ThreadStatic] private static bool _isInstantiated;

        private readonly FakeConfigMapPath _configMapPath = new FakeConfigMapPath();

        public FakeHostEnvironment()
        {
            if (_isInstantiated) throw new InvalidOperationException(AlreadyInstantiatedMessage);

            Monitor.Enter(SyncObject);
            if (_isInstantiated) throw new InvalidOperationException(AlreadyInstantiatedMessage);

            try
            {
                Switchers.Add(new HostingEnvironmentInitializator());
                var hostringEnvironmentType = typeof(HostingEnvironment);
                var theHostingEnvironment =
                    hostringEnvironmentType.GetPrivateStaticFieldValue("_theHostingEnvironment");

                Switchers.Add(
                    new AppDomainDataSwitcher
                    {
                        {".appPath", AppDomain.CurrentDomain.BaseDirectory},
                        {".appDomain", "*"},
                        {".appVPath", "/"}
                    });
                Switchers.Add(new PrivateFieldSwitcher(theHostingEnvironment, "_appVirtualPath", GetVirtualPath()));
                Switchers.Add(new PrivateFieldSwitcher(theHostingEnvironment, "_configMapPath", _configMapPath));
                Switchers.Add(new PrivateFieldSwitcher(theHostingEnvironment, "_appPhysicalPath",
                    AppDomain.CurrentDomain.BaseDirectory));
                _isInstantiated = true;
            }
            catch
            {
                Monitor.Exit(SyncObject);
                throw;
            }
        }

        public string BasePath
        {
            set { _configMapPath.BasePath = value; }
        }

        public override void Dispose()
        {
            try
            {
                base.Dispose();
            }
            finally
            {
                Monitor.Exit(SyncObject);
                _isInstantiated = false;
            }
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