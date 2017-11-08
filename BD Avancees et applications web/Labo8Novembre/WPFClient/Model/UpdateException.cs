using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Model
{
    public class UpdateException:Exception
    {
        public UpdateException(string message)
            :base("Une erreur s'est produite lors de la mise à jour: "+message)
        {

        }
    }
}
