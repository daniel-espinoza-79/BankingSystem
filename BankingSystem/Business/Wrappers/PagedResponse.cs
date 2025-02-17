namespace Business.Wrappers
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public PagedResponse(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? new List<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public static PagedResponse<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            var itemsList = items.ToList();
            return new PagedResponse<T>(itemsList, totalCount, pageNumber, pageSize);
        }
    }
}
