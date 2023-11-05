using Catharsium.Images.Watermarking.Helpers;
using Catharsium.Images.Watermarking.Models;

namespace Catharsium.Images.Watermarking.Tests.Helpers;

[TestClass]
public class PositionsTests
{
    [TestMethod]
    [DataRow(Anchor.TopLeft, 20, 10, 6, 4, 1, 1, 20, 10)]
    [DataRow(Anchor.TopLeft, 20, 10, 6, 4, -1, -1, -20, -10)]
    [DataRow(Anchor.TopLeft, 20, 10, 6, 4, 0.1, 0.1, 2, 1)]
    [DataRow(Anchor.TopLeft, 20, 10, 6, 4, -0.1, -0.1, -2, -1)]
    [DataRow(Anchor.TopLeft, 20, 10, 6, 4, 0.5, 0.5, 10, 5)]
    [DataRow(Anchor.TopLeft, 20, 10, 6, 4, -0.5, -0.5, -10, -5)]
    public void Get_WithOffset_ReturnsExpected(Anchor anchor, double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight, double offsetX, double offsetY, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.GetCoordinates(anchor, imageWidth, imageHeight, watermarkWidth, watermarkHeight, offsetX, offsetY);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(Anchor.TopLeft, 20, 10, 6, 4, 0, 0)]
    [DataRow(Anchor.TopCenter, 20, 10, 6, 4, 7, 0)]
    [DataRow(Anchor.TopRight, 20, 10, 6, 4, 14, 0)]
    [DataRow(Anchor.CenterLeft, 20, 10, 6, 4, 0, 3)]
    [DataRow(Anchor.CenterCenter, 20, 10, 6, 4, 7, 3)]
    [DataRow(Anchor.CenterRight, 20, 10, 6, 4, 14, 3)]
    [DataRow(Anchor.BottomLeft, 20, 10, 6, 4, 0, 6)]
    [DataRow(Anchor.BottomCenter, 20, 10, 6, 4, 7, 6)]
    [DataRow(Anchor.BottomRight, 20, 10, 6, 4, 14, 6)]
    public void Get_WithAnchor_ReturnsExpected(Anchor anchor, double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.GetCoordinates(anchor, imageWidth, imageHeight, watermarkWidth, watermarkHeight, 0, 0);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(0, 0)]
    public void TopLeft_ReturnsExpected(int expectedX, int expectedY) {
        var (actualX, actualY) = Position.TopLeft();
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(10, 4, 3, 0)]
    [DataRow(10, 5, 2, 0)]
    [DataRow(10, 6, 2, 0)]
    public void TopCenter_ReturnsExpected(double imageWidth, double watermarkWidth, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.TopCenter(imageWidth, watermarkWidth);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(10, 4, 6, 0)]
    [DataRow(10, 5, 5, 0)]
    [DataRow(10, 6, 4, 0)]
    public void TopRight_ReturnsExpected(double imageWidth, double watermarkWidth, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.TopRight(imageWidth, watermarkWidth);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(10, 4, 0, 3)]
    [DataRow(10, 5, 0, 2)]
    [DataRow(10, 6, 0, 2)]
    public void CenterLeft_ReturnsExpected(double imageHeight, double watermarkHeight, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.CenterLeft(imageHeight, watermarkHeight);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(20, 10, 6, 4, 7, 3)]
    [DataRow(20, 10, 7, 5, 6, 2)]
    [DataRow(20, 10, 8, 6, 6, 2)]
    public void CenterCenter_ReturnsExpected(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.CenterCenter(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(20, 10, 6, 4, 14, 3)]
    [DataRow(20, 10, 7, 5, 13, 2)]
    [DataRow(20, 10, 8, 6, 12, 2)]
    public void CenterRight_ReturnsExpected(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.CenterRight(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(10, 4, 0, 6)]
    [DataRow(10, 5, 0, 5)]
    [DataRow(10, 6, 0, 4)]
    public void BottomLeft_ReturnsExpected(double imageHeight, double watermarkHeight, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.BottomLeft(imageHeight, watermarkHeight);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(20, 10, 6, 4, 7, 6)]
    [DataRow(20, 10, 7, 5, 6, 5)]
    [DataRow(20, 10, 8, 6, 6, 4)]
    public void BottomCenter_ReturnsExpected(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.BottomCenter(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }


    [TestMethod]
    [DataRow(20, 10, 6, 4, 14, 6)]
    [DataRow(20, 10, 7, 5, 13, 5)]
    [DataRow(20, 10, 8, 6, 12, 4)]
    public void BottomRight_ReturnsExpected(double imageWidth, double imageHeight, double watermarkWidth, double watermarkHeight, int expectedX, int expectedY) {
        var (actualX, actualY) = Position.BottomRight(imageWidth, imageHeight, watermarkWidth, watermarkHeight);
        Assert.AreEqual(expectedX, actualX);
        Assert.AreEqual(expectedY, actualY);
    }
}