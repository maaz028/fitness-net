namespace infrastructure.AddionalClasses
{
    public class Pagination
    {
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public Pagination(int pageSize, int totalItems)
        {
            PageSize = pageSize;
            TotalItems = totalItems;
        }
        public int GeneratePages()
        {
            return (int)Math.Ceiling((decimal)TotalItems/(decimal)PageSize);
        }
    }
}
