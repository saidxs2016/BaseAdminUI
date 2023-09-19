using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Core.Functions_Extensions;

public class ImageFunctions
{
    private readonly IWebHostEnvironment _env;
    private static readonly string slashed = BaseFunctions._slashed;

    public ImageFunctions(IWebHostEnvironment env)
    {
        _env = env;
    }


    /// <summary>
    /// 
    /// 
    /// - file dosyanın kendisi
    /// - 
    /// - dimension için "800x480", "1920x1080" şeklinde istenildiği kadar boyut eklenebilir.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="path"></param>
    /// <param name="dimension"></param>
    /// <returns></returns>
    public async Task<FileHelper> ImageUploadAsync(IFormFile file, int quality = 100, params string[] dimensions)
    {
        FileHelper imgdetail = new();
        try
        {
            if (file == null || file.Length < 1)
                throw new FormatException("Dosya Yüklenmedi.");

            if (!file.ContentType.ToLowerInvariant().Contains("image"))
                throw new FormatException("Yüklenen Dosya Bir Resim Değildir.");

            var uploadDir = $"{_env.WebRootPath}{slashed}files{slashed}upload";
            var originalDir = Path.Combine(uploadDir, "original");

            if (!Directory.Exists(originalDir))
                Directory.CreateDirectory(originalDir);

            var myUniqueFileName = Guid.NewGuid().ToString();
            var FileExtension = Path.GetExtension(file.FileName);

            var newFileName = $"{myUniqueFileName}{FileExtension}";

            var file_path = Path.Combine(originalDir, newFileName);

            //using var fileStream = new FileStream(file_path, FileMode.Create);
            //await file.CopyToAsync(fileStream);
            using MemoryStream ms = new();
            await file.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            using MagickImage orginal_image = new(ms);
            MagickGeometry orginal_size = new(orginal_image.Width, orginal_image.Height);
            orginal_image.Quality = quality;
            orginal_size.IgnoreAspectRatio = true;
            orginal_image.Resize(orginal_size);
            await orginal_image.WriteAsync(file_path);

            foreach (var dimension in dimensions)
            {
                try
                {
                    var tmp_dimensions = dimension.Split("x");
                    if (tmp_dimensions == null || tmp_dimensions.Length == 0)
                        continue;

                    _ = int.TryParse(tmp_dimensions.First().Replace("_", "").Replace("-", ""), out int width);
                    if (width == 0)
                        continue;
                    _ = int.TryParse(tmp_dimensions.Last().Replace("_", "").Replace("-", ""), out int height);
                    if (height == 0)
                        continue;

                    var dir_name = BaseFunctions.FriendlyUrl(dimension);
                    var dir_path = Path.Combine(uploadDir, dir_name);
                    if (!Directory.Exists(dir_path))
                        Directory.CreateDirectory(dir_path);

                    var dimension_file_path = Path.Combine(dir_path, newFileName);

                    ms.Seek(0, SeekOrigin.Begin);
                    using MagickImage image = new(ms);
                    MagickGeometry size = new(width, height);
                    image.Quality = quality;
                    size.IgnoreAspectRatio = true;
                    image.Resize(size);
                    await image.WriteAsync(dimension_file_path);
                }
                catch
                {
                    continue;
                }

            }


            imgdetail.Width = orginal_image.Width;
            imgdetail.Height = orginal_image.Height;
            imgdetail.Address = newFileName;
            imgdetail.Size = file.Length * 1024;
        }
        catch (Exception)
        {
            throw;
        }


        return imgdetail;
    }

    // Resize Metoda
    // Imza Atma Metodu
    // Renk Değiştirme

    public Task ImageDelete(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return Task.CompletedTask;

        var uploadDir = $"{_env.WebRootPath}{slashed}files{slashed}upload";

        var dimensionsDirs = Directory.GetDirectories(uploadDir);
        foreach (var dimensionsDir in dimensionsDirs)
        {
            var files = Directory.GetFiles(dimensionsDir);

            foreach (var file in files)
                if (fileName == file.Split(slashed).Last())
                    File.Delete(file);

        }

        return Task.CompletedTask;
    }

}
