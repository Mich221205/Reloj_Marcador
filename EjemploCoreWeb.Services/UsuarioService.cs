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
            @"^(?=[A-Za-z][A-Za-z0-9+\-\*\$\.]*$)(?=.*\d)(?=.*[+\-\*\$\.])",
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
            //if (usuario is null) throw new ArgumentNullException(nameof(usuario));

            //var contra = usuario.Contrasena ?? usuario.Contrasena;

            //if (string.IsNullOrWhiteSpace(contra))
            //    throw new ArgumentException("La contraseña es obligatoria.", nameof(usuario));

            //if (!_Contra_Regex.IsMatch(contra))
            //    throw new ArgumentException(
            //        "La contraseña debe iniciar con letra, contener números y al menos un símbolo (+-*$.)"
            //    );

            return _usuarioRepository.Cambiar_Clave(usuario);
        }

        //METODO PARA AUTOGENERAR LA CONTRASEÑA
        public string Autogenerar_Clave()
        {
            const string Letras = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string Nums = "0123456789";
            const string Simbolos = "+-*$.";
            int longitud = 16;
            const string Clave_Nueva = Letras + Nums + Simbolos;

            char PrimerLetra() => Letras[RandomNumberGenerator.GetInt32(Letras.Length)];
            char Rnd(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];

            // Primer carácter siempre letra
            var Contra = PrimerLetra().ToString();

            // Garantizar al menos un número y un símbolo
            Contra += Rnd(Nums) + Rnd(Simbolos);

            // Rellenar el resto
            while (Contra.Length < longitud)
                Contra += Rnd(Clave_Nueva);

                return Contra;
        }

    }
}
