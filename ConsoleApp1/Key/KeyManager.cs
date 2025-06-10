using System.Collections.Concurrent;
using System.Linq;
using ConsoleApp1.Database;
using ConsoleApp1.Services;
using ConsoleApp1.Update;
using ConsoleApp1.User;
using Microsoft.Data.Sqlite;
using Telegram.Bot;

namespace ConsoleApp1.Keys
{
    public class KeyManager 
    {
        private readonly ITelegramBotClient _client;
        private readonly KeyRepository _repository;

        public KeyManager(ITelegramBotClient client, UserDataManager userDataManager)
        {
            _client = client;
            _repository = new KeyRepository();
        }

        public async Task<string?> GetAvailableKey(UpdateInfo updateInfo)
        {
            var availableKey = _repository.GetAndDeactivateKey(updateInfo);

            return availableKey?.Link;

        }

        public void GetKeysFromFile()
        {
            var parser = new KeyParser();
            var parsedKeys = parser.Keys;

            if (parsedKeys == null || !parsedKeys.Any())
            {
                Console.WriteLine("Предупреждение: Не удалось загрузить ключи или файл пуст.");
                return;
            }

            foreach (var key in parsedKeys)
            {
                _repository.Add(key);
            }
        }

        public ImportResult AddKeysFromFile(List<Key> keys)
        {
            int addedCount = 0;
            int duplicatesCount = 0;

            using (var transaction = _repository.BeginTransaction())
            {
                try
                {
                    foreach (var key in keys)
                    {
                        if (!_repository.KeyExists(key.Id))
                        {
                            _repository.Add(key);
                            addedCount++;
                        }
                        else
                        {
                            duplicatesCount++;
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return new ImportResult(true, addedCount, duplicatesCount);
        }
    }
}