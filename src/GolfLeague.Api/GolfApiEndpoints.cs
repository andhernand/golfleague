namespace GolfLeague.Api;

public static  class GolfApiEndpoints
{
    private const string ApiBase = "api";

    public static class Members
    {
        private const string Base = $"{ApiBase}/members";

        public const string GroupName = "Members";
        
        public const string GetAllMembers = Base;
        public const string GetMemberById = $"{Base}/{{id}}";
    }
}