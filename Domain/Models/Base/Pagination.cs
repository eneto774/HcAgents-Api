namespace HcAgents.Domain.Models.Base;

public class Pagination
{
    public int Page { get; set; }
    public int ItensPerPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}
