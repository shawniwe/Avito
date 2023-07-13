using Avito2.Abstract;
using Avito2.Domains;
using Avito2.Models;
using Avito2.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Avito2.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository<Advertisement> advertisementRepository;
        private readonly IRepository<Message> messagesRepository;
        private readonly IRepository<Rate> rateRepository;

        public MessagesController(UserManager<ApplicationUser> userManager, IRepository<Advertisement> advertisementRepository, IRepository<Message> messagesRepository, IRepository<Rate> rateRepository)
        {
            this.userManager = userManager;
            this.advertisementRepository = advertisementRepository;
            this.messagesRepository = messagesRepository;
            this.rateRepository = rateRepository;
        }

        public IActionResult Dialog(string interlocutor, long advertisement)
        {
            var interlocutorUser = userManager.FindByIdAsync(interlocutor).Result;
            var currentUser = userManager.GetUserAsync(HttpContext.User).Result;
            var ad = advertisementRepository.Read(advertisement);
            var rates = rateRepository.ReadList().Where(x => x.TargetUserId == interlocutor);
            float averageGrade = (float)(rates.Any() ? rates.Average(x => x.Grade) : 0);
            var model = new DialogMessagesModel()
            {
                Advertisement = ad,
                InterlocutorId = interlocutorUser.Id,
                InterlocutorName = interlocutorUser.FullName,
                Messages = messagesRepository
                .ReadList()
                .Where(x => (x.ReceiverId == interlocutor && x.SenderId == currentUser.Id && x.Advertisement == ad) 
                || (x.ReceiverId == currentUser.Id && x.SenderId == interlocutor && x.Advertisement == ad)),
                AverageGrade = averageGrade
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Send(string interlocutor, long advertisement, string message)
        {
            if(ModelState.IsValid)
            {
                var ad = advertisementRepository.Read(advertisement);
                var currentUser = userManager.GetUserAsync(HttpContext.User).Result;
                var interloc = userManager.FindByIdAsync(interlocutor).Result;

                messagesRepository.Create(new Message()
                {
                    Advertisement = ad,
                    Date = DateTime.Now,
                    ReceiverId = interlocutor,
                    SenderId = currentUser.Id,
                    Text = message
                });


                var model = new DialogMessagesModel()
                {
                    Advertisement = ad,
                    InterlocutorId = interlocutor,
                    InterlocutorName = interloc.FullName,
                    Messages = messagesRepository
                .ReadList()
                .Where(x => (x.ReceiverId == interloc.Id && x.SenderId == currentUser.Id && x.Advertisement == ad)
                || (x.ReceiverId == currentUser.Id && x.SenderId == interloc.Id && x.Advertisement == ad))
                };

                return RedirectToAction("Dialog", new { interlocutor = interlocutor, advertisement = ad.Id });
            }
            return View("Error");
        }

        public IActionResult Dialogs()
        {
            var currentUser = userManager.GetUserAsync(HttpContext.User).Result;
            var dialogs = messagesRepository
                .ReadList()
                .Where(x => (x.ReceiverId == currentUser.Id || x.SenderId == currentUser.Id))
                .Select(x => new
                {
                    Interlocutor = (x.SenderId != currentUser.Id ? x.SenderId : x.ReceiverId),
                    Advertisement = advertisementRepository.Read(x.Advertisement.Id)
                })
                .Where(x => x.Interlocutor != currentUser.Id)
                .Distinct()
                .Select(x => new DialogItemModel()
                {
                    Advertisement = x.Advertisement,
                    InterlocutorId = x.Interlocutor,
                    AverageRate = rateRepository.ReadList().Where(y => y.TargetUserId == x.Interlocutor).Any() ? rateRepository.ReadList().Where(y => y.TargetUserId == x.Interlocutor).Sum(x => x.Grade) /
                    rateRepository.ReadList().Where(y => y.TargetUserId == x.Interlocutor).Count() : 0,
                    LastMessage = messagesRepository.ReadList()
                    .Where(y => (y.SenderId == x.Interlocutor && y.ReceiverId == currentUser.Id) ||
                                (y.SenderId == currentUser.Id && y.ReceiverId == x.Interlocutor)).OrderByDescending(x => x.Date).FirstOrDefault(),
                    InterlocutorName = userManager.FindByIdAsync(x.Interlocutor).Result.FullName
                });

            return View(dialogs);
        }
    }
}
