using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFClient.Model;
using System.Data.Entity;
namespace WPFClient.Services
{
    public class DatabaseDataService : IDataservice
    {
        EFDataAccess.mdpschsa_labaccconcEntities context = new EFDataAccess.mdpschsa_labaccconcEntities();
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return (await context.Customers.ToArrayAsync()).Select(MapCustomer);
        }

        private static WPFClient.Model.Customer MapCustomer(EFDataAccess.Customer c)
        {
            return new Customer()
            {
                AccountBalance = c.AccountBalance,
                AddressLine1 = c.AddressLine1,
                AddressLine2 = c.AddressLine2,
                City = c.City,
                Country = c.Country,
                EMail = c.EMail,
                Id = c.Id,
                Name = c.Name,
                PostCode = c.PostCode,
                Remark = c.Remark,
            };
        }

        private static void ReverseMapCustomer(WPFClient.Model.Customer c, EFDataAccess.Customer entity)
        {
            entity.AccountBalance = c.AccountBalance;
            entity.AddressLine1 = c.AddressLine1;
            entity.AddressLine2 = c.AddressLine2;
            entity.City = c.City;
            entity.Country = c.Country;
            entity.EMail = c.EMail;
            entity.Id = c.Id;
            entity.Name = c.Name;
            entity.PostCode = c.PostCode;
            entity.Remark = c.Remark;

        }

        public async Task<Customer> GetCustomerInfoAsync(long customerId)
        {
            EFDataAccess.Customer customerFound = await context.Customers.FindAsync(customerId);
            return customerFound != null ? MapCustomer(customerFound) : null;
        }

        public async Task SaveCustomerChangesAsync(Customer customer)
        {
            EFDataAccess.Customer entity = await context.Customers.FindAsync(customer.Id);
            ReverseMapCustomer(customer, entity);
            await context.SaveChangesAsync();
        }
    }
}
