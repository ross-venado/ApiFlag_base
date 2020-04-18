using ApiFlag.DB;
using ApiFlag.Models;
using ApiFlag.ResponseCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ApiFlag.Services
{
    [Authorize]
    public class FlagService
    {



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








        public static ResponseModel UpdateFlag(FlagModel flag)
        {
            try
            {
                ResponseModel vrlResponse = new ResponseModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);
                int resultado;

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_flag", "@oSalida",
               new DBManager.ParametrosStoredP("@iOperacion", "B", ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iID",flag.id,ParameterDirection.Input,DbType.Int64),
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











        public static ResponseModel DeleteFlag(FlagModel flag)
        {
            try
            {
                ResponseModel vrlResponse = new ResponseModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);
                int resultado;

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_flag", "@oSalida",
               new DBManager.ParametrosStoredP("@iOperacion", "C", ParameterDirection.Input, DbType.String),               
               new DBManager.ParametrosStoredP("@iID", flag.id, ParameterDirection.Input, DbType.Int64),
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





    }
}