using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDAL
{
    public class BankAccountNotFoundException : Exception
    {
        public BankAccountNotFoundException()
            : base("Le compte en banque donné n'a pas pu être retrouvé")
        {

        }
    }
}
