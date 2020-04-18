using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using ApiFlag.Services;

namespace ApiFlag.DB
{
    public class DBManager
    {

        public readonly static string dbflags = "dbflags";
        public readonly static string TAG_SQL = ConfigurationManager.AppSettings["TAG_SQL"].ToString();
        public readonly static string sp_transaccion = "sp_transaccion";

        public static SqlConnection conn;

        string currentTag;

        public DBManager(string tag)
        {
            currentTag = tag;
        }

        /// <summary>
        /// crea cadena de conexion
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="dbname"></param>
        /// <param name="usernamedb"></param>
        /// <param name="passworddb"></param>
        /// <returns></returns>
        protected string cadena(string servername, string dbname, string usernamedb, string passworddb)
        {
            var cad = "Data Source=" + servername + ";Initial Catalog=" + dbname + ";Persist Security Info=True;User ID=" + usernamedb + ";Password=" + passworddb;
            return cad;
        }


        /// <summary>
        ///  Obtiene credenciales de para la conexion
        /// </summary>
        /// <param name="tag"></param>
        public void GetConnectionSQL2(string tag)
        {

            if (tag == "tag_dbflags")
            {

                string server = "DESKTOP-RTFCK87\\SQL2014";
                string dbname = "dbflags";
                string user = "sa";
                string pass = "1";

                conn = new SqlConnection(cadena(server, dbname, user, pass));
            }
        }

        /// <summary>
        /// Genera conexion y obtiene un data table de la consulta realizada
        /// </summary>
        /// <param name="pmNameDB"></param>
        /// <param name="pmNameSP"></param>
        /// <param name="pmParametros"></param>
        /// <param name="pmMensaje"></param>
        /// <returns></returns>
        public DataTable GetDataTableByNameSP(string pmNameDB, string pmNameSP, List<SqlParameter> pmParametros, ref string pmMensaje)
        {
            try
            {
                DataTable tblRes = new DataTable();

                GetConnectionSQL2(currentTag);
                conn.Open();
                conn.ChangeDatabase(pmNameDB);


                SqlCommand cmd = new SqlCommand(pmNameSP, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Clear();

                foreach (SqlParameter pm in pmParametros)
                {
                    cmd.Parameters.Add(pm);
                }


                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(tblRes);
                conn.Close();
                return tblRes;
            }
            catch (Exception ex)
            {
                LogService.WriteLine("GetDataTableByNameSP. " + ex.ToString());
                pmMensaje = ex.Message.ToString();
                return null;
            }

        }

        /// <summary>
        /// parsea parametros del sp
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tipo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter CreateCustomParameter(string name, SqlDbType tipo, object value)
        {
            var sqlParameter = new SqlParameter(name, tipo);
            sqlParameter.Value = value;
            return sqlParameter;

        }


        //fin clase
    }

    }
