using Catharsium.Images.Watermarking._Configuration;
using Catharsium.Images.Watermarking.Interfaces;
using Catharsium.Images.Watermarking.Models;
using Catharsium.Util.IO.Files.Interfaces;

namespace Catharsium.Images.Watermarking.Services;

public class WatermarkApplicator : IWatermarkApplicator
{
    private readonly IFileFactory fileFactory;
    private readonly IWatermarkingService<IFile> imageWatermarkingService;
    private readonly IWatermarkingService<string> textWatermarkingService;
    private readonly WatermarkingSettings settings;


    public WatermarkApplicator(
        IFileFactory fileFactory,
        IWatermarkingService<IFile> imageWatermarkingService,
        IWatermarkingService<string> textWatermarkingService,
        WatermarkingSettings settings) {
        this.fileFactory = fileFactory;
        this.imageWatermarkingService = imageWatermarkingService;
        this.textWatermarkingService = textWatermarkingService;
        this.settings = settings;
    }


    public void Apply(string[] files) {
        foreach (var file in files) {
            var sourceFile = this.fileFactory.CreateFile(file);
            var outputFile = sourceFile;

            if (!this.settings.OverrideInputFiles) {
                var folder = this.fileFactory.CreateDirectory(Path.Combine(sourceFile.Directory.FullName, "Watermarked"));
                if (!folder.Exists) {
                    folder.Create();
                }

                outputFile = this.fileFactory.CreateFile(Path.Combine(folder.FullName, sourceFile.Name));
            }

            if (this.settings.ImageWatermarks != null) {
                this.imageWatermarkingService.ApplyTo<IFile>(sourceFile, outputFile, this.settings.ImageWatermarks.Select(this.MapToImageWatermark));
            }

            if (this.settings.TextWatermarks != null) {
                this.textWatermarkingService.ApplyTo<string>(outputFile, outputFile, this.settings.TextWatermarks.Select(this.MapToTextWatermark));
            }
        }
    }


    private WatermarkRequest<IFile> MapToImageWatermark(Watermark w) {
        return new WatermarkRequest<IFile> {
            Mark = this.fileFactory.CreateFile(w.Mark),
            Scale = w.Scale,
            Anchor = w.Anchor,
            OffsetX = w.OffsetX,
            OffsetY = w.OffsetY
        };
    }


    private WatermarkRequest<string> MapToTextWatermark(Watermark w) {
        return new WatermarkRequest<string> {
            Mark = w.Mark,
            Scale = w.Scale,
            Anchor = w.Anchor,
            OffsetX = w.OffsetX,
            OffsetY = w.OffsetY
        };
    }
}