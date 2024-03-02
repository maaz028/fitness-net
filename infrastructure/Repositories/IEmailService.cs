
using core.Models;

namespace infrastructure.Repositories
{
    public interface IEmailService
    {
        Task SendTestEmail(UserEmailOptions options);
    }
}
