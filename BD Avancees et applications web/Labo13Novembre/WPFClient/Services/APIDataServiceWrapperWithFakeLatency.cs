using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Model;

namespace WPFClient.Services
{
    public class APIDataServiceWrapperWithFakeLatency:APIDataService
    {
        private const int FAKE_LATENCY_DURATION_IN_SECONDS = 2;
        public override async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(FAKE_LATENCY_DURATION_IN_SECONDS));
            return await base.GetAllCustomersAsync();
        }

        public override async Task SaveCustomerChangesAsync(Customer customer)
        {
            await Task.Delay(TimeSpan.FromSeconds(FAKE_LATENCY_DURATION_IN_SECONDS));
            await base.SaveCustomerChangesAsync(customer);
        }

        public override async Task<Customer> GetCustomerInfoAsync(long customerId)
        {
            await Task.Delay(TimeSpan.FromSeconds(FAKE_LATENCY_DURATION_IN_SECONDS));
            return await base.GetCustomerInfoAsync(customerId);
        }

    }
}
