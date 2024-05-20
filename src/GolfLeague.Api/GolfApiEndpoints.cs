namespace GolfLeague.Api;

public static class GolfApiEndpoints
{
    private const string ApiBase = "api";

    public static class Members
    {
        private const string Base = $"{ApiBase}/members";

        public const string Create = Base;
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:int}}";
        public const string Delete = $"{Base}/{{id:int}}";
        public const string Tag = "Members";
    }
}