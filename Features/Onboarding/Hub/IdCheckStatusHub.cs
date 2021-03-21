using System;
using System.Threading.Tasks;

namespace Origin08.CustomerOnboarding.Features.Onboarding.Hub
{
    public record IdCheckStatusUpdate(
        string IdCheckWorkflowId,
        int IdCheckIndex,
        string Status
    );

    public interface IIdCheckStatusHubClient
    {
        Task IdCheckStatusUpdateReceived(IdCheckStatusUpdate statusUpdate);
    }

    public class IdCheckStatusHub : Microsoft.AspNetCore.SignalR.Hub<IIdCheckStatusHubClient>
    {
        public Task BroadcastIdCheckStatus(IdCheckStatusUpdate statusUpdate) =>
            Clients.All.IdCheckStatusUpdateReceived(statusUpdate);

        public async Task JoinGroup(string onboardingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, onboardingId);
        }

        public async Task LeaveGroup(string onboardingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, onboardingId);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}