using Avito2.Abstract;
using Avito2.Domains;
using Avito2.Models;
using Avito2.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Avito2.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly IRepository<Advertisement> advertisementRepository;
        private readonly IRepository<Category> categoriesRepository;
        private readonly IRepository<AdvertisementStatement> statementRepository;
        private readonly IRepository<Photo> photoRepository;
        private readonly UserManager<Avito2.Users.ApplicationUser> userManager;
        private readonly IRepository<Rate> rateRepository;

        public AdvertisementsController(IRepository<Advertisement> advertisementRepository,
            IRepository<Category> categoriesRepository,
            IRepository<AdvertisementStatement> statementRepository,
            IRepository<Photo> photoRepository,
            UserManager<Avito2.Users.ApplicationUser> userManager,
            IRepository<Rate> rateRepository)
        {
            this.advertisementRepository = advertisementRepository;
            this.categoriesRepository = categoriesRepository;
            this.statementRepository = statementRepository;
            this.photoRepository = photoRepository;
            this.userManager = userManager;
            this.rateRepository = rateRepository;
        }

        public IActionResult Info(long id)
        {
            var ad = advertisementRepository.Read(id);
            var currentUser = userManager.GetUserAsync(HttpContext.User).Result;
            var creationAuthor = userManager.FindByIdAsync(ad.CreationAuthorId).Result;

            if (ad.Statement.Name != "Активно" && currentUser != creationAuthor)
            {
                return View("Error", new ErrorViewModel()
                {
                    RequestId = "Ошибка доступа!"
                });
            }

            var rateList = rateRepository.ReadList().Where(x => x.TargetUserId == creationAuthor.Id);
            float avg = 0;
            if (rateList.Any())
            {
                avg = (float)rateList.Average(x => x.Grade);
            }

            int currentGrade = 0;
            if(currentUser != null)
            {
                var rate = rateRepository.ReadList().Where(x => x.TargetUserId == creationAuthor.Id && x.SoruceUserId == currentUser.Id).FirstOrDefault();
                if (rate != null)
                {
                    currentGrade = rate.Grade;
                }
            }

            var model = new AdInfoModel()
            {
                Advertisement = ad,
                CreationAuthor = creationAuthor,
                Grades = (new List<int> { 1, 2, 3, 4, 5 }).Select(x => new SelectListItem() { Text = x.ToString(), Value = x.ToString() }).ToList(),
                SelectedGrade = currentGrade,
                AverageGrade = avg,
            };
            if(currentUser != null)
            {
                var currentUserRole = userManager.GetRolesAsync(currentUser).Result.First();
                model.CanEdit = (creationAuthor == currentUser || currentUserRole == "Moderator") && ad.Statement.Name != "Снято с публикации";
            }
            return View(model);
        }

        [Authorize]
        public IActionResult Create()
        {
            var model = new AdEditModel(categoriesRepository.ReadList().ToList());
            return View(model);
        }

        private List<string> UploadImages(AdEditModel model)
        {
            if (model.ImagesFiles == null)
            {
                Debug.WriteLine("картинка не найдена");
                return null;
            }

            List<string> loadedFiles = new List<string>();

            foreach (var file in model.ImagesFiles)
            {
                string ext = Path.GetExtension(file.FileName);
                string uniqueName = Guid.NewGuid().ToString() + ext;
                string filename = Path.Combine(
                Directory.GetCurrentDirectory(),
                @"wwwroot\images",
                uniqueName);
                using (var stream = System.IO.File.Create(filename))
                {
                    file.CopyTo(stream);
                }

                loadedFiles.Add(uniqueName);
            }

            return loadedFiles;
        }

        [Authorize]
        public IActionResult Edit(long id)
        {
            var entry = advertisementRepository.Read(id);
            var currentUser = userManager.GetUserAsync(HttpContext.User).Result;
            if(entry.CreationAuthorId != currentUser.Id)
            {
                var role = userManager.GetRolesAsync(currentUser).Result.First();
                if(role == "Moderator")
                {
                    return RedirectToAction("Edit", "Moderator", new { Id = entry.Id });
                }
                else
                {
                    return View("Error", new ErrorViewModel()
                    {
                        RequestId = "Ошибка доступа!"
                    });
                }
            }
            var model = new AdEditModel(categoriesRepository.ReadList().ToList())
            {
                // дописать всё
                Address = entry.Address,
                City = entry.City,
                CreationAuthor = userManager.FindByIdAsync(entry.CreationAuthorId).Result,
                Description = entry.Description,
                Id = entry.Id,
                Photos = entry.Photos,
                PlacementDate = entry.PlacementDate,
                Price = entry.Price,
                Title = entry.Title,
                Category = entry.Category.Id
            };
                

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(AdEditModel model)
        {
            if(ModelState.IsValid)
            {
                var entry = advertisementRepository.Read(model.Id);
                entry.Title = model.Title;
                entry.Address = model.Address;
                entry.Price = model.Price;
                entry.City = model.City;
                var category = categoriesRepository.Read(model.Category);
                entry.Category = category;
                entry.Description = model.Description;
                entry.Statement = statementRepository.ReadList().Where(x => x.Name == "На модерации").First();
                var loaded = UploadImages(model);
                if (loaded != null)
                {
                    var photos = new List<Photo>();

                    foreach (var fileName in loaded)
                    {
                        var ph = new Photo()
                        {
                            Advertisement = entry,
                            FilePath = fileName
                        };

                        photos.Add(ph);
                    }

                    foreach (var ph in photos)
                    {
                        photoRepository.Create(ph);
                        entry.Photos.Add(ph);
                    }
                }
                advertisementRepository.Update(entry);
                return RedirectToAction("Info", new { Id = entry.Id });
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult Create(AdEditModel model)
        {
            if(ModelState.IsValid)
            {
                var category = categoriesRepository.Read(model.Category.HasValue ? model.Category.Value : 0);
                var user = userManager
                    .GetUserAsync(HttpContext.User)
                    .Result;

                var ad = new Advertisement()
                {
                    Title = model.Title,
                    Address = model.Address,
                    Category = category,
                    City = model.City,
                    Description = model.Description,
                    PlacementDate = System.DateTime.Now,
                    Price = model.Price,
                    Statement = statementRepository.ReadList().Where(x => x.Name == "На модерации").First(),
                    CreationAuthorId = user.Id
                };

                advertisementRepository.Create(ad);

                var loaded = UploadImages(model);
                if (loaded != null)
                {
                    var photos = new List<Photo>();

                    foreach (var fileName in loaded)
                    {
                        var ph = new Photo()
                        {
                            Advertisement = ad,
                            FilePath = fileName
                        };

                        photos.Add(ph);
                    }

                    foreach (var ph in photos)
                    {
                        photoRepository.Create(ph);
                        ad.Photos.Add(ph);
                    }

                    advertisementRepository.Update(ad);
                }
                return RedirectToAction("Info", new { Id = ad.Id });
            }
            var list = new List<SelectListItem>();
            list.AddRange(categoriesRepository.ReadList().Select(x => new SelectListItem { Text = x.Title, Value = x.Id.ToString() }));
            model.Categories = list;
            return View(model);
        }

        [Authorize]
        public IActionResult My()
        {
            var currentUser = userManager.GetUserAsync(HttpContext.User).Result;
            var entries = advertisementRepository.ReadList().Where(x => x.CreationAuthorId == currentUser.Id);
            return View(entries);
        }

        [Authorize]
        public IActionResult Delete(long id)
        {
            var entry = advertisementRepository.Read(id);
            entry.Statement = statementRepository.ReadList().Where(x => x.Name == "Снято с публикации").First();
            advertisementRepository.Update(entry);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult List(SearchQueryModel m)
        {
            var ads = advertisementRepository.ReadList().Where(x => x.Statement.Name == "Активно");
            if (!string.IsNullOrWhiteSpace(m.Query))
            {
                ads = ads.Where(x => x.Title.ToLower().Contains(m.Query.ToLower()) || x.Description.ToLower().Contains(m.Query));
            }

            if (m.CategoryId != null && m.CategoryId != -1)
            {
                ads = ads.Where(x => x.Category.Id == m.CategoryId);
            }

            AdSearchResultModel model = new AdSearchResultModel()
            {
                Query = m.Query,
                Advertisements = ads.Select(x => new AdInfoModel()
                {
                    Advertisement = x,
                    CreationAuthor = userManager.FindByIdAsync(x.CreationAuthorId).Result
                })
                
            };

            return View(model);
        }
    }
}
