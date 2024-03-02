using core.Models.TrainerModels;

namespace infrastructure.Repositories.Trainer
{
    public interface ITrainerRepository
    {
        Task<TrainerModel> GetSingleTrainerAsync(string id);

        IEnumerable<TrainerModel> GetAllTrainers();

        Task<TrainerModel> RemoveTrainerAsync(string id);

        Task<TrainerModel> UpdateTrainerAsync(TrainerModel Trainer);

        Task<TrainerModel> AddTrainerAsync(TrainerModel Trainer);
    }
}
