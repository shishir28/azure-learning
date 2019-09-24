using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ColorWebsite.Models;
using ColorWebsite.Data.Services;

namespace ColorWebsite.Controllers
{
    public class HomeController : Controller
    {
        private IColorService _colorService;
        public HomeController(IColorService colorService)
        {
            _colorService = colorService;

        }
        public ActionResult Index()
        {
            var model = _colorService.GetAllColors();

            return View(model);
        }

       
    }
}
