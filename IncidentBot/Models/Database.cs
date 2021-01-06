// <copyright file="Database.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.IncidentBot.Models
{
    using System;
    using System.Text;
    using Microsoft.Data.SqlClient;

    /// <summary>
    /// COnectar a la BDD y guardar info.
    /// </summary>
    public class Database
    {
        /// <summary>
        /// Get info from the DB.
        /// </summary>
        public void GetClasses()
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "ngg-dm-sql-server.database.windows.net";
                builder.UserID = "sqladmin";
                builder.Password = "Ng4g3-5q1-dtpd4+U7";
                builder.InitialCatalog = "ngg-dm-sqldb-database";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    Console.WriteLine("\nQuery data example:");
                    Console.WriteLine("=========================================\n");

                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT TOP 20 * ");
                    sb.Append("FROM [dbo].[Class] ");
                    string sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
                            }
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.ReadLine();
        }
    }
}
