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
    public class AccountsManagmentController : Controller
    {
        private AccountsContext _AccountsContext;
        private static RegulatorAccount regulatorAcc;


        public AccountsManagmentController(AccountsContext context)
        {
            _AccountsContext = context;
        }



        public async Task<IActionResult> ShowAccountsPage()
        {        
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
        {
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
            return "Success";
        }


        public async Task<double> CheckPublicKeyValidity(string PublicKey)
        {
            var YourPublicKey = regulatorAcc.publicKey;
            var PublicKeyToCheck = PublicKey;
            double Balance = await DappAccountController.get_ETH_BalanceOfAnyAccount(YourPublicKey, PublicKeyToCheck);
            return Balance;
        }

    }
}