using Catharsium.Images.Watermarking.Helpers;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace Catharsium.Images.Watermarking.Services;

public class PictureImageWatermarkingService : WatermarkingService<IFile>
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public override Bitmap ApplyTo<T>(Bitmap picture, WatermarkRequest<T> request, bool useGrayScale) {
        using var watermarkBitmap = new Bitmap((request.Mark as IFile).OpenRead());
        var watermarkWidth = (int)(picture.Width * request.Scale);
        var watermarkHeight = (int)(watermarkWidth / (double)watermarkBitmap.Width * watermarkBitmap.Height);
        if(picture.Height > picture.Width) {
            var factor = picture.Height / (double)picture.Width;
            watermarkWidth = (int)(watermarkWidth * factor);
            watermarkHeight = (int)(watermarkHeight * factor);
        }

        var width = picture.Width + this.LimitsX[request.Anchor];
        var height = picture.Height + this.LimitsY[request.Anchor];
        (var x, var y) = Position.GetCoordinates(request.Anchor, width, height, watermarkWidth, watermarkHeight, request.OffsetX, request.OffsetY);
        if(request.Flow == FlowDirection.Left) {
            this.LimitsX[request.Anchor] -= width - x;
        }
        if(request.Flow == FlowDirection.Right) {
            this.LimitsX[request.Anchor] += width - x;
        }
        if(request.Flow == FlowDirection.Up) {
            this.LimitsY[request.Anchor] -= height - y;
        }
        if(request.Flow == FlowDirection.Down) {
            this.LimitsY[request.Anchor] += height - y;
        }

        using(var g = Graphics.FromImage(picture)) {
            var attributes = new ImageAttributes();
            var colorMatrix = ColorHelper.GetColorMatrix(useGrayScale);
            attributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            g.DrawImage(watermarkBitmap,
                new Rectangle(x, y, watermarkWidth, watermarkHeight),
                0, 0, watermarkBitmap.Width,
                watermarkBitmap.Height,
                GraphicsUnit.Pixel,
                attributes);
        }

        return picture;
    }
}