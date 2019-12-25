using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ContractOffer
    {
        
        public int AssetID { get; set; } 

        public int OwnerID { get; set; }

        public string SellerPublicKey { get; set; }

        public string Loaction { get; set; }

        public int AreaIn { get; set; }

        public int Rooms { get; set; }

        public string BuyerPublicKey { get; set; }
   
        public double PriceETH { get; set; }

        
        public double PriceILS { get; set; }

        public int BuyerID { get; set; }

        public int TimeToBeOpen { get; set; }

        public string ImageURL { get; set; }

        public bool SellerSign { get; set; }

        public bool BuyerSign { get; set; }

        public bool RegulatorSign { get; set; }

        public string NewOwnerPublicKey { get; set; }

        public int NewOwnerID { get; set; }

        public bool IsDeniedByBuyer { get; set; }

        public bool IsDeniedByRegulator { get; set; }

        public double Tax { get; set; }

        public string DenyReason { get; set; }

        public string EtherscanURL { get; set; }
        
        public string ContractAddress { get; set; }
    }
}
