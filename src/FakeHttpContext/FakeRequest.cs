namespace FakeHttpContext
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web;

    public class FakeRequest : IEnumerable
    {
        private readonly FakeWorkerRequest workerRequest;

        internal FakeRequest(FakeWorkerRequest workerRequest)
        {
            this.workerRequest = workerRequest;
        }

        public IEnumerable<string> AcceptTypes
        {
            set
            {
                this.workerRequest.AcceptTypes = string.Join(",", value);
            }
        }

        public void Add(string headerKey, string headerValue)
        {
            this.workerRequest.Headers.Add(headerKey, headerValue);

            HttpContext.Current.Request.SetPrivateFieldValue("_headers", null);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}