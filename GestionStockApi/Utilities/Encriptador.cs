using System.Security.Cryptography;
using System.Text;

namespace GestionStockApi.Uitilities
{
    public class Encriptador
    {
        /// Realiza la encriptación del texto utilizando el algoritmo SHA256.
        public static string Encriptar(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                throw new ArgumentException("El input no puede ser Null o Empty", nameof(texto));
            }

            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));

                // Convertir bytes a string hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
