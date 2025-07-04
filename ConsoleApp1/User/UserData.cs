﻿using ConsoleApp1.User;
using NPOI.HSSF.Record;

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
        public bool IsReceivedKey { get; set; } = false;
    }
}