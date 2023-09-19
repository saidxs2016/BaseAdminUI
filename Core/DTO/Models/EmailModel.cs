using Microsoft.AspNetCore.Http;

namespace Core.DTO.Models;

public class EmailModel
{
    /// <summary>
    /// Mail kime gidecek, Alıcı kişi emaili
    /// </summary>
    public string ReceiveUser { get; set; }
    public string DisplayName { get; set; }
    public string Subject { get; set; }

    /// <summary>
    /// Template içinde kullanılacak resimler, bu resimler template içinde gömülecek olan resimler, yani attachement değiller.
    /// </summary>
    public Dictionary<string, string> EmailImages { get; set; }

    /// <summary>
    /// Dosya göndermek - Arayüzden yüklenen dosyalar için
    /// </summary>
    public List<IFormFile> Attachments { get; set; }

    /// <summary>
    /// Dosya göndermek - Localdeki dosyalar için
    /// </summary>
    public List<string> AttachmentsAsPaths { get; set; }

    public dynamic? MailInfo { get; set; }

}
