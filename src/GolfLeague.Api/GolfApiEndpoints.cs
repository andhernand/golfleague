namespace GolfLeague.Api;

public static  class GolfApiEndpoints
{
    private const string ApiBase = "api";

    public static class Members
    {
        private const string Base = $"{ApiBase}/members";

        public const string GetAllMembers = Base;
    }
}