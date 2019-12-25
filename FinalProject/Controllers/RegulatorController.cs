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
    public class RegulatorController : Controller
    {
        const string DB_TABLE_NAME = "AssetsInContract";
        const string PENDING = "pending";
        private AssetsInContractContext _AssetsContext;
        public static DappAccount _regulator;
        public RegulatorController(AssetsInContractContext context)
        {
            _AssetsContext = context;
        }

        public async Task<IActionResult> RegulatorMainPage() //here the login succeeded , e initialized the key
        {

            await DappAccountController.RefreshAccountData(_regulator.publicKey);
            
            _regulator.ContractsList = await _AssetsContext.AssetsInContract.FromSqlRaw("select * from " + DB_TABLE_NAME + " where status = {0}", PENDING).ToListAsync();

            return View(_regulator);
        }
    }
}