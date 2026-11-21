using System.Text;

namespace Catharsium.Images.Core.Tests.Metadata;

/// <summary>
/// Builds a minimal JPEG file with an IPTC block (APP13) and/or an XMP packet (APP1),
/// similar to what Lightroom writes on export. There is no actual image data, but that
/// is enough for MetadataExtractor to read the metadata segments.
/// </summary>
public class TestJpegBuilder
{
    private readonly List<(byte Record, byte DataSet, string Value)> iptcEntries = [];
    private string? xmpPacket;


    public TestJpegBuilder WithIptc(byte dataSet, string value) {
        this.iptcEntries.Add((2, dataSet, value));
        return this;
    }


    public TestJpegBuilder WithObjectName(string value) => this.WithIptc(5, value);
    public TestJpegBuilder WithKeyword(string value) => this.WithIptc(25, value);
    public TestJpegBuilder WithDateCreated(string yyyyMMdd) => this.WithIptc(55, yyyyMMdd);
    public TestJpegBuilder WithTimeCreated(string hhmmssZone) => this.WithIptc(60, hhmmssZone);
    public TestJpegBuilder WithCity(string value) => this.WithIptc(90, value);
    public TestJpegBuilder WithSublocation(string value) => this.WithIptc(92, value);
    public TestJpegBuilder WithCountry(string value) => this.WithIptc(101, value);
    public TestJpegBuilder WithCaption(string value) => this.WithIptc(120, value);


    public TestJpegBuilder WithXmp(int? rating = null, string? label = null) {
        var attributes = new StringBuilder();
        if(rating != null) {
            attributes.Append($" xmp:Rating=\"{rating}\"");
        }

        if(label != null) {
            attributes.Append($" xmp:Label=\"{label}\"");
        }

        this.xmpPacket =
            "<?xpacket begin=\"﻿\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>" +
            "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\">" +
            "<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">" +
            $"<rdf:Description rdf:about=\"\" xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\"{attributes}/>" +
            "</rdf:RDF>" +
            "</x:xmpmeta>" +
            "<?xpacket end=\"w\"?>";
        return this;
    }


    public byte[] Build() {
        using var stream = new MemoryStream();
        stream.Write([0xFF, 0xD8]); // SOI

        if(this.xmpPacket != null) {
            var payload = Concat(Encoding.ASCII.GetBytes("http://ns.adobe.com/xap/1.0/\0"), Encoding.UTF8.GetBytes(this.xmpPacket));
            WriteSegment(stream, 0xE1, payload);
        }

        if(this.iptcEntries.Count > 0) {
            WriteSegment(stream, 0xED, this.BuildPhotoshopIptcBlock());
        }

        stream.Write([0xFF, 0xD9]); // EOI
        return stream.ToArray();
    }


    public string WriteToTempFile() {
        var path = Path.Combine(Path.GetTempPath(), $"catharsium-images-test-{Guid.NewGuid():N}.jpg");
        File.WriteAllBytes(path, this.Build());
        return path;
    }


    private byte[] BuildPhotoshopIptcBlock() {
        using var iptc = new MemoryStream();

        // 1:90 Coded character set = UTF-8 (ESC % G)
        WriteIptcEntry(iptc, 1, 90, [0x1B, 0x25, 0x47]);
        foreach(var (record, dataSet, value) in this.iptcEntries) {
            WriteIptcEntry(iptc, record, dataSet, Encoding.UTF8.GetBytes(value));
        }

        var iptcBytes = iptc.ToArray();
        using var block = new MemoryStream();
        block.Write(Encoding.ASCII.GetBytes("Photoshop 3.0\0"));
        block.Write(Encoding.ASCII.GetBytes("8BIM"));
        block.Write([0x04, 0x04]); // IPTC-NAA resource
        block.Write([0x00, 0x00]); // Empty pascal name, padded to even length
        block.Write(BigEndian(iptcBytes.Length));
        block.Write(iptcBytes);
        if(iptcBytes.Length % 2 == 1) {
            block.WriteByte(0x00);
        }

        return block.ToArray();
    }


    private static void WriteIptcEntry(Stream stream, byte record, byte dataSet, byte[] value) {
        stream.WriteByte(0x1C);
        stream.WriteByte(record);
        stream.WriteByte(dataSet);
        stream.WriteByte((byte)(value.Length >> 8));
        stream.WriteByte((byte)(value.Length & 0xFF));
        stream.Write(value);
    }


    private static void WriteSegment(Stream stream, byte marker, byte[] payload) {
        var length = payload.Length + 2;
        stream.Write([0xFF, marker, (byte)(length >> 8), (byte)(length & 0xFF)]);
        stream.Write(payload);
    }


    private static byte[] BigEndian(int value) =>
        [(byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value];


    private static byte[] Concat(byte[] first, byte[] second) => [.. first, .. second];
}
