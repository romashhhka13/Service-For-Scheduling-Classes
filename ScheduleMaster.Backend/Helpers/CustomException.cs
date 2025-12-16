namespace ScheduleMaster.Helpers
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message) : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }

    public class BadRequestExceptions : Exception
    {
        public BadRequestExceptions(string message) : base(message) { }
    }
    public class UnauthorizedExceptions : Exception
    {
        public UnauthorizedExceptions(string message) : base(message) { }
    }
}