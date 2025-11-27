using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Front.Data__bd_;

namespace Front.SignosVitales
{
    class SERVICIOS
    {
        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnHealthyU"].ConnectionString);
        }
    }
}
