using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ApiFlag.ResponseCode
{
/// <summary>
/// Clase guarda el listado de errores Controlados.
/// </summary>
    public class CodeManager
    {
        // request Respuestas.
        public string ReasonPhrase { get; private set; }
        public HttpRequestMessage Request { get; private set; }

        public static readonly string CODE_200 = "200";
        public static readonly string CODE_100 = "100";
        public static readonly string MSG_User_200 = "Error al conectar al servicio, Por favor contacte al Adminsitrador del Sistema";

        public static readonly string CODE_10001 = "10001";
        public static readonly string DESC_10001 = "Respuesta Procesada con Exito";
        public static readonly string CODE_10002 = "10002";
        public static readonly string DESC_10002 = "OPERACION NO PROCESADA";
        public static readonly string CODE_10003 = "10003";
        public static readonly string DESC_10003 = "LA CLAVE SECRETA NO CUMPLE LOS REQUISITOS";
        public static readonly string CODE_10018 = "10018";
        public static readonly string DESC_10018 = "Problema en la autenticación con las credenciales proporcionadas. Intente de nuevo, por favor.";

        
    }
}