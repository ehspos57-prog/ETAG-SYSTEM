using System.Data.SQLite;
using System.IO;

internal static class DatabaseHelperHelpersHelpers
{
    private static string _dbPath;

    public static string DatabasePath { get; private set; }

    public static string SetDatabasePath(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            DatabasePath = path;
        }

        return DatabasePath;
    }

}