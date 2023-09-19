using Core.DTO.Models;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Application.Functions_Extensions;

public class MailFunctions : BaseFunctions
{
    public static readonly string mailDir = Path.Combine(wwwroot, "mail");
    public static readonly string mailImageDir = Path.Combine(mailDir, "images");



    // ============= Her Email templatei için body replace işlemlerini gerçekleştirecek metot alta yer alacak =============
    public static AlternateView PasswordForgetTrTemplate(EmailModel mailModel)
    {
        // ====================== html dosyayı oku ======================
        string filePath = Path.Combine(mailDir, "password_forget_tr_template.html");
        using StreamReader sr = new(filePath);
        string body_txt = sr.ReadToEnd();

        // ====================== Kullanılacak değişkenleri tanımla ======================
        string Name = Convert.ToString(mailModel.MailInfo.Name);
        string Surname = Convert.ToString(mailModel.MailInfo.Surname);
        string BaseAddress = Convert.ToString(mailModel.MailInfo.BaseAddress);
        string ResetPasswordPath = Convert.ToString(mailModel.MailInfo.ResetPasswordPath);
        string Token = Convert.ToString(mailModel.MailInfo.Token);
        var redirectUrl = $"{BaseAddress}{ResetPasswordPath}/{Token}";

        // ====================== html dosyayı içinde text olan değişkenleri replace yap ======================
        body_txt = body_txt.Replace("__NAME_SURNAME__", $"{Name} {Surname}");
        body_txt = body_txt.Replace("__REDIRECT_URL__", redirectUrl);

        //eğer default logo varsa burada ekle
        mailModel.EmailImages ??= new() { { "#LOGO-IMG1", "star.png" } }; // id, filename in mail/images folder

        // ====================== html dosyayı içinde img olan değişkenleri replace yap ======================
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_txt, Encoding.UTF8, MediaTypeNames.Text.Html);

        if (mailModel.EmailImages != null)
            foreach (var (id, path) in mailModel.EmailImages)
                htmlView.LinkedResources.Add(AddLinkedResource(Path.Combine(mailImageDir, path), id));

        return htmlView;
    }
    public static AlternateView AccountConfirmTrTemplate(EmailModel mailModel)
    {
        // ====================== html dosyayı oku ======================
        string filePath = Path.Combine(mailDir, "account_confirm_tr_template.html");
        using StreamReader sr = new(filePath);
        string body_txt = sr.ReadToEnd();

        // ====================== Kullanılacak değişkenleri tanımla ======================
        string Name = Convert.ToString(mailModel.MailInfo.Name);
        string Surname = Convert.ToString(mailModel.MailInfo.Surname);
        string BaseAddress = Convert.ToString(mailModel.MailInfo.BaseAddress);
        string ActivationPath = Convert.ToString(mailModel.MailInfo.ActivationPath);
        string Token = Convert.ToString(mailModel.MailInfo.Token);
        var redirectUrl = $"{BaseAddress}{ActivationPath}/{Token}";

        // ====================== html dosyayı içinde text olan değişkenleri replace yap ======================
        body_txt = body_txt.Replace("__NAME_SURNAME__", $"{Name} {Surname}");
        body_txt = body_txt.Replace("__ACTIVATION_URL__", redirectUrl);

        //eğer default logo varsa burada ekle
        mailModel.EmailImages ??= new() { { "#LOGO-IMG1", "star.png" } }; // id, filename in mail/images folder

        // ====================== html dosyayı içinde img olan değişkenleri replace yap ======================
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_txt, Encoding.UTF8, MediaTypeNames.Text.Html);

        if (mailModel.EmailImages != null)
            foreach (var (id, path) in mailModel.EmailImages)
                htmlView.LinkedResources.Add(AddLinkedResource(Path.Combine(mailImageDir, path), id));

        return htmlView;


    }

    public static AlternateView NotificationFromUnHealthy(EmailModel mailModel)
    {

        // ====================== Kullanılacak değişkenleri tanımla ======================
        string Message = Convert.ToString(mailModel.MailInfo.Message);
        // ====================== html dosyayı oku ======================
        //....

        // ====================== html dosyayı içinde text olan değişkenleri replace yap ======================
        // body_txt = body_txt.Replace("__NAME_SURNAME__", $"{Name} {Surname}");
        string body_txt = Message;
        //...

        //eğer default logo varsa burada ekle
        //mailModel.EmailImages ??= new() { { "#LOGO-IMG1", "star.png" } }; // id, filename in mail/images folder

        // ====================== html dosyayı içinde img olan değişkenleri replace yap ======================
        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body_txt, Encoding.UTF8, MediaTypeNames.Text.Html);

        if (mailModel.EmailImages != null)
            foreach (var (id, path) in mailModel.EmailImages)
                htmlView.LinkedResources.Add(AddLinkedResource(Path.Combine(mailImageDir, path), id));

        return htmlView;


    }

    private static LinkedResource AddLinkedResource(string path, string sectionId)
    {
        StreamReader img = new(path);
        LinkedResource res = new(img.BaseStream, new ContentType("image/png"))
        {
            ContentId = sectionId,
            TransferEncoding = TransferEncoding.Base64,
            //ContentLink = new Uri("cid:" + sectionId)
        };

        //img.Close();
        return res;
    }

}
