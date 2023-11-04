using Catharsium.Images.Watermarking.Helpers;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using System.Drawing;
using System.Drawing.Imaging;

namespace Catharsium.Images.Watermarking.Services;

public class PictureTextWatermarkingService : IWatermarkingService<string>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public Bitmap ApplyTo(Bitmap picture, WatermarkRequest<string> request) {

        using (var g = Graphics.FromImage(picture)) {
            var font = new Font("Arial", 80f, FontStyle.Bold, GraphicsUnit.Pixel);
            var watermarkSize = g.MeasureString(request.Watermark, font);

            var watermarkWidth = (int)(picture.Width * request.Scale);
            var watermarkHeight = (int)(watermarkWidth / (double)watermarkSize.Width * watermarkSize.Height);
            (var x, var y) = Position.GetCoordinates(request.Anchor, picture.Width, picture.Height, watermarkWidth, watermarkHeight, request.OffsetX, request.OffsetY);

            var matrix = new ColorMatrix {
                Matrix33 = 0.5f
            };
            var attributes = new ImageAttributes();
            attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            Brush brush = new SolidBrush(Color.FromArgb((int)(0.5f * 255), Color.LightBlue));
            g.DrawString(request.Watermark, font, brush, x, y);
        }

        return picture;
    }
}