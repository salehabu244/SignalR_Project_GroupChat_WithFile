using Microsoft.AspNetCore.SignalR;

namespace SignalR
{
    public class CustomerUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            throw new NotImplementedException();
        }
    }
}
