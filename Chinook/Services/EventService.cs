namespace Chinook.Services
{
    public class EventService : IEventService
    {
        public event Action RefreshPlaylistEvent;

        public void SendRefreshPlaylistEvent()
        {
            RefreshPlaylistEvent?.Invoke();
        }
    }
}
