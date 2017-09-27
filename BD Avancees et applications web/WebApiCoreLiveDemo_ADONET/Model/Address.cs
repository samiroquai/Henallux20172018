using System.ComponentModel.DataAnnotations;

namespace webapidemo2.Model
{
public class Address
{
    [Required]
    public string Street { get; set; }
    [Required]
    public string Locality { get; set; }
    [Required]
    public string Country { get; set; }
    [Required]
    public string PostalCode { get; set; }
}
}