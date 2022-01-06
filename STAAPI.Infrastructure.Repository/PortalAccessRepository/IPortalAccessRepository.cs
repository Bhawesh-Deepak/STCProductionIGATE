using STCAPI.Core.Entities.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STAAPI.Infrastructure.Repository.PortalAccessRepository
{
    public interface IPortalAccessRepository
    {
        Task<List<PortalAccessVm>> GetPortalAccessDetail();
    }
}
