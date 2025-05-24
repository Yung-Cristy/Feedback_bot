using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Feedback.User;
using ConsoleApp1.Pages;
using ConsoleApp1.User;
using ConsoleApp1.Keys;

class Program
{
    private static UserStateManager _userStateManager;
    private static UserDataManager _userDataManager;
    private static KeyManager _keyManager;

    static async Task Main(string[] args)
    {
        var telegramClient = new TelegramBotClient(token: "7567444597:AAGTAeZ3tvitYv_CHqf0ZYhMy8fvh1TcIz8");
        _userStateManager = new UserStateManager(telegramClient);
        _userDataManager = new UserDataManager();
        _keyManager = new KeyManager(telegramClient);

        telegramClient.StartReceiving(updateHandler: HandleUpdate, errorHandler: HandleError);

        Console.ReadLine();
    }

    private static async Task HandleError(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
    }

    private static async Task HandleUpdate(ITelegramBotClient client, Update update, CancellationToken token)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                await HandleMessage(client, update);
                break;
            case UpdateType.CallbackQuery:
                await HandleCallbackQuery(client, update);
                break;
        }
    }

    private static async Task HandleCallbackQuery(ITelegramBotClient client, Update update)
    {
        var inputCallback = update.CallbackQuery.Data;
        var messageId = update.CallbackQuery.Message.Id;
        var chatId = update.CallbackQuery.Message.Chat.Id;
        var userId = update.CallbackQuery.From.Id;

        var userData = _userDataManager.GetOrCreateUserData(userId);

        switch (inputCallback) {
            case "Выдать ключ":

                
    }

    private static async Task HandleMessage(ITelegramBotClient client, Update update)
    {
        var chatId = update.Message.Chat.Id;
        var messageId = update.Message.MessageId;
        var text = update.Message.Text;
        var userId = update.Message.From.Id;

        var userData = _userDataManager.GetOrCreateUserData(userId);

        if (text == "/start")
        {
            await _userStateManager.ShowPageAsync(
                userId: userId,
                page: new MainMenu(),
                userData: userData);

            await DeleteUserMessage(client, chatId, messageId);
        }
    }

    private static async Task DeleteUserMessage(ITelegramBotClient client, long chatId, int messageId)
    {
        try
        {
            await client.DeleteMessage(
                chatId: chatId,
                messageId: messageId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении сообщения: {ex.Message}");
        }
    }
}