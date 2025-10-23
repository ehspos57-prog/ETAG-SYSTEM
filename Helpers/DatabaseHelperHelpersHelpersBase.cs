using System.Data.SQLite;
using System.IO;

internal static class DatabaseHelperHelpersHelpersBase
{
    private static string _dbPath;

    public static string SetDatabasePath(string path)
    {
        _dbPath = path;
        if (!File.Exists(_dbPath))
            SQLiteConnection.CreateFile(_dbPath);
    }
}