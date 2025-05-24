using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Keys;
using ConsoleApp1.User;
using Feedback.Pages;
using SixLabors.ImageSharp.PixelFormats;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Pages
{
    public class SendKeyPage : Page
    {
        public override string Text => string.Empty;
        

        public SendKeyPage(KeyManager keyManager)
        {
            AvailableKeys = new List<Key>();

            keyManager.
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

        


    }
}
