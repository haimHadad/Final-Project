using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace FinalProject.Controllers
{
    public class InGoingRequestsController : Controller
    {

        private AssetsInContractContext _AssetInContractsContext; // AssetInContracts db table
        private AccountsContext _AccountsContext; // Accounts db table
        private AssetContext _AssetsContext; // Assets db table
        public static DappAccount _regulator; // Regulaotr account
        public InGoingRequestsController(AssetsInContractContext context, AccountsContext context2, AssetContext context3)
        {
            _AssetInContractsContext = context;
            _AccountsContext = context2;
            _AssetsContext = context3;
        }



        public async Task<IActionResult> ShowInGoingRequestsAsync()
        { //show the pending contracts page for regulator approval
            DappAccount account = RegulatorController._regulator;
            await DappAccountController.RefreshAccountData(account.publicKey);
            account.DeployedContractList =  await GetPendingContracts();            
            return View("InGoingRequests", account);
        }


        private async Task<List<ContractOffer>> GetPendingContracts()
        { //get all the open contracts (pending) that waits for the regulaotr approval
            DappAccount account = RegulatorController._regulator;
            await DappAccountController.RefreshAccountData(account.publicKey);
            List<AssetInContract> deployedContractsFromDB = new List<AssetInContract>();
            deployedContractsFromDB = await _AssetInContractsContext.AssetsInContract.FromSqlRaw("select * from AssetsInContract where Status= 'Pending'").ToListAsync();
            List<ContractOffer> deployedContractsFromBlockchain = new List<ContractOffer>();
            foreach (AssetInContract assCon in deployedContractsFromDB)
            {
                ContractOffer offer = new ContractOffer();
                string contractAddress = assCon.ContractAddress;
                offer.ContractAddress = contractAddress;
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

                offer.EtherscanURL = "https://ropsten.etherscan.io/address/" + assCon.ContractAddress;
                deployedContractsFromBlockchain.Add(offer);
            }
            return deployedContractsFromBlockchain;
        }



        public async Task<string> UpdatePendingContracts()
        { //update the table of the pending contract if the regulaotr click on the refresh button
            List<ContractOffer> deployedContractsFromBlockchain = await GetPendingContracts();
            RegulatorController._regulator.DeployedContractList = deployedContractsFromBlockchain;
            var ContractsListJson = Newtonsoft.Json.JsonConvert.SerializeObject(deployedContractsFromBlockchain);
            return ContractsListJson;
        }

        public async Task<int> GetAddressID(string PublicKey)
        { //return Israeli ID number from attached blockchain address
            List<AccountID> result = new List<AccountID>();
            result = await _AccountsContext.Accounts.FromSqlRaw("select * from Accounts where PublicKey = {0} ", PublicKey).ToListAsync();
            if (result.Count == 0)
                return 0;

            return result[0].ID;
        }

        public void DownloadExcelPendingContracts()
        { //downlad all the pending contract in the HTML table to excel file
            var collection = RegulatorController._regulator.DeployedContractList;
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
            Sheet.Cells["I1"].Value = "URL";
            Sheet.Cells["J1"].Value = "Tax Amount-ETH";
            Sheet.Cells["K1"].Value = "Tax Amount-ILS";
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
                Sheet.Cells[string.Format("I{0}", row)].Value = contract.EtherscanURL;
                Sheet.Cells[string.Format("J{0}", row)].Value = contract.PriceETH*0.17;
                Sheet.Cells[string.Format("K{0}", row)].Value = contract.PriceILS*0.17;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Headers.Add("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.Body.WriteAsync(Ep.GetAsByteArray());
        }

        
        public async Task<string> ApproveContractAsRegulatorAsync(string ContractAddress)
        { //approve the contract (regulator do it)
            DappAccount account = RegulatorController._regulator;
            await DappAccountController.RefreshAccountData(account.publicKey);
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            Asset assetDetials = await deployedContract.getAssetDestails();
            double DealPriceEth = assetDetials.Price;
            double taxToGet = DealPriceEth * 0.17;
            double beforeBalanceETH = await DappAccountController.get_ETH_Balance(account.publicKey, account.publicKey);
            double beforeBalanceILS = await DappAccountController.get_ILS_Balance(account.publicKey, account.publicKey);
            double exchangeRate = DappAccountController.getExchangeRate_ETH_To_ILS();
            double afterBalanceETH; 
            double feeETH;
            double feeILS;
            var isApproved = await deployedContract.approveAndExcecute(0.17);
            if(isApproved ==true)
            {
                afterBalanceETH = await DappAccountController.get_ETH_Balance(account.publicKey, account.publicKey);
                feeETH = afterBalanceETH - beforeBalanceETH - taxToGet;
                feeILS = feeETH * exchangeRate;
                feeILS = Math.Truncate(feeILS * 100) / 100; //make the double number to be with 3 digits after dot
                await UpdateContractToApprovedInDB(ContractAddress);
                await SwitchOwnership(ContractAddress);


            }

            else
            {
                throw new Exception("Out of money");
            }


            RegulatorConfirmationRecipt recipt = new RegulatorConfirmationRecipt();
            recipt.ContractAddress = ContractAddress;
            recipt.feeETH = feeETH;
            recipt.feeILS = feeILS;
            var ReciptJson = Newtonsoft.Json.JsonConvert.SerializeObject(recipt);
            return ReciptJson;
        }

        private async Task UpdateContractToApprovedInDB(string ContractAddress)
        { //update registry - contract in "InGoing state" to "Approve" state
            DappAccount account = RegulatorController._regulator;
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            Asset dealAsset = await deployedContract.getAssetDestails();
            int dealAssetID = dealAsset.AssetID;
            AssetInContract report ; 
            try 
            {
                report = (from d in _AssetInContractsContext.AssetsInContract
                              where d.AssetID == dealAssetID && d.Status.Equals("Approved")
                              select d).Single();
            } 
            catch(Exception e) 
            {
                report = null;
            }
            

            if(report != null)
            {
                _AssetInContractsContext.AssetsInContract.Remove(report);
                _AssetInContractsContext.SaveChanges();
            }



            var report2 = (from d in _AssetInContractsContext.AssetsInContract
                          where d.ContractAddress == ContractAddress
                          select d).Single();
            report2.Status = "Approved";
            _AssetInContractsContext.AssetsInContract.Update(report2);
            _AssetInContractsContext.SaveChanges();
        }

        private async Task SwitchOwnership(string ContractAddress)
        { //replace the ownership upon the asset - buyer = new owner
            string PublicKeySeller, PublicKeyNewOwner; 
            DappAccount account = RegulatorController._regulator;
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            var report = (from d in _AssetInContractsContext.AssetsInContract
                          where d.ContractAddress == ContractAddress
                          select d).Single();
            PublicKeySeller = report.SellerPublicKey;
            PublicKeyNewOwner = await deployedContract.getNewAssetOwner();
            Asset dealAsset = await deployedContract.getAssetDestails();
            int dealAssetID = dealAsset.AssetID;
            var PublicKeyNewOwnerToCheck = report.BuyerPublicKey.ToLower();
            

            if (PublicKeyNewOwnerToCheck.Equals(PublicKeyNewOwner))
            {
                PublicKeyNewOwner = report.BuyerPublicKey;
                int newOwnerID = await GetAddressID(PublicKeyNewOwner);
               
                var report2 = (from d in _AssetsContext.Assets
                                where d.AssetID == dealAssetID
                               select d).Single();
                report2.OwnerPublicKey = PublicKeyNewOwner;
                report2.Price = dealAsset.Price;
                report2.OwnerID = newOwnerID;
                _AssetsContext.Assets.Update(report2);
                _AssetsContext.SaveChanges();
            }
        }

      public async Task<string> CancelContractAsRegulator(string ContractAddress, string DenyNotes) 
      {     //cancel the contract and return the money to the buyer
            DappAccount account = RegulatorController._regulator;
            await DappAccountController.RefreshAccountData(account.publicKey);
            SmartContractService deployedContract = new SmartContractService(account, ContractAddress);
            double beforeBalanceETH = await DappAccountController.get_ETH_Balance(account.publicKey, account.publicKey);
            double beforeBalanceILS = await DappAccountController.get_ILS_Balance(account.publicKey, account.publicKey);
            double exchangeRate = DappAccountController.getExchangeRate_ETH_To_ILS();
            double afterBalanceETH;
            double feeETH;
            double feeILS;
            var ReciptJson = "";
            var isCanceled = await deployedContract.cancelDeal();
            if (isCanceled == true)
            {
                afterBalanceETH = await DappAccountController.get_ETH_Balance(account.publicKey, account.publicKey);
                feeETH = beforeBalanceETH - afterBalanceETH ;
                feeILS = feeETH * exchangeRate;
                feeILS = Math.Truncate(feeILS * 100) / 100; //make the double number to be with 3 digits after dot
                RegulatorConfirmationRecipt recipt = new RegulatorConfirmationRecipt();
                recipt.ContractAddress = ContractAddress;
                recipt.feeETH = feeETH;
                recipt.feeILS = feeILS;
                ReciptJson = Newtonsoft.Json.JsonConvert.SerializeObject(recipt);
                UpdateContractToDeniedAsRegulatorInDB(ContractAddress, DenyNotes);


                return ReciptJson;
            }

            else
            {
                throw new Exception("No money the sign the transaction");       
            }

        }

        private void UpdateContractToDeniedAsRegulatorInDB(string ContractAddress, string Notes)
        {  //update registry - contract in "InGoing state" to "Denied" state            

            var report = (from d in _AssetInContractsContext.AssetsInContract
                           where d.ContractAddress == ContractAddress
                           select d).Single();
            report.Status = "Denied";
            report.DeniedBy = "Regulator";
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

    }

    internal class RegulatorConfirmationRecipt
    { //response class after approve / deny operation
        public string ContractAddress { get; set; }

        public double feeETH { get; set; }

        public double feeILS { get; set; }

    }
}