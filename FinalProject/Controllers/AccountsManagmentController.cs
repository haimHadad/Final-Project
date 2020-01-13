using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace FinalProject.Controllers
{
    public class AccountsManagmentController : Controller
    {
        private AccountsContext _AccountsContext; // Accounts db table
        private static RegulatorAccount regulatorAcc; //regulator account


        public AccountsManagmentController(AccountsContext context)
        {
            _AccountsContext = context;
        }



        public async Task<IActionResult> ShowAccountsPage()
        {   //show the public-key -> ID table in the page, read all web accounts     
            DappAccount account = RegulatorController._regulator;
            await DappAccountController.RefreshAccountData(account.publicKey);
            regulatorAcc = new RegulatorAccount();
            regulatorAcc.publicKey = account.publicKey;
            regulatorAcc.IsValidated = account.IsValidated;
            regulatorAcc.EthBalance = account.EthBalance;
            regulatorAcc.IlsBalance = account.IlsBalance;
            regulatorAcc.exchangeRateETH_ILS = account.exchangeRateETH_ILS;
            regulatorAcc.BlockchainAcount = account.BlockchainAcount;
            regulatorAcc.Blockchain = account.Blockchain;
            
            regulatorAcc.AllAccounts = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts").ToListAsync();

            return View("AccountsManagment", regulatorAcc);
        }

        public async Task<string> AddNewAccount(int ID, string PublicKey)
        { //add new account to the db so our web will be able to recognize it in new contract deployments
            List<AccountID> listToCheck;

            listToCheck = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts where ID = {0}", ID).ToListAsync();

            if(listToCheck.Count > 0)
            {
                return "ID already been registered";
            }

            listToCheck = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts where PublicKey = {0}", PublicKey).ToListAsync();

            if (listToCheck.Count > 0)
            {
                return "Public-Key already been registered";
            }

            AccountID newAccountID = new AccountID();
            newAccountID.ID = ID;
            newAccountID.PublicKey = PublicKey;
            _AccountsContext.Accounts.Add(newAccountID);
            _AccountsContext.SaveChanges();
            regulatorAcc.AllAccounts.Add(newAccountID);
            return "Success";
        }


        public async Task<double> CheckPublicKeyValidity(string PublicKey)
        { //check the legality of the public-key in order to save it in the db table
            var YourPublicKey = regulatorAcc.publicKey;
            var PublicKeyToCheck = PublicKey;
            double Balance = await DappAccountController.get_ETH_BalanceOfAnyAccount(YourPublicKey, PublicKeyToCheck);
            return Balance;
        }


        public void DownloadExcelAccounts()
        { //download the account html table into an excel file
            var collection = regulatorAcc.AllAccounts;
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "ID";
            Sheet.Cells["B1"].Value = "Public-Key";
            
            int row = 2;
            foreach (var account in collection)
            {
                string idNumber = ""+account.ID;
                if(idNumber.Length==8)
                {
                    idNumber = "0" + idNumber;
                }
                Sheet.Cells[string.Format("A{0}", row)].Value = idNumber;
                Sheet.Cells[string.Format("B{0}", row)].Value = account.PublicKey; 
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Add("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.Body.WriteAsync(Ep.GetAsByteArray());
        }

    }
}