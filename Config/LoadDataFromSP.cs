using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CingHuTang.Config
{
    public class LoadDataFromSP
    {

        private static string connectionString = Config.Connection();
        public static DataTable GetDataTableSP(string commandText, string[] param, object[] valueParam)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                var dt = new DataTable();
                SqlParameter sqlParam;
                SqlCommand cmd = new SqlCommand(commandText, conn);
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        sqlParam = new SqlParameter(param[i], valueParam[i]);
                        cmd.Parameters.Add(sqlParam);
                    }
                }
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.CommandTimeout = 10;
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataSet GetDataSetSP(string commandText, string[] param, object[] valueParam)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                DataSet dataSet = new DataSet();
                SqlParameter sqlParam;
                SqlCommand cmd = new SqlCommand(commandText, conn);
                if (param != null)
                {
                    for (int i = 0; i < param.Length; i++)
                    {
                        sqlParam = new SqlParameter(param[i], valueParam[i]);
                        cmd.Parameters.Add(sqlParam);
                    }
                }

                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dataSet);
                conn.Close();
                return dataSet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        public static int Delete(string commandText)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                
                SqlCommand cmd = new SqlCommand(commandText, conn);
                cmd.CommandType = CommandType.Text;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
