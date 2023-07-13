using Avito2.Abstract;
using Avito2.Data;
using Avito2.Data.Repositories;
using Avito2.Domains;
using Avito2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Avito2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Advertisement> _repository;
        public HomeController(IRepository<Advertisement> repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var models = _repository.ReadList().Where(x => x.Statement.Name == "Активно");
            return View(models);
        }
    }
}
