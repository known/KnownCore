namespace Known.Validation
{
    public class ValidInfo
    {
        public ValidInfo(ValidLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        public ValidLevel Level { get; }
        public string Message { get; }
    }
}
