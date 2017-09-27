using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Customer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string EMail { get; set; }
        public string Remark { get; set; }
        public IList<Account> Accounts { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Customer()
        {
            Accounts = new List<Account>();
        }
    }
}
