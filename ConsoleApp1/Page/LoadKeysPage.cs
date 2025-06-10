using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.User;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Page
{
    public class LoadKeysPage : Feedback.Pages.Page
    {
        public override string Text => "Загрузите Excel-файл (.xlsx) с ключами";

        public override InlineKeyboardMarkup GetKeyboard(UserRole userRole)
        {
            return new InlineKeyboardMarkup(
                InlineKeyboardButton.WithCallbackData("Вернуться в главное меню"));
        }
    }
}
