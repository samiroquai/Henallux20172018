using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapidemo2.Model;
using System.Data.SqlClient;
using System.Data;

namespace webapidemo2.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<Client> Get()
        {
            List<Client> clients=new List<Client>();
            using(var connection=GetConnection())
            {
                connection.Open();

                    var command=new SqlCommand("SELECT Id, Nom, Adr_Pays, Adr_CodePostal, Adr_Localite, Adr_Rue, EMail FROM Client ORDER BY 1",connection);
                    SqlDataReader reader=command.ExecuteReader();
                    while(reader.Read())
                    {
                        clients.Add(new Client(){
                            Id=reader.GetInt32(0),
                            Name=reader.GetString(1),
                            EMail=reader.GetString(6),
                            Address=new Address()
                                {
                                Country=reader.GetString(2),
                                PostalCode=reader.GetString(3),
                                Locality=reader.GetString(4),
                                Street=reader.GetString(5)
                                }
                        });
                    }
                    reader.Close();
                connection.Close();
            }
            return clients;
        }

        private SqlConnection GetConnection(){
            return new SqlConnection(@"");
        }
        // // GET api/values/5
        // [HttpGet("{id}")]
        // public string Get(int id)
        // {
        //     return "value";
        // }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Client nouveauClient)
        {
            if(ModelState.IsValid){

                using(var connection=GetConnection())
                {
                    connection.Open();
                    SqlCommand command=new SqlCommand("INSERT INTO Client (Nom, EMail, Adr_Rue, Adr_CodePostal, Adr_Localite, Adr_Pays,  Adr_Rue_1, Adr_CodePostal_1, Adr_Localite_1, Adr_Pays_1) VALUES (@nom, @email, @rue, @codePostal, @localite, @pays, @rue, @codePostal, @localite, @pays); SELECT SCOPE_IDENTITY(); ",connection);
                    command.Parameters.AddWithValue("@nom",nouveauClient.Name);
                    command.Parameters.AddWithValue("@email",nouveauClient.EMail);
                    command.Parameters.AddWithValue("@rue",nouveauClient.Address.Street);
                    command.Parameters.AddWithValue("@codePostal", nouveauClient.Address.PostalCode);
                    command.Parameters.AddWithValue("@localite", nouveauClient.Address.Locality);
                    command.Parameters.AddWithValue("@pays", nouveauClient.Address.Country);
                    var idOfNewCustomer=(decimal)command.ExecuteScalar();
                    nouveauClient.Id=(int)idOfNewCustomer;
                    return Created("/api/Client/"+nouveauClient.Id,nouveauClient);        
                }
                
            }else{
                return BadRequest(ModelState);
            }
        }

        // // PUT api/values/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody]string value)
        // {
        // }

        // // DELETE api/values/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}
