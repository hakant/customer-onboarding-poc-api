using System;
using System.Threading.Tasks;
using Origin08.CustomerOnboarding.Data;

namespace Origin08.CustomerOnboarding.Features.Onboarding.Hub
{
    public record IdCheckStatusUpdate(string IdCheckWorkflowId, int IdCheckIndex, string Status);

    public interface IIdCheckStatusHubClient
    {
        Task IdCheckStatusUpdateReceived(IdCheckStatusUpdate statusUpdate);
    }

    public class IdCheckStatusHub : Microsoft.AspNetCore.SignalR.Hub<IIdCheckStatusHubClient>
    {
        public Task BroadcastIdCheckStatus(IdCheckStatusUpdate statusUpdate) =>
            Clients.All.IdCheckStatusUpdateReceived(statusUpdate);
        
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR");
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR");
            await base.OnDisconnectedAsync(exception);
        }
    }
}