using Avito2.Domains;
using Avito2.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace Avito2.Models
{
    public class AdEditModel
    {
        public AdEditModel()
        {

        }
        public AdEditModel(List<Category> categories)
        {
            Categories = new List<SelectListItem>();
            Categories.AddRange(categories.Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }));
        }
        [HiddenInput(DisplayValue = false)]
        public long? Id { get; set; }
        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public List<IFormFile> ImagesFiles { get; set; }
        [Required]
        [Display(Name = "Цена")]
        public float Price { get; set; }
        [Required]
        [Display(Name = "Город")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        public List<SelectListItem> Categories { get; set; }

        [Required]
        [Display(Name = "Категория")]
        public long? Category { get; set; }
        public IEnumerable<Photo> Photos { get; set; }
        public DateTime PlacementDate { get; set; }
        public ApplicationUser CreationAuthor { get; set; }
    }
}
