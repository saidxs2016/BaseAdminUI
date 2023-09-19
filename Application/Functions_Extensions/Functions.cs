using Application.DTO.DataObjects;
using System.Text;

namespace Application.Functions_Extensions;

public class Functions : BaseFunctions
{
    public static readonly string UserLayoutDir = Path.Combine(wwwroot, "user-layout");
    public static readonly string UserLayoutMenuDir = Path.Combine(UserLayoutDir, "menu");
    public static string MakeUserMenuHtml(List<ModuleDO> modules)
    {
        if (modules == null)
            return "";

        var orderedModules = modules.OrderBy(w => w.Order).ToList();
        orderedModules.ForEach(module =>
            {
                if (module.SubModuleList != null)
                {
                    module.SubModuleList = module.SubModuleList.OrderBy(w => w.Order).ToList();
                    module.SubModuleList.ForEach(s_module =>
                    {
                        if (s_module.SubModuleList != null)
                            s_module.SubModuleList = s_module.SubModuleList.OrderBy(w => w.Order).ToList();
                        else
                            s_module.SubModuleList = new List<ModuleDO>();
                    });
                }
                else
                    module.SubModuleList = new List<ModuleDO>();
            }
        );

        StringBuilder sb = new();
        foreach (var item in orderedModules)
        {
            sb.Append($"<li class='menu'>");
            if (item.Type == ModuleHelper.Page)
                sb.Append($"<a href='#page{item.Uid}' data-url='{item.Address}' ondblclick='location.href=\"{item.Address}\"' data-bs-toggle='collapse' aria-expanded='false' class='dropdown-toggle'>");

            else
                sb.Append($"<a href='#page{item.Uid}' data-category='true' data-url='{item.Address}' data-bs-toggle='collapse' aria-expanded='false' class='dropdown-toggle'>");


            sb.Append($"<div>");
            sb.Append($"<i class='{(!string.IsNullOrEmpty(item.Icon) ? item.Icon : "fas fa-circle")}' style='color:white;font-size:18px;'></i>");
            sb.Append($"<span style='font-size:12px;'>{item.Name}</span>");
            sb.Append($"</div>");

            sb.Append($"<div>");
            if (item.SubModuleList is not null && item.SubModuleList.Count > 0)
                sb.Append($"<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' class='feather feather-chevron-right'>");

            else
                sb.Append($"<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' class='feather feather-chevron-right' style='color: transparent;'>");

            sb.Append($"<polyline points='9 18 15 12 9 6'></polyline>");
            sb.Append($"</svg>");
            sb.Append($"</div>");

            sb.Append($"</a>");
            if (item.SubModuleList != null && item.SubModuleList.Count > 0)
            {
                sb.Append($"<ul class='collapse submenu list-unstyled' id='page{item.Uid}' data-bs-parent='#accordionExample{(item.ParentUid.HasValue ? item.ParentUid : "")}'>");
                sb.Append(Children(item.SubModuleList));
                sb.Append($"</ul>");
            }
            sb.Append($"</li>");
        }
        return sb.ToString();
    }

    private static StringBuilder Children(List<ModuleDO> page)
    {
        var sb = new StringBuilder();

        foreach (var item in page)
        {
            sb.Append($"<li>");
            if (item.Type == ModuleHelper.Page)
                sb.Append($"<a href='#page{item.Uid}' data-url='{item.Address}' ondblclick='location.href=\"{item.Address}\"' data-bs-toggle='collapse' aria-expanded='false' class='dropdown-toggle'>{item.Name}");
            else
                sb.Append($"<a href='#page{item.Uid}' data-category='true' data-url='{item.Address}' data-bs-toggle='collapse' aria-expanded='false' class='dropdown-toggle'>{item.Name}");

            if (item.SubModuleList is not null && item.SubModuleList.Count > 0)
                sb.Append($"<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' class='feather feather-chevron-right'><polyline points='9 18 15 12 9 6'></polyline></svg>");
            else
                sb.Append($"<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' class='feather feather-chevron-right' style='color: transparent;'><polyline points='9 18 15 12 9 6'></polyline></svg>");

            sb.Append($"</a>");
            if (item.SubModuleList != null && item.SubModuleList.Count > 0)
            {
                sb.Append($"<ul class='collapse sub-submenu list-unstyled' id='page{item.Uid}' data-bs-parent='#page{(item.ParentUid.HasValue ? item.ParentUid : "")}'>");
                sb.Append(Children(item.SubModuleList));
                sb.Append($"</ul>");
            };


            sb.Append($"</li>");
        }
        return sb;
    }

}
