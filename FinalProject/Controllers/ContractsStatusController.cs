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
        private AssetsInContractContext _context;
        private AccountsContext _context2;
        private AssetContext _context3;

        public ContractsStatusController(AssetsInContractContext context, AccountsContext context2, AssetContext context3)
        {
            _context = context;
            _context2 = context2;
            _context3 = context3;
        }

        public async Task<IActionResult> ContractsStatusPage() //now we are going to read all the open contracts
        {
            await DappAccountController.RefreshAccountData();
            DappAccount account = DappAccountController.myAccount;
            List<AssetInContract> deployedContractsFromDB = new List<AssetInContract>();
            deployedContractsFromDB = await _context.AssetsInContract.FromSqlRaw("select * from AssetsInContract where ( SellerPublicKey = {0} or BuyerPublicKey = {0} )", account.publicKey).ToListAsync();
            List <ContractOffer> deployedContractsFromBlockchain = new List<ContractOffer>();

            foreach (AssetInContract assCon in deployedContractsFromDB)
            {
                ContractOffer offer = new ContractOffer();
                string contractAddress = assCon.ContractAddress;
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
                    offer.OwnertID = await GetAddressID(offer.SellerPublicKey);
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

                else //For the destroyed contracts, we need to get the data from db , except the deal price which is lost until we figure out what to do
                {
                    offer.SellerPublicKey = assCon.SellerPublicKey;
                    offer.BuyerPublicKey = assCon.BuyerPublicKey;
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
                    AssetsInDB = await _context3.Assets.FromSqlRaw("select * from Assets where AssetID = {0}", offer.AssetID).ToListAsync();
                    offer.Loaction = AssetsInDB[0].Loaction;
                    offer.OwnertID = await GetAddressID(offer.SellerPublicKey);
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
            return View(DappAccountController.myAccount);
        }

        public async Task<int> GetAddressID(string PublicKey) //give me blockchain address, I will give you Israeli ID number
        {
            List<AccountID> result = new List<AccountID>();
            result = await _context2.Accounts.FromSqlRaw("select * from Accounts where PublicKey = {0} ", PublicKey).ToListAsync();
            if (result.Count == 0)
                return 0;

            return result[0].ID;
        }
    }
}