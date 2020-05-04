using ApiFlag.DB;
using ApiFlag.Models;
using ApiFlag.ResponseCode;
using Newtonsoft.Json;
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

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_post", "@oSalida",
               new DBManager.ParametrosStoredP("@iOperacion", "A", ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iMensaje", post.post_mensaje, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iFlag", post.id_flag, ParameterDirection.Input, DbType.Int64),
               new DBManager.ParametrosStoredP("@iUsuario", post.id_usuario, ParameterDirection.Input, DbType.Int64),
               new DBManager.ParametrosStoredP("@iLatitud", post.post_latitud, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iLongitud", post.post_longitud, ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@oSalida", 0, ParameterDirection.Output, DbType.Int64));



                if (resultado != 0)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    // vrlResponse.RsContent = "Creado correctamente";
                    vrlResponse.RsContent = resultado;


                }
                else
                {
                    vrlResponse.RsCode = CodeManager.CODE_100;
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







        public static ResponseModel Posts(PagingParameterModel pagin, int id = 0)
        {
            ResponseModel vrlResponse = new ResponseModel();
            var VL_BD = new DBManager(DBManager.TAG_SQL);

            
            DataSet dsPost;

            dsPost = new DataSet();

            if (id==0)
            {
                dsPost = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_post",
                                       new DBManager.ParametrosStoredP("@iOperacion", "E", ParameterDirection.Input, DbType.String),
                                       new DBManager.ParametrosStoredP("@oSalida", 0, ParameterDirection.Output, DbType.Int64));
            }
            else
            {
                dsPost = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_post",
                                   new DBManager.ParametrosStoredP("@iOperacion", "D", ParameterDirection.Input, DbType.String),
                                   new DBManager.ParametrosStoredP("@iUsuario",id,ParameterDirection.Input,DbType.Int16),
                                   new DBManager.ParametrosStoredP("@oSalida", 0, ParameterDirection.Output, DbType.Int64));
            }








            if (dsPost.Tables[0].Rows.Count > 0)
            {
                List<PostModel> ListaPosts = new List<PostModel>();
                foreach (DataRow item in dsPost.Tables[0].Rows)
                {
                    PostModel post = new PostModel();
                    post.post_id = int.Parse(item["post_id"].ToString());
                    post.post_mensaje = item["post_mensaje"].ToString();
                    post.id_flag = int.Parse(item["id_flag"].ToString());
                    post.id_usuario = int.Parse(item["id_usuario"].ToString());
                    post.post_ts = DateTime.Parse(item["post_ts"].ToString());
                    post.post_latitud = item["post_latitud"].ToString();
                    post.post_longitud = item["post_longitud"].ToString();
                    post.post_estado = int.Parse(item["post_estado"].ToString());
                    post.nombre_usuario = item["nombre_usuario"].ToString();
                    post.avatar = item["usr_avatar"].ToString();
                    // post.image = item["rec_url_img"].ToString();





                    DataSet dsRecursos;
                    dsRecursos = new DataSet();

                    dsRecursos = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_img",
                        new DBManager.ParametrosStoredP("@iOperacion", "B", ParameterDirection.Input, DbType.String),
                        new DBManager.ParametrosStoredP("@iPost", post.post_id, ParameterDirection.Input, DbType.Int64));

                    // Recorremos las imagenes por cada post
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


                    //var source = dsPost.Tables[0].AsEnumerable()
                    // .Select(dataRow => new PostModel
                    // {
                    //     post_id = dataRow.Field<int>("post_id"),
                    //     post_mensaje = dataRow.Field<string>("post_mensaje"),
                    //     id_flag = dataRow.Field<int>("id_flag"),
                    //     id_usuario = dataRow.Field<int>("id_usuario"),
                    //     nombre_usuario = dataRow.Field<string>("nombre_usuario"),
                    //     post_ts = dataRow.Field<DateTime>("post_ts"),
                    //     post_latitud = dataRow.Field<string>("post_latitud"),
                    //     post_longitud = dataRow.Field<string>("post_longitud"),
                    //     post_estado = dataRow.Field<int>("post_estado")
                    // }).ToList();
                    post.image = ListaImages;

                    ListaPosts.Add(post);
                }


                int count = ListaPosts.Count;
                int CurrentPage = pagin.pageNumber;
                int PageSize = pagin.pageSize;
                int TotalCount = count;
                int TotalPages = (int)Math.Ceiling(count / (double)PageSize);
                var items = ListaPosts.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                var previousPage = CurrentPage > 1 ? "Yes" : "No";
                var nextPage = CurrentPage < TotalPages ? "Yes" : "No";




                var paginationMetadata = new
                {
                    totalCount = TotalCount,
                    pageSize = PageSize,
                    currentPage = CurrentPage,
                    totalPages = TotalPages,
                    previousPage,
                    nextPage
                };

                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                vrlResponse.RsContent = items;
            }
            else
            {
                vrlResponse.RsCode = CodeManager.CODE_100;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                vrlResponse.RsContent = "No existe informacion para mostrar";
            }


            
            // HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));


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




        public static ResponseModel ValidaToken()
        {
            ResponseModel vrlResponse = new ResponseModel();
            try
            {
                

                vrlResponse.RsContent = true;

                vrlResponse.RsCode = CodeManager.CODE_200;
                vrlResponse.RsMessage = CodeManager.DESC_10001;
                



                return vrlResponse;
            }
            catch (Exception ex)
            {
                vrlResponse.RsContent = ex.Message.ToString();
                throw;
            }
        }


    }
}