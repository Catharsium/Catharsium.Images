using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;
using Catharsium.Watermarker._Configuration;
using Catharsium.Watermarker.Interfaces.Menu;
using System.Drawing;
using System.Drawing.Imaging;

namespace Catharsium.Watermarker.Menu.Apply;

public class AddAction : IApplyActionHandler
{
    public string MenuName => "NK Pool 2023";

    private readonly IFileFactory fileFactory;
    private readonly IWatermarkingService<IFile> imageWatermarkingService;
    private readonly ImagesSettings settings;


    public AddAction(
        IFileFactory fileFactory,
        IWatermarkingService<IFile> imageWatermarkingService,
        ImagesSettings settings) {
        this.fileFactory = fileFactory;
        this.imageWatermarkingService = imageWatermarkingService;
        this.settings = settings;
    }


    public async Task Run() {
        foreach (var file in this.settings.Files) {
            var picture = this.fileFactory.CreateFile(file);

            var requests = new List<WatermarkRequest<IFile>>();
            foreach (var watermark in this.settings.ImageWatermarks) {
                requests.Add(new WatermarkRequest<IFile> {
                    Watermark = this.fileFactory.CreateFile(watermark.Source),
                    Scale = watermark.Scale,
                    Anchor = watermark.Anchor,
                    OffsetX = watermark.OffsetX,
                    OffsetY = watermark.OffsetY
                });
            }

            Bitmap outputImage;
            using (var sourceImage = new Bitmap(picture.OpenRead())) {
                outputImage = sourceImage;
                foreach (var request in requests) {
                    outputImage = this.imageWatermarkingService.ApplyTo(sourceImage, request);
                }
            }

            if (!this.settings.OverrideInputFiles) {
                var folder = this.fileFactory.CreateDirectory(Path.Combine(picture.Directory.FullName, "Watermarked"));
                if (!folder.Exists) {
                    folder.Create();
                }

                outputImage.Save(Path.Combine(folder.FullName, picture.Name), ImageFormat.Jpeg);
            }
            else {
                outputImage.Save(Path.Combine(picture.FullName), ImageFormat.Jpeg);
            }
        }
    }
}