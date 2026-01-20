namespace API.Utility
{
    public static class SD
    {
        //regex
        public const string UserNameRegex = "^[]a-zA-z0-9_.-]*$";
        public const string EmailRegex = "^.+@[^\\.].*\\.[a-z]{2,}$";

        //Application rules
        public const int RequiredPasswordLength = 6;
        public const int MaxFailedAccessAttempts = 3;
        public const int DefaultLockoutTimeSpanInDays = 6;
    }
}
