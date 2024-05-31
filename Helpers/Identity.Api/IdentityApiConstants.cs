namespace Identity.Api;

public static class IdentityApiConstants
{
    private const string ApiBase = "api";

    public static class Tokens
    {
        private const string Base = $"{ApiBase}/tokens";

        public const string Create = Base;
        public const string Tag = "Tokens";
    }
}