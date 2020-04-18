using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiFlag.Models;
using ApiFlag.ResponseCode;
using ApiFlag.DB;
using System.Data.SqlClient;

namespace ApiFlag.Services
{


    public class UserServices
    {
        public static string VL_Mensaje = "";

        public static ResponseModel VerifyModel(string usuario, string clave)
        {
            // Modelo de respuestas personalizadas
            ResponseModel vrlResponse = new ResponseModel();
            //usuarioModel jsonM = new usuarioModel();

           
            var VL_BD = new DBManager(DBManager.TAG_SQL);
            List<SqlParameter> VL_Parametros = new List<SqlParameter>();
            VL_Parametros.Add(DBManager.CreateCustomParameter("@operacion", SqlDbType.Int, "1"));
            VL_Parametros.Add(DBManager.CreateCustomParameter("@usr_email", SqlDbType.VarChar, usuario));
            VL_Parametros.Add(DBManager.CreateCustomParameter("@usr_clave", SqlDbType.VarChar, clave));
            var VL_DT = VL_BD.GetDataTableByNameSP(
                                                    DBManager.dbflags, 
                                                    DBManager.sp_transaccion, 
                                                    VL_Parametros, 
                                                    ref VL_Mensaje
                                                  );
            // FiltroParse filtroModel = new FiltroParse();

            //TODO: Validate credentials Correctly, this code is only for demo !!
            List<usuarioModel> listprv = new List<usuarioModel>();
            if (VL_DT.Rows.Count != 0)
            {
                var token = TokenGenerator.GenerateTokenJwt(usuario);
                foreach (DataRow renglon in VL_DT.Rows)
                {
                    usuarioModel pr = new usuarioModel();
                    pr.Nombre = renglon["Nombre"].ToString();
                    pr.Correo = renglon["usr_correo"].ToString();
                    pr.Direccion = renglon["usr_direccion"].ToString();
                    pr.Latitud = renglon["usr_latitud"].ToString();
                    pr.Longitud = renglon["usr_longitud"].ToString();
                    pr.Sexo = renglon["usr_sexo"].ToString();
                    pr.jwt = token.ToString();

                    listprv.Add(pr);
                }
                    
                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                vrlResponse.RsContent = listprv;
                return vrlResponse;
            }
            else
            {
                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                //vrlResponse.RsContent = '';
                return vrlResponse;
            }




        }

    }
}