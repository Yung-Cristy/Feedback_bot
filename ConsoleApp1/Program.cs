using ConsoleApp1.Database;
using ConsoleApp1.Keys;
using ConsoleApp1.Page;
using ConsoleApp1.Pages;
using ConsoleApp1.Services;
using ConsoleApp1.Update;
using ConsoleApp1.User;
using Feedback.User;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static UserStateManager _userStateManager;
    private static UserDataManager _userDataManager;
    private static KeyManager _keyManager;
    private static KeysFileImporter _keysImporter;

    static async Task Main(string[] args)
    {
        var telegramClient = new TelegramBotClient(token: "8033400730:AAEnqbeUmWZoHZEa7ed-RZjwQAcEHAF-kVo");
        _userDataManager = new UserDataManager();
        _userStateManager = new UserStateManager(telegramClient, _userDataManager);
        _keyManager = new KeyManager(telegramClient, _userDataManager);
        _keysImporter = new KeysFileImporter(_keyManager);

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };

        telegramClient.StartReceiving(
            updateHandler: HandleUpdate,
            errorHandler: HandleError,
            receiverOptions: receiverOptions);

        Console.WriteLine("Бот запущен и ожидает сообщений...");
        await Task.Delay(-1);
    }

    private static async Task HandleError(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {exception.GetType()}: {exception.Message}");
        if (exception is ApiRequestException apiEx)
        {
            Console.WriteLine($"Код ошибки Telegram: {apiEx.ErrorCode}");
        }
    }

    private static async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        try
        {
            var updateInfo = new UpdateInfo(update);
            var userData = _userDataManager.GetOrCreateUserData(updateInfo);

            switch (update.Type)
            {
                case UpdateType.Message:
                    if (update.Message?.Document != null && userData.Role == UserRole.Admin)
                        await HandleFile(client, updateInfo);
                    else if (update.Message?.Text != null)
                        await HandleMessage(client, updateInfo);
                    break;
                case UpdateType.CallbackQuery:
                    await HandleCallbackQuery(client, updateInfo);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки апдейта: {ex}");
        }
    }

    private static async Task HandleCallbackQuery(ITelegramBotClient client, UpdateInfo updateInfo)
    {
        try
        {
            await client.AnswerCallbackQuery(updateInfo.Update.CallbackQuery.Id);

            switch (updateInfo.Text)
            {
                case "Выдать ключ":
                case "SendKey": 
                    if (!_userDataManager.CheckReceivedKey(updateInfo.UserId))
                    {
                        await _userStateManager.UpdatePageAsync(updateInfo, new SendKeyPage(_keyManager, updateInfo));
                        _userDataManager.MarkSendingOfKey(updateInfo);
                    }
                    else
                    {
                        await client.AnswerCallbackQuery(
                            updateInfo.Update.CallbackQuery.Id,
                            "❌ Вы уже получали ключ ранее",
                            showAlert: true);

                        await _userStateManager.UpdatePageAsync(updateInfo, new LimitKeyPage());
                    }
                    break;
                case "Загрузить ключи":
                    await _userStateManager.UpdatePageAsync(updateInfo, new LoadKeysPage());
                    break;
                case "Вернуться в главное меню":
                    await _userStateManager.ShowPageAsync(updateInfo, new MainMenu());
                    break;
                default:
                    Console.WriteLine($"Неизвестный callback: {updateInfo.Text}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки callback: {ex}");
        }
    }

    private static async Task HandleFile(ITelegramBotClient client, UpdateInfo updateInfo)
    {
        if (_userStateManager.GetCurrentPage(updateInfo.UserId) is LoadKeysPage)
        {
            await ProcessKeyFileUpload(client, updateInfo);
        }
    }

    private static async Task HandleMessage(ITelegramBotClient client, UpdateInfo updateInfo)
    {
        switch (updateInfo.Text)
        {
            case "/start":
                await _userStateManager.ShowPageAsync(updateInfo, new MainMenu());
                break;
            case "/send_key":
                await _userStateManager.UpdatePageAsync(updateInfo, new SendKeyPage(_keyManager, updateInfo));
                break;
        }
    }

    private static async Task ProcessKeyFileUpload(ITelegramBotClient client, UpdateInfo updateInfo)
    {
        try
        {
            var document = updateInfo.Update.Message.Document;

            if (!document.FileName.EndsWith(".xlsx"))
            {
                await client.SendMessage(updateInfo.ChatId, "Требуется файл .xlsx");
                return;
            }

            var result = await _keysImporter.ImportKeysFile(client, document, updateInfo.ChatId);

            if (result.IsSuccess)
            {
                await _userStateManager.ShowPageAsync(updateInfo, new MainMenu());
            }
        }
        catch (Exception ex)
        {
            await client.SendMessage(updateInfo.ChatId, $"Ошибка: {ex.Message}");
        }
    }
}