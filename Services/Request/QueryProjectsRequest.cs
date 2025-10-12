using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Request
{
    public class QueryProjectsRequest
    {
        [FromQuery(Name = "q")]
        public string? SearchQuery { get; set; }

        [FromQuery(Name = "categoryId")]
        public Guid? CategoryId { get; set; }

        [FromQuery(Name = "status")]
        public string Status { get; set; } = "Published"; // Giá trị mặc định

        [FromQuery(Name = "sort")]
        public string SortBy { get; set; } = "newest"; // Giá trị mặc định

        [FromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        public int PageSize { get; set; } = 10;
    }
}
