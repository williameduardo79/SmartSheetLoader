
namespace SmartSheetLoader.SignalR
{
    public interface IMessagingMemoryAccess
    {
        void AddMessage(string transactionId, string message);
        IDictionary<string, string> GetAllMessages();
        string? GetMessage(string transactionId);
        void RemoveMessage(string transactionId);
    }
}