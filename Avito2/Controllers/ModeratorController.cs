using Avito2.Abstract;
using Avito2.Domains;
using Avito2.Models;
using Avito2.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Avito2.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class ModeratorController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository<Advertisement> advertisementRepository;
        private readonly IRepository<AdvertisementStatement> statementsRepository;

        public ModeratorController(UserManager<ApplicationUser> userManager, IRepository<Advertisement> advertisementRepository, IRepository<AdvertisementStatement> statementsRepository)
        {
            this.userManager = userManager;
            this.advertisementRepository = advertisementRepository;
            this.statementsRepository = statementsRepository;
        }
        
        public IActionResult Edit(long id)
        {
            var entry = advertisementRepository.Read(id);
            var model = new AdModeratorModel()
            {
                Address = entry.Address,
                City = entry.City,
                CreationAuthor = userManager.FindByIdAsync(entry.CreationAuthorId).Result,
                Description = entry.Description,
                Id = entry.Id,
                Photos = entry.Photos,
                PlacementDate = entry.PlacementDate,
                Price = entry.Price,
                Title = entry.Title,
                Category = entry.Category,
                StatementId = entry.Statement.Id
            };
            model.Statements = new List<SelectListItem>();
            model.Statements.AddRange(statementsRepository.ReadList().Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }));
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(AdModeratorModel model)
        {
            var entry = advertisementRepository.Read(model.Id);
            entry.Statement = statementsRepository.Read(model.StatementId);
            advertisementRepository.Update(entry);
            return RedirectToAction("Advertisements", "Moderator");
        }

        public IActionResult Advertisements()
        {
            var models = advertisementRepository
                .ReadList()
                .Where(x => x.Statement.Name == "На модерации");
            return View(models);
        }
    }
}
