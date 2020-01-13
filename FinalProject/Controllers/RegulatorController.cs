using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using FinalProject.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace FinalProject.Controllers
{
    public class RegulatorController : Controller
    {

        private AssetsInContractContext _AssetInContractsContext; // AssetInContracts db table
        private AccountsContext _AccountsContext; // Accounts db table
        private AssetContext _AssetsContext; // Assets db table
        public static DappAccount _regulator; // Regulaotr account

        public RegulatorController(AssetsInContractContext context, AccountsContext context2, AssetContext context3)
        {
            _AssetInContractsContext = context;
            _AccountsContext = context2;
            _AssetsContext = context3;
        }

        public async Task<IActionResult> RegulatorMainPage()
        {  //here the login succeeded 
            await DappAccountController.RefreshAccountData(_regulator.publicKey);
            return RedirectToAction("ShowHomePage", "Regulator");
            
        }


        public async Task<IActionResult> ShowHomePage()
        {   //read all closed contracts and show it in page
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

                else 
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


        public async Task<int> GetAddressID(string PublicKey)
        {   //return Israeli ID number from attached blockchain address
            List<AccountID> result = new List<AccountID>();
            result = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts where PublicKey = {0} ", PublicKey).ToListAsync();
            if (result.Count == 0)
                return 0;

            return result[0].ID;
        }

        public void DownloadExcelFinalDecitions()
        { //download the closed contract html table into an excel file
            var collection = _regulator.DeployedContractList;
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Asset ID";
            Sheet.Cells["B1"].Value = "Loaction";
            Sheet.Cells["C1"].Value = "Rooms";
            Sheet.Cells["D1"].Value = "AreaIn";
            Sheet.Cells["E1"].Value = "Deal Price - ETH";
            Sheet.Cells["F1"].Value = "Deal Price - ILS";
            Sheet.Cells["G1"].Value = "Seller ID";
            Sheet.Cells["H1"].Value = "Buyer ID";
            int row = 2;
            foreach (var contract in collection)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = contract.AssetID;
                Sheet.Cells[string.Format("B{0}", row)].Value = contract.Loaction;
                Sheet.Cells[string.Format("C{0}", row)].Value = contract.Rooms;
                Sheet.Cells[string.Format("D{0}", row)].Value = contract.AreaIn;
                Sheet.Cells[string.Format("E{0}", row)].Value = contract.PriceETH;
                Sheet.Cells[string.Format("F{0}", row)].Value = contract.PriceILS;
                Sheet.Cells[string.Format("G{0}", row)].Value = contract.OwnerID;
                Sheet.Cells[string.Format("H{0}", row)].Value = contract.BuyerID;
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