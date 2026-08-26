using System.ComponentModel;
using SkiaSharp;
using Svg.Skia;

namespace EasyWindowsApplication.Core;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class IconGenerator
{
    public static void GenerateIco(string svgPath, string icoPath, int[] sizes)
    {
        var svg = new SKSvg();
        var picture = svg.Load(svgPath);
        if (picture is null)
            return;

        var bounds = picture.CullRect;
        var pngs = new Dictionary<int, byte[]>();

        foreach (var size in sizes)
        {
            using var surface = SKSurface.Create(new SKImageInfo(size, size));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            float sx = (float)size / bounds.Width;
            float sy = (float)size / bounds.Height;
            float scale = Math.Min(sx, sy);
            float ox = (size - bounds.Width * scale) / 2f;
            float oy = (size - bounds.Height * scale) / 2f;

            canvas.Translate(ox, oy);
            canvas.Scale(scale, scale);
            canvas.DrawPicture(picture);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            pngs[size] = data.ToArray();
        }

        WriteIco(icoPath, pngs);
    }

    static void WriteIco(string icoPath, Dictionary<int, byte[]> pngs)
    {
        using var fs = new FileStream(icoPath, FileMode.Create);
        using var bw = new BinaryWriter(fs);

        bw.Write((ushort)0);
        bw.Write((ushort)1);
        bw.Write((ushort)pngs.Count);

        var entries = pngs.OrderBy(kv => kv.Key).ToList();
        int offset = 6 + entries.Count * 16;

        foreach (var (size, data) in entries)
        {
            bw.Write((byte)(size >= 256 ? 0 : size));
            bw.Write((byte)(size >= 256 ? 0 : size));
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((ushort)1);
            bw.Write((ushort)32);
            bw.Write((uint)data.Length);
            bw.Write((uint)offset);
            offset += data.Length;
        }

        foreach (var (_, data) in entries)
        {
            bw.Write(data);
        }
    }
}
