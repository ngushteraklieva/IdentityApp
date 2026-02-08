namespace API.Utility
{
    public static class SD
    {
        //Cookie
        public static readonly string IdentityAppCookie = "identityappcookie";    

        //Application claims
        public const string UserId = "uid";
        public const string Name = "name";
        public const string UserName = "username";
        public const string Email = "email"; 

        //regex
        public const string UserNameRegex = "^[a-zA-Z][a-zA-Z0-9]*$";
        public const string EmailRegex = "^.+@[^\\.].*\\.[a-z]{2,}$";

        //Application rules
        public const int RequiredPasswordLength = 6;
        public const int MaxFailedAccessAttempts = 3;
        public const int DefaultLockoutTimeSpanInDays = 6;

        public const string DefaultPassword = "123456"; 
    }
}
