using ConsoleApp1.Update;
using ConsoleApp1.User;
using Feedback.Pages;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Feedback.User
{
    public class UserStateManager
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ConcurrentDictionary<long, Page> _userPages = new();
        private readonly UserDataManager _userDataManager;
        private readonly object _lock = new();

        public UserStateManager(ITelegramBotClient botClient, UserDataManager userDataManager)
        {
            _botClient = botClient;
            _userDataManager = userDataManager;
        }

        public async Task ShowPageAsync(UpdateInfo updateInfo, Page page)
        {
            var userData = _userDataManager.GetOrCreateUserData(updateInfo);
            UpdateCurrentPage(updateInfo.UserId, page);
            await page.SendAsync(_botClient, updateInfo.ChatId, userData);
        }

        public async Task UpdatePageAsync(UpdateInfo updateInfo, Page page)
        {
            var userData = _userDataManager.GetOrCreateUserData(updateInfo);
            UpdateCurrentPage(updateInfo.UserId, page);

            if (updateInfo.MessageId.HasValue)
            {
                await page.UpdateAsync(_botClient, updateInfo.ChatId, updateInfo.MessageId.Value, userData);
            }
            else
            {
                await page.SendAsync(_botClient, updateInfo.ChatId, userData);
            }
        }

        public async Task EditPageAsync(UpdateInfo updateInfo, Page page)
        {
            var userData = _userDataManager.GetOrCreateUserData(updateInfo);
            UpdateCurrentPage(updateInfo.UserId, page);

            if (userData.LastBotMessageId != 0)
            {
                try
                {
                    await page.EditAsync(_botClient, updateInfo.ChatId, userData.LastBotMessageId, userData);
                }
                catch
                {
                    await UpdatePageAsync(updateInfo, page);
                }
            }
            else
            {
                await ShowPageAsync(updateInfo, page);
            }
        }

        public Page GetCurrentPage(long userId)
        {
            return _userPages.TryGetValue(userId, out var page) ? page : null;
        }

        public async Task ReturnToPreviousPage(UpdateInfo updateInfo)
        {
            
        }

        public void UpdateCurrentPage (long userId,Page page)
        {
            _userPages[userId] = page;
        }
    }
}