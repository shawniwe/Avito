using Avito2.Abstract;
using Avito2.Domains;
using Avito2.Models;
using Avito2.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Avito2.Controllers
{
    public class RateController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository<Rate> rateRepository;

        public RateController(UserManager<ApplicationUser> userManager, IRepository<Rate> rateRepository)
        {
            this.userManager = userManager;
            this.rateRepository = rateRepository;
        }

        [HttpPost]
        public IActionResult Set(string targetUser, string sourceUser, int grade, string backUrl)
        {
            if (string.IsNullOrWhiteSpace(targetUser)) return View("Error", new ErrorViewModel() { RequestId = "Error" });
            if (grade <= 0 || grade > 5) return View("Error", "Ошибка выставления оценки");

            var rate = rateRepository.ReadList().Where(x => x.TargetUserId == targetUser && x.SoruceUserId == sourceUser).FirstOrDefault();
            if(rate == null)
            {
                rateRepository.Create(new Rate()
                {
                    TargetUserId = targetUser,
                    SoruceUserId = sourceUser,
                    Grade = grade
                });
            }
            else
            {
                rate.Grade = grade;
                rateRepository.Update(rate);
            }

            return Redirect(backUrl);
        }

        //public IActionResult Info(string user)
        //{
        //    if (string.IsNullOrWhiteSpace(user)) return View("Error", new ErrorViewModel { RequestId = "Ошибка" });

        //    var appUser = userManager.GetUserAsync(HttpContext.User).Result;


        //    return View();
        //}
    }
}
