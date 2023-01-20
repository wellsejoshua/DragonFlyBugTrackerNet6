using System.Threading.Tasks;
using DragonFlyBugTrackerNet6.Models;

namespace DragonFlyBugTrackerNet6.Services.Interfaces
{
    public interface IBTCompanyManagement
    {
        public Task CreateCompany(Company company);

        public Task ArchiveCompany();

        public Task UpdateCompany(Company company);
    }
}
