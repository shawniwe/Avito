using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Avito2.Models
{
    public class SearchQueryModel
    {
        public string Query { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public long? CategoryId { get; set; }
    }
}
