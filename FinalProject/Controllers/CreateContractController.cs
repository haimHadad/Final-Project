using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class CreateContractController : Controller
    {
        private OpenContractsContext _context;


        public CreateContractController(OpenContractsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateContract()
        {
            DappAccount account =  DappAccountController.myAccount;
            List<OpenContract> openContractsToCheck = new List<OpenContract>(); //we will check if there are asset the included in the table, if there are, we will delete the assets from OwnAssetsList in the Create contract View 
            List<int> AssetsNumsIncludedInDeals = new List<int>();
            List<Asset> AssetsToDelete = new List<Asset>();
            openContractsToCheck = await _context.OpenContracts.FromSqlRaw("select * from OpenContracts where SellerPublicKey = {0}", account.publicKey).ToListAsync();
            
            foreach (OpenContract cnrt in openContractsToCheck) //Here we take all the assets ID that included in open contracts
            {
                AssetsNumsIncludedInDeals.Add(cnrt.AssetID);
            }


            foreach (Asset ast in account.OwnAssetsList) //Here we check each one of our assets if it is included in a deal, if yes, we will take the instance of this asset
            {
                if(AssetsNumsIncludedInDeals.Contains(ast.AssetID))
                {
                    AssetsToDelete.Add(ast);
                }
            }

            foreach (Asset astDel in AssetsToDelete) //We return to our assets list, and delete all the assets that we collected before (assets that included in the deal)
            {             
                account.OwnAssetsList.Remove(astDel);  
            }

            return View(DappAccountController.myAccount); //Here we will return own assets in the account that not included in open contracts 
        }
    }
}