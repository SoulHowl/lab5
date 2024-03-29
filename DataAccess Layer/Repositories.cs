﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErrorLogger;
using Modelss;
using Modelss.MModels;

namespace DataAccess_Layer
{
    public class Repository:IFiller<PersonalInfo>
    {
        private DALconfig props;

        public Repository(string connectionStringg,int num)
        {
            props=new DALconfig(connectionStringg,num);
        }

        public async Task<SearchRes<PersonalInfo>> GetPersonsAsync()
        {
            return await Task.Run(async() =>
            {
                using (var connection = new SqlConnection(props.connectionString))
                {
                    var findres = new SearchRes<PersonalInfo>();
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        SqlCommand command = new SqlCommand();
                        command.Transaction = transaction;
                        command.Connection = connection;
                        command.Parameters.AddWithValue("@Start", (int) props.criteria.start);
                        command.Parameters.AddWithValue("@Count", (int) props.criteria.count);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = props.StoredFunction;
                        command.CommandTimeout = 45;
                        var outputparam = new SqlParameter("@Total", SqlDbType.Int);
                        outputparam.Direction = ParameterDirection.Output;
                        command.Parameters.Add(outputparam);
                        var entities = await Task.Run(() => command.ReadAll<PersonalInfo>());

                        transaction.Commit();
                        findres.Entities = entities;
                        findres.Total = Convert.ToInt32(outputparam.Value);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        IErLogger logger = new Logger();
                        await Task.Run(() => logger.WriteErrorAsync(ex));
                    }
                    finally
                    {
                        connection.Close();
                    }

                    return findres;
                }
            });
        }
    }
}
