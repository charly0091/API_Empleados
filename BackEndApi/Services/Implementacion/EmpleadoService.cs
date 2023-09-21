using BackEndApi.Models;
using BackEndApi.Services.Contrato;
using Microsoft.EntityFrameworkCore;

namespace BackEndApi.Services.Implementacion
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly DbempleadoContext _dbContext;

        public EmpleadoService(DbempleadoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Empleado>> GetList()
        {
            try
            {
                List<Empleado> lista = new List<Empleado>();
                lista = await _dbContext.Empleados.Include(dpt => dpt.IdDepartamentoNavigation).ToListAsync();

                return lista; 
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Empleado> Get(int idEmpleado)
        {
            try
            {
                Empleado? encontrado = new Empleado();
                encontrado = await _dbContext.Empleados.Include(dpt => dpt.IdDepartamentoNavigation)
                    .Where(x => x.IdEmpleado == idEmpleado).FirstOrDefaultAsync();

                return encontrado;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Empleado> Add(Empleado modelo)
        {
            try
            {
                _dbContext.Empleados.Add(modelo);
                await _dbContext.SaveChangesAsync();

                return modelo;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(Empleado modelo)
        {
            try
            {
                _dbContext.Empleados.Update(modelo);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(Empleado modelo)
        {
            try
            {
                _dbContext.Empleados.Remove(modelo);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
