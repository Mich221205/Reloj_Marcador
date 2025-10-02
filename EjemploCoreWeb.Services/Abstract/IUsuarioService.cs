using EjemploCoreWeb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services.Abstract
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllAsync();

    }
}
