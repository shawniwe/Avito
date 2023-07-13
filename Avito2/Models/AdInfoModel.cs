using Avito2.Domains;
using Avito2.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Avito2.Models
{
    public class AdInfoModel
    {
        public bool CanEdit { get; set; }
        public ApplicationUser CreationAuthor { get; set; }
        public Advertisement Advertisement { get; set; }
        public float AverageGrade { get; set; }
        public List<SelectListItem> Grades { get; set; }
        public int SelectedGrade { get; set; }
    }
}
