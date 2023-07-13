using Avito2.Domains;
using System.Collections.Generic;

namespace Avito2.Models
{
    public class AdSearchResultModel
    {
        public string Query { get; set; }
        public IEnumerable<AdInfoModel> Advertisements { get; set; }

    }
}
