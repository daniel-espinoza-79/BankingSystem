using FluentValidation.Results;

namespace Business.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; set; }
        public ValidationException() : base("One or more Validations failures have been produced")
        {
            Errors = [];
        }

        public ValidationException(IEnumerable<ValidationFailure> validationFailures) : this()
        {
            foreach (ValidationFailure failure in validationFailures)
            {
                Errors.Add(failure.ErrorMessage);
            }

        }

    }
}