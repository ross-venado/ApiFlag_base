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
        private SqlCommand query;

        private SqlCommand queryStoredProc = new SqlCommand(); // se crea una nueva variable para no entorpecer el utilizado

        public static SqlConnection conn;
        private SqlDataAdapter daDatos;

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
            else if (tag== "tag_sc")
            {
                string server = "localhost";
                string dbname = "dbflags";
                string user = "devsc";
                string pass = "Clavesecreta123!";

                conn = new SqlConnection(cadena(server, dbname, user, pass));
            }
            else if(tag== "tag_app")
            {
                string server = "3.133.147.160";
                string dbname = "dbflags";
                string user = "usrapp";
                string pass = "Appsecreto1!";

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



        public class ParametrosStoredP
        {
            public string nombreParametro = string.Empty;
            public object valorParametro;
            public ParameterDirection direccionParametro;
            public DbType tipoValor;
            public ParametrosStoredP(string _nombreParametro, object _valorParametro, ParameterDirection _direccionParametro, DbType _tipoValor)
            {
                nombreParametro = _nombreParametro;
                valorParametro = _valorParametro;
                direccionParametro = _direccionParametro;
                tipoValor = _tipoValor;
            }
        }






        public  int  EjecutarStoredProc(string pmNameDB, string _nombreStoredProc, string psalida, params ParametrosStoredP[] parametros)
        {
            int tmpEstadoConexion = 0;
            int val;
            try
            {


                GetConnectionSQL2(currentTag);
                conn.Open();
                conn.ChangeDatabase(pmNameDB);



                SqlCommand queryStoredProc = new SqlCommand(_nombreStoredProc, conn);


                // regularmente...
                // si la conexion esta cerrada entonces la abrimos...
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    tmpEstadoConexion = 1;
                }

               // queryStoredProc.CommandText = _nombreStoredProc; // le enviamos el nombre del stored proc....
                queryStoredProc.CommandType = CommandType.StoredProcedure; // Asignamos el tipo stored proc...

                // ahora ejecutamos el query...
                //queryStoredProc.CommandTimeout = _timeOutStoredProc;

                SqlParameter tmpParametro;
                // agregamos todos los parametros 
                foreach (ParametrosStoredP elemento in parametros)
                {
                    {
                        var withBlock = elemento;
                        tmpParametro = new SqlParameter(withBlock.nombreParametro, withBlock.valorParametro);
                        tmpParametro.Direction = withBlock.direccionParametro;
                        tmpParametro.DbType = withBlock.tipoValor;
                    }
                    queryStoredProc.Parameters.Add(tmpParametro);
                }


                queryStoredProc.ExecuteNonQuery();


                if (psalida.Length>0)
                {
                    object o = queryStoredProc.Parameters[psalida].Value;

                    val = int.Parse(o.ToString());
                    
                }
                val = 0;
                return val;
            }
            catch (Exception ex)
            {            
                throw new InvalidOperationException(ex.Message);
                throw ex;
            }
            finally
            {
                // si la conexion estaba cerrada entonces la cerramos...
                if (tmpEstadoConexion == 1)
                    conn.Close();
            }
        }




        public DataSet LlenarDatasetStoredProc(string pmNameDB, string _nombreStoredProc, params ParametrosStoredP[] parametros)
        {
            int tmpEstadoConexion = 0;

            try
            {
                DataSet dstemporal = new DataSet();
                GetConnectionSQL2(currentTag);
                conn.Open();
                conn.ChangeDatabase(pmNameDB);

                SqlCommand queryStoredProc = new SqlCommand(_nombreStoredProc, conn);


                // si la conexion esta cerrada entonces la abrimos...
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    tmpEstadoConexion = 1;
                }

                
                queryStoredProc.CommandType = CommandType.StoredProcedure; // Asignamos el tipo stored proc...

                

                SqlParameter tmpParametro;
                // agregamos todos los parametros 
                foreach (ParametrosStoredP elemento in parametros)
                {
                    {
                        var withBlock = elemento;
                        tmpParametro = new SqlParameter(withBlock.nombreParametro, withBlock.valorParametro);
                        tmpParametro.Direction = withBlock.direccionParametro;
                        tmpParametro.DbType = withBlock.tipoValor;
                    }
                    queryStoredProc.Parameters.Add(tmpParametro);
                }

                // le asignamos el query de seleccion y llenamos el dataset...
                // daDatos.SelectCommand = queryStoredProc;

                daDatos = new SqlDataAdapter(queryStoredProc);
                daDatos.Fill(dstemporal);

                return dstemporal;
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException(ex.Message);
                throw ex;
            }
            finally
            {
                // si la conexion estaba cerrada entonces la cerramos...
                if (tmpEstadoConexion == 1)
                    conn.Close();
                // LimpiarQueryStored_CommandText();
            }
        }




        public object EjecutarEscalar(string pmNameDB, string _squery)
        {
            int tmpEstadoConexion = 0;
            try
            {
                GetConnectionSQL2(currentTag);
                conn.Open();
                conn.ChangeDatabase(pmNameDB);

                // si la conexion esta cerrada entonces la abrimos...
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    tmpEstadoConexion = 1;
                }

                object numero;
                // traemos el dato solicitado...

                SqlCommand query = new SqlCommand(_squery, conn);

                // query.CommandText = _squery;

                
                numero = query.ExecuteScalar();
                return numero;
                // EjecutarEscalar = query.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
                throw ex;
            }
            finally
            {
                // si la conexion estaba cerrada entonces la cerramos...
                if (tmpEstadoConexion == 1)
                    conn.Close();                
            }
        }




        private void LimpiarQueryStored_CommandText()
        {
            query.CommandText = string.Empty;
        }

        //fin clase
    }

}
