using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class ClosedContract
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
        public bool IsApproved { get; set; }

        [Required]
        public string RejectedBy { get; set; }

        [Required]
        public string Reason { get; set; }

    }
}
