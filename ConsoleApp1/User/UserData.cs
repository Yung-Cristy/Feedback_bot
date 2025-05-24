
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.User;
using SixLabors.ImageSharp.PixelFormats;

namespace Feedback.User
{
    public class UserData
    {
        public string Name { get; set; }
        public long TelegramId { get; set; }
        public int LastBotMessageId { get; set; }
        public UserRole Role { get; set; } = UserRole.User;

        public string Username { get; set; }
    }
}