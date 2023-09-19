namespace UI.Attributes;

public class ModuleAttribute : Attribute
{

    public string Name { get; set; }
    public int Menu { get; set; }
    public string PermissionsPaths { get; set; }        
    public string Icon { get; set; }

    /// <summary>
    /// Modullleri eklemede yardımıcı olacak bir attribute
    /// </summary>
    /// <param name="name">Modüle bir ad verilemsi bu değer null kalırsa default olarak action adı verilecek</param>
    /// <param name="menu">0 ise menüde gösterme, 1 ise menude göster, default değer 0'dır</param>
    /// <param name="permissionsPaths">bu action içinde kullanılan servislerin url'leri. Birden çok varsa virgül ile ayırtılmış şekilde yayan bir string içide olacaklar</param>        
    /// <param name="icon"></param>
    public ModuleAttribute(string name = null, int menu = 0, string permissionsPaths = null, string icon = null)
    {
        Name = name;
        Menu = menu;
        PermissionsPaths = permissionsPaths;            
        Icon = icon;
    }

}


