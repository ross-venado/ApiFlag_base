using ApiFlag.DB;
using ApiFlag.Models;
using ApiFlag.ResponseCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ApiFlag.Services
{
    [Authorize]
    public class FlagService
    {

        public static string VL_Mensaje = "";


        public static ResponseModel CreateFlag(FlagModel flag)
        {
            try
            {
                ResponseModel vrlResponse = new ResponseModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);
                int resultado;

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_flag", "@oSalida",
               new DBManager.ParametrosStoredP("@iOperacion", "A", ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iDescripcion", flag.Descripcion, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iTitulo", flag.titulo, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@oSalida",0,ParameterDirection.Output,DbType.Int64));


             
                if (resultado==0)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Creado correctamente";


                }
                else if (resultado ==100)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Ya existe un registro con esa descripcion";
                }
                return vrlResponse;
            }
            catch (Exception ex)
            {

                throw new Exception("Error: " + ex.Message.ToString());
            }
        }








        public static ResponseModel UpdateFlag(int id,FlagModel flag)
        {
            try
            {
                ResponseModel vrlResponse = new ResponseModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);
                int resultado;

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_flag", "@oSalida",
               new DBManager.ParametrosStoredP("@iOperacion", "B", ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iID",id,ParameterDirection.Input,DbType.Int64),
               new DBManager.ParametrosStoredP("@iEstado", flag.estado, ParameterDirection.Input, DbType.Int64),               
               new DBManager.ParametrosStoredP("@iDescripcion", flag.Descripcion, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@oSalida", 0, ParameterDirection.Output, DbType.Int64));



                if (resultado == 0)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Actualizado correctamente";


                }
                else if (resultado==200)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "No existe ninguna bandera con ese ID";
                }


                return vrlResponse;
            }
            catch (Exception ex)
            {

                throw new Exception("Error: " + ex.Message.ToString());
            }
        }











        public static ResponseModel DeleteFlag(int id)
        {
            try
            {
                ResponseModel vrlResponse = new ResponseModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);
                int resultado;

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_flag", "@oSalida",
               new DBManager.ParametrosStoredP("@iOperacion", "C", ParameterDirection.Input, DbType.String),               
               new DBManager.ParametrosStoredP("@iID",id, ParameterDirection.Input, DbType.Int64),
               new DBManager.ParametrosStoredP("@oSalida", 0, ParameterDirection.Output, DbType.Int64));



                if (resultado == 0)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Eliminado correctamente";


                }
                else if (resultado == 300)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "No existe ninguna bandera con ese ID";
                }
                return vrlResponse;
            }
            catch (Exception ex)
            {

                throw new Exception("Error: " + ex.Message.ToString());
            }
        }





        public static ResponseModel SelectFlags(int id = 0)
        {

            try
            {
                DataSet dsFlags;

                // Modelo de respuestas personalizadas
                ResponseModel vrlResponse = new ResponseModel();
                //usuarioModel jsonM = new usuarioModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);

                dsFlags = new DataSet();

                if (id==0)
                {
                    dsFlags = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_flag",
                                new DBManager.ParametrosStoredP("@iOperacion", "D", ParameterDirection.Input, DbType.String),
                                new DBManager.ParametrosStoredP("@oSalida", 0, ParameterDirection.Output, DbType.Int64));
                }
                else 
                {
                    dsFlags = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_flag",
                                new DBManager.ParametrosStoredP("@iOperacion", "E", ParameterDirection.Input, DbType.String),
                                new DBManager.ParametrosStoredP("@iID", id, ParameterDirection.Input, DbType.Int64),
                                new DBManager.ParametrosStoredP("@oSalida", 0, ParameterDirection.Output, DbType.Int64));
                }



                if (dsFlags.Tables.Count > 0 )
                {
                    List<FlagModel> ListaFlags = new List<FlagModel>();
                    foreach (DataRow item in dsFlags.Tables[0].Rows)
                    {
                        FlagModel flag = new FlagModel();

                        flag.id = int.Parse(item["flag_id"].ToString());
                        flag.Descripcion = item["flag_desc"].ToString();
                        flag.titulo = item["flag_title"].ToString();
                        flag.estado = int.Parse(item["flag_estado"].ToString());

                        ListaFlags.Add(flag);
                    }

                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = ListaFlags;
                    


                }
                else
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "No existe informacion para mostrar";
                }


                return vrlResponse;

            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message.ToString());
            }
        }




    }
}