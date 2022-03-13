using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STAAPI.Infrastructure.Repository.GenericRepository;
using STCAPI.Core.Entities.MenuSubMenu;
using STCAPI.Core.ViewModel.RequestModel;
using STCAPI.Core.ViewModel.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STCAPI.Controllers.MenuSubMenuAccess
{
    /// <summary>
    /// Mensubmenu Access based on userName
    /// Table Used for this API=> MenuSubMenu,MenuSubMenuAccess
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
 
    public class MenuSubMenuAccessController : ControllerBase
    {
        private readonly IGenericRepository<MenuSubMenuModel, int> _IMenuSubMenuRepository;
        private readonly IGenericRepository<MenuSubMenuAccessModel, int> _IMenuSubMenuAccessRepository;

        /// <summary>
        /// Inject required service to the controller
        /// </summary>
        /// Table Used: MenuSubMenu
        /// <param name="menuSubMenuRepository"></param>
        /// <param name="menuSubMenuAccessReopsitory"></param>
        public MenuSubMenuAccessController(IGenericRepository<MenuSubMenuModel, int> menuSubMenuRepository, IGenericRepository<MenuSubMenuAccessModel, int> menuSubMenuAccessReopsitory)
        {
            _IMenuSubMenuRepository = menuSubMenuRepository;
            _IMenuSubMenuAccessRepository = menuSubMenuAccessReopsitory;
        }

        /// <summary>
        /// Create menu and submenu dynamically
        /// </summary>
        /// <param name="models"></param>
        /// Table Used: 
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateMenuSubMenu(List<MenuSubMenuVm> models)
        {
            List<MenuSubMenuModel> dbModels = new List<MenuSubMenuModel>();
            models.ForEach(item =>
            {
                var dbModel = new MenuSubMenuModel()
                {
                    MenuName = item.MenuName,
                    SubMenuName = item.SubMenuName,
                    RouteUrl = item.RouteUrl,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = "Deepak"

                };
                dbModels.Add(dbModel);
            });
            var response = await _IMenuSubMenuRepository.CreateEntity(dbModels.ToArray());
            return Ok(response);
        }

        /// <summary>
        /// Create Menu submenu access details based on UserName
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> MenuSubMenuAccess(List<MenuSubMenuAccessVm> models)
        {
            var response = await _IMenuSubMenuAccessRepository.GetAllEntities(x => x.UserName.ToLower() == models.First().UserName.ToLower());

            response.TEntities.ToList().ForEach(item =>
            {
                item.IsActive = false;
                item.IsDeleted = true;
                item.UpdatedBy = "Deepak";
                item.UpdatedDate = DateTime.Now.Date;
            });

            var deleteResponse = await _IMenuSubMenuAccessRepository.DeleteEntity(response.TEntities.ToArray());

            var dbModels = new List<MenuSubMenuAccessModel>();

            models.ForEach(data =>
            {
                var dbModel = new MenuSubMenuAccessModel()
                {
                    UserName = data.UserName.Trim().ToLower(),
                    SubMenuId = data.SubMenuId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = data.UserName,
                    CreatedDate = DateTime.Now.Date,
                    UpdatedDate = DateTime.Now.Date
                };
                dbModels.Add(dbModel);
            });

            var createResponse = await _IMenuSubMenuAccessRepository.CreateEntity(dbModels.ToArray());
            return Ok(createResponse);


        }

        /// <summary>
        /// Get Menu submenu access detail based on userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetMenuSubMenuAccess(string userName)
        {
            var menuSubMenuDetails = await _IMenuSubMenuRepository.GetAllEntities(x => x.IsActive && !x.IsDeleted);
            var menuSubMenuAccess = await _IMenuSubMenuAccessRepository.GetAllEntities(x => x.UserName.ToLower().Trim()
             == userName.ToLower().Trim());

            var responseModels = new List<MenuSubMenuResponseVm>();

            menuSubMenuDetails.TEntities.ToList().ForEach(data =>
            {
                var responseVm = new MenuSubMenuResponseVm()
                {
                    MenuName = data.MenuName,
                    SubMenuId = data.Id,
                    SubMenuName = data.SubMenuName,
                    RouteUrl = data.RouteUrl
                };
                menuSubMenuAccess.TEntities.Where(x => x.SubMenuId == data.Id).ToList().ForEach(item =>
                    {
                        if (item.IsActive && !item.IsDeleted && item.UserName.ToLower().Trim() == userName.ToLower().Trim())
                        {
                            responseVm.IsMapped = true;
                        }
                    });
                responseModels.Add(responseVm);

            });

            var response = new List<MenuSubMenuReponseModel>();

            foreach (var data in responseModels.GroupBy(x => x.MenuName))
            {
                var model = new MenuSubMenuReponseModel();
                model.MenuName = data.First().MenuName;

                var childerns = new List<Children>();

                foreach (var item in data)
                {
                    var chilModel = new Children() {
                        SubMenuId=item.SubMenuId,
                        RouteUrl=item.RouteUrl,
                        SubMenuName=item.SubMenuName,
                       IsMapped=item.IsMapped
                    };
                    childerns.Add(chilModel);
                }
                model.Childrens = childerns;
                response.Add(model);
            }


            return Ok(response);

        }
    }
}
