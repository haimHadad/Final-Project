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
    public class ContractsStatusController : Controller
    {
        private AssetsInContractContext _context;

        public ContractsStatusController(AssetsInContractContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ContractsStatusPage() //now we are going to read all the open contracts
        {
            await DappAccountController.RefreshAccountData();
            DappAccount account = DappAccountController.myAccount;
            List<AssetInContract> openContractsFromDB = new List<AssetInContract>();
            openContractsFromDB = await _context.AssetsInContract.FromSqlRaw("select * from AssetsInContract where ( SellerPublicKey = {0} or BuyerPublicKey = {0} )", account.publicKey).ToListAsync();
            return View(DappAccountController.myAccount);
        }
    }
}