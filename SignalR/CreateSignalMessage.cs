using Microsoft.AspNetCore.SignalR;

namespace SmartSheetLoader.SignalR
{
    public class CreateSignalMessage : Hub<ICreateSignalMessage>
    {
        public async Task SendStringMessageAsync(string transactionId)
        {
            await Clients.All.SendStringMessageAsync(transactionId);
            Console.WriteLine("SendStringMessageAsync");
        }
    }
}
