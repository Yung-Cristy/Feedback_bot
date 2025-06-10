using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ConsoleApp1.Database
{
    public class Database : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly string _dbPath;

        public Database()
        {

            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _dbPath = Path.Combine(appDirectory, "Database", "nag-feedback.db");
            Directory.CreateDirectory(Path.GetDirectoryName(_dbPath));

            _connection = new SqliteConnection($"Data Source={_dbPath}");
            _connection.Open();

            InitializeTable();
        }

        public void DeleteKeys()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Keys";
                command.ExecuteNonQuery();
            }
        }

        private void InitializeTable()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Keys (
                        id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        value TEXT NOT NULL UNIQUE,
                        link TEXT NOT NULL UNIQUE,
                        isActive INTEGER NOT NULL DEFAULT 1 CHECK(isActive IN (0, 1)),
                        assigned_to TEXT,
                        assignment_date TEXT
                    )";
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}