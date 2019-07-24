using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADInventory
{
   public class SoftwareInventory
    {
        public List<String> GetSoftwareInventory(InventoryContext context)
        {
            //use wmic or powershell or remote registry to connect to context.Host
            String host = context.RemoteHost;

            return new List<String>();

        }
    }
}
