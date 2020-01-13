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
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FinalProject.Controllers
{
    public class DappAccountController : Controller
    {
        private AssetContext _AssetsContext; // Assets db table
        public static Dictionary<string, DappAccount> openWith = new Dictionary<string, DappAccount>(); //all the accounts in our web
        private const string REGULATOR_PUBLIC_KEY = "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80"; //this is the publick key of the regulator (=govrenment) 

        public DappAccountController(AssetContext context)
        {
            _AssetsContext = context;
        }

        [HttpGet]
        [HttpPost]
        public IActionResult AccountMainPage(DappAccount account)
        {   //here the login succeeded , we initialized the basic details of the blockchain account
            account.BlockchainAcount = new Nethereum.Web3.Accounts.Account(account.privateKey);
            account.IsValidated = true;         
                string myPublicKey = account.publicKey.ToLower();
                if (openWith.ContainsKey(myPublicKey))
                    openWith.Remove(myPublicKey);
                openWith.Add(myPublicKey, account);
            
            ConnectToBlockchain(account.publicKey);

            if (String.Compare(account.publicKey, REGULATOR_PUBLIC_KEY) == 0)
            {// account is regulator 
                RegulatorController._regulator = account;
                return RedirectToAction("RegulatorMainPage", "Regulator");        
            }
            

            return RedirectToAction("ShowOwnAssets", "DappAccount", new { PublicKey = account.publicKey } );
        }

        [HttpPost]
        public bool CheckAccount(String PublicKey, string PrivateKey)
        { //initilize login validiton status

            bool IsValidated = CheckLogin(PublicKey, PrivateKey); //here we just check the login, the model is not initilized in the key properties

            if (IsValidated)
            {
                return true;
            }
            return false;
        }

        public bool CheckLogin(string _publicKey, string _privateKey)
        { //check login details
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



        public static async Task RefreshAccountData(string publicKey)
        { //update the balance of the account (read it from the blockchain)
            string publicKeyToCheck = publicKey;
            publicKey = publicKey.ToLower();
            DappAccount account = DappAccountController.openWith[publicKey];
            account.EthBalance = await get_ETH_Balance(publicKey, publicKeyToCheck);
            account.IlsBalance = await get_ILS_Balance(publicKey, publicKeyToCheck);
            account.exchangeRateETH_ILS = getExchangeRate_ETH_To_ILS();
        }




        public bool ConnectToBlockchain(String PublicKey)
        { //connect to blockchain API using Nethereum
            PublicKey = PublicKey.ToLower();
            DappAccount account = openWith[PublicKey];
            if (account.IsValidated == true)
            {
                try
                {
                    var infuraURL = "https://ropsten.infura.io/v3/4dc41c6f591d4d61a3a2e32a219c6635";
                    account.Blockchain = new Web3(account.BlockchainAcount, infuraURL);

                }
                catch (Exception e)
                {
                    account.IsConnectedToBlockChain = false;
                    return false;
                }

            }
            account.IsConnectedToBlockChain = true;
            return account.IsConnectedToBlockChain;
        }

        [HttpPost]
        public async Task<double> RecheckBalanceAfterBlockchainOperation(string PublicKey)
        { //read the current account balance from the blockchain, used after blockchain operation to update balance
            string PublicKeyToCheck = PublicKey;
            PublicKey = PublicKey.ToLower();
            double balanceETH = await get_ETH_Balance(PublicKey, PublicKeyToCheck);
            return balanceETH;
        }


        public static async Task<double> get_ETH_BalanceOfAnyAccount(String PublicKey, String PublicKeyToCheck)
        { //read the balance of any account (ETH), from the blockchain, used after blockchain operation
            PublicKey = PublicKey.ToLower();
            DappAccount account = openWith[PublicKey];
                if (PublicKey == null || account.IsValidated == false)
                    return -1;
            if (account.IsConnectedToBlockChain == false)
                return -1;
            try
            {
                var balance = await account.Blockchain.Eth.GetBalance.SendRequestAsync(PublicKeyToCheck);
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

        public static async Task<double> get_ETH_Balance(string PublicKey, string PublicKeyToCheck)
        { //read the current account balance from the blockchain (ETH)
            double balance = await get_ETH_BalanceOfAnyAccount(PublicKey, PublicKeyToCheck);
            return balance;
        }

        public static double getExchangeRate_ETH_To_ILS()
        { //read the real and live exchange rate from ETH to Shekel 
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

        public static async Task<double> get_ILS_Balance(string PublicKey, string PublicKeyToCheck)
        { //read the current account balance from the blockchain (ILS)
            double tempBalance = await get_ILS_BalanceOfAnyAccount(PublicKey, PublicKeyToCheck);
            return tempBalance;

        }

        public static async Task<double> get_ILS_BalanceOfAnyAccount(string PublicKey, string PublicKeyToCheck)
        { //read the balance of any account (ILS), from the blockchain, used after blockchain operation
            double exchangeRate = getExchangeRate_ETH_To_ILS();
            double tempBalance = await get_ETH_BalanceOfAnyAccount(PublicKey, PublicKeyToCheck);
            tempBalance = exchangeRate * tempBalance;
            tempBalance = Math.Truncate(tempBalance * 1000) / 1000; //make the double number to be with 3 digits after dot
            return tempBalance;
        }

        public async Task<IActionResult> ShowOwnAssets(string PublicKey)
        { //get the assets owned by the accounts 
            PublicKey = PublicKey.ToLower();
            DappAccount account = openWith[PublicKey];
            account.OwnAssetsList = null;
            account.OwnAssetsList = await _AssetsContext.Assets.FromSqlRaw("select * from Assets where OwnerPublicKey = {0}", account.publicKey).ToListAsync();

            await RefreshAccountData(PublicKey);

            return View("AccountMainPage", account);
        }


    }

}

