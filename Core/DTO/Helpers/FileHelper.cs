namespace Core.DTO.Helpers;

public record FileHelper
{
    public int Width { get; set; }
    public int Height { get; set; }
    public double Size { get; set; }
    public string? Address { get; set; }
    public string? Extention { get; set; }
    public string? Fullpathname { get; set; }
    public FileHelper() { }

    public FileHelper(int width, int height, double size, string? address, string? extention, string? fullpathname)
    {
        Width = width;
        Height = height;
        Size = size;
        Address = address;
        Extention = extention;
        Fullpathname = fullpathname;
    }
}
