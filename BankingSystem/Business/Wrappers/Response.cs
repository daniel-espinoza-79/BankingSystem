namespace Business.Wrappers
{
    public class Response<T>
    {
        public bool Succeded { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public T? Data { get; set; }

        public Response() { }

        public Response(T data, string? message = null)
        {
            Data = data;
            Message = message;
            Succeded = true;
        }

        public Response(string message)
        {
            Message = message;
            Succeded = false;
        }


        public static Response<T> Success(T data, string? message = null)
        {
            return new Response<T>(data, message);
        }

        public static Response<T> Error(string message)
        {
            return new Response<T>(message);
        }

        public static Response<T> ValidationError(List<string> errors)
        {
            var response = new Response<T>();
            response.Succeded = false;
            response.Errors = errors;
            return response;
        }

        public static Response<T> Failure(string message, List<string>? errors = null)
        {
            var response = new Response<T>(message)
            {
                Errors = errors ?? new List<string>()
            };
            return response;
        }
    }
}
