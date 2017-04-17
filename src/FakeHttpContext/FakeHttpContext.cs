using System;
using System.Collections;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using FakeHttpContext.Switchers;

namespace FakeHttpContext
{
    public class FakeHttpContext : SwitcherContainer
    {
        private readonly HttpContext _conextBackup;

        private readonly FakeWorkerRequest _fakeWorkerRequest = new FakeWorkerRequest();

        public FakeHttpContext()
        {
            Request = new FakeRequest(_fakeWorkerRequest);

            _conextBackup = HttpContext.Current;
            Switchers.Add(new FakeHostEnvironment());

            HttpContext.Current = new HttpContext(_fakeWorkerRequest);

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
                _fakeWorkerRequest.UserAgent = value;
                Capabilities["browser"] = value;
            }
        }

        public Uri Uri
        {
            set
            {
                _fakeWorkerRequest.Uri = value;
            }
        }

        public IDictionary Capabilities
        {
            get { return HttpContext.Current.Request.Browser.Capabilities; }
            set { HttpContext.Current.Request.Browser.Capabilities = value; }
        }

        public FakeRequest Request { get; }

        public override void Dispose()
        {
            base.Dispose();
            HttpContext.Current = _conextBackup;
        }
    }
}