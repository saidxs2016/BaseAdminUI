using System.Text;
using System.Text.RegularExpressions;

namespace Core.Functions_Extensions;

public class BaseFunctions
{
    public static readonly string _slashed = Path.DirectorySeparatorChar.ToString();
    public static readonly string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
  
    public static bool IsNumeric(string value) => value.All(char.IsNumber);
    public static bool IsEmail(string email)
    {
        Regex rx = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        bool sonuc = string.IsNullOrEmpty(email) && rx.IsMatch(email);
        return sonuc;

    }
    public static string GenerateCode(int length = 6)
    {
        string value = ""; //Boş bir değer tanımlıyoruz.
        Random rnd = new(); // Burada Rastgele değeri tanımlıyouz.
        for (int c = 0; c < length; c++) //25 haneli rakam-harf üretmek için döngü yaptık.
        {
            int ck = rnd.Next(0, 2); // 0 veya 1
            if (ck == 0) // Rastgele üretilen sayı 0 ise sayı üret.
            {
                int num = rnd.Next(1, 10);
                value += num.ToString();
            }
            else // Değilse harf üret (65 ile 91 arası ascii kodlar olduğu için rakam değerleri girdik.)
            {
                int x = rnd.Next(65, 91);
                char chr = Convert.ToChar(x); //ascii kod olarak üretilen sayıyı harfe çevirdik.
                value += chr; //Değere atadık.
            }
        }
        return value;
    }


    public static void WriteToFile(string DirectoryPath, string FileName, string Text)
    {
        //Check Whether directory exist or not if not then create it
        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        string FilePath = Path.Combine(DirectoryPath, FileName);
        //Check Whether file exist or not if not then create it new else append on same file
        File.WriteAllText(FilePath, Text);
    }
    public static string ReadFromFile(string DirectoryPath, string FileName)
    {
        string result = "";

        string FilePath = Path.Combine(DirectoryPath, FileName);
        if (File.Exists(FilePath))
            result = File.ReadAllText(FilePath, Encoding.UTF8);

        return result;
    }

    public static async Task<long> WriteBytesToFileAsync(string directoryPath, string fileName, byte[] data, string fileExt, bool initShowCorrectExt = false, bool LastPacket = false)
    {
        //Check Whether directory exist or not if not then create it
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        var tmpFileName = fileName;
        if (!initShowCorrectExt)
            tmpFileName = fileName.Replace($".{fileExt}", ".dat");

        string FilePath = Path.Combine(directoryPath, tmpFileName);
        if (!File.Exists(FilePath))
        {
            using FileStream destinationStream = new(FilePath, FileMode.Create, FileAccess.Write);
            await destinationStream.WriteAsync(data);
        }
        else
        {
            using FileStream destinationStream = new(FilePath, FileMode.Append, FileAccess.Write);
            await destinationStream.WriteAsync(data);
        }

        if (LastPacket && !initShowCorrectExt)
        {
            File.Move(FilePath, Path.Combine(directoryPath, fileName), true);
            return FilePath.Length;
        }

        using FileStream fileStream = new(FilePath, FileMode.Open, FileAccess.Read);
        return fileStream.Length;
    }

    public static async IAsyncEnumerable<LargeFileHelper> ReadBytesFromFileAsync(string directoryPath, string fileName, int bufferSize = 1024 * 512)
    {
        string FilePath = Path.Combine(directoryPath, fileName);

        if (File.Exists(FilePath))
        {
            using FileStream fileStream = new(FilePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[bufferSize];
            while (fileStream.Position < fileStream.Length)
            {
                int bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                byte[] chunk = new byte[bytesRead];
                Array.Copy(buffer, chunk, bytesRead);
                yield return new LargeFileHelper
                {
                    FileName = fileName,
                    Extension = "",
                    Data = chunk,
                    Length = fileStream.Length,
                    Position = fileStream.Position,
                    BytesRead = bytesRead
                };
            }
        }
        else
        {
            Console.WriteLine("File does not exist.");
        }
    }


    public static string HtmlStringBuilderClear(string url)
    {
        if (string.IsNullOrEmpty(url)) return "";
        url = url.Trim();
        url = url.Replace("\r", "");
        url = url.Replace("\n", "");
        url = url.Replace("\t", "");
        url = url.Replace("  ", " ");

        return url;
    }
    public static string FriendlyUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return "";
        url = url.ToLowerInvariant();
        url = url.Trim();
        if (url.Length > 250)
            url = url[..250];

        url = url.Replace("İ", "I");
        url = url.Replace("ı", "i");
        url = url.Replace("ğ", "g");
        url = url.Replace("Ğ", "G");
        url = url.Replace("ç", "c");
        url = url.Replace("Ç", "C");
        url = url.Replace("ö", "o");
        url = url.Replace("Ö", "O");
        url = url.Replace("ş", "s");
        url = url.Replace("Ş", "S");
        url = url.Replace("ü", "u");
        url = url.Replace("Ü", "U");
        url = url.Replace("'", "");
        url = url.Replace("\"", "");
        //char[] replacerList = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();
        //for (int i = 0; i < replacerList.Length; i++)
        //{
        //    string strChr = replacerList[i].ToString();
        //    if (url.Contains(strChr))
        //        url = url.Replace(strChr, string.Empty);
        //}
        Regex r = new("[^a-zA-Z0-9_-]");
        url = r.Replace(url, "-");
        while (url.IndexOf("--") > -1)
            url = url.Replace("--", "-");
        return url;
    }


    // ====================== Pagination Algorithm Version-2 (advenced algorithm) ======================
    public static BasePaginate GetPaginateInfo(long contentListCount, long page = 1, long pagesize = 10)
    {
        long current = 0, prev = -1, next = -1;
        List<string> rangeWithDots = new();


        if (page > contentListCount)
            page = 0;

        if (pagesize <= 0 || pagesize >= contentListCount)
        {
            page = 1;
            pagesize = contentListCount;
        }

        if (page > 0 && pagesize > 0)
        {
            current = page;
            long items;
            if (contentListCount % pagesize == 0)
                items = contentListCount / pagesize;
            else
                items = contentListCount / pagesize + 1;

            prev = current == 1 ? -1 : current - 1;
            next = current == items ? -1 : current + 1;

            if (items > 0)
            {
                long last = items;
                long delta = 2, l = 0;
                long left = current - delta;
                long right = current + delta + 1;

                List<int> range = new();
                for (int i = 1; i <= last; i++)
                {
                    if (i == 1 || i == last || i >= left && i < right)
                        range.Add(i);
                }

                foreach (var i in range)
                {
                    if (l > 0)
                    {
                        if (i - l == 2)
                        {
                            rangeWithDots.Add(Convert.ToString(l + 1));
                        }
                        else if (i - l != 1)
                            rangeWithDots.Add("...");
                    }
                    rangeWithDots.Add(Convert.ToString(i));
                    l = i;
                }

            }
        }
        string pages_str = string.Join(",", rangeWithDots);
        return new BasePaginate { Current = current, Prev = prev, Next = next, Pages = rangeWithDots, PagesStr = pages_str };
    }


    public partial class BasePaginate
    {
        public long Current { get; set; }
        public long Prev { get; set; }
        public long Next { get; set; }
        public List<string>? Pages { get; set; }
        public string? PagesStr { get; set; }
    }
}
