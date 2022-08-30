namespace WebApiAutores.DTOs
{
    public class PaginationDTO
    {

        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private readonly int MaxPageSize = 50;
        public int RecordsPerPage
        {
            get {
                return recordsPerPage;
            }

            set {
                recordsPerPage = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
    }
}
