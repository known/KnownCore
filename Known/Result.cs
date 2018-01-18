namespace Known
{
    public class Result
    {
        public Result(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }

        public bool IsValid { get; }
        public string Message { get; }
        public object Data { get; set; }

        public static Result Error(string message, object data = null)
        {
            return new Result(false, message) { Data = data };
        }

        public static Result Success(string message, object data = null)
        {
            return new Result(true, message) { Data = data };
        }
    }
}
