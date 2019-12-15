using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using FinalProject.Data;
using Nethereum.Web3;
using System.Net;
using Microsoft.EntityFrameworkCore.Internal;

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
            account.BlockchainAcount = new Nethereum.Web3.Accounts.Account(account.privateKey);
            account.IsValidated = true;
            myAccount = account; //and save this account in static account so the other controllers be able to read it
            ConnectToBlockchain();   
            myAccount.OwnAssetsList = await _context.Assets.FromSqlRaw("select * from Assets where OwnerPublicKey = {0}", account.publicKey).ToListAsync();
            await RefreshAccountData();
            return View(account);

        }

        [HttpPost]
        public bool CheckAccount(String PublicKey, string PrivateKey)
        {
            
            bool IsValidated = CheckLogin(PublicKey, PrivateKey); //here we just check the login, the model is not initilized in the key properties

            if (IsValidated)
            {
                return true;
            }
            return false;
        }

        public bool CheckLogin(string _publicKey, string _privateKey)
        {
            DappAccount tempAccount = new DappAccount();
            try
            {
                tempAccount.BlockchainAcount = new Nethereum.Web3.Accounts.Account(_privateKey);
                String declaredAddress = _publicKey;                         //address from son = textbox of our web dApp = Account class that Haim created
                String realAddress = tempAccount.BlockchainAcount.Address;               //BlockchainAcount.Address is the real address of the private key  
                if (!declaredAddress.Equals(realAddress))                     //If the addresses are matched => Login details are correct = > dApp will show wallt(account) content in the next view 
                {               
                    return false;
                }
            }
            catch (Exception e)
            {

                return false;
            }
            
            return true;
        }



        public static async Task RefreshAccountData()
        {
            myAccount.EthBalance = await get_ETH_Balance();
            myAccount.IlsBalance = await get_ILS_Balance();
            myAccount.exchangeRateETH_ILS = getExchangeRate_ETH_To_ILS();
        }


        

        public bool ConnectToBlockchain()
        {
            if (myAccount.IsValidated == true)
            {
                try
                {
                    var infuraURL = "https://ropsten.infura.io/v3/4dc41c6f591d4d61a3a2e32a219c6635";      
                    myAccount.Blockchain = new Web3(myAccount.BlockchainAcount, infuraURL);

                }
                catch (Exception e)
                {
                    myAccount.IsConnectedToBlockChain = false;
                    return false;
                }

            }
            myAccount.IsConnectedToBlockChain = true;
            return myAccount.IsConnectedToBlockChain;
        }

        [HttpPost]
        public async Task<double> RecheckBalanceAfterBlockchainOperation()
        {

            double balanceETH = await get_ETH_Balance();
            return balanceETH;
        }


        public static async Task<double> get_ETH_BalanceOfAnyAccount(String AccountAddress)
        {
            if (AccountAddress == null || myAccount.IsValidated == false)
                return -1;
            if (myAccount.IsConnectedToBlockChain == false)
                return -1;
            try
            {
                var balance = await myAccount.Blockchain.Eth.GetBalance.SendRequestAsync(AccountAddress);
                var etherAmount = Web3.Convert.FromWei(balance.Value);
                double tempBalance = (double)etherAmount;
                tempBalance = Math.Truncate(tempBalance * 1000) / 1000; //make the double number to be with 3 digits after dot
                return tempBalance;

            }
            catch (Exception e)
            {
                return -1;
            }

        }

        public static async Task<double> get_ETH_Balance()
        {
            double balance = await get_ETH_BalanceOfAnyAccount(myAccount.publicKey);
            return balance;
        }

        public static double getExchangeRate_ETH_To_ILS()
        {
            double exchangeRate;
            string[] words;
            int indexOfExchangeRate;

            using (var client = new WebClient())
            {
                try
                {
                    var htmlPage = client.DownloadString("https://coinyep.com/he/rss/ETH-ILS.xml");
                    words = htmlPage.Split(' ');
                    indexOfExchangeRate = words.IndexOf("ILS");
                    exchangeRate = Convert.ToDouble(words[indexOfExchangeRate - 1]);

                }
                catch (Exception e)
                {
                    exchangeRate = -1;
                }
            }
            return exchangeRate;
        }

        public static async Task<double> get_ILS_Balance()
        {
            double tempBalance = await get_ILS_BalanceOfAnyAccount(myAccount.publicKey);
            return tempBalance;

        }

        public static async Task<double> get_ILS_BalanceOfAnyAccount(String AccountAddress)
        {
            double exchangeRate = getExchangeRate_ETH_To_ILS();
            double tempBalance = await get_ETH_BalanceOfAnyAccount(AccountAddress);
            tempBalance = exchangeRate * tempBalance;
            tempBalance = Math.Truncate(tempBalance * 1000) / 1000; //make the double number to be with 3 digits after dot
            return tempBalance;
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
