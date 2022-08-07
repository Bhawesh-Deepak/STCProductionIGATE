using System.Collections.Generic;

namespace STCAPI.Core.ViewModel.ResponseModel
{
    public class MenuSubMenuReponseModel
    {
        public List<Children> Childrens { get; set; }
        public string MenuName { get; set; }
    }

    public class MenuSubMenuResponseVm
    {
        public int SubMenuId { get; set; }
        public string MenuName { get; set; }
        public string SubMenuName { get; set; }
        public string RouteUrl { get; set; }
        public bool IsMapped { get; set; }
    }
    public class Children
    {
        public int SubMenuId { get; set; }
        public string SubMenuName { get; set; }
        public string RouteUrl { get; set; }
        public bool IsMapped { get; set; }
    }
}
