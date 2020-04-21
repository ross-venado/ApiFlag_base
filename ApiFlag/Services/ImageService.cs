using ApiFlag.DB;
using ApiFlag.Models;
using ApiFlag.ResponseCode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;

namespace ApiFlag.Services
{
    public class ImageService
    {

        private static string sNombreImagen;
        private static string idUsuario;


        private static string extension;
        private static  string idPost;

        public static async System.Threading.Tasks.Task<ResponseModel> UploadImageAsync(HttpRequestMessage request)
        {
            ResponseModel vrlResponse = new ResponseModel();
            var VL_BD = new DBManager(DBManager.TAG_SQL);

            var ctx = HttpContext.Current;
            var root = ctx.Server.MapPath("~/ImagesUpload");
            var provider = new MultipartFormDataStreamProvider(root);

            object IdRecurso;
       
            try
            {
                await request.Content.ReadAsMultipartAsync(provider);


                // REcuperamos la informacion del body - Form-Data
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        // Console.WriteLine(string.Format("{0}: {1}", key, val));
                        if (key=="id_usuario")
                        {
                            idUsuario = val;
                        }
                        else if (key=="id_post")
                        {
                            idPost = val;
                            
                        }
                    }
                }

                IdRecurso = VL_BD.EjecutarEscalar(DBManager.dbflags, "SELECT coalesce(MAX(rec_id), 0) + 1 AS MaxX FROM recursos");

                foreach (var file in provider.FileData)
                {
                    var name = file.Headers.ContentDisposition.FileName;
                    name = name.Trim('"');
                    var ext = GetFileExtension(name);

                    var localFileName = file.LocalFileName;
                    extension = System.IO.Path.GetExtension(localFileName);
                    sNombreImagen = idPost + "-" + idUsuario + "-" + IdRecurso + "-" + DateTime.Now.ToString("yyyyMMdd") + ext;
                    var filePath = Path.Combine(root, sNombreImagen);
                    File.Move(localFileName, filePath);

                    
                }



                int resultado;

                resultado = VL_BD.EjecutarStoredProc(DBManager.dbflags, "sp_transacciones_img", "",
               new DBManager.ParametrosStoredP("@iOperacion", "A", ParameterDirection.Input, DbType.String),
               new DBManager.ParametrosStoredP("@iPost", idPost, ParameterDirection.Input, DbType.Int64),
               new DBManager.ParametrosStoredP("@iUrlImg", sNombreImagen, ParameterDirection.Input, DbType.String));               



                if (resultado == 0)
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Imagen cargada correctamente";


                }
                else
                {
                    vrlResponse.RsCode = CodeManager.CODE_200;
                    vrlResponse.RsMessage = CodeManager.DESC_10001;
                    vrlResponse.RsContent = "Ocurrio un problema";
                }

            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message.ToString());
            }

            return vrlResponse;
        }











        public static HttpResponseMessage GetImage(int idpost)
        {
            HttpResponseMessage vrlResponse = new HttpResponseMessage();
            var VL_BD = new DBManager(DBManager.TAG_SQL);


            DataSet dsRecursos;

            dsRecursos = new DataSet();

            dsRecursos = VL_BD.LlenarDatasetStoredProc(DBManager.dbflags, "sp_transacciones_img",
                new DBManager.ParametrosStoredP("@iOperacion", "B", ParameterDirection.Input, DbType.String),
                new DBManager.ParametrosStoredP("@iPost", idpost, ParameterDirection.Input, DbType.Int64));

            if (dsRecursos.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow item in dsRecursos.Tables[0].Rows)
                {
                    string ruta;

                    ruta = item["rec_url_img"].ToString();



                    var result = new HttpResponseMessage(HttpStatusCode.OK);
                    String filePath = HostingEnvironment.MapPath("~/ImagesUpload/"+ruta);
                    FileStream fileStream = new FileStream(filePath, FileMode.Open);
                    Image image = Image.FromStream(fileStream);
                    MemoryStream memoryStream = new MemoryStream();
                    image.Save(memoryStream, ImageFormat.Jpeg);
                    result.Content = new ByteArrayContent(memoryStream.ToArray());
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");


                    vrlResponse = result;

                    fileStream.Close();

                }



                //return vrlResponse;


                //var result = new HttpResponseMessage(HttpStatusCode.OK);
                //String filePath = HostingEnvironment.MapPath("~/ImagesUpload/1lado.jpg");
                //FileStream fileStream = new FileStream(filePath, FileMode.Open);
                //Image image = Image.FromStream(fileStream);
                //MemoryStream memoryStream = new MemoryStream();
                //image.Save(memoryStream, ImageFormat.Jpeg);
                //result.Content = new ByteArrayContent(memoryStream.ToArray());
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                








            }










            return vrlResponse;

        }



        private static string Concatena(string idpost,  string idusuario, string idRecurso )
        {
            string nombre;
            nombre = idpost + idusuario + idRecurso;

            return nombre;
        }


        public static string GetFileExtension( string fileName)
        {
            string ext = string.Empty;
            int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
            if (fileExtPos >= 0)
                ext = fileName.Substring(fileExtPos, fileName.Length - fileExtPos);

            return ext;
        }


    }
}