namespace SmartSheetLoader.SignalR
{
    public class MessagingMemoryAccess: IMessagingMemoryAccess
    {
        private IDictionary<string, string> _backgroundMemoryAccesses;
        public MessagingMemoryAccess()
        {
            _backgroundMemoryAccesses = new Dictionary<string, string>();
        }
        public string? GetMessage(string transactionId)
        {
            if (_backgroundMemoryAccesses.ContainsKey(transactionId))
            {
                return _backgroundMemoryAccesses[transactionId];
            }
            return null;
        }
        public void AddMessage(string transactionId, string message)
        {
            if (!_backgroundMemoryAccesses.ContainsKey(transactionId))
            {
                _backgroundMemoryAccesses.Add(transactionId, message);
            }
        }
        public void RemoveMessage(string transactionId)
        {
            if (_backgroundMemoryAccesses.ContainsKey(transactionId))
            {
                _backgroundMemoryAccesses.Remove(transactionId);
                Console.WriteLine($"removing {transactionId}");
            }
        }
        public IDictionary<string, string> GetAllMessages()
        {

            return _backgroundMemoryAccesses;
        }
    }
}
