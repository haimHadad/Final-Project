using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;

namespace FinalProject.Models
{
    public class DappAccount //this class i a general class of user ! ! ! from this class the regulator and buyer/seller will will be inherited
    {
        public string publicKey { get; set; }

        public string privateKey { get; set; }

        public Boolean IsValidated { get; set; }

        public Boolean IsConnectedToBlockChain { get; set; }

        public String AccountNetwork { get; set; }

        public double EthBalance { get; set; }

        public double IlsBalance { get; set; }

        public double exchangeRateETH_ILS { get; set; }

        public Web3 Blockchain { get; set; }


        public Nethereum.Web3.Accounts.Account BlockchainAcount;

        public List<Asset> OwnAssetsList { get; set; }

        public List<AssetInContract> ContractsList { get; set; } //here we read the data from the db, in order to get the contract's address

        public List<ContractOffer> DeployedContractList { get; set; } //here the data will be loaded from the blockchain (contract address)

        public DappAccount()
        {

        }


        
    }
}
