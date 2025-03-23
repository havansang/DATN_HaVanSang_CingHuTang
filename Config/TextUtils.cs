using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CingHuTang.Config
{
    public static class TextUtils
    {

        private static readonly IDictionary<Type,ICollection<PropertyInfo>> _Properties = new Dictionary<Type, ICollection<PropertyInfo>>();
        public class Status
        {
            public int key { get; set; }
            public string text { get; set; }

            public Status()
            {
                
            }

            public Status(int key, string text)
            {
                this.key = key;
                this.text = text;
            }
        }

    
        public static string ToString(object x)
        {
            try
            {
                return Convert.ToString(x);
            }
            catch
            {

                return "";
            }
        }


      
        public static int ToInt(object x)
        {
            try
            {
                return Convert.ToInt32(x);
            }
            catch
            {

                return 0;
            }
        }


        public static float ToFloat(object x)
        {
            try
            {
                return Convert.ToSingle(x);
            }
            catch
            {

                return 0;
            }
        }

        public static decimal ToDecimal(object x)
        {
            try
            {
                return Convert.ToDecimal(x);
            }
            catch
            {

                return 0;
            }
        }

        /// <summary>
        /// Convert object to bool
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool ToBoolean(object x)
        {
            try
            {
                return Convert.ToBoolean(x);
            }
            catch
            {

                return false;
            }
        }

        /// <summary>
        /// Convert object to Datetime VN
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static DateTime? ToDateVN(object x)
        {
            string date = "";
            if (x != null)
            {
                date = x.ToString();
            }
            try
            {
                try
                {
                    return DateTime.Parse(date, new CultureInfo("vi-VN", true));
                }
                catch
                {

                    return DateTime.Parse(date, new CultureInfo("fr-FR", true));
                }
            }
            catch
            {

                return null;
            }
        }

        /// <summary>
        /// Convert DataTable to List object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            try
            {
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = GetItem<T>(row);
                    data.Add(item);
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static T GetItem<T>(DataRow dr)
        {
            string columnName = "";
            try
            {
                Type temp = typeof(T);
                T obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in dr.Table.Columns)
                {
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name.ToUpper() == column.ColumnName.ToUpper())
                        {
                            columnName = column.ColumnName;
                            var value = Convert.IsDBNull(dr[column.ColumnName]) ? null : dr[column.ColumnName];
                            pro.SetValue(obj, value, null);
                        }

                        else
                        {
                            continue;
                        }
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + "\n" + columnName);
            }
        }

        public static void ExcuteProcedure(string storeProcedureName, string[] paramName, object[] paramValue)
        {
            SqlConnection cn = new SqlConnection(Config.Connection());
            try
            {
                cn = new SqlConnection(Config.Connection());
                SqlCommand cmd = new SqlCommand(storeProcedureName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                SqlParameter sqlParam;
                cn.Open();
                if (paramName != null)
                {
                    for (int i = 0; i < paramName.Length; i++)
                    {
                        sqlParam = new SqlParameter(paramName[i], paramValue[i]);
                        cmd.Parameters.Add(sqlParam);
                    }
                }
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cn.Close();
            }

        }
 
        public static void ExcuteSQL(string strSQL)
        {
            SqlConnection cn = new SqlConnection(Config.Connection());
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, cn);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cn.Open();
                cmd.CommandText = strSQL;
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cn.Close();
            }
        }
    }
}
