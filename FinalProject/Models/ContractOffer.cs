using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ContractOffer
    {
        
        public int AssetID { get; set; }

        public int OwnertID { get; set; }

        public string SellerPublicKey { get; set; }

        public string Loaction { get; set; }

        public int AreaIn { get; set; }

        public int Rooms { get; set; }

        public string BuyerPublicKey { get; set; }

        
        public double PriceETH { get; set; }

        
        public float PriceILS { get; set; }

        
        public int TimeToBeOpen { get; set; }

        public string ImageURL { get; set; }
    }
}
