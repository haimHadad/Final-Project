using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class RegulatorAccount:DappAccount
    {
        //the account of the regulaotr- inherited from dappAccount class
        public List<AccountID> AllAccounts;
        public RegulatorAccount() :base()
        {

        }


    }
}
