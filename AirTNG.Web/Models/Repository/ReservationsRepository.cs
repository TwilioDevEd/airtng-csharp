using System.Threading.Tasks;

namespace AirTNG.Web.Models.Repository
{
    public interface IReservationsRepository
    {
        Task<int> CreateAsync(Reservation reservation);
    }

    public class ReservationsRepository : IReservationsRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationsRepository()
        {
            _context = new ApplicationDbContext();
        }

        public async Task<int> CreateAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            return await _context.SaveChangesAsync();
        }
    }
}