using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinalProject.Controllers
{
    public class ContractsStatusController : Controller
    {
        private AssetsInContractContext _AssetInContractsContext; // AssetInContracts db table
        private AccountsContext _AccountsContext; // Accounts db table
        private AssetContext _AssetsContext; // Assets db table

        public ContractsStatusController(AssetsInContractContext context, AccountsContext context2, AssetContext context3)
        {
            _AssetInContractsContext = context;
            _AccountsContext = context2;
            _AssetsContext = context3;
        }

        public async Task<IActionResult> ContractsStatusPage(string PublicKey) //now 
        {  //load the page of the open/closed contracts, we read all the related open contracts and sort them
            await DappAccountController.RefreshAccountData(PublicKey);
            DappAccount account = DappAccountController.openWith[PublicKey.ToLower()];
            List<AssetInContract> deployedContractsFromDB = new List<AssetInContract>();
            deployedContractsFromDB = await _AssetInContractsContext.AssetsInContract.FromSqlRaw("select * from AssetsInContract where ( SellerPublicKey = {0} or BuyerPublicKey = {0} )", account.publicKey).ToListAsync();
            List <ContractOffer> deployedContractsFromBlockchain = new List<ContractOffer>();

            foreach (AssetInContract assCon in deployedContractsFromDB)
            {
                ContractOffer offer = new ContractOffer();
                string contractAddress = assCon.ContractAddress;
                offer.ContractAddress = contractAddress;
                if (!assCon.Status.Equals("Denied"))  //For the non-destroyed contracts, "Denied" => Self Destruction
                {   
                    SmartContractService deployedContract = new SmartContractService(account, contractAddress);
                    Asset assetInContract = await deployedContract.getAssetDestails(); //read from blockchain
                    offer.AssetID = assetInContract.AssetID;
                    offer.Loaction = assetInContract.Loaction;
                    offer.Rooms = assetInContract.Rooms;
                    offer.AreaIn = assetInContract.AreaIn;
                    offer.ImageURL = assetInContract.ImageURL;
                    offer.PriceETH = assetInContract.Price;
                    offer.PriceILS = offer.PriceETH * account.exchangeRateETH_ILS;
                    offer.PriceILS = Math.Truncate(offer.PriceILS * 1000) / 1000;
                    offer.BuyerPublicKey = await deployedContract.getBuyerAddress();
                    offer.SellerPublicKey = await deployedContract.getOldAssetOwner();
                    offer.SellerSign = await deployedContract.getSellerSign();
                    offer.BuyerSign = await deployedContract.getBuyerSign();
                    offer.RegulatorSign = await deployedContract.getRegulatorSign();
                    offer.Tax = await deployedContract.getTax();
                    offer.BuyerID = await GetAddressID(offer.BuyerPublicKey);                    
                    offer.OwnerID = await GetAddressID(offer.SellerPublicKey);
                    if (assCon.Status.Equals("Approved"))
                    {       
                        offer.NewOwnerPublicKey = await deployedContract.getNewAssetOwner();
                        offer.NewOwnerID = await GetAddressID(offer.NewOwnerPublicKey);
                    }
                        
                    if (assCon.Status.Equals("Ongoing"))
                    {
                        ulong time = await deployedContract.getTimeLeftInSeconds();
                        int timeLeft = (int)time;
                        if (timeLeft > 21)
                            timeLeft = timeLeft - 20;
                        offer.TimeToBeOpen = timeLeft;
                    }

                }

                else 
                {
                    offer.SellerPublicKey = assCon.SellerPublicKey;
                    offer.BuyerPublicKey = assCon.BuyerPublicKey;
                    offer.PriceETH = assCon.DealPrice;
                    offer.PriceILS = offer.PriceETH * account.exchangeRateETH_ILS;
                    if (assCon.DeniedBy.Equals("Buyer"))
                    {
                        offer.BuyerSign = false;
                        offer.RegulatorSign = false;
                        offer.IsDeniedByBuyer = true;
                        offer.IsDeniedByRegulator = false;
                    }

                    else if (assCon.DeniedBy.Equals("Regulator"))
                    {
                        offer.BuyerSign = true;
                        offer.RegulatorSign = false;
                        offer.IsDeniedByBuyer = false;
                        offer.IsDeniedByRegulator = true;
                    }

                    offer.SellerSign = true;
                    offer.DenyReason = assCon.Reason;
                    offer.AssetID = assCon.AssetID;
                    List<Asset> AssetsInDB = new List<Asset>();
                    AssetsInDB = await _AssetsContext.Assets.FromSqlRaw("select * from Assets where AssetID = {0}", offer.AssetID).ToListAsync();
                    offer.Loaction = AssetsInDB[0].Loaction;
                    offer.OwnerID = await GetAddressID(offer.SellerPublicKey);
                    offer.BuyerID = await GetAddressID(offer.BuyerPublicKey);
                    offer.AreaIn = AssetsInDB[0].AreaIn;
                    offer.Rooms = AssetsInDB[0].Rooms;
                    offer.ImageURL = AssetsInDB[0].ImageURL;
                    offer.DenyReason = assCon.Reason;

                }

                offer.EtherscanURL = "https://ropsten.etherscan.io/address/"+assCon.ContractAddress;
                deployedContractsFromBlockchain.Add(offer);
            }


            account.DeployedContractList = deployedContractsFromBlockchain;
            return View(account);
        }

        public async Task<int> GetAddressID(string PublicKey)
        {   //return Israeli ID number from attached blockchain address
            List<AccountID> result = new List<AccountID>();
            result = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts where PublicKey = {0} ", PublicKey).ToListAsync();
            if (result.Count == 0)
                return 0;

            return result[0].ID;
        }

        public async Task<int> GetTimeLeft(string ContractAddress, string PublicKey)
        {
            //get the time left in seconds for the contract to be signed buy the buyer  
            DappAccount account = DappAccountController.openWith[PublicKey.ToLower()];
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            ulong time = await deployedContract.getTimeLeftInSeconds();
            int timeLeft = (int)time;
            if (timeLeft > 21)
                timeLeft = timeLeft - 15;
            return timeLeft;
        }

        public async Task<string> CancelDealAsSeller(string ContractAddress, string PublicKey)
        {   //cancel the contract if the time is over (the buyer didn`t sign in time)
            DappAccount account = DappAccountController.openWith[PublicKey];
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            double beforeBalanceETH = await DappAccountController.get_ETH_Balance(PublicKey, PublicKey);
            double beforeBalanceILS = await DappAccountController.get_ILS_Balance(PublicKey, PublicKey);
            double exchangeRate = DappAccountController.getExchangeRate_ETH_To_ILS();
            double afterBalanceETH;
            double afterBalanceILS;
            double feeETH;
            double feeILS;
            bool isCanceled = false;
            isCanceled = await deployedContract.abort();
            if(isCanceled == true)
            {
                afterBalanceETH = await DappAccountController.get_ETH_Balance(PublicKey, PublicKey);
                afterBalanceILS = await DappAccountController.get_ILS_Balance(PublicKey, PublicKey);
                feeETH = beforeBalanceETH - afterBalanceETH;
                feeILS = beforeBalanceILS - afterBalanceILS;
                ConfirmationRecipt recipt = new ConfirmationRecipt();
                recipt.ContractAddress = ContractAddress;
                recipt.feeETH = feeETH;
                feeILS = Math.Truncate(feeILS * 100) / 100; //make the double number to be with 3 digits after dot               
                recipt.feeILS = feeILS;
                var ReciptJson = Newtonsoft.Json.JsonConvert.SerializeObject(recipt);
                deleteCanceledOfferBySeller(ContractAddress);
                return ReciptJson;
            }

            return "Fail";
        }

        public void deleteCanceledOfferBySeller(string ContractAddress)
        { //delete the record from db of the unsigned contracts
            var report = (from d in _AssetInContractsContext.AssetsInContract
                          where d.ContractAddress == ContractAddress
                          select d).Single();

            _AssetInContractsContext.AssetsInContract.Remove(report);
            _AssetInContractsContext.SaveChanges();
        }

        public async Task<string> CancelDealAsBuyer(string ContractAddress, string Notes, string PublicKey)
        { //deny the contract /cancel the contract sent by seller 
            DappAccount account = DappAccountController.openWith[PublicKey];
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            double beforeBalanceETH = await DappAccountController.get_ETH_Balance(PublicKey, PublicKey);
            double beforeBalanceILS = await DappAccountController.get_ILS_Balance(PublicKey, PublicKey);
            double exchangeRate = DappAccountController.getExchangeRate_ETH_To_ILS();
            double afterBalanceETH;
            double afterBalanceILS;
            double feeETH;
            double feeILS;
            bool isDenied = false;
            isDenied = await deployedContract.denyDeal();
            if (isDenied == true)
            {
                afterBalanceETH = await DappAccountController.get_ETH_Balance(PublicKey, PublicKey);
                afterBalanceILS = await DappAccountController.get_ILS_Balance(PublicKey, PublicKey);
                feeETH = beforeBalanceETH - afterBalanceETH;
                feeILS = beforeBalanceILS - afterBalanceILS;
                ConfirmationRecipt recipt = new ConfirmationRecipt();
                recipt.ContractAddress = ContractAddress;
                recipt.feeETH = feeETH;
                feeILS = Math.Truncate(feeILS * 100) / 100; //make the double number to be with 3 digits after dot               
                recipt.feeILS = feeILS;
                var ReciptJson = Newtonsoft.Json.JsonConvert.SerializeObject(recipt);
                updateOfferToDenied(ContractAddress, Notes);
                return ReciptJson;
            }

            return "Fail";
        }

        public void updateOfferToDenied(string ContractAddress, string Notes)
        { //update db open contract record from "Ongoing to denied"
            var report = (from d in _AssetInContractsContext.AssetsInContract
                          where d.ContractAddress == ContractAddress
                          select d).Single();
            report.Status = "Denied";
            report.DeniedBy = "Buyer";
            if(Notes==null)
            {
                report.Reason = "None";
            }
            else
            {
                report.Reason = Notes;
            }

            _AssetInContractsContext.AssetsInContract.Update(report);
            _AssetInContractsContext.SaveChanges();
        }


        public async Task<string> ApproveContract(string ContractAddress, string PublicKey)
        {
            //approve the contract seny by the seller (send money and sign the contract)

            DappAccount account = DappAccountController.openWith[PublicKey.ToLower()];
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            double beforeBalanceETH = await DappAccountController.get_ETH_Balance(PublicKey, PublicKey);
            double beforeBalanceILS = await DappAccountController.get_ILS_Balance(PublicKey, PublicKey);
            double exchangeRate = DappAccountController.getExchangeRate_ETH_To_ILS();
            double afterBalanceETH;
            double afterBalanceILS;
            double feeETH;
            double feeILS;
            Asset dealAsset = await deployedContract.getAssetDestails();
            double ethToPay = dealAsset.Price;
            bool isPaid = false;
            bool isSigned = false;
            isPaid = await deployedContract.sendEtherToContract(ethToPay);
            
            if (isPaid == true)
            {
                isSigned = await deployedContract.setBuyerSign();
            }
            else
            {
                throw new Exception("Out of money");
            }

            afterBalanceETH = await DappAccountController.get_ETH_Balance(PublicKey, PublicKey);
            afterBalanceILS = await DappAccountController.get_ILS_Balance(PublicKey, PublicKey);
            feeETH = beforeBalanceETH - (afterBalanceETH+ ethToPay);
             
            feeILS = exchangeRate* feeETH;
            ConfirmationRecipt recipt = new ConfirmationRecipt();
            recipt.ContractAddress = ContractAddress;
            recipt.feeETH = feeETH;
            feeILS = Math.Truncate(feeILS * 100) / 100; //make the double number to be with 3 digits after dot               
            recipt.feeILS = feeILS;
            var ReciptJson = Newtonsoft.Json.JsonConvert.SerializeObject(recipt);
            updateOfferToPending(ContractAddress);
            return ReciptJson;
        }


        public void updateOfferToPending(string ContractAddress)
        { //change state of the open contract from "ongoing" to pending, so the regulator be able to read it
            var report = (from d in _AssetInContractsContext.AssetsInContract
                          where d.ContractAddress == ContractAddress
                          select d).Single();
            report.Status = "Pending";
            _AssetInContractsContext.AssetsInContract.Update(report);
            _AssetInContractsContext.SaveChanges();
        }

       
           
    }

    internal class ConfirmationRecipt
    { //response class after deny/approve contract by the buyer or cancel expierd contract by the seller
        public string ContractAddress { get; set; }

        public double feeETH { get; set; }

        public double feeILS { get; set; }

    }
}