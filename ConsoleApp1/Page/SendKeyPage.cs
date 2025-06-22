using ConsoleApp1.Keys;
using ConsoleApp1.Update;
using ConsoleApp1.User;
using Feedback.Pages;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Pages
{
    public class SendKeyPage : Feedback.Pages.Page
    {
        public override string Text { get; }

        private readonly KeyManager _keyManager;
        private readonly UpdateInfo _updateInfo;
        private readonly string _link;

        public SendKeyPage(KeyManager keyManager, UpdateInfo updateInfo)
        {
            _keyManager = keyManager;
            _updateInfo = updateInfo;
            _link = InitializeMessageAsync().GetAwaiter().GetResult(); 
            Text = _link;
        }

        public override InlineKeyboardMarkup GetKeyboard(UserRole userRole)
        {
            return new InlineKeyboardMarkup(
            [
                [
                    InlineKeyboardButton.WithCallbackData("Вернуться в главное меню")
                ]
            ]);
        }

        private async Task<string> InitializeMessageAsync()
        {
            var key = await _keyManager.GetAvailableKey(_updateInfo);

            if (string.IsNullOrEmpty(key))
            {
                return "Свободных ключей нет. Просьба обратиться в службу технической поддержки";
            }

            return $"Ваша ссылка для прохождение опроса - {key}";
        }
    }
}