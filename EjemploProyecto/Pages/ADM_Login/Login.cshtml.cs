using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace EjemploProyecto.Pages.ADM_Login
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string Identificacion { get; set; } = string.Empty;
        [BindProperty] public string Password { get; set; } = string.Empty;
        [BindProperty] public bool Recordarme { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        private readonly string connectionString = "Server=localhost;Database=reloj_marcador;Uid=reloj_user;Pwd=RELOJ123;";

        // ✅ Clave y vector de inicialización (iguales a los usados en Registro)
        private static readonly string AesKey = "12345678901234567890123456789012"; // 32 caracteres = 256 bits
        private static readonly string AesIV = "1234567890123456"; // 16 caracteres = 128 bits

        public IActionResult OnGet(string? expired)
        {
            ViewData["Expired"] = expired == "1";
            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Identificacion) || string.IsNullOrWhiteSpace(Password))
            {
                Mensaje = "Por favor, ingrese la identificación y la contraseña.";
                return Page();
            }

            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();
                    string query = @"SELECT Contrasena, Nombre, Apellido_1, Estado 
                                     FROM Usuario 
                                     WHERE Identificacion = @Identificacion";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Identificacion", Identificacion);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool activo = reader.GetBoolean("Estado");
                                if (!activo)
                                {
                                    Mensaje = "Usuario inactivo. Contacte al administrador.";
                                    return Page();
                                }

                                string contrasenaEncriptada = reader.GetString("Contrasena");
                                string nombre = reader.GetString("Nombre");
                                string apellido = reader.GetString("Apellido_1");

                                // 🔓 Desencriptar la contraseña almacenada
                                string contrasenaDesencriptada = DesencriptarAES(contrasenaEncriptada);

                                if (Password == contrasenaDesencriptada)
                                {
                                    // ✅ Login correcto → guardar sesión
                                    HttpContext.Session.SetString("Nombre", nombre);
                                    HttpContext.Session.SetString("Apellido1", apellido);
                                    HttpContext.Session.SetString("RolUsuario", "Administrador");
                                    HttpContext.Session.SetString("AvatarUrl", "/images/avatar-default.png");

                                    return RedirectToPage("/ADM_Login/Bienvenida");
                                }
                                else
                                {
                                    Mensaje = "Usuario y/o contraseña incorrectos.";
                                    return Page();
                                }
                            }
                            else
                            {
                                Mensaje = "Usuario no encontrado.";
                                return Page();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = $"Error de conexión: {ex.Message}";
                return Page();
            }
        }

        // 🔒 Encriptar con AES
        public static string EncriptarAES(string textoPlano)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(AesKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(AesIV);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(textoPlano);
                    sw.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        // 🔓 Desencriptar con AES
        public static string DesencriptarAES(string textoEncriptado)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(AesKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(AesIV);
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (var ms = new MemoryStream(Convert.FromBase64String(textoEncriptado)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
