using System.Collections.Concurrent;
using System.Linq;
using Telegram.Bot;


namespace ConsoleApp1.Keys
{
    public class KeyManager
    {
        private ConcurrentDictionary<string, Key> _keys;
        private readonly ITelegramBotClient _client;

        private readonly object _lock = new object();
        public KeyManager(ITelegramBotClient client)
        {
            _client = client;
            _keys = new ConcurrentDictionary<string, Key>();       
            ParseKeysFromFile();
        }

        public async Task GetAvailableKey()
        {
            Key availableKey;

            lock (_lock)
            {
                 availableKey = _keys.FirstOrDefault(x => x.Value.IsActive).Value;

                if (availableKey is null) 
                {
                       _client.SendMessage
                }
            }

            
        }

        public void ParseKeysFromFile()
        {
            var parser = new KeyParser();
            var parsedKeys = parser.keys;

            parsedKeys.ForEach(key => _keys.TryAdd(key.Id, key));
        }
    }
}
