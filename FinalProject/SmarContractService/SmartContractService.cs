﻿using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Util;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;


namespace FinalProject.Models
{
    public class SmartContractService
    {
        public string ContractAddress;
        
        public Web3 Blockchain { get; set; }

        public static string ABI = @"[{""constant"":false,""inputs"":[],""name"":""denyContract"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""setIsExpired"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""cancelContract"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""_taxPay"",""type"":""uint256""}],""name"":""approveAndExcecuteContract"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getAssetDetails"",""outputs"":[{""internalType"":""uint256"",""name"":""AssetID"",""type"":""uint256""},{""internalType"":""string"",""name"":""AssetLoaction"",""type"":""string""},{""internalType"":""uint256"",""name"":""AssetRooms"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""AssetAreaIn"",""type"":""uint256""},{""internalType"":""string"",""name"":""AssetImageURL"",""type"":""string""},{""internalType"":""uint256"",""name"":""AssetPrice"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getOldAssetOwner"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getContractBalance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""abortContract"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getAssetBuyer"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getOwnerSigning"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getIsExpired"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""setBuyerSigning"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getNewAssetOwner"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""isTimerOver"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""TimeLeft"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getBuyerSigning"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""internalType"":""uint256"",""name"":""_timeToBeOpen"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_Id"",""type"":""uint256""},{""internalType"":""string"",""name"":""_Loaction"",""type"":""string""},{""internalType"":""uint256"",""name"":""_Rooms"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""_AreaIn"",""type"":""uint256""},{""internalType"":""string"",""name"":""_Image"",""type"":""string""},{""internalType"":""uint256"",""name"":""_price"",""type"":""uint256""},{""internalType"":""address payable"",""name"":""_buyer"",""type"":""address""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""payable"":true,""stateMutability"":""payable"",""type"":""fallback""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toBuyer"",""type"":""address""}],""name"":""notifyNewOffer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toSeller"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toBuyer"",""type"":""address""}],""name"":""notifyContractApproved"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toGovrenment"",""type"":""address""}],""name"":""notifyContractSigned"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""fromBuyer"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toSeller"",""type"":""address""}],""name"":""notifyDenyOffer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""address"",""name"":""fromContract"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toBuyer"",""type"":""address""},{""indexed"":false,""internalType"":""address"",""name"":""toSeller"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""codeAction"",""type"":""uint256""}],""name"":""notifyCancelOffer"",""type"":""event""}]";
       
        public static string ByteCode = "0x6080604052600b805464ffffffffff60a01b191690553480156200002257600080fd5b50604051620014203803806200142083398181016040526101008110156200004957600080fd5b815160208301516040808501805191519395929483019291846401000000008211156200007557600080fd5b9083019060208201858111156200008b57600080fd5b8251640100000000811182820188101715620000a657600080fd5b82525081516020918201929091019080838360005b83811015620000d5578181015183820152602001620000bb565b50505050905090810190601f168015620001035780820380516001836020036101000a031916815260200191505b506040818152602083015190830151606090930180519195939492939291846401000000008211156200013557600080fd5b9083019060208201858111156200014b57600080fd5b82516401000000008111828201881017156200016657600080fd5b82525081516020918201929091019080838360005b83811015620001955781810151838201526020016200017b565b50505050905090810190601f168015620001c35780820380516001836020036101000a031916815260200191505b50604090815260208201519101519092509050336001600160a01b03821614156200023a576040517f08c379a0000000000000000000000000000000000000000000000000000000008152600401808060200182810382526024815260200180620013fc6024913960400191505060405180910390fd5b610e1088026001556040805160c0810182528881526020808201899052918101879052606081018690526080810185905260a0810184905260048981558851919290916200028f91600591908b01906200036f565b50604082015160028201556060820151600382015560808201518051620002c19160048401916020909101906200036f565b5060a09190910151600590910155600380546001600160a01b03199081163317909155600a80546001600160a01b03848116919093161790819055600b805460ff60a01b191674010000000000000000000000000000000000000000179055426000556040805130815291909216602082015281517ff9b1ce7940a665cc8c05b494835a3f22f87e0e601847e295e2aa576cc8556c6d929181900390910190a1505050505050505062000414565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f10620003b257805160ff1916838001178555620003e2565b82800160010185558215620003e2579182015b82811115620003e2578251825591602001919060010190620003c5565b50620003f0929150620003f4565b5090565b6200041191905b80821115620003f05760008155600101620003fb565b90565b610fd880620004246000396000f3fe6080604052600436106100f35760003560e01c80638aa2964a1161008a578063bebdd82311610059578063bebdd82314610444578063db1f64b014610459578063de824f931461046e578063eba0941514610483576100f3565b80638aa2964a146103f057806399c5578914610405578063a86918ed1461041a578063a93942c41461042f576100f3565b80635c5a0f93116100c65780635c5a0f931461027457806360af135b146103835780636f9fb98a146103b457806384f12bfe146103db576100f3565b8063202dd0b7146101f5578063204e735b1461020c5780632b68bb2d146102215780635780765014610236575b600a546001600160a01b03163314610152576040805162461bcd60e51b815260206004820181905260248201527f4f6e6c79206275796572206d617920706179207468697320636f6e7472616374604482015290519081900360640190fd5b60095434146101925760405162461bcd60e51b815260040180806020018281038252602e815260200180610ebc602e913960400191505060405180910390fd5b600b54600160b81b900460ff161515600114156101e05760405162461bcd60e51b8152600401808060200182810382526025815260200180610e976025913960400191505060405180910390fd5b600b805460ff60b81b1916600160b81b179055005b34801561020157600080fd5b5061020a610498565b005b34801561021857600080fd5b5061020a61058e565b34801561022d57600080fd5b5061020a610601565b34801561024257600080fd5b506102606004803603602081101561025957600080fd5b5035610707565b604080519115158252519081900360200190f35b34801561028057600080fd5b5061028961094f565b604051808781526020018060200186815260200185815260200180602001848152602001838103835288818151815260200191508051906020019080838360005b838110156102e25781810151838201526020016102ca565b50505050905090810190601f16801561030f5780820380516001836020036101000a031916815260200191505b50838103825285518152855160209182019187019080838360005b8381101561034257818101518382015260200161032a565b50505050905090810190601f16801561036f5780820380516001836020036101000a031916815260200191505b509850505050505050505060405180910390f35b34801561038f57600080fd5b50610398610aa5565b604080516001600160a01b039092168252519081900360200190f35b3480156103c057600080fd5b506103c9610ab5565b60408051918252519081900360200190f35b3480156103e757600080fd5b5061020a610aba565b3480156103fc57600080fd5b50610398610bb4565b34801561041157600080fd5b50610260610bc3565b34801561042657600080fd5b50610260610bd3565b34801561043b57600080fd5b5061020a610be3565b34801561045057600080fd5b50610398610d2a565b34801561046557600080fd5b50610260610d39565b34801561047a57600080fd5b506103c9610d5f565b34801561048f57600080fd5b50610260610d89565b600a546001600160a01b031633146104e15760405162461bcd60e51b8152600401808060200182810382526021815260200180610f5c6021913960400191505060405180910390fd5b600b54600160a81b900460ff1615156001141561052f5760405162461bcd60e51b815260040180806020018281038252602c815260200180610e11602c913960400191505060405180910390fd5b600a54600354604080513081526001600160a01b039384166020820152919092168183015290517f60fd86fe4a800e746cd40800c4b85092b1036ec613534ae7fee8d9851dd517969181900360600190a1600a546001600160a01b0316ff5b33737988dfd8e9cecb888c1aea7cb416d44c6160ef80146105e05760405162461bcd60e51b8152600401808060200182810382526026815260200180610f366026913960400191505060405180910390fd5b600b805460ff60c01b198116600160c01b9182900460ff1615909102179055565b737988dfd8e9cecb888c1aea7cb416d44c6160ef8033146106535760405162461bcd60e51b8152600401808060200182810382526026815260200180610f366026913960400191505060405180910390fd5b600b54600160b01b900460ff161515600114156106a15760405162461bcd60e51b815260040180806020018281038252602b815260200180610f0b602b913960400191505060405180910390fd5b600a54600354604080513081526001600160a01b03938416602082015291909216818301526001606082015290517f06af57916f4dcbcd64868bf1d68e647c602cf1b42d36da9d90d332b55eae43639181900360800190a1600a546001600160a01b0316ff5b600033737988dfd8e9cecb888c1aea7cb416d44c6160ef801461075b5760405162461bcd60e51b8152600401808060200182810382526026815260200180610f366026913960400191505060405180910390fd5b610763610ab5565b82106107a05760405162461bcd60e51b8152600401808060200182810382526034815260200180610e636034913960400191505060405180910390fd5b600b54600160a81b900460ff1615156001146107ed5760405162461bcd60e51b815260040180806020018281038252602f815260200180610dc1602f913960400191505060405180910390fd5b600b54600160b01b900460ff1615156001141561083b5760405162461bcd60e51b815260040180806020018281038252602b815260200180610f0b602b913960400191505060405180910390fd5b600282905560095460035460405191849003916001600160a01b039091169082156108fc029083906000818181858888f19350505050158015610882573d6000803e3d6000fd5b50600254604051737988dfd8e9cecb888c1aea7cb416d44c6160ef809180156108fc02916000818181858888f193505050501580156108c5573d6000803e3d6000fd5b50600a54600b805460ff60b01b196001600160a01b03199091166001600160a01b039384161716600160b01b17908190556003546040805130815291841660208301529190921682820152517f99da6eb11c504fc59be599d1e506e1f5e2b84cfce73f19c546700516ed1029b49181900360600190a15050600b54600160b01b900460ff16919050565b6004546006546007546009546005805460408051602060026001851615610100026000190190941693909304601f8101849004840282018401909252818152600097606097899788978a9789979496909593949293600893909187918301828280156109fc5780601f106109d1576101008083540402835291602001916109fc565b820191906000526020600020905b8154815290600101906020018083116109df57829003601f168201915b5050855460408051602060026001851615610100026000190190941693909304601f8101849004840282018401909252818152959a5087945092508401905082828015610a8a5780601f10610a5f57610100808354040283529160200191610a8a565b820191906000526020600020905b815481529060010190602001808311610a6d57829003601f168201915b50505050509150955095509550955095509550909192939495565b6003546001600160a01b03165b90565b303190565b6003546001600160a01b03163314610b035760405162461bcd60e51b8152600401808060200182810382526027815260200180610f7d6027913960400191505060405180910390fd5b600b54600160a81b900460ff16151560011415610b515760405162461bcd60e51b8152600401808060200182810382526021815260200180610df06021913960400191505060405180910390fd5b610b59610d39565b1515600114610ba6576040805162461bcd60e51b8152602060048201526014602482015273151a5b5948191a591b981d081bdd995c881e595d60621b604482015290519081900360640190fd5b600a546001600160a01b0316ff5b600a546001600160a01b031690565b600b54600160a01b900460ff1690565b600b54600160c01b900460ff1690565b600a546001600160a01b03163314610c2c5760405162461bcd60e51b8152600401808060200182810382526021815260200180610eea6021913960400191505060405180910390fd5b600b54600160b81b900460ff161515600114610c795760405162461bcd60e51b8152600401808060200182810382526027815260200180610d9a6027913960400191505060405180910390fd5b600b54600160a81b900460ff16151560011415610cc75760405162461bcd60e51b8152600401808060200182810382526026815260200180610e3d6026913960400191505060405180910390fd5b600b805460ff60a81b1916600160a81b17905560408051308152737988dfd8e9cecb888c1aea7cb416d44c6160ef80602082015281517f56ced8198726b4967f1b6fd0dd9fc3e3e1b9a6273b4fb654135b45458715ba1c929181900390910190a1565b600b546001600160a01b031690565b60008060005442039050600154811115610d57576001915050610ab2565b600091505090565b6000610d69610d39565b151560011415610d7b57506000610ab2565b600054420360015403905090565b600b54600160a81b900460ff169056fe596f75206469646e60742070617965642074686520707269636520666f7220746865206465616c54686520636f6e7472616374206469646e60742072656369657665207468652062757965726073207369676e696e67427579657220616c7265616479207369676e65642074686520636f6e747261637442757965722063616e6e6f742064656e792074686520636f6e7472616374206166746572207369676e696e6754686520627579657220616c7265616479207369676e6564207468697320636f6e747261637454686520746178207061796d656e74206d757374206265206c6f776572207468616e20746865206275796572207061796d656e74427579657220616c726561647920706179656420666f72207468697320636f6e7472616374596f757220706179206973206e6f7420657175616c20746f207468652061737365746073207072696365207461674f6e6c79206275796572206d6179207369676e207468697320636f6e747261637454686520676f7672656e6d656e7420616c7265616479207369676e6564207468697320636f6e74726163744f6e6c7920676f7672656e6d656e74206d61792063616c6c20746869732066756e6374696f6e4f6e6c79206275796572206d61792063616c6c20746869732066756e6374696f6e4f6e6c79206173736574206f776e6572206d61792063616c6c20746869732066756e6374696f6ea265627a7a723158209f0aad40d41fc25f23d6c72571ebd5f117f57346205090aa5ea9661573669e3b64736f6c634300050b0032427579657220616e642073656c6c657220617265207468652073616d6520706572736f6e";

        public DappAccount accountCaller { get; set; }

        public string EtherscanURL;

        Contract deployedContractIsntance; 

        public SmartContractService(DappAccount _yourAccount, string contractAddress)
        {
            accountCaller = _yourAccount;
            EtherscanURL = "https://ropsten.etherscan.io/address/" + contractAddress;
            deployedContractIsntance = accountCaller.Blockchain.Eth.GetContract(ABI, contractAddress);
            ContractAddress = contractAddress;
        }

        public static async Task<string> Deploy(DappAccount yourAccount, uint hoursTimeAmount, uint assetID, string assetLoaction, uint assetRooms, uint assetAreaIn, string assetURL, double assetPrice, string buyerAddress)
        {
              var TimeHours = hoursTimeAmount;
              var ID = assetID;
              var Loaction = assetLoaction;
              var Rooms = assetRooms;
              var AreaIn = assetAreaIn;
              var Image = assetURL;
              var Price = UnitConversion.Convert.ToWei(assetPrice);
              var Buyer = buyerAddress;

              object[] contractParams = new object[]{ TimeHours, ID, Loaction, Rooms, AreaIn, Image, Price, Buyer };

            //-----------------Seller create a contract and sign it---------------------
            var estimateGasForDeploy = await yourAccount.Blockchain.Eth.DeployContract.EstimateGasAsync(ABI, ByteCode, yourAccount.publicKey, contractParams);
            var receiptSalesContract = await yourAccount.Blockchain.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(ABI, ByteCode, yourAccount.publicKey, estimateGasForDeploy, null, contractParams); 
            var salesContractAddress = receiptSalesContract.ContractAddress; //after deployment, we get contract address. 
            return salesContractAddress;
        }


        public async Task<string> getAssetDestails() //Buyer the asset that included in the deal
        {
            var contractHandlerAsBuyer = accountCaller.Blockchain.Eth.GetContractHandler(ContractAddress);                   
            var getAssetDetailsOutputDTOAsBuyer = await contractHandlerAsBuyer.QueryDeserializingToObjectAsync<GetAssetDetailsFunction, GetAssetDetailsOutputDTO>();
            BigInteger assetID = getAssetDetailsOutputDTOAsBuyer.AssetID;
            string assetLoaction = getAssetDetailsOutputDTOAsBuyer.AssetLoaction;
            BigInteger assetRooms = getAssetDetailsOutputDTOAsBuyer.AssetRooms;
            BigInteger assetAreaIn = getAssetDetailsOutputDTOAsBuyer.AssetAreaIn;
            string assetImageUrl = getAssetDetailsOutputDTOAsBuyer.AssetImageURL;
            BigInteger assetPriceAtWie = getAssetDetailsOutputDTOAsBuyer.AssetPrice;
            //var assetPriceAtEther = Web3.Convert.FromWei(assetPriceAtWie);      
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(getAssetDetailsOutputDTOAsBuyer);
            return json;

            //string assetDetialsAllTogether = "" + assetID + ", " + assetLoaction + ", " + assetRooms + ", " + assetAreaIn + ", " + assetImageUrl + ", " + assetPriceAtEther;
        }

        public async Task<string> getBuyerAddress() //get the address of the buyer which included in the deal
        {
            var getAssetBuyerFunctionAsBuyer = deployedContractIsntance.GetFunction("getAssetBuyer");
            var assetBuyerAddress = await getAssetBuyerFunctionAsBuyer.CallAsync<string>();
            return assetBuyerAddress;
        }

        public async Task<bool> getBuyerSign()
        {
            var getBuyerSigningFunctionAsBuyer = deployedContractIsntance.GetFunction("getBuyerSigning");
            var buyerSign = await getBuyerSigningFunctionAsBuyer.CallAsync<bool>();
            return buyerSign;
        }

        public async Task<bool> sendEtherToContract(double amount) //send money to the contract, if fail return false, else true
        {
            try
            {
                decimal EtherToPay = Convert.ToDecimal(amount);
                if (this.getBuyerAddress().Equals(this.accountCaller.publicKey))
                    return false;
                var payTransaction = await accountCaller.Blockchain.Eth.GetEtherTransferService().TransferEtherAndWaitForReceiptAsync(this.ContractAddress, EtherToPay, 4, new BigInteger(45000));
                
            }
            catch(Exception e) //can fail if the acount not belong to the buyer, or if the didn`t send the exact amount of money
            {
                return false; //send failed;
            }
            return true;
        }

        public async Task<double> getBalance()
        {
            double returnedEther;
            var getContractBalanceFunction = deployedContractIsntance.GetFunction("getContractBalance");        
                var salesContractBalanceAsBuyer = await getContractBalanceFunction.CallAsync<UInt64>();
                var assetPriceAtEther = Web3.Convert.FromWei(salesContractBalanceAsBuyer);
                returnedEther = Convert.ToDouble(assetPriceAtEther);
                return returnedEther;
        }

        public async Task<string> getNewAssetOwner()
        {
            var getNewAssetOwnerFunction = deployedContractIsntance.GetFunction("getNewAssetOwner");
            var newOwner = await getNewAssetOwnerFunction.CallAsync<string>();
            return newOwner;
        }

        public async Task<string> getOldAssetOwner()
        {
            var getNewAssetOwnerFunction = deployedContractIsntance.GetFunction("getOldAssetOwner");
            var newOwner = await getNewAssetOwnerFunction.CallAsync<string>();
            return newOwner;
        }

        public async Task<ulong> getTimeLeftInSeconds()
        {
            var TimeLeftFunction = deployedContractIsntance.GetFunction("TimeLeft");
            var timeLeft = await TimeLeftFunction.CallAsync<UInt64>();
            return timeLeft;
        }
  

        public async Task<bool> cancelDeal() //For regulator (=Govrenment)
        {
            string GovrenmentAddress = "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80";
            if (!GovrenmentAddress.Equals(accountCaller.publicKey)) //if the caller is not the regulator
                return false;
            try 
            {
                var cancelContractFunction = deployedContractIsntance.GetFunction("cancelContract");  //find the method of the contract 
                var gasEstimationForCancel = await cancelContractFunction.EstimateGasAsync(accountCaller.publicKey, null, null);
                var receiptOfCancel = await cancelContractFunction.SendTransactionAndWaitForReceiptAsync(accountCaller.publicKey, gasEstimationForCancel, null, null);

            }
            catch (Exception e) //if regulator already signed - I didn`t have energy to right a getter in Solidity....
            {
                return false;
            }
            return true;
        }

        public async Task<bool> approveAndExcecute(double taxPercentage) //for regulator only
        {
            string GovrenmentAddress = "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80";
            if (!GovrenmentAddress.Equals(accountCaller.publicKey)) //if the caller is not the regulator
                return false;
            try 
            {
                double salesContractBalance = await getBalance();
                double contractBalanceAsDouble = Convert.ToDouble(salesContractBalance); ;
                double taxEtherAmountAsDouble = contractBalanceAsDouble * taxPercentage;
                bool buyerSign = await getBuyerSign();
                if (buyerSign == true) //check if buyer signed
                {
                    var approveAndExcecuteContractFunction = deployedContractIsntance.GetFunction("approveAndExcecuteContract");  //find the method of the contract 
                    var gasEstimationForApproval = await approveAndExcecuteContractFunction.EstimateGasAsync(accountCaller.publicKey,
                        null, null, new BigInteger(taxEtherAmountAsDouble));
                    var receiptOfApproval = await approveAndExcecuteContractFunction.SendTransactionAndWaitForReceiptAsync(accountCaller.publicKey,
                        gasEstimationForApproval, null, null, new BigInteger(taxEtherAmountAsDouble));
                }
                else
                    return false;
            }
            catch(Exception e)
            {
                return false;
            }
            

            return true;
        }


        public async Task<bool> denyDeal() //For regulator (=Govrenment)
        {
            string BuyerAddress = await getBuyerAddress();
            bool BuyerSign = await getBuyerSign();

            if (BuyerSign == true) //if buyer already signed the deal!
                return false;

            if (!BuyerAddress.Equals(accountCaller.publicKey)) //if the caller is not the buyer
                return false;
            try
            {
                var denyContractFunction = deployedContractIsntance.GetFunction("denyContract");  //find the method of the contract 
                var gasEstimationForCancel = await denyContractFunction.EstimateGasAsync(accountCaller.publicKey, null, null);
                var receiptOfDeny = await denyContractFunction.SendTransactionAndWaitForReceiptAsync(accountCaller.publicKey, gasEstimationForCancel, null, null);

            }
            catch (Exception e) //if regulator already signed - I didn`t have energy to right a getter in Solidity....
            {
                return false;
            }
            return true;
        }

        public async Task<bool> abort() //in case buyer didnt take action in time
        {
            string SellerAddress = await getOldAssetOwner();
            bool BuyerSign = await getBuyerSign();
            ulong timeLeft = await getTimeLeftInSeconds();

            if (!SellerAddress.Equals(accountCaller.publicKey)) //if the caller is not the buyer
                return false;

            if (BuyerSign == true) //if buyer already signed the deal, the seller cannot abort!
                return false;

            if (timeLeft > 0) //if there is a time for the buyer to choose
                return false;

            try
            {
                var abortContractFunction = deployedContractIsntance.GetFunction("abortContract");  //find the method of the contract 
                var gasEstimationForCancel = await abortContractFunction.EstimateGasAsync(accountCaller.publicKey, null, null);
                var receiptOfAbort = await abortContractFunction.SendTransactionAndWaitForReceiptAsync(accountCaller.publicKey, gasEstimationForCancel, null, null);

            }
            catch (Exception e) //if regulator already signed - I didn`t have energy to right a getter in Solidity....
            {
                return false;
            }
            return true;
        }

        public async Task<bool> setIsExpired() //For regulator (=Govrenment)
        {
            string GovrenmentAddress = "0x7988dfD8E9ceCb888C1AeA7Cb416D44C6160Ef80";
            if (!GovrenmentAddress.Equals(accountCaller.publicKey)) //if the caller is not the regulator
                return false;
            try
            {
                var setIsExpiredFunction = deployedContractIsntance.GetFunction("setIsExpired");  //find the method of the contract 
                var gasEstimationForCancel = await setIsExpiredFunction.EstimateGasAsync(accountCaller.publicKey, null, null);
                var receiptSetIsExpired = await setIsExpiredFunction.SendTransactionAndWaitForReceiptAsync(accountCaller.publicKey, gasEstimationForCancel, null, null);

            }
            catch (Exception e) //if regulator already signed - I didn`t have energy to right a getter in Solidity....
            {
                return false;
            }
            return true;
        }

    }

    internal partial class GetAssetDetailsFunction : GetAssetDetailsFunctionBase { } //this is Ethereum API - no need to check this module!!

    [Function("getAssetDetails", typeof(GetAssetDetailsOutputDTO))]
    internal class GetAssetDetailsFunctionBase : FunctionMessage
    {

    }


    internal partial class GetAssetDetailsOutputDTO : GetAssetDetailsOutputDTOBase { }

    [FunctionOutput]
    internal class GetAssetDetailsOutputDTOBase : IFunctionOutputDTO
    {
        [Parameter("uint256", "AssetID", 1)]
        public virtual BigInteger AssetID { get; set; }
        [Parameter("string", "AssetLoaction", 2)]
        public virtual string AssetLoaction { get; set; }
        [Parameter("uint256", "AssetRooms", 3)]
        public virtual BigInteger AssetRooms { get; set; }
        [Parameter("uint256", "AssetAreaIn", 4)]
        public virtual BigInteger AssetAreaIn { get; set; }
        [Parameter("string", "AssetImageURL", 5)]
        public virtual string AssetImageURL { get; set; }
        [Parameter("uint256", "AssetPrice", 6)]
        public virtual BigInteger AssetPrice { get; set; }
    }

   
}
