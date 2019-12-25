using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using FinalProject.Data;
using Newtonsoft.Json;

namespace FinalProject.Controllers
{
    public class RegulatorController : Controller
    {

        private AssetsInContractContext _AssetInContractsContext;
        private AccountsContext _AccountsContext;
        private AssetContext _AssetsContext;
        public static DappAccount _regulator;
        public RegulatorController(AssetsInContractContext context, AccountsContext context2, AssetContext context3)
        {
            _AssetInContractsContext = context;
            _AccountsContext = context2;
            _AssetsContext = context3;
        }

        public async Task<IActionResult> RegulatorMainPage() //here the login succeeded , e initialized the key
        {

            await DappAccountController.RefreshAccountData(_regulator.publicKey);

            //_regulator.ContractsList = await _AssetsContext.AssetsInContract.FromSqlRaw("select * from " + DB_TABLE_NAME + " where status = {0}", PENDING).ToListAsync();

            return RedirectToAction("ShowHomePage", "Regulator");
        }


        public async Task<IActionResult> ShowHomePage()
        {
            DappAccount account = _regulator;
            List<AssetInContract> deployedContractsFromDB = new List<AssetInContract>();
            deployedContractsFromDB = await _AssetInContractsContext.AssetsInContract.FromSqlRaw("select * from AssetsInContract where Status= 'Approved' or (Status= 'Denied' and DeniedBy = 'Regulator') ").ToListAsync();
            List<ContractOffer> deployedContractsFromBlockchain = new List<ContractOffer>();

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
                    offer.NewOwnerPublicKey = await deployedContract.getNewAssetOwner();
                    offer.NewOwnerID = await GetAddressID(offer.NewOwnerPublicKey);

                }

                else //For the destroyed contracts, we need to get the data from db , except the deal price which is lost until we figure out what to do
                {
                    offer.SellerPublicKey = assCon.SellerPublicKey;
                    offer.BuyerPublicKey = assCon.BuyerPublicKey;
                    offer.PriceETH = assCon.DealPrice;
                    offer.PriceILS = offer.PriceETH * account.exchangeRateETH_ILS;
                    offer.BuyerSign = true;
                    offer.RegulatorSign = false;
                    offer.IsDeniedByBuyer = false;
                    offer.IsDeniedByRegulator = true;
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

                offer.EtherscanURL = "https://ropsten.etherscan.io/address/" + assCon.ContractAddress;
                deployedContractsFromBlockchain.Add(offer);
            }


            account.DeployedContractList = deployedContractsFromBlockchain;
            return View("RegulatorMainPage", _regulator);

        }


        public async Task<int> GetAddressID(string PublicKey) //give me blockchain address, I will give you Israeli ID number
        {
            List<AccountID> result = new List<AccountID>();
            result = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts where PublicKey = {0} ", PublicKey).ToListAsync();
            if (result.Count == 0)
                return 0;

            return result[0].ID;
        }
    }
}