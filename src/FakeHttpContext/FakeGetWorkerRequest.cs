namespace FakeHttpContext
{
    public class FakeGetWorkerRequest : FakeWorkerRequest
    {
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
    }
}
