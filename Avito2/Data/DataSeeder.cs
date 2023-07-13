using Avito2.Abstract;
using Avito2.Domains;
using Avito2.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Avito2.Data
{
    public static class DataSeeder
    {
        public static void SeedRolesAndUsers(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            if(roleManager.Roles.Any())
            {
                return;
            }

            string[] roles = new string[]
            {
                "Admin",
                "Moderator",
                "User"
            };

            foreach (var r in roles)
            {
                var role = new IdentityRole { Name = r };
                var result = roleManager.CreateAsync(role).Result;
            }


            var user = new ApplicationUser()
            {
                Email = "testmail@yandex.ru",
                UserName = "testmail@yandex.ru",
                FirstName = "Испытатель",
                LastName = "Сайта",
                RegistrationDate = DateTime.Now
            };

            var userResult = userManager.CreateAsync(user, "test123").Result;
            var roleResult = userManager.AddToRoleAsync(user, "Moderator").Result;
        }
        
        public static void Seed(IServiceProvider provider, UserManager<ApplicationUser> userManager)
        {
            var advertisements = (IRepository<Advertisement>)provider.GetService(typeof(IRepository<Advertisement>));
            var categories = (IRepository<Category>)provider.GetService(typeof(IRepository<Category>));
            var adStatements = (IRepository<AdvertisementStatement>)provider.GetService(typeof(IRepository<AdvertisementStatement>));
            var photos = (IRepository<Photo>)provider.GetService(typeof(IRepository<Photo>));



            if (categories.ReadList().Any()) return;
            if (advertisements.ReadList().Any()) return;

            string[] categoriesNames =
            {
                "Транспорт",
                "Недвижимость",
                "Работа",
                "Услуги",
                "Личные вещи",
                "Для дома и дачи",
                "Запчасти и аксессуары",
                "Электроника",
                "Хобби и отдых",
                "Животные",
                "Бизнес и оборудование"
            };

            foreach (var c in categoriesNames)
            {
                categories.Create(new Category()
                {
                    Title = c
                });
            }

            var photo = new Photo()
            {
                FilePath = "image1.png",
            };

            photos.Create(photo);

            var photo2 = new Photo()
            {
                FilePath = "image2.png",
            };

            photos.Create(photo2);

            var adStatement = new AdvertisementStatement()
            {
                Name = "Активно"
            };

            var adStatement1 = new AdvertisementStatement()
            {
                Name = "На модерации"
            };

            var adStatement2 = new AdvertisementStatement()
            {
                Name = "Отклонено"
            };

            var adStatement3 = new AdvertisementStatement()
            {
                Name = "Снято с публикации"
            };

            adStatements.Create(adStatement);
            adStatements.Create(adStatement1);
            adStatements.Create(adStatement2);
            adStatements.Create(adStatement3);

            var advertisement = new Advertisement()
            {
                Address = "Челябинская область, Челябинск, Тракторозаводский район р - н Тракторозаводский",
                Category = categories.Read(1),
                City = "Челябинск",
                Description = "Описание этого прекрасного автомобиля!!! МАШИНА В ИДЕАЛЬНОМ СОСТОЯНИИ ПРОБЕГ 450000км",
                Photos = new List<Photo>() { photo, photo2 },
                PlacementDate = DateTime.Now,
                Price = 250000.945f,
                Statement = adStatement,
                Title = "Huyndai Elantra 2007",
                CreationAuthorId = userManager.Users.First().Id
            };

            photo.Advertisement = advertisement;
            photo2.Advertisement = advertisement;

            photos.Update(photo);
            photos.Update(photo2);
        }
    }
}
