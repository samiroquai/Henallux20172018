using System.ComponentModel.DataAnnotations;

namespace webapidemo2.Model
{

public class Client{
    public Client()
    {
        
    }

    [Required]
    public string Name { get; set; }
    public int Id { get; set; }

    [Required]
    public Address Address { get; set; }

    [EmailAddress]
    [Required]
    public string EMail{get;set;}
}
}