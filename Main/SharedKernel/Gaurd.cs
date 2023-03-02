namespace DMT.SharedKernel
{
    public static class Gaurd
    {
        public static void AgainstNull<T>(T value, string message)
              where T : class
        {
            if (value == null)
                throw new DomainException(message);
        }
        public static void AgainstNotEqual<T>(T value1, T value2, string message)
        {
            if (!value1.Equals(value2))
                throw new DomainException(message);
        }

        public static void AgainstEmpty(string value, string message)
        {
            if (value == "")
                throw new DomainException(message);
        }
    }
}
