using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class Regulator
    {
        public string publicKey { get; set; }
        public string privateKey { get; set; }
        public List<Asset> assetsList { get; set; }
        public Regulator()
        {

        }
    }
}
