using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    public class InGoingRequestsController : Controller
    {
        public IActionResult ShowInGoingRequests()
        {
            DappAccount account = RegulatorController._regulator;
            //return View("~/Views/InGoingRequests/InGoingRequests.cshtml", account);
            return View("InGoingRequests", account);


        }
    }
}