using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AirTNG.Web.Models.Repository
{
    public interface IReservationsRepository
    {
        Task<IEnumerable<Reservation>> FindPendingReservationsAsync();
        Task<int> CreateAsync(Reservation reservation);
    }

    public class ReservationsRepository : IReservationsRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationsRepository()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<IEnumerable<Reservation>> FindPendingReservationsAsync()
        {
            return await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Pending).ToListAsync();
        }

        public async Task<int> CreateAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            return await _context.SaveChangesAsync();
        }
    }
}