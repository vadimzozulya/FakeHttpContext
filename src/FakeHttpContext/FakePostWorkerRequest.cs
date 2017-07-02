using System.Text;
using System.Web;

namespace FakeHttpContext
{
    public class FakePostWorkerRequest : FakeWorkerRequest
    {
        private readonly string _postData;
        private int _postDataBufferPointer = 0;

        public FakePostWorkerRequest(string postData)
        {
            _postData = postData;
        }

        /// <summary>
        /// Returns the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The HTTP verb returned in the request header.
        /// </returns>
        public override string GetHttpVerbName()
        {
            return "POST";
        }

        /// <summary>
        /// Overrides the body being sent from the client to the server
        /// </summary>
        public override int ReadEntityBody(byte[] buffer, int offset, int size)
        {
            var temporaryBuffer = new byte[size - offset];
            var read = ReadEntityBody(temporaryBuffer, size - offset);

            for (var i = 0; i < read; i++)
                buffer[i + offset] = temporaryBuffer[i];

            return read;
        }

        /// <summary>
        /// Overrides the body being sent from the client to the server
        /// </summary>
        public override int ReadEntityBody(byte[] buffer, int size)
        {
            var postDataBuffer = HttpContext.Current.Request.ContentEncoding.GetBytes(_postData);
            var postDataBufferLeft = postDataBuffer.Length - _postDataBufferPointer;
            if (postDataBufferLeft <= 0)
                return 0;
            if (postDataBufferLeft > size)
                postDataBufferLeft = size;

            for (var i = 0; i < postDataBufferLeft; i++)
                buffer[i] = postDataBuffer[i + _postDataBufferPointer];

            _postDataBufferPointer += postDataBufferLeft;

            return postDataBufferLeft;
        }
    }
}
