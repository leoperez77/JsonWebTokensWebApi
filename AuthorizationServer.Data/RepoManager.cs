using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationServer.Data
{ 
    public class RepoManager
    {
        public readonly IDataLayer DataLayer;

        public RepoManager(IDataLayer dataLayer)
        {
            if (dataLayer == null)
            {
                throw new ArgumentException("please pass an IDataLayer concrete implementation");
            }

            this.DataLayer = dataLayer;
        }
    }
}
