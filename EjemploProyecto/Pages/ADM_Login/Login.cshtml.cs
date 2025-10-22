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

        // ❗ Ajusta si cambiaste usuario/clave del conector
        private readonly string connectionString =
            "Server=127.0.0.1;Port=3306;Database=reloj_marcador;" +
            "Uid=reloj_user;Pwd=RELOJ123;Protocol=Tcp;AllowPublicKeyRetrieval=True;SslMode=None;";

        // ✅ Deben coincidir con los usados al registrar
        private static readonly string AesKey = "12345678901234567890123456789012"; // 32 chars = 256 bits
        private static readonly string AesIV = "1234567890123456";                // 16 chars = 128 bits

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
                using var conexion = new MySqlConnection(connectionString);
                conexion.Open();

                // Traigo lo mínimo para validar y armar sesión
                const string query = @"
                    SELECT ID_Usuario, Contrasena, Nombre, Apellido_1, Estado, ID_Rol_Usuario
                    FROM usuario
                    WHERE Identificacion = @Identificacion
                    LIMIT 1;";

                using var cmd = new MySqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Identificacion", Identificacion);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    Mensaje = "Usuario no encontrado.";
                    return Page();
                }

                var activo = reader.GetBoolean(reader.GetOrdinal("Estado"));
                if (!activo)
                {
                    Mensaje = "Usuario inactivo. Contacte al administrador.";
                    return Page();
                }

                var contrasenaDb = reader.GetString(reader.GetOrdinal("Contrasena"));
                var nombre = reader.GetString(reader.GetOrdinal("Nombre"));
                var apellido1 = reader.GetString(reader.GetOrdinal("Apellido_1"));
                var idRol = reader.GetInt32(reader.GetOrdinal("ID_Rol_Usuario"));

                // 🔐 Validación tolerante: AES+Base64 -> Base64 simple -> texto plano
                bool ok = ValidatePassword(Password, contrasenaDb, out string? detalleError);

                if (!ok)
                {
                    // Mensaje genérico para usuario final; log interno opcional con detalleError
                    Mensaje = "Usuario y/o contraseña incorrectos.";
                    return Page();
                }

                // ✅ Login correcto → sesión
                HttpContext.Session.SetString("Nombre", nombre);
                HttpContext.Session.SetString("Apellido1", apellido1);
                HttpContext.Session.SetString("RolUsuario", idRol == 1 ? "Administrador" : "Usuario");
                HttpContext.Session.SetString("AvatarUrl", "/images/avatar-default.png");

                return RedirectToPage("/ADM_Login/Bienvenida");
            }
            catch (Exception ex)
            {
                Mensaje = $"Error de conexión: {ex.Message}";
                return Page();
            }
        }

        // ================== Helpers de validación ==================

        /// <summary>
        /// Intenta validar la contraseña probando, en este orden:
        /// 1) AES-CBC-PKCS7 con Key/IV + Base64
        /// 2) Base64 simple del texto plano
        /// 3) Texto plano directo
        /// </summary>
        private bool ValidatePassword(string inputPlain, string stored, out string? error)
        {
            error = null;

            // 1) AES + Base64
            if (LooksLikeBase64(stored))
            {
                try
                {
                    var decrypted = DecryptAesFromBase64(stored);
                    if (TimingSafeEquals(inputPlain, decrypted))
                        return true;
                }
                catch (Exception ex)
                {
                    // No rompo el flujo: si no “encaja el bloque” u otro error, sigo probando
                    error = $"AES/Base64 fallo: {ex.Message}";
                }

                // 2) Base64 simple
                try
                {
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(stored));
                    if (TimingSafeEquals(inputPlain, decoded))
                        return true;
                }
                catch (Exception ex2)
                {
                    // No interrumpo; sigo con texto plano
                    error = (error == null) ? $"Base64 simple fallo: {ex2.Message}" : error + $" | {ex2.Message}";
                }
            }

            // 3) Texto plano directo
            if (TimingSafeEquals(inputPlain, stored))
                return true;

            // 4) (Opcional) Si algún día agregan bcrypt ($2y$...), aquí iría la verificación con BCrypt.Net
            // if (stored.StartsWith("$2")) { return BCrypt.Net.BCrypt.Verify(inputPlain, stored); }

            return false;
        }

        private static string DecryptAesFromBase64(string cipherBase64)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var cipherBytes = Convert.FromBase64String(cipherBase64);
            using var ms = new MemoryStream(cipherBytes);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        private static bool LooksLikeBase64(string s)
        {
            // Patrón base64 estándar con 0-2 '=' al final
            // Evita intentar Convert.FromBase64String con cosas evidentemente no base64
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            if (s.Length % 4 != 0) return false;
            foreach (char c in s)
            {
                if ((c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') ||
                    c == '+' || c == '/' || c == '=')
                    continue;
                return false;
            }
            return true;
        }

        private static bool TimingSafeEquals(string a, string b)
        {
            // Comparación constante para evitar short-circuit (defensa básica)
            if (a == null || b == null) return false;
            var ba = Encoding.UTF8.GetBytes(a);
            var bb = Encoding.UTF8.GetBytes(b);
            if (ba.Length != bb.Length) return false;

            int diff = 0;
            for (int i = 0; i < ba.Length; i++)
                diff |= ba[i] ^ bb[i];

            return diff == 0;
        }

        // (Útiles si luego necesitás cifrar al registrar)
        public static string EncriptarAES(string textoPlano)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(AesKey);
            aesAlg.IV = Encoding.UTF8.GetBytes(AesIV);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs, Encoding.UTF8))
            {
                sw.Write(textoPlano);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
