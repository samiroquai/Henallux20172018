using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Model;

namespace WPFClient.Services
{
    public interface IDataservice
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerInfoAsync(long customerId);
        Task SaveCustomerChangesAsync(Customer customer);
    }
}
