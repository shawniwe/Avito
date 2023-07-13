using Avito2.Abstract;
using Avito2.Domains;
using Avito2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Avito2.Components
{
    [ViewComponent]
    public class SearchViewComponent : ViewComponent
    {
        private readonly IRepository<Category> _categories;

        public SearchViewComponent(IRepository<Category> categories)
        {
            _categories = categories;
        }
        public IViewComponentResult Invoke()
        {
            var model = new SearchQueryModel();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "Все", Value = "-1" });
            items.AddRange(_categories.ReadList().Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }));

            model.Categories = items;
            model.Query = string.Empty;

            return View(model);
        }
    }
}
