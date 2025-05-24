using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Keys;
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
        }

        public UserData GetOrCreateUserData(long userId)
        {
            lock (_lock) {

                return _userDataCache.GetOrAdd(userId, id => new UserData );
            }
        }     
    }
}
