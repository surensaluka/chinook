namespace Chinook.Services
{
    public interface IEventService
    {
        event Action RefreshPlaylistEvent;

        void SendRefreshPlaylistEvent();
    }
}