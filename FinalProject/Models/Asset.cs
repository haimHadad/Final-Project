using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace FinalProject.Models
{
    
    public class Asset
    {
        [Key]
        public int AssetID { get; set; }
        
        [Required]
        public int OwnerID { get; set; }

        [Required]
        public string OwnerPublicKey { get; set; }

        [Required]
        public string Loaction { get; set; }

        [Required]
        public int AreaIn { get; set; }

        [Required]
        public int Rooms { get; set; }

        [Required]
        public string ImageURL { get; set; }

        [Required]
        public double Price { get; set; }
    }
}