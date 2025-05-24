using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Feedback.User;
using ConsoleApp1.User;

namespace Feedback.Pages
{
    public abstract class Page
    {
        public abstract string Text { get; }
        public abstract InlineKeyboardMarkup GetKeyboard(UserRole userRole);

        public async Task SendAsync(ITelegramBotClient client, long chatId, UserData userData)
        {
            var sentMessage = await client.SendMessage(
                chatId: chatId,
                text: Text,
                replyMarkup: GetKeyboard(userData.Role)
            );

            userData.LastBotMessageId = sentMessage.MessageId;
        }
        public async Task UpdateAsync(ITelegramBotClient client, long chatId, int messageId, UserData userData)
        {
            await client.DeleteMessage(chatId, messageId);

            var sentMessage = await client.SendMessage(
                chatId: chatId,
                text: Text,
                replyMarkup: GetKeyboard(userData.Role)
            );

            userData.LastBotMessageId = sentMessage.MessageId;
        }

        public async Task EditAsync(ITelegramBotClient client, long chatId, int messageId, UserData userData)
        {
            try
            {
                var keyboard = GetKeyboard(userData.Role);

                if (keyboard != null)
                {
                    await client.EditMessageText(
                        chatId: chatId,
                        messageId: userData.LastBotMessageId,
                        text: Text,
                        replyMarkup: GetKeyboard(userData.Role)
                    );
                }
                else
                {
                    await client.EditMessageText(
                        chatId: chatId,
                        messageId: userData.LastBotMessageId,
                        text: Text
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при редактировании сообщения: {ex.Message}");

                await UpdateAsync(
                    client: client,
                    chatId: chatId,
                    messageId: userData.LastBotMessageId,
                    userData: userData);
            }
        }
    }
}
