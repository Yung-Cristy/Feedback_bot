using ConsoleApp1.Keys;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.IO;
using ConsoleApp1.Database;

namespace ConsoleApp1.Services
{
    public class KeysFileImporter
    {
        private readonly KeyManager _keyManager;

        public KeysFileImporter(KeyManager keyManager)
        {
            _keyManager = keyManager;
        }

        public async Task<ImportResult> ImportKeysFile(ITelegramBotClient client, Document document, long chatId)
        {
            try
            {
                if (!document.FileName.EndsWith(".xlsx"))
                {
                    await client.SendMessage(chatId, "❌ Файл должен быть в формате .xlsx");
                    return new ImportResult(false, 0, 0);
                }

                var filePath = await DownloadFile(client, document);
                var parsedKeys = new KeyParser(filePath).Keys;

                if (parsedKeys == null || !parsedKeys.Any())
                {
                    await client.SendMessage(chatId, "❌ Не удалось прочитать ключи из файла");
                    return new ImportResult(false, 0, 0);
                }

                var result = _keyManager.AddKeysFromFile(parsedKeys);

                if (result.AddedCount > 0)
                {
                    await client.SendMessage(
                        chatId,
                        $"✅ Успешно добавлено {result.AddedCount} новых ключей\n" +
                        $"⚡ Всего в файле: {parsedKeys.Count} ключей\n" +
                        $"🔍 Пропущено (дубликаты): {result.DuplicatesCount}");
                }
                else
                {
                    await client.SendMessage(chatId, "ℹ Все ключи из файла уже существуют в базе");
                }

                File.Delete(filePath);
                return result;
            }
            catch (Exception ex)
            {
                await client.SendMessage(chatId, $"⚠ Ошибка при импорте: {ex.Message}");
                return new ImportResult(false, 0, 0);
            }
        }

        private async Task<string> DownloadFile(ITelegramBotClient client, Document document)
        {
            var downloadsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
            Directory.CreateDirectory(downloadsPath);
            var filePath = Path.Combine(downloadsPath, document.FileName);

            var file = await client.GetFile(document.FileId);
            using (var saveStream = File.Create(filePath))
            {
                await client.DownloadFile(file.FilePath, saveStream);
            }

            return filePath;
        }
    }

    public record ImportResult(bool IsSuccess, int AddedCount, int DuplicatesCount);
}