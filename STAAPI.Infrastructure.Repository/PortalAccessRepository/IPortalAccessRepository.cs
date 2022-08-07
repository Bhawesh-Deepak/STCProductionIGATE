using STCAPI.Core.Entities.UserManagement;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STAAPI.Infrastructure.Repository.PortalAccessRepository
{
    public interface IPortalAccessRepository
    {
        Task<List<PortalAccessVm>> GetPortalAccessDetail();
    }
}
