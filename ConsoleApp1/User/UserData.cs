using ConsoleApp1.User;

namespace Feedback.User
{
    public class UserData
    {
        public string Name { get; set; }
        public long ChatId { get; set; }
        public long TelegramId { get; set; }
        public int LastBotMessageId { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        public string Username { get; set; }
    }
}