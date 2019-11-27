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
        public string address { get; set; }
        [Required]
        public string accountAddress { get; set; }
    }
}