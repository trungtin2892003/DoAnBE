﻿using Microsoft.AspNetCore.Mvc;

namespace ShopCake.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
