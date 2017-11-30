using System;
using System.Collections.Generic;

namespace ProjectUsingDbFirst
{
    public partial class Customers
    {
        public long Id { get; set; }
        public double AccountBalance { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string PostCode { get; set; }
        public string Remark { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
