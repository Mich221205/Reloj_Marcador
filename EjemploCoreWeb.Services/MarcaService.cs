using EjemploCoreWeb.Entities;
using EjemploCoreWeb.Repository.Interfaces;
using EjemploCoreWeb.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjemploCoreWeb.Services
{
    public class MarcaService : IMarcaService
    {

        private readonly IMarca _repository;

        public MarcaService(IMarca repository)
        {
            _repository = repository;
        }


        //Jocsan 
        //Reportes de marcas ADM 16
        public Task<IEnumerable<Marca>> Reporte_Marcas(
        int page,
        int pageSize,
        string? identificacion,
        DateTime? fecha)
        => _repository.Reporte_Marcas(page, pageSize, identificacion, fecha);


        public Task<int> Contar_Reporte_Marca(
        string? identificacion,
        DateTime? fecha)
        => _repository.Contar_Reporte_Marca(identificacion, fecha);


    }
}
