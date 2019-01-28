using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SE.PopCom.DataAccess
{
    public class DataAccessBase
    {
        private static string _DBConnString = string.Empty;

        protected IDbConnection Database
        {
            get
            {
                if(string.IsNullOrEmpty(_DBConnString))
                {
                    
                }
                return new SqlConnection(_DBConnString);
            }
        }

        public virtual void InitConnString(string connString)
        {
            _DBConnString = connString;
        }
    }

    public static class DataAccessFactory
    {
        public static IHost UseInitDataAccessBase(this IHost host, string connString)
        {
            DataAccessBase da = (DataAccessBase)host.Services.GetService(typeof(DataAccessBase));
            da.InitConnString(connString);
            return host;
        }
    }
}
