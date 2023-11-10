using Catharsium.Images.Watermarking.Helpers;
using Catharsium.Images.Watermarking.Models;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Catharsium.Images.Watermarking.Services;

public class PictureTextWatermarkingService : WatermarkingService<string>
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public override Bitmap ApplyTo<T>(Bitmap picture, WatermarkRequest<T> request, bool useGrayScale) {
        using (var g = Graphics.FromImage(picture)) {
            var font = new Font("Arial", 80f, FontStyle.Bold, GraphicsUnit.Pixel);
            var watermarkSize = g.MeasureString(request.Mark as string, font);
            var watermarkWidth = (int)(picture.Width * request.Scale);
            var watermarkHeight = (int)(watermarkWidth / (double)watermarkSize.Width * watermarkSize.Height);
            if (picture.Height > picture.Width) {
                var factor = picture.Height / (double)picture.Width;
                watermarkWidth = (int)(watermarkWidth * factor);
                watermarkHeight = (int)(watermarkHeight * factor);
            }

            (var x, var y) = Position.GetCoordinates(request.Anchor, picture.Width, picture.Height, watermarkWidth, watermarkHeight, request.OffsetX, request.OffsetY);
            //  var colorMatrix = ColorHelper.GetColorMatrix(useGrayScale);

            //  var attributes = new ImageAttributes();
            //  attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            Brush brush = new SolidBrush(Color.FromArgb((int)(0.5f * 255), Color.LightBlue));
            g.DrawString(request.Mark as string, font, brush, x, y);
        }

        return picture;
    }
}