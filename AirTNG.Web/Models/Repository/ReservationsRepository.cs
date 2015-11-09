using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace AirTNG.Web.Models.Repository
{
    public interface IReservationsRepository
    {
        Task<IEnumerable<Reservation>> FindPendingReservationsAsync();
        Task<Reservation> FindFirstPendingReservationByHostAsync(string userId);
        Task<int> CreateAsync(Reservation reservation);
        Task<int> UpdateAsync(Reservation reservation);
        Task LoadNavigationPropertiesAsync(Reservation reservation);
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

        public async Task<int> UpdateAsync(Reservation reservation)
        {
            _context.Entry(reservation).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public async Task<Reservation> FindFirstPendingReservationByHostAsync(string userId)
        {
            return await _context.Reservations.FirstAsync(
                r => r.VacationProperty.UserId == userId && r.Status == ReservationStatus.Pending);
        }

        public async Task LoadNavigationPropertiesAsync(Reservation reservation)
        {
            await _context.Entry(reservation).Reference(r => r.VacationProperty).LoadAsync();
            await _context.Entry(reservation).Reference(r => r.Reservee).LoadAsync();
        }
    }
}