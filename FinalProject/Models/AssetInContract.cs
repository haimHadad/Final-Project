using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class AssetInContract
    {
        [Key]
        public int AssetID { get; set; }

        [Key]
        public string ContractAddress { get; set; }

        [Required]
        public string SellerPublicKey { get; set; }

        [Required]
        public string BuyerPublicKey { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string DeniedBy { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public double DealPrice { get; set; }
    }


}
