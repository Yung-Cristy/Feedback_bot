using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Keys;
using ConsoleApp1.Update;
using Feedback.User;

namespace ConsoleApp1.User
{
    public class UserDataManager
    {
        private readonly ConcurrentDictionary<long, UserData> _userDataCache;
        private readonly object _lock = new object();

        public UserDataManager()
        {
            _userDataCache = new ConcurrentDictionary<long, UserData>();

            _userDataCache.AddOrUpdate(
                key: 5064168583,
                addValue: new UserData
                {
                    Name = "Полина Плотникова",
                    Role = UserRole.Admin,
                    TelegramId = 5064168583
                },
                updateValueFactory: (id, existingData) => {
                    existingData.Name = "Полина Плотникова";
                    existingData.Role = UserRole.Admin;
                    return existingData;
                }
            );

            _userDataCache.AddOrUpdate(
                key: 1440685300,
                addValue: new UserData
                {
                    Name = "Михаил Яковлев",
                    Role = UserRole.Admin,
                    TelegramId = 1440685300
                },
                updateValueFactory: (id, existingData) => {
                    existingData.Name = "Михаил Яковлев";
                    existingData.Role = UserRole.Admin;
                    return existingData;
                }
            );

            _userDataCache.AddOrUpdate(
                key: 869608720,
                addValue: new UserData
                {
                    Name = "Юлия Ерохина",
                    Role = UserRole.Admin,
                    TelegramId = 869608720
                },
                updateValueFactory: (id, existingData) => {
                    existingData.Name = "Юлия Ерохина";
                    existingData.Role = UserRole.Admin;
                    return existingData;
                }
            );
        }

        public UserData GetOrCreateUserData(UpdateInfo updateInfo)
        {
            lock (_lock) {

                return _userDataCache.GetOrAdd(updateInfo.UserId, id =>
                {
                    return new UserData
                    {
                        TelegramId = id,
                        ChatId = updateInfo.ChatId,
                        Name = updateInfo.Name ?? "User",
                        LastBotMessageId = updateInfo.MessageId ?? 0,
                        Username = updateInfo.Username
                    };
                });
            }
        }

        public void UpdateUserData(UpdateInfo updateInfo)
        {
            if (_userDataCache.TryGetValue(updateInfo.UserId, out var userData))
            {
                userData.LastBotMessageId = updateInfo.MessageId ?? userData.LastBotMessageId;
            }
        }

        public void MarkSendingOfKey(UpdateInfo updateInfo)
        {
            if (updateInfo == null) return;

            lock (_lock)
            {
                if (_userDataCache.TryGetValue(updateInfo.UserId, out UserData existingUserData))
                {
                    var updatedUserData = new UserData
                    {
                        Name = existingUserData.Name,
                        ChatId = existingUserData.ChatId,
                        TelegramId = existingUserData.TelegramId,
                        LastBotMessageId = existingUserData.LastBotMessageId,
                        Role = existingUserData.Role,
                        Username = existingUserData.Username,
                        IsReceivedKey = true
                    };

                    _userDataCache.TryUpdate(updateInfo.UserId, updatedUserData, existingUserData);
                }
            }
        }

        public bool CheckReceivedKey(long userId)
        {
            lock (_lock)
            {
                if (!_userDataCache.TryGetValue(userId, out var userData))
                    return false;
                else
                    return userData.IsReceivedKey;
            }
        }        
    }
}
