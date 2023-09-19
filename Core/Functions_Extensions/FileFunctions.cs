using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Core.Functions_Extensions;

public class FileFunctions
{
    private readonly IWebHostEnvironment _env;
    private static readonly string slashed = BaseFunctions._slashed;
    public readonly IConfiguration _configuration;

    public readonly ExtensionSelector _fileSelectorOptions;

    public FileFunctions(IWebHostEnvironment env, IConfiguration configuration, IOptions<ExtensionSelector> fileSelectorOptions)
    {
        _env = env;
        _configuration = configuration;
        _fileSelectorOptions = fileSelectorOptions.Value;
    }

    public async Task<FileHelper> FileUploadAsync(IFormFile file, ExtensionsSelectorEnum type = ExtensionsSelectorEnum.Valid)
    {
        FileHelper fileDetail = new();
        List<string> target_etensions = new();

        if (type == ExtensionsSelectorEnum.InValid)
            target_etensions = _fileSelectorOptions.InValid;
        else
            target_etensions = _fileSelectorOptions.Valid;

        target_etensions.ForEach(i => i.ToLowerInvariant());


        try
        {
            if (file == null || file.Length < 1)
                throw new FormatException("Dosya Yüklenmedi.");

            if (type == ExtensionsSelectorEnum.InValid)
                if (target_etensions.Any(i => i == Path.GetExtension(file.FileName).ToLower()))
                    throw new FormatException("Geçersiz Dosya Tipi");

            if (type == ExtensionsSelectorEnum.Valid)
                if (!target_etensions.Any(i => i == Path.GetExtension(file.FileName).ToLower()))
                    throw new FormatException("Geçersiz Dosya Tipi");



            var documentDir = $"{_env.WebRootPath}{slashed}files{slashed}documents";


            if (!Directory.Exists(documentDir))
                Directory.CreateDirectory(documentDir);

            var myUniqueFileName = Guid.NewGuid().ToString();
            var FileExtension = Path.GetExtension(file.FileName);

            var newFileName = $"{myUniqueFileName}{FileExtension}";

            var file_path = Path.Combine(documentDir, newFileName);

            using var fileStream = new FileStream(file_path, FileMode.Create);
            await file.CopyToAsync(fileStream);

            fileDetail.Address = newFileName;
            fileDetail.Size = file.Length * 1024;
        }
        catch (Exception)
        {
            throw;
        }

        return fileDetail;
    }


    public Task FileDelete(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return Task.CompletedTask;

        var documentDir = $"{_env.WebRootPath}{slashed}files{slashed}documents";
        var file_path = Path.Combine(documentDir, fileName);

        if (File.Exists(file_path))
            File.Delete(file_path);


        return Task.CompletedTask;
    }

}
