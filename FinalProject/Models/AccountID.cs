using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class AccountID
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string PublicKey { get; set; }


    }
}
