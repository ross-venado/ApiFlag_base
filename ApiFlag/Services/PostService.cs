using ApiFlag.DB;
using ApiFlag.Models;
using ApiFlag.ResponseCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace ApiFlag.Services
{
    public class PostService
    {
        public static string VL_Mensaje = "";


        public static ResponseModel CreatePost(PostModel post)
        {
            try
            {
                ResponseModel vrlResponse = new ResponseModel();
                var VL_BD = new DBManager(DBManager.TAG_SQL);
                int resultado;

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_post", "",
               new DBManager.ParametrosStoredP("@iOperacion", "A", ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iMensaje", post.post_mensaje, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iFlag", post.id_flag, ParameterDirection.Input, DbType.Int64),
               new DBManager.ParametrosStoredP("@iUsuario", post.id_usuario, ParameterDirection.Input, DbType.Int64),
               new DBManager.ParametrosStoredP("@iLatitud", post.post_latitud, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iLongitud", post.post_longitud, ParameterDirection.Input, DbType.String));



                if (resultado == 0)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Creado correctamente";


                }
                else
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Ocurrio un problema";
                }
                return vrlResponse;
            }
            catch (Exception ex)
            {

                throw new Exception("Error: " + ex.Message.ToString());
            }
        }







        public static ResponseModel Posts(int id = 0)
        {
            ResponseModel vrlResponse = new ResponseModel();
            var VL_BD = new DBManager(DBManager.TAG_SQL);

            DataSet dsPost;

            dsPost = new DataSet();

            if (id==0)
            {
                dsPost = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_post",
                                       new DBManager.ParametrosStoredP("@iOperacion", "E", ParameterDirection.Input, DbType.String));
            }
            else
            {
                dsPost = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_post",
                                   new DBManager.ParametrosStoredP("@iOperacion", "D", ParameterDirection.Input, DbType.String),
                                   new DBManager.ParametrosStoredP("@iUsuario",id,ParameterDirection.Input,DbType.Int16));
            }









            if (dsPost.Tables.Count > 0)
            {
                List<PostModel> ListaPosts = new List<PostModel>();
                foreach (DataRow item in dsPost.Tables[0].Rows)
                {
                    PostModel post = new PostModel();          
                    post.post_id = int.Parse(item["post_id"].ToString());
                    post.post_mensaje = item["post_mensaje"].ToString();
                    post.id_flag = int.Parse(item["id_flag"].ToString());
                    post.id_usuario = int.Parse(item["id_usuario"].ToString());
                    post.post_ts = DateTime.Parse( item["post_ts"].ToString());
                    post.post_latitud = item["post_latitud"].ToString();
                    post.post_longitud = item["post_longitud"].ToString();
                    post.post_estado = int.Parse(item["post_estado"].ToString());
                    post.nombre_usuario = item["nombre_usuario"].ToString();
                    // post.image = item["rec_url_img"].ToString();





                    DataSet dsRecursos;

                    dsRecursos = new DataSet();

                    dsRecursos = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_img",
                        new DBManager.ParametrosStoredP("@iOperacion", "B", ParameterDirection.Input, DbType.String),
                        new DBManager.ParametrosStoredP("@iPost", post.post_id, ParameterDirection.Input, DbType.Int64));

                    
                    List<ImageModel> ListaImages = new List<ImageModel>();

                    if (dsRecursos.Tables[0].Rows.Count > 0)
                    {
                        
                        foreach (DataRow item2 in dsRecursos.Tables[0].Rows)
                        {
                            ImageModel img = new ImageModel();

                            img.image_id = int.Parse(item2["rec_id"].ToString());
                            img.url_image = "http://18.216.107.69/AppBanderas/ImagesUpload/" + item2["rec_url_img"].ToString();

            


                            ListaImages.Add(img);

                        }

                    }



                    post.image = ListaImages;








                    ListaPosts.Add(post);
                }

                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                vrlResponse.RsContent = ListaPosts;



            }
            else
            {
                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                vrlResponse.RsContent = "No existe informacion para mostrar";
            }




            return vrlResponse;
        }












        public static string GetMappedPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                if (!Path.IsPathRooted(path))
                {
                    // We are about to call MapPath, so need to ensure that 
                    // we do not pass an absolute path.
                    // 
                    // We use HostingEnvironment.MapPath, rather than 
                    // Server.MapPath, to allow this method to be used
                    // in application startup. Server.MapPath calls 
                    // HostingEnvironment.MapPath internally.
                    return HostingEnvironment.MapPath(path);
                }
                else
                {
                    return path;
                }
            }
            else
            {
                throw new ApplicationException(
                        "I'm not in an ASP.NET hosted environment :-(");
            }
        }


    }
}