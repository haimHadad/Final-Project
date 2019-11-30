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
        public async Task<IActionResult> AccountMainPage(string PublicKey, string PrivateKey)
        {
            
            DappAccount account = new DappAccount(PublicKey, PrivateKey);
            account.OwnAssetsList = await _context.assets.FromSqlRaw("select * from Assets2 where OwnerPublicKey = {0}", account.publicKey).ToListAsync();
            return View(account);

        }


        [HttpPost]
        public async Task<bool> CheckAccount(String PublicKey, string PrivateKey)
        {
            DappAccount account = new DappAccount(PublicKey, PrivateKey);
            if (account.IsValidated)
            {
                return true;
            }

           

            return false;
        }
    }
}