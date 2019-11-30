using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class OpenContract
    {
        [Key]
        public int AssetID { get; set; }

        [Required]
        public string ContractAddress { get; set; }

        [Required]
        public string SellerPublicKey { get; set; }

        [Required]
        public string BuyerPublicKey { get; set; }

        [Required]
        public bool SellerSign { get; set; }

        [Required]
        public int BuyerSign { get; set; }

         

    }


}
