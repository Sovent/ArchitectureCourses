using System.Threading.Tasks;

namespace Patterns
{
    public class Chat_TransactionExample
    {
        private readonly ICache _cache;
        private readonly IChatRepository _repository;

        public async Task PostMessage(int chatId, string message)
        {
            var chat = await _cache.GetValue<IChat>(chatId.ToString()) ?? await _repository.LoadChat(chatId);
            chat.AppendMessage(message);
            await _cache.SaveValue(chatId.ToString(), chat);
            await _repository.SaveChat(chat);
        }
    }

    public interface ICache
    {
        Task SaveValue<T>(string key, T value);

        Task<T> GetValue<T>(string key);
    }

    public interface IChatRepository
    {
        Task<IChat> LoadChat(int id);

        Task SaveChat(IChat chat);
    }

    public interface IChat
    {
        int ChatId { get; }
        void AppendMessage(string message);
    }
}