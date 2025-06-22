using ConsoleApp1.Keys;
using ConsoleApp1.Update;
using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace ConsoleApp1.Database
{
    public class KeyRepository : IDisposable
    {
        private readonly SqliteConnection _connection;

        public KeyRepository()
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var dbPath = Path.Combine(appDirectory, "Database", "nag-feedback.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

            _connection = new SqliteConnection($"Data Source={dbPath}");
            _connection.Open();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            var command = _connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Keys (
                    value TEXT PRIMARY KEY,
                    link TEXT NOT NULL,
                    isActive INTEGER NOT NULL,
                    assigned_to TEXT,
                    assignment_date TEXT
                )";
            command.ExecuteNonQuery();
        }

        public void Add(Key key)
        {
            var command = _connection.CreateCommand();
            command.CommandText = @"
                                    INSERT INTO Keys (value, link, isActive)
                                    SELECT @value, @link, @isActive
                                    WHERE NOT EXISTS (SELECT 1 FROM Keys WHERE value = @value)";

            command.Parameters.AddWithValue("@value", key.Id);
            command.Parameters.AddWithValue("@link", key.Link);
            command.Parameters.AddWithValue("@isActive", key.IsActive ? 1 : 0);

            command.ExecuteNonQuery();
        }

        public Key GetAndDeactivateKey(UpdateInfo update)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var key = GetActiveKeyWithLock(transaction);
                    if (key == null)
                    {
                        transaction.Commit();
                        return null;
                    }

                    DeactivateKey(key.Id, transaction, update);
                    transaction.Commit();
                    return key;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private Key GetActiveKeyWithLock(SqliteTransaction transaction)
        {
            var command = _connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"
                SELECT value, link, isActive 
                FROM Keys 
                WHERE isActive = 1 
                LIMIT 1";

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Key
                    {
                        Id = reader.GetString(0),
                        Link = reader.GetString(1),
                        IsActive = reader.GetInt32(2) == 1
                    };
                }
            }
            return null;
        }

        private void DeactivateKey(string keyId, SqliteTransaction transaction, UpdateInfo update)
        {         
            var command = _connection.CreateCommand();
            var deactivationTime = GetDeactivationTime();
            command.Transaction = transaction;
            command.CommandText = @"
                UPDATE Keys 
                SET isActive = 0,
                    assigned_to = @assigned_to,
                    assignment_date = @assignment_date
                    
                WHERE value = @value";

            command.Parameters.AddWithValue("@value", keyId);
            command.Parameters.AddWithValue("@assigned_to", $"{update.Name} {update.Username}");
            command.Parameters.AddWithValue("@assignment_date", deactivationTime);
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        public bool KeyExists(string keyId)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT 1 FROM Keys WHERE value = @value LIMIT 1";
            command.Parameters.AddWithValue("@value", keyId);

            return command.ExecuteScalar() != null;
        }

        public SqliteTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        private string GetDeactivationTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeSpan offset = TimeSpan.FromHours(5);
            DateTime ekaterinburgTime = utcNow + offset;
            return ekaterinburgTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}