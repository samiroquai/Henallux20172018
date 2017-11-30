using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AccesConcurrentsNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly CompanyContext _context;
        public CustomerController(CompanyContext context)
        {
            _context=context;
        }
        // GET api/Customer
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Customers.ToArray());
        }

        // GET api/Customer/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            long idOfCustomer=(long)id;
            return Ok(_context.Customers.Find(idOfCustomer));
        }

        // PUT api/Customer/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Customer value)
        {
            long idOfCustomer = (long)id;
            if(idOfCustomer!=value.Id)
            return BadRequest();
            _context.Attach(value);
            _context.Entry(value).State=EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        // // DELETE api/Customer/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
