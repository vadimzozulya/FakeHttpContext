using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FakeHttpContext
{
    internal class FakeWorkerRequest : HttpWorkerRequest
    {
        private Uri _uri;

        public string UserAgent { get; set; }

        public Uri Uri
        {
            get
            {
                return _uri ?? (_uri = new Uri("http://[::1]:4293/Default"));
            }

            set
            {
                _uri = value;
            }
        }

        public Dictionary<string, string > Headers { get; } = new Dictionary<string, string>();

        public string AcceptTypes { get; set; }

        /// <summary>
        /// Returns the virtual path to the requested URI.
        /// </summary>
        /// <returns>
        /// The path to the requested URI.
        /// </returns>
        public override string GetUriPath()
        {
            return Uri.LocalPath;
        }

        /// <summary>
        /// Returns the query string specified in the request URL.
        /// </summary>
        /// <returns>
        /// The request query string.
        /// </returns>
        public override string GetQueryString()
        {
            return Uri.Query.TrimStart('?');
        }

        /// <summary>
        /// Returns the URL path contained in the request header with the query string appended.
        /// </summary>
        /// <returns>
        /// The raw URL path of the request header.
        /// </returns>
        public override string GetRawUrl()
        {
            return "/Default";
        }

        /// <summary>
        /// Returns the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The HTTP verb returned in the request header.
        /// </returns>
        public override string GetHttpVerbName()
        {
            return "GET";
        }

        /// <summary>
        /// Provides access to the HTTP version of the request (for example, "HTTP/1.1").
        /// </summary>
        /// <returns>
        /// The HTTP version returned in the request header.
        /// </returns>
        public override string GetHttpVersion()
        {
            return "HTTP/1.1";
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The client's IP address.
        /// </returns>
        public override string GetRemoteAddress()
        {
            return "::1";
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The client's HTTP port number.
        /// </returns>
        public override int GetRemotePort()
        {
            return 5932;
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The server IP address returned in the request header.
        /// </returns>
        public override string GetLocalAddress()
        {
            return Uri.Host;
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The server port number returned in the request header.
        /// </returns>
        public override int GetLocalPort()
        {
            return Uri.Port;
        }

        /// <summary>
        /// Specifies the HTTP status code and status description of the response, such as SendStatus(200, "Ok").
        /// </summary>
        /// <param name="statusCode">The status code to send </param><param name="statusDescription">The status description to send. </param>
        public override void SendStatus(int statusCode, string statusDescription)
        {
        }

        /// <summary>
        /// Adds a standard HTTP header to the response.
        /// </summary>
        /// <param name="index">The header index. For example, <see cref="F:System.Web.HttpWorkerRequest.HeaderContentLength"/>. </param><param name="value">The value of the header. </param>
        public override void SendKnownResponseHeader(int index, string value)
        {
        }

        /// <summary>
        /// Adds a nonstandard HTTP header to the response.
        /// </summary>
        /// <param name="name">The name of the header to send. </param><param name="value">The value of the header. </param>
        public override void SendUnknownResponseHeader(string name, string value)
        {
        }

        /// <summary>
        /// Adds the specified number of bytes from a byte array to the response.
        /// </summary>
        /// <param name="data">The byte array to send. </param><param name="length">The number of bytes to send, starting at the first byte. </param>
        public override void SendResponseFromMemory(byte[] data, int length)
        {
        }

        /// <summary>
        /// Adds the contents of the specified file to the response and specifies the starting position in the file and the number of bytes to send.
        /// </summary>
        /// <param name="filename">The name of the file to send. </param><param name="offset">The starting position in the file. </param><param name="length">The number of bytes to send. </param>
        public override void SendResponseFromFile(string filename, long offset, long length)
        {
        }

        /// <summary>
        /// Adds the contents of the specified file to the response and specifies the starting position in the file and the number of bytes to send.
        /// </summary>
        /// <param name="handle">The handle of the file to send. </param><param name="offset">The starting position in the file. </param><param name="length">The number of bytes to send. </param>
        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
        }

        /// <summary>
        /// Sends all pending response data to the client.
        /// </summary>
        /// <param name="finalFlush">true if this is the last time response data will be flushed; otherwise, false. </param>
        public override void FlushResponse(bool finalFlush)
        {
        }

        /// <summary>
        /// Used by the runtime to notify the <see cref="T:System.Web.HttpWorkerRequest"/> that request processing for the current request is complete.
        /// </summary>
        public override void EndOfRequest()
        {
        }

        /// <summary>
        /// Returns the standard HTTP request header that corresponds to the specified index.
        /// </summary>
        /// <returns>
        /// The HTTP request header.
        /// </returns>
        /// <param name="index">The index of the header. For example, the <see cref="F:System.Web.HttpWorkerRequest.HeaderAllow"/> field. </param>
        public override string GetKnownRequestHeader(int index)
        {
            switch (index)
            {
                case HttpWorkerRequest.HeaderUserAgent:
                    return UserAgent;
                case HttpWorkerRequest.HeaderHost:
                    return Uri.Host + ":" + Uri.Port;
                case HttpWorkerRequest.HeaderAccept:
                    return AcceptTypes;
            }

            return null;
        }

        /// <summary>
        /// When overridden in a derived class, returns the HTTP protocol (HTTP or HTTPS).
        /// </summary>
        /// <returns>
        /// HTTPS if the <see cref="M:System.Web.HttpWorkerRequest.IsSecure"/> method is true, otherwise HTTP.
        /// </returns>
        public override string GetProtocol()
        {
            return Uri.Scheme;
        }

        public override string[][] GetUnknownRequestHeaders()
        {
            var unknownRequestHeaders = Headers.Select(x => new[] { x.Key, x.Value }).ToArray();
            return unknownRequestHeaders;
        }
    }
}
