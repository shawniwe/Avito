using Avito2.Domains;
using Avito2.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Avito2.Models
{
    public class AdModeratorModel
    {
        [HiddenInput(DisplayValue = false)]
        public long Id { get; set; }
        [ReadOnly(true)]
        [Display(Name ="Заголовок")]
        public string Title { get; set; }
        [ReadOnly(true)]
        [Display(Name = "Цена")]
        public float Price { get; set; }
        [ReadOnly(true)]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [ReadOnly(true)]
        [Display(Name = "Город")]
        public string City { get; set; }
        [ReadOnly(true)]
        [Display(Name = "Адрес")]
        public string Address { get; set; }
        [ReadOnly(true)]
        [HiddenInput(DisplayValue = false)]
        public long? StatementId { get; set; }
        [ReadOnly(true)]
        [Display(Name = "Фото")]
        public ICollection<Photo> Photos { get; set; }
        [ReadOnly(true)]
        [Display(Name = "Дата публикации")]
        public DateTime PlacementDate { get; set; }
        [HiddenInput()]
        [ReadOnly(true)]
        [Display(Name = "Автор")]
        public ApplicationUser CreationAuthor { get; set; }
        [Display(Name = "Статус")]
        public List<SelectListItem> Statements { get; set; }
        public Category Category { get; set; }
    }
}
