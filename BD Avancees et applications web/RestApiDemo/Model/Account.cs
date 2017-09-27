using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Account
    {
        public long Id { get; set; }
        public string IBAN { get; set; }
        public double Balance { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
