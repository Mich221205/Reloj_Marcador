using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository;
using EjemploCoreWeb.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions; //Para validación de la contraseña
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;

        private static readonly Regex _Contra_Regex = new Regex(
        @"^(?=[A-Za-z])(?=.*\d)(?=.*[+\-\*\$\.])[A-Za-z0-9+\-\*\$\.]{12,}$",
        RegexOptions.Compiled);


        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return _usuarioRepository.GetAllAsync();
        }


        public Task<Usuario> Obtener_Usuario_X_Identificacion(string id)
        {
            return _usuarioRepository.Obtener_Usuario_X_Identificacion(id);
        }


        public Task<int> Cambiar_Clave(Usuario usuario)
        {
            if (usuario is null)
                throw new ArgumentNullException(nameof(usuario));

            var contra = usuario.Contrasena;

            if (string.IsNullOrWhiteSpace(contra))
                throw new ArgumentException("La contraseña es obligatoria.");

            if (contra.Length < 12)
                throw new ArgumentException("La contraseña debe tener al menos 12 caracteres.");

            if (!_Contra_Regex.IsMatch(contra))
                throw new ArgumentException(
                    "La contraseña debe iniciar con letra, contener números y al menos un símbolo (+-*$.)"
                );

            return _usuarioRepository.Cambiar_Clave(usuario);
        }

        //METODO PARA AUTOGENERAR LA CONTRASEÑA
        public string Autogenerar_Clave()
        {
            const string Letras = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string Nums = "0123456789";
            const string Simbolos = "+-*$.";
            const string Clave_Nueva = Letras + Nums + Simbolos;
            int longitud = 12;

            char PrimerLetra() => Letras[RandomNumberGenerator.GetInt32(Letras.Length)];
            char Rnd(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];

            // Construir la clave con los requisitos mínimos
            var chars = new List<char>
            {
                PrimerLetra(),     // primera siempre letra
                Rnd(Nums),         // al menos un número
                Rnd(Simbolos)      // al menos un símbolo
            };

            // Rellenar hasta la longitud deseada
            while (chars.Count < longitud)
                chars.Add(Rnd(Clave_Nueva));

            //Mezclar excepto la primera letra (para que siempre arranque con letra)
            var middle = chars.Skip(1).OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToList();
            var contra = chars[0] + string.Concat(middle);

            // Validar con el regex por seguridad
            if (!_Contra_Regex.IsMatch(contra))
                return Autogenerar_Clave(); // reintenta si algo salió raro

            return contra;
        }

    }
}
