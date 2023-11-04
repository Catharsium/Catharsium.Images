using Catharsium.Images.Watermarking.Models;

namespace Catharsium.Images.Watermarking.Helpers;

public class Position
{
    public static (int x, int y) GetCoordinates(
        Anchor anchor,
        double imageWidth, double imageHeight,
        double watermarkWidth, double watermarkHeight,
        double offsetX, double offsetY
    ) {
        var x = 0;
        var y = 0;

        switch (anchor) {
            case Anchor.TopLeft:
                (x, y) = TopLeft();
                break;
            case Anchor.TopCenter:
                (x, y) = TopCenter(imageWidth, watermarkWidth);
                break;
            case Anchor.TopRight:
                (x, y) = TopRight(imageWidth, watermarkWidth);
                break;
            case Anchor.CenterLeft:
                (x, y) = CenterLeft(imageHeight, watermarkHeight);
                break;
            case Anchor.CenterCenter:
                (x, y) = CenterCenter(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
                break;
            case Anchor.CenterRight:
                (x, y) = CenterRight(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
                break;
            case Anchor.BottomLeft:
                (x, y) = BottomLeft(imageHeight, watermarkHeight);
                break;
            case Anchor.BottomCenter:
                (x, y) = BottomCenter(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
                break;
            case Anchor.BottomRight:
                (x, y) = BottomRight(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
                break;
        }

        return (x + (int)Math.Round(imageWidth * offsetX), y + (int)Math.Round(imageHeight * offsetY));
    }


    public static (int x, int y) TopLeft() {
        return (0, 0);
    }


    public static (int x, int y) TopCenter(double imageWidth, double watermarkWidth) {
        return (Center(imageWidth, watermarkWidth), 0);
    }


    public static (int x, int y) TopRight(double imageWidth, double watermarkWidth) {
        return ((int)(imageWidth - watermarkWidth), 0);
    }


    public static (int x, int y) CenterLeft(double imageHeight, double watermarkHeight) {
        return (0, Center(imageHeight, watermarkHeight));
    }


    public static (int x, int y) CenterCenter(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight) {
        return (Center(imageWidth, watermarkWidth), Center(imageHeight, watermarkHeight));
    }


    public static (int x, int y) CenterRight(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight) {
        return ((int)(imageWidth - watermarkWidth), Center(imageHeight, watermarkHeight));
    }


    public static (int x, int y) BottomLeft(double imageHeight, double watermarkHeight) {
        return (0, (int)(imageHeight - watermarkHeight));
    }


    public static (int x, int y) BottomCenter(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight) {
        return (Center(imageWidth, watermarkWidth), (int)(imageHeight - watermarkHeight));
    }


    public static (int x, int y) BottomRight(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight) {
        return ((int)(imageWidth - watermarkWidth), (int)(imageHeight - watermarkHeight));
    }


    private static int Center(double imageDimension, double watermarkDimension) {
        return (int)Math.Round(imageDimension / 2 - watermarkDimension / 2);
    }
}