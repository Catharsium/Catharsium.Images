using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;

namespace Catharsium.Images.Watermarking.Helpers;

public class ColorHelper
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public static ColorMatrix GetColorMatrix(bool useGrayScale) {
        return useGrayScale ?
            new ColorMatrix(new float[][]      {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
            }) :
            new ColorMatrix {
                Matrix33 = 0.8f
            };
    }
}