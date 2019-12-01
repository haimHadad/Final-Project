using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using FinalProject.Data;

namespace FinalProject.Controllers
{
    public class DappAccountController : Controller
    {
        private AssetContext _context;

        public DappAccountController(AssetContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AccountMainPage(DappAccount account)
        {
            account.assetsList = await _context.Assets.FromSqlRaw("select * from assets where accountAddress = {0}", account.publicKey).ToListAsync();
            return View(account);
        }
    }
}