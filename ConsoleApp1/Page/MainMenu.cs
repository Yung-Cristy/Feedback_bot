using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.User;
using Feedback.Pages;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Pages
{
    public class MainMenu : Page
    {
        public override string Text => "Привет! Данный бот предназначен для выдачи персональных ссылок для прохождения опросников." +
                                        "При нажатии на кнопку \"Выдать ключ\" бот направит вам ссылку на опрос. Данная ссылка будет действительна" +
                                        "пока вы не пройдете опрос.";

        public override InlineKeyboardMarkup GetKeyboard(UserRole userRole)
        {
            if (userRole == UserRole.Admin)
            {
                return new InlineKeyboardMarkup(
                    [
                        [
                            InlineKeyboardButton.WithCallbackData("Выдать ключ"),
                            InlineKeyboardButton.WithCallbackData("Загрузить ключи")
                        ]
                    ]);
            }
            return new InlineKeyboardMarkup(
                    [
                        [
                            InlineKeyboardButton.WithCallbackData("Выдать ключ")
                        ]
                    ]);
        }
    }
}
