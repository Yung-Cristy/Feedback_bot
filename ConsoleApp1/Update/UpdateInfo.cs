using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace ConsoleApp1.Update
{
    public class UpdateInfo
    {

        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string Text { get; }
        public int? MessageId { get; }
        public string Username { get; }
        
        public string Name { get; }

        public Telegram.Bot.Types.Update Update { get; }

        public UpdateInfo(Telegram.Bot.Types.Update update)
        {
            var type = update.Type;
            Update = update;

            switch (type)
            {
                case UpdateType.Message:
                    UserId = update.Message.From.Id;
                    ChatId = update.Message.Chat.Id;
                    Text = update.Message.Text;
                    MessageId = update.Message.MessageId;
                    Name = $"{update.Message.From.LastName} {update.Message.From.FirstName}";
                    Username = update.Message.From.Username;
                    break;

                case UpdateType.CallbackQuery:
                    UserId = update.CallbackQuery.From.Id;
                    ChatId = update.CallbackQuery.Message.Chat.Id;
                    Text = update.CallbackQuery.Data;
                    MessageId = update.CallbackQuery.Message.MessageId;
                    Username = update.CallbackQuery.From.Username;
                    break;

                default:
                    throw new NotSupportedException($"Unsupported update type: {update.Type}");
            }
        }

    }
}
