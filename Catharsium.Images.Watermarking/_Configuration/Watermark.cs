﻿using Catharsium.Images.Watermarking.Models;

namespace Catharsium.Images.Watermarking._Configuration;

public class Watermark
{
    public required string Mark { get; set; }
    public double Scale { get; set; }
    public Anchor Anchor { get; set; }
    public FlowDirection Flow { get; set; }
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
}