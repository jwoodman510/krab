namespace Krab.Web.Models.Response
{
    public interface IOkResponse
    {
        object Result { get; }
        long ServerResponseTimeMs { get; set; }
    }

    public class OkResponse : IOkResponse
    {
        public object Result => null;
        public long ServerResponseTimeMs { get; set; }
    }

    public class OkResponse<T> : IOkResponse
    {
        public object Result { get; }
        public long ServerResponseTimeMs { get; set; }

        public OkResponse(T result)
        {
            Result = result;
        }
    }
}