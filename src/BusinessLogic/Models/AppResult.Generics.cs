namespace BusinessLogic.Models
{
    public class AppResult<TValue>
    {
        public TValue Value { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> ErrorMessages { get; set; }

        public static AppResult<TValue> Success(TValue value)
        {
            return new AppResult<TValue> { Value = value, IsSuccess = true };
        }

        public static AppResult<TValue> Failed(params string[] errorMessages)
        {
            return new AppResult<TValue>
            {
                Value = default(TValue),
                IsSuccess = false,
                ErrorMessages = errorMessages ?? Enumerable.Empty<string>()
            };
        }
    }
}
