namespace GolfLeague.Api.Tests.Integration;

public static class SolutionDirectoryFinder
{
    public static string GetSolutionDirectory(string solutionFileName)
    {
        if (string.IsNullOrWhiteSpace(solutionFileName))
        {
            throw new ArgumentNullException(nameof(solutionFileName),
                "Solution file name cannot be null or whitespace.");
        }

        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (directory != null && !File.Exists(Path.Combine(directory.FullName, solutionFileName)))
        {
            directory = directory.Parent;
        }

        if (directory == null)
        {
            throw new InvalidOperationException("Solution directory could not be found.");
        }

        return directory.FullName;
    }
}