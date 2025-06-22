using ConsoleApp1.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Page
{
    public class LimitKeyPage : Feedback.Pages.Page
    {
        public override string Text =>
            "❌ Вы уже использовали свой доступный ключ.\n " +
            "Каждый пользователь может получить только один ключ.";

        public override InlineKeyboardMarkup GetKeyboard(UserRole userRole)
        {
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("↩️ Вернуться в главное меню", "Вернуться в главное меню"));
        }
    }
}
