using System.Text;

namespace FakeHttpContext
{
    public class FakePostData
    {
        public FakePostData(string data, Encoding encoding = null)
        {
            Data = data;
            Encoding = encoding;
        }

        public string Data { get; }
        public Encoding Encoding { get; }
    }
}