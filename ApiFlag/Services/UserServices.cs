using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiFlag.Models;
using ApiFlag.ResponseCode;
using ApiFlag.DB;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;

namespace ApiFlag.Services
{


    public class UserServices
    {
        public static string VL_Mensaje = "";



        public static string Code;
        private static string StaticUrlOauth = "https://www.facebook.com/dialog/oauth?client_id={{CODIGO_API}}&redirect_uri={{URL_DESTINO}}";
        private static string StaticOauthAcessToke = "https://graph.facebook.com/oauth/access_token?client_id={{CODIGO_API}}&redirect_uri={{URL_DESTINO}}&client_secret={{CODIGO_API_SECRETO}}&code={{CODIGO_RETORNO_USUARIO}}";
        private static string StaticMeAcessToken = "https://graph.facebook.com/me?access_token={{TOKEN_RETORNO}}&fields=gender,name,first_name,last_name,picture,email";



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

                   
                    pr.avatar = renglon["usr_avatar"].ToString();

                    if (pr.avatar == "" & pr.Sexo.ToString().ToLower() == "m")
                    {
                        pr.avatar = "av-1.png";
                    }
                    else if(pr.avatar == "" & pr.Sexo.ToString().ToLower() == "f")
                    {
                        pr.avatar = "av-5.png";
                    }


                    listprv.Add(pr);
                }
                    
                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                vrlResponse.RsContent = listprv;
                return vrlResponse;
            }
            else
            {
                vrlResponse.RsCode = CodeManager.CODE_100;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                //vrlResponse.RsContent = '';
                return vrlResponse;
            }
        }







        public static ResponseModel LogueoFacebook()
        {

            WebRequest request;
            WebResponse response;

            ResponseModel vrlResponse = new ResponseModel();

            //Monta url OauthAcessToke
            var OauthAcessToke = StaticOauthAcessToke;
            OauthAcessToke = OauthAcessToke.Replace("{{CODIGO_API}}", ConfigurationManager.AppSettings["AppId"]);
            OauthAcessToke = OauthAcessToke.Replace("{{URL_DESTINO}}", ConfigurationManager.AppSettings["UrlDestino"]);
            OauthAcessToke = OauthAcessToke.Replace("{{CODIGO_API_SECRETO}}", ConfigurationManager.AppSettings["AppSecret"]);
            OauthAcessToke = OauthAcessToke.Replace("{{CODIGO_RETORNO_USUARIO}}", Code);

            request = WebRequest.Create(OauthAcessToke);
            response = request.GetResponse();
            var retOauthAcessToke = new StreamReader(response.GetResponseStream()).ReadToEnd().Split('&')[0].Replace("access_token=", "");


            ClaseJson serFb = JsonConvert.DeserializeObject<ClaseJson>(retOauthAcessToke);


            //Monta url MeAcessToken
            var MeAcessToken = StaticMeAcessToken;
            MeAcessToken = MeAcessToken.Replace("{{TOKEN_RETORNO}}", serFb.access_token);

            // MeAcessToken = MeAcessToken.Replace("{{ID_USER}}", serFb.);

            request = WebRequest.Create(MeAcessToken);
            response = request.GetResponse();


            var dadosUsuario = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();
            //var dadosSerializer = new JavaScriptSerializer().Serialize(dadosUsuario);

            vrlResponse.RsContent = dadosUsuario;
            return vrlResponse;
        }









        public static ResponseModel RegistrarFacebook(RegisterUserModel usuario)
        {
            try
            {
                ResponseModel vrlResponse = new ResponseModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);
                int resultado;


                // Validamos si el usuario ingresado con el correo ya existe
                object respuestaSQL;

                respuestaSQL = VL_BD.EjecutarEscalarStoredProc(DBManager.dbflags, "sp_transaccion_usuario",new DBManager.ParametrosStoredP("@iOperacion","A",ParameterDirection.Input,DbType.String),
                    new DBManager.ParametrosStoredP("@iCorreo",usuario.Correo,ParameterDirection.Input,DbType.String));

                bool ExisteUsuario;

                ExisteUsuario = Convert.ToBoolean(respuestaSQL);





                

                if (usuario.avatar == null & usuario.Genero.ToString().ToLower() == "m")
                {
                    usuario.avatar = "av-1.png";
                }
                else if (usuario.avatar == null & usuario.Genero.ToString().ToLower() == "f")
                {
                    usuario.avatar = "av-5.png";
                }



                if (ExisteUsuario)
                {
                    //Si existe
                    vrlResponse.RsCode = CodeManager.CODE_100;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Ya existe una cuenta registrada con ese correo.";
                }
                else
                {
                    resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transaccion_usuario", "",
                    new DBManager.ParametrosStoredP("@iOperacion", "B", ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iNombre", usuario.Nombre, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iApellido", usuario.apellido, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iGenero", usuario.Genero, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iDireccion", usuario.Direccion, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iLatitud", usuario.Latitud, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iLongitud", usuario.Longitud, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iCorreo", usuario.Correo, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iPassword", usuario.clave, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iToken", usuario.Token, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iTipoLogin", usuario.TipoLogin, ParameterDirection.Input, DbType.String),
                    new DBManager.ParametrosStoredP("@iAvatar", usuario.avatar, ParameterDirection.Input, DbType.String));




                    if (resultado == 0)
                    {
                        vrlResponse.RsCode = CodeManager.CODE_200;
                        vrlResponse.RsMessage = CodeManager.DESC_10001;
                        vrlResponse.RsContent = "Creado correctamente";


                    }
                    else if (resultado == 100)
                    {
                        vrlResponse.RsCode = CodeManager.CODE_100;
                        vrlResponse.RsMessage = CodeManager.DESC_10001;
                        vrlResponse.RsContent = "Ya existe un registro con esa descripcion";
                    }

                }

            

          
                return vrlResponse;
            }
            catch (Exception ex)
            {

                throw new Exception("Error: " + ex.Message.ToString());
            }
        }

        public static string UrlOauth()
        {
            return StaticUrlOauth.Replace("{{CODIGO_API}}", ConfigurationManager.AppSettings["AppId"]).Replace("{{URL_DESTINO}}", ConfigurationManager.AppSettings["UrlDestino"]);
        }



    }
}