using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class CreateContractController : Controller
    {
        public IActionResult CreateContract()
        {  
            return View(DappAccountController.myAccount);
        }
    }
}