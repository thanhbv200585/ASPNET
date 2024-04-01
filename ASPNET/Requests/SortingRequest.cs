namespace ASPNET.Requests
{
    public record SortingRequest(int Page, int Take, string OrderBy, string OrderType);
}