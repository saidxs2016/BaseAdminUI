using Net.Codecrete.QrCodeGenerator;
using SkiaSharp;

namespace Core.Functions_Extensions;

public class QrCodeFunction
{
    public static string GenerateBaseQRCode(string encrypted_data, ImageContentTypeEnum type = ImageContentTypeEnum.PNG)
    {


        //var qr_As_byte_arr = BitmapByteQRCodeHelper.GetQRCode(encrypted_data, QRCodeGenerator.ECCLevel.M, 10);
        //return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(qr_As_byte_arr));


        var qr = QrCode.EncodeText(encrypted_data, QrCode.Ecc.Medium);
        using SKBitmap bitmap = ToBitmap(qr, 20, 2, SKColors.Black, SKColors.White);

        SKImage image = SKImage.FromPixels(bitmap.PeekPixels());
        // encode the image (defaults to PNG)
        SKData encoded = image.Encode();
        // get a stream over the encoded data
        using Stream stream = encoded.AsStream();

        using MemoryStream ms = new();
        stream.CopyTo(ms);

        if (type == ImageContentTypeEnum.NOTYPE)
            return Convert.ToBase64String(ms.ToArray());
        else if (type == ImageContentTypeEnum.JPEG)
            return string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(ms.ToArray()));
        else if (type == ImageContentTypeEnum.PDF)
            return string.Format("data:application/pdf;base64,{0}", Convert.ToBase64String(ms.ToArray()));
        else
            return string.Format("data:image/png;base64,{0}", Convert.ToBase64String(ms.ToArray()));

    }

    public static SKBitmap ToBitmap(QrCode qrCode, int scale, int border, SKColor foreground, SKColor background)
    {
        // check arguments
        if (scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Value out of range");
        }
        if (border < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(border), "Value out of range");
        }

        int size = qrCode.Size;
        int dim = (size + border * 2) * scale;

        if (dim > short.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale or border too large");
        }

        // create bitmap
        SKBitmap bitmap = new(dim, dim, SKColorType.Rgb888x, SKAlphaType.Opaque);

        using (SKCanvas canvas = new(bitmap))
        {
            // draw background
            using (SKPaint paint = new() { Color = background })
            {
                canvas.DrawRect(0, 0, dim, dim, paint);
            }

            // draw modules
            using (SKPaint paint = new() { Color = foreground })
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        if (qrCode.GetModule(x, y))
                        {
                            canvas.DrawRect((x + border) * scale, (y + border) * scale, scale, scale, paint);
                        }
                    }
                }
            }
        }

        return bitmap;
    }



}
