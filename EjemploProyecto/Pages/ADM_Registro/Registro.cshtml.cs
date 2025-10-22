using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace EjemploProyecto.Pages.ADM_Registro
{
    public class RegistroModel : PageModel
    {
        [BindProperty] public int ID_Tipo_Identificacion { get; set; }
        [BindProperty] public string Identificacion { get; set; } = string.Empty;
        [BindProperty] public string Nombre { get; set; } = string.Empty;
        [BindProperty] public string Apellido1 { get; set; } = string.Empty;
        [BindProperty] public string Apellido2 { get; set; } = string.Empty;
        [BindProperty] public string Correo { get; set; } = string.Empty;
        [BindProperty] public string Telefono { get; set; } = string.Empty;
        [BindProperty] public int ID_Rol_Usuario { get; set; }
        [BindProperty] public string Password { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        private readonly string connectionString =
            "Server=127.0.0.1;Port=3306;Database=reloj_marcador;" +
            "Uid=reloj_user;Pwd=RELOJ123;Protocol=Tcp;AllowPublicKeyRetrieval=True;SslMode=None;";




        // 🔐 Clave AES-256 (32 caracteres) y IV (16 caracteres)
        private static readonly string AES_KEY = "12345678901234567890123456789012";
        private static readonly string AES_IV = "1234567890123456";

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Identificacion) ||
                string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Apellido1) ||
                string.IsNullOrWhiteSpace(Correo) ||
                string.IsNullOrWhiteSpace(Password))
            {
                Mensaje = "Por favor, complete todos los campos obligatorios.";
                return Page();
            }

            try
            {
                string passwordEncriptado = EncriptarAES(Password);

                using (var conexion = new MySqlConnection(connectionString))
                {
                    conexion.Open();

                    // Verificar duplicados
                    string check = @"SELECT COUNT(*) 
                                     FROM usuario 
                                     WHERE Identificacion=@Identificacion 
                                     OR Correo=@Correo";

                    using (var cmdCheck = new MySqlCommand(check, conexion))
                    {
                        cmdCheck.Parameters.AddWithValue("@Identificacion", Identificacion);
                        cmdCheck.Parameters.AddWithValue("@Correo", Correo);

                        long existe = (long)cmdCheck.ExecuteScalar();
                        if (existe > 0)
                        {
                            Mensaje = "Ya existe un usuario con esa identificación.";
                            return Page();
                        }
                    }

                    // Insertar nuevo registro
                    string insert = @"INSERT INTO usuario 
                                      (ID_Tipo_Identificacion, Identificacion, Nombre, Apellido_1, Apellido_2, 
                                       Correo, Telefono, ID_Rol_Usuario,  Contrasena, Estado)
                                      VALUES (@ID_Tipo_Identificacion, @Identificacion, @Nombre, @Apellido_1, 
                                              @Apellido_2, @Correo, @Telefono, @ID_Rol_Usuario, 
                                               @Contrasena, TRUE)";

                    using (var cmd = new MySqlCommand(insert, conexion))
                    {
                        cmd.Parameters.AddWithValue("@ID_Tipo_Identificacion", ID_Tipo_Identificacion);
                        cmd.Parameters.AddWithValue("@Identificacion", Identificacion);
                        cmd.Parameters.AddWithValue("@Nombre", Nombre);
                        cmd.Parameters.AddWithValue("@Apellido_1", Apellido1);
                        cmd.Parameters.AddWithValue("@Apellido_2", Apellido2);
                        cmd.Parameters.AddWithValue("@Correo", Correo);
                        cmd.Parameters.AddWithValue("@Telefono", Telefono);
                        cmd.Parameters.AddWithValue("@ID_Rol_Usuario", ID_Rol_Usuario);
                        cmd.Parameters.AddWithValue("@Contrasena", passwordEncriptado);
                        cmd.ExecuteNonQuery();
                    }
                }

                Mensaje = "✅ Registro exitoso. Ahora puede iniciar sesión.";
                return RedirectToPage("/ADM_Login/Login");
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al registrar usuario: {ex.Message}";
                return Page();
            }
        }

        // 🔒 Encriptar con AES
        private string EncriptarAES(string textoPlano)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(AES_KEY);
                aesAlg.IV = Encoding.UTF8.GetBytes(AES_IV);
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
    }
}