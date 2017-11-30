using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Model;

namespace WPFClient.Services
{
    public class APIDataService : IDataservice
    {

        private const string BASE_URI = "http://localhost:5000/api/Customer";
        public virtual async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(BASE_URI);
            return JsonConvert.DeserializeObject<IEnumerable<Customer>>(json);
        }

        public virtual async Task<Customer> GetCustomerInfoAsync(long customerId)
        {
            HttpClient client = new HttpClient();
            string json = await client.GetStringAsync(BASE_URI + "/ 1");
            return JsonConvert.DeserializeObject<Customer>(json);
        }

        public virtual async Task SaveCustomerChangesAsync(Customer customer)
        {
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(customer);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync(BASE_URI + "/" + customer.Id, body);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    throw new UpdateException(response.ReasonPhrase + ". Le client a été mis à jour par quelqu'un d'autre. Veuillez rafraîchir les données");
                else
                    throw new UpdateException(response.ReasonPhrase);
            }
        }
    }
}
