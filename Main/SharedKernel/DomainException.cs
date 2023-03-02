using System;

namespace DMT.SharedKernel
{
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException(string message) : 
            base(CreateMessageForDomainException(message))
        {
            
        }

        private static string CreateMessageForDomainException(String message)
        {
            string exceptionMessage = "Domain Exception - " +  message;
            return (exceptionMessage);
        }

        public static string FormatExceptionMessage(Exception exception)
        {
            string message = exception.Message;
            if (exception.InnerException != null)
                message += " " + exception.InnerException.Message;
            return (message);
        }
    }
}
