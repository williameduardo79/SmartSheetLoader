namespace SmartSheetLoader.SignalR
{
    public interface ICreateSignalMessage
    {
      
           
        Task SendStringMessageAsync(string transactionId);
        
    }
}
