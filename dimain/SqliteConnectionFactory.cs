﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dimain
{
    //public class SQLiteConnectionFactory : IDbConnectionFactory
    //{
    //    public DbConnection CreateConnection(string nameOrConnectionString)
    //    {
    //        //System.Data.SQLite.EF6.SQLiteProviderFactory.Instance.CreateConnection();
    //        return new SQLiteConnection("Data Source=:memory:;Version=3;New=True;");
    //    }
    //}

    //public class SQLiteConfiguration : DbConfiguration
    //{
    //    public SQLiteConfiguration()
    //    {
    //        SetDefaultConnectionFactory(new SQLiteConnectionFactory());
    //        SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
    //        SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
    //        Type t = Type.GetType("System.Data.SQLite.EF6.SQLiteProviderServices, System.Data.SQLite.EF6");
    //        FieldInfo fi = t.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static);
    //        SetProviderServices("System.Data.SQLite", (DbProviderServices)fi.GetValue(null));
    //    }
    //}
}
