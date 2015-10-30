using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace AirTNG.Web.Models.Repository
{
    public interface IVacationPropertiesRepository
    {
        Task<IEnumerable<VacationProperty>> AllAsync();
        Task<int> CreateAsync(VacationProperty property);
        Task<int> UpdateAsync(VacationProperty property);
        Task<VacationProperty> FindAsync(int id);
    }

    public class VacationPropertiesRepository : IVacationPropertiesRepository
    {
        private readonly ApplicationDbContext _context;

        public VacationPropertiesRepository()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<IEnumerable<VacationProperty>> AllAsync()
        {
            return await _context.VacationProperties.ToListAsync();
        }

        public async Task<int> CreateAsync(VacationProperty property)
        {
            _context.VacationProperties.Add(property);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(VacationProperty property)
        {
            _context.Entry(property).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<VacationProperty> FindAsync(int id)
        {
            return await _context.VacationProperties.FindAsync(id);
        }
    }
}