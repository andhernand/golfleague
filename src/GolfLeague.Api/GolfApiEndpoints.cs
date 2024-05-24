namespace GolfLeague.Api;

public static class GolfApiEndpoints
{
    private const string ApiBase = "api";

    public static class Golfers
    {
        private const string Base = $"{ApiBase}/golfers";

        public const string Create = Base;
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:int}}";
        public const string Delete = $"{Base}/{{id:int}}";
        public const string Tag = "Golfers";
    }

    public static class Tournaments
    {
        private const string Base = $"{ApiBase}/tournaments";
        public const string Create = Base;
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        public const string Update = $"{Base}/{{id:int}}";
        public const string Delete = $"{Base}/{{id:int}}";
        public const string Tag = "Tournaments";
    }
}