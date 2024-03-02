using core.Models.TrainerModels;
using infrastructure.AddionalClasses;

namespace infrastructure.Repositories.Trainer
{
    public class MockTrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext _context;

        public MockTrainerRepository(AppDbContext Context)
        {
            _context = Context;
        }

        public async Task<TrainerModel> AddTrainerAsync(TrainerModel trainer)
        {
            await _context.Trainer.AddAsync(trainer);
            await _context.SaveChangesAsync();

            return trainer;
        }

        public IEnumerable<TrainerModel> GetAllTrainers()
        {
            return _context.Trainer.ToList();
        }

        public async Task<TrainerModel> GetSingleTrainerAsync(string id)
        {
            TrainerModel? trainer = await _context.Trainer.FindAsync(id);

            return trainer;
        }

        public async Task<TrainerModel> RemoveTrainerAsync(string id)
        {
            TrainerModel? trainer = await _context.Trainer.FindAsync(id);
            _context.Trainer.Remove(trainer);
            await _context.SaveChangesAsync();

            return trainer;
        }

        public async Task<TrainerModel> UpdateTrainerAsync(TrainerModel trainer)
        {
            var std = _context.Trainer.Attach(trainer);
            std.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();

            return trainer;
        }
    }
}
