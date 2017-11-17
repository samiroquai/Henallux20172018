using System;
using System.Collections.Generic;

namespace ProjectUsingDbFirst
{
    public partial class Annonces
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
