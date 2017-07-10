using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace FakeHttpContext
{
    public class FakeRequest : IEnumerable
    {
        private readonly FakeWorkerRequest _workerRequest;

        internal FakeRequest(FakeWorkerRequest workerRequest)
        {
            _workerRequest = workerRequest;
        }

        public IEnumerable<string> AcceptTypes
        {
            set
            {
                _workerRequest.AcceptTypes = string.Join(",", value);
            }
        }

        public FakePostData PostData
        {
            set
            {
                var encoding = value.Encoding ?? Encoding.ASCII;
                _workerRequest.SetPostData(encoding.GetBytes(value.Data));
                HttpContext.Current.Request.ContentEncoding = encoding;
            }
        }

        public void Add(string headerKey, string headerValue)
        {
            _workerRequest.Headers.Add(headerKey, headerValue);

            HttpContext.Current.Request.SetPrivateFieldValue("_headers", null);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}