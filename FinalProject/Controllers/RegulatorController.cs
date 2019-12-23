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
        const string DB_TABLE_NAME = "assetsincontract";
        const string PENDING = "pending";
        private AssetsInContractContext _context;
        public static Regulator _regulator;
        public RegulatorController(AssetsInContractContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> RegulatorMainPage(Regulator regulator) //here the login succeeded , e initialized the key
        {
            _regulator = regulator;
            _regulator.assetsList = await _context.AssetInContract.FromSqlRaw("select * from " + DB_TABLE_NAME + " where status = {0}", PENDING).ToListAsync();

            return View(_regulator);
        }
    }
}