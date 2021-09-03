namespace DotNetFive.Core.Pagination.DTO
{
    public abstract class PagedResultBase
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}
