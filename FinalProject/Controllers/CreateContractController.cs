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
        private AssetsInContractContext _AssetInContractsContext; // AssetInContracts db table
        private AccountsContext _AccountsContext; // Accounts db table

        public CreateContractController(AssetsInContractContext context, AccountsContext context2)
        {
            _AssetInContractsContext = context;
            _AccountsContext = context2;
        }

        public async Task<IActionResult> CreateContractPage(string PublicKey)
        { //here we load the page that responsible for the contracts deployment (=creation), we read all the asset which owned by the account
            await DappAccountController.RefreshAccountData(PublicKey);
            PublicKey = PublicKey.ToLower();
            DappAccount account = DappAccountController.openWith[PublicKey];

            List<AssetInContract> openContractsToCheck = new List<AssetInContract>(); //we will check if there are asset the included in the table, if there are, we will delete the assets from OwnAssetsList in the Create contract View 
            List<int> AssetsNumsIncludedInDeals = new List<int>();
            List<Asset> AssetsToDelete = new List<Asset>();
            openContractsToCheck = await _AssetInContractsContext.AssetsInContract.FromSqlRaw("select * from AssetsInContract where SellerPublicKey = {0} and (Status ='Ongoing' or Status ='Pending' )", account.publicKey).ToListAsync();
            
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

            return View(account); //Here we will return own assets in the account that not included in open contracts 

        }


        public async Task<double> CheckBuyerPublicKeyLegality(string BuyerPublicKey, string YourPublicKey)
        { //check legality of the public key form
            YourPublicKey = YourPublicKey.ToLower();
            DappAccount account = DappAccountController.openWith[YourPublicKey];
            double buyerBalance = await DappAccountController.get_ETH_BalanceOfAnyAccount(YourPublicKey, BuyerPublicKey); 
            return buyerBalance;
        }

        [HttpPost]
        public async Task<string> DeployContract (ContractOffer offer )
        {   //deploy (=create) the contract, save it in the blockahin and in the DB table - AssetInContracts 
            double beforeBalanceETH = await DappAccountController.get_ETH_Balance(offer.SellerPublicKey, offer.SellerPublicKey);
            double beforeBalanceILS = await DappAccountController.get_ILS_Balance(offer.SellerPublicKey, offer.SellerPublicKey);
            double exchangeRate = DappAccountController.getExchangeRate_ETH_To_ILS();
            double afterBalanceETH;
            double afterBalanceILS;
            double feeETH;
            double feeILS;
            try
            {
                InsertAssetInContractToDB(offer, "Busy");
                var account = DappAccountController.openWith[offer.SellerPublicKey.ToLower()];
                var ContractAddress =await SmartContractService.Deploy(account, offer);
                afterBalanceETH = await DappAccountController.get_ETH_Balance(offer.SellerPublicKey, offer.SellerPublicKey);
                afterBalanceILS = await DappAccountController.get_ILS_Balance(offer.SellerPublicKey , offer.SellerPublicKey);
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
        { //update the AssetInContracts table after the contract deployment (=creation)
            AssetInContract newOffer = new AssetInContract();
            newOffer.AssetID = offer.AssetID;
            newOffer.ContractAddress = "" + contractAddress;
            newOffer.SellerPublicKey = offer.SellerPublicKey;
            newOffer.BuyerPublicKey = offer.BuyerPublicKey;
            newOffer.Status = "Ongoing";
            newOffer.DeniedBy = "None";
            newOffer.Reason = "None";
            newOffer.DealPrice = offer.PriceETH;
            _AssetInContractsContext.AssetsInContract.Add(newOffer);
            _AssetInContractsContext.SaveChanges();
            return true;
        }

        private void RemoveBusyAssetInContractFromDB(ContractOffer offer)
        { //when we create the contract, we set a new record in AssetInContracts as "Busy" to prevent double contract deployment (=cration)

            var report = (from d in _AssetInContractsContext.AssetsInContract
                          where d.AssetID == offer.AssetID && d.ContractAddress=="Busy"
                          select d).Single();

            _AssetInContractsContext.AssetsInContract.Remove(report);
            _AssetInContractsContext.SaveChanges();
            
        }


        public async Task<int> GetAddressIDAsync(string PublicKey)
        { //return Israeli ID number from attached blockchain address
            List<AccountID> result = new List<AccountID>();
            result = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts where PublicKey = {0} ", PublicKey).ToListAsync(); 
            if(result.Count==0)
                return 0;

            return result[0].ID;
        }

    }

    internal class DeploymentRecipt //response class after contract deployment
    {
        public string ContractAddress { get; set; }

        public double feeETH { get; set; }

        public double feeILS { get; set; }

    }

    
}