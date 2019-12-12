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
        public static DappAccount myAccount;

        public DappAccountController(AssetContext context)
        {
            _context = context;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> AccountMainPage(DappAccount account) //here the login succeeded , e initialized the key
        { 
            bool IsValidated = account.CheckLogin(account.publicKey, account.privateKey); //so now call againg to the login method in order to load more properties 
            if (!IsValidated)
                return RedirectToAction("Login", "Home");

            account.ConnectToBlockchain();
            myAccount = account; //and save this account in static account so the other controllers be able to read it
            myAccount.OwnAssetsList = await _context.Assets.FromSqlRaw("select * from Assets where OwnerPublicKey = {0}", account.publicKey).ToListAsync();
            return View(account);

        }


        [HttpPost]
        public bool CheckAccount(String PublicKey, string PrivateKey)
        {
            DappAccount account = new DappAccount();
            bool IsValidated = account.CheckLogin(PublicKey, PrivateKey); //here we just check the login, the model is not initilized in the key properties

            if (IsValidated)
            {
                return true;
            }
            return false;
        }


    }


}

/* account.OwnAssetsList = await _context.assets.FromSqlRaw("SELECT Assets2.AssetID, Accounts.ID as OwnerID, Assets2.OwnerPublicKey,"
                                                         + " Assets2.Loaction, Assets2.AreaIn, Assets2.Rooms, Assets2.ImageURL, Assets2.Price"
                                                         + " FROM Assets2" 
                                                         + " JOIN Accounts ON Assets2.OwnerPublicKey = Accounts.PublicKey"
                                                         + " WHERE Assets2.OwnerPublicKey = {0}",account.publicKey).ToListAsync();   */

//account.RecievedContractsList = await _context.recievedOpenContracts.FromSqlRaw("select * from OpenContracts where BuyerPublicKey = {0}", account.publicKey).ToListAsync();
//account.ClosedContractsList = await _context.ClosedContracts.FromSqlRaw("select * from ClosedContracts").ToListAsync();
