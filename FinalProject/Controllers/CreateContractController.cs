using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FinalProject.Controllers
{
    public class CreateContractController : Controller
    {
        private AssetsInContractContext _context;

        public CreateContractController(AssetsInContractContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateContract()
        {
            await DappAccountController.RefreshAccountData();
            DappAccount account =  DappAccountController.myAccount;
            List<AssetInContract> openContractsToCheck = new List<AssetInContract>(); //we will check if there are asset the included in the table, if there are, we will delete the assets from OwnAssetsList in the Create contract View 
            List<int> AssetsNumsIncludedInDeals = new List<int>();
            List<Asset> AssetsToDelete = new List<Asset>();
            openContractsToCheck = await _context.AssetsInContract.FromSqlRaw("select * from AssetsInContract where SellerPublicKey = {0} and (Status ='Ongoing' or Status ='Pending' )", account.publicKey).ToListAsync();
            
            foreach (AssetInContract cnrt in openContractsToCheck) //Here we take all the assets ID that included in open contracts
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


        public async Task<double> CheckBuyerPublicKeyLegality(string BuyerPublicKey)
        {
            DappAccount account = DappAccountController.myAccount;
            double buyerBalance = await DappAccountController.get_ETH_BalanceOfAnyAccount(BuyerPublicKey);
            return buyerBalance;
        }

        [HttpPost]
        public async Task<string> DeployContractAsync (ContractOffer offer )
        {
            double beforeBalanceETH = await DappAccountController.get_ETH_Balance();
            double beforeBalanceILS = await DappAccountController.get_ILS_Balance();
            double exchangeRate = DappAccountController.getExchangeRate_ETH_To_ILS();
            double afterBalanceETH;
            double afterBalanceILS;
            double feeETH;
            double feeILS;
            try
            {
                InsertAssetInContractToDB(offer, "Busy");  
                var account = DappAccountController.myAccount;
                var ContractAddress =await SmartContractService.Deploy(account, offer);
                afterBalanceETH = await DappAccountController.get_ETH_Balance();
                afterBalanceILS = await DappAccountController.get_ILS_Balance();
                feeETH = beforeBalanceETH - afterBalanceETH;
                feeILS = beforeBalanceILS - afterBalanceILS;
                
                InsertAssetInContractToDB(offer, ContractAddress);
                RemoveBusyAssetInContractFromDB(offer);

                DeploymentRecipt recipt = new DeploymentRecipt();
                recipt.ContractAddress = ContractAddress;
                recipt.feeETH = feeETH;
                feeILS = Math.Truncate(feeILS * 100) / 100; //make the double number to be with 3 digits after dot               
                recipt.feeILS = feeILS;
                var ReciptJson = Newtonsoft.Json.JsonConvert.SerializeObject(recipt);

                return ReciptJson;
            } 

            catch(Exception e) 
            {
                RemoveBusyAssetInContractFromDB(offer);
                return "Error";
            }
             
        }


        private bool InsertAssetInContractToDB(ContractOffer offer, string contractAddress)
        {
            AssetInContract newOffer = new AssetInContract();
            newOffer.AssetID = offer.AssetID;
            newOffer.ContractAddress = "" + contractAddress;
            newOffer.SellerPublicKey = offer.SellerPublicKey;
            newOffer.BuyerPublicKey = offer.BuyerPublicKey;
            newOffer.Status = "Ongoing";
            newOffer.DeniedBy = "None";
            newOffer.Reason = "None";
            _context.AssetsInContract.Add(newOffer);
            _context.SaveChanges();
            return true;
        }

        private void RemoveBusyAssetInContractFromDB(ContractOffer offer)
        {

            var report = (from d in _context.AssetsInContract
                          where d.AssetID == offer.AssetID && d.ContractAddress=="Busy"
                          select d).Single();

            _context.AssetsInContract.Remove(report);
            _context.SaveChanges();
            
        }

        

    }

    internal class DeploymentRecipt
    {
        public string ContractAddress { get; set; }

        public double feeETH { get; set; }

        public double feeILS { get; set; }

    }
}