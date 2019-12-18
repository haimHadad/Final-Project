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

        public ContractsStatusController(AssetsInContractContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ContractsStatusPage() //now we are going to read all the open contracts
        {
            await DappAccountController.RefreshAccountData();
            DappAccount account = DappAccountController.myAccount;
            List<AssetInContract> openContractsFromDB = new List<AssetInContract>();
            openContractsFromDB = await _context.AssetsInContract.FromSqlRaw("select * from AssetsInContract where ( SellerPublicKey = {0} or BuyerPublicKey = {0} )", account.publicKey).ToListAsync();
            

            foreach (AssetInContract assCon in openContractsFromDB)
            {
                if (!assCon.Status.Equals("Denied"))  //For the non-destroyed contracts, "Denied" => Self Destruction
                {
                    ContractOffer offer = new ContractOffer();
                    string contractAddress = assCon.ContractAddress;
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
                    if (assCon.Status.Equals("Approved"))
                        offer.NewOwnerPublicKey = await deployedContract.getNewAssetOwner();
                    if (assCon.Status.Equals("Ongoing"))
                    {
                        ulong time = await deployedContract.getTimeLeftInSeconds();
                        int timeLeft = (int)time;
                        offer.TimeToBeOpen = timeLeft;
                    }
                        
                    //DenyReason
                    //IsDeniedByBuyer
                    //IsDeniedByRegulator
                    //OwnerID
                    //TimeToBeOpen



                    int k = 0;
                }

                else
                {

                }
                

                int i = 0;

            }






            return View(DappAccountController.myAccount);
        }
    }
}