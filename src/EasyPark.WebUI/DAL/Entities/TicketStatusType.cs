namespace EasyPark.WebUI.DAL.Entities
{
    public enum TicketStatusType
    {
        CheckedOut = 0,
        Dispatching = 1,
        Dispatched = 2,
        Parked = 3,
        Requested = 4,
        Returning = 5,
        CheckedIn = 6
    }
}