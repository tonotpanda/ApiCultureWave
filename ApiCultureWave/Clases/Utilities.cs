using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ApiCultureWave.Clases
{
    public static class Utilities
    {
        public static string GetErrorMessage(SqlException sqlException)
        {
            string message = "";
            switch (sqlException.Number)
            {
                case 2:
                    message = "El servidor no está operativo";
                    break;
                case 53:
                    message = "No hay conexión con el servidor de bases de datos";
                    break;
                case 547:
                    message = "El registro tiene datos relacionados";
                    break;
                case 2601:
                    message = "Registro duplicado";
                    break;
                case 2627:
                    message = "Registro duplicado";
                    break;
                case 4060:
                    message = "No se puede abrir la base de datos";
                    break;
                case 18456:
                    message = "Error al iniciar la sesión";
                    break;
                default:
                    message = sqlException.Number + "__" + sqlException.Message;
                    break;
            }
            return message;
        }
    }
}