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
        private AccountContext _context;

        public DappAccountController(AccountContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AccountMainPage(string PublicKey, string PrivateKey)
        {
            
            DappAccount account = new DappAccount(PublicKey, PrivateKey);
            //account.OwnAssetsList = await _context.assets.FromSqlRaw("select * from Assets2 where OwnerPublicKey = {0}", account.publicKey).ToListAsync();         
            /*account.OwnAssetsList = await _context.assets.FromSqlRaw("SELECT Assets.AssetID, Accounts.OwnerID, Assets.OwnerPublicKey, Assets.Loaction,"
                                                                    +"Assets.AreaIn, Assets.Rooms Assets.ImageURL, Assets.Price" 
                                                                    +"FROM Assets2" 
                                                                    +"INNER JOIN Accounts ON Assets.AssetID = Accounts.ID"
                                                                    +"WHERE Assets.AssetID = {0}", account.publicKey).ToListAsync();*/


            account.OwnAssetsList = await _context.assets.FromSqlRaw("SELECT Assets2.AssetID, Accounts.ID as OwnerID, Assets2.OwnerPublicKey,"
                                                                    + " Assets2.Loaction, Assets2.AreaIn, Assets2.Rooms, Assets2.ImageURL, Assets2.Price"
                                                                    + " FROM Assets2" 
                                                                    + " JOIN Accounts ON Assets2.OwnerPublicKey = Accounts.PublicKey"
                                                                    + " WHERE Assets2.OwnerPublicKey = {0}",account.publicKey).ToListAsync();         


            //account.RecievedContractsList = await _context.recievedOpenContracts.FromSqlRaw("select * from OpenContracts where BuyerPublicKey = {0}", account.publicKey).ToListAsync();
            //account.ClosedContractsList = await _context.ClosedContracts.FromSqlRaw("select * from ClosedContracts").ToListAsync();

            return View(account);

        }


        [HttpPost]
        public bool CheckAccount(String PublicKey, string PrivateKey)
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