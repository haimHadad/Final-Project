using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class ContractsStatusController : Controller
    {
        public IActionResult ContractsStatusPage()
        {
            int i = 0;
            i = 1;
            return View(DappAccountController.myAccount);
        }
    }
}