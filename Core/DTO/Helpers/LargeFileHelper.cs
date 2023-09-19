namespace Core.DTO.Helpers;

public record LargeFileHelper
{
    public required string FileName { get; set; }
    public required string Extension { get; set; }
    public int BytesRead { get; set; }
    public long Length { get; set; }
    public long Position { get; set; }
    public required byte[] Data { get; set; }


}
