namespace YAF.Core.Utilities.Captcha;

using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using Color = SixLabors.ImageSharp.Color;
using PointF = SixLabors.ImageSharp.PointF;

public class SixLaborsCaptchaModule : ISixLaborsCaptchaModule
{
    private readonly SixLaborsCaptchaOptions _options;
    
    public SixLaborsCaptchaModule()
    {
        this._options = new SixLaborsCaptchaOptions
                            {
                                DrawLines = 4
                            };
    }

    public byte[] Generate(string stringText)
    {
        byte[] result;

        using var imgText = new Image<Rgba32>(this._options.Width, this._options.Height);
        float position = 0;
        var random = new Random();
        var startWith = (byte)random.Next(5, 10);
        imgText.Mutate(ctx => ctx.BackgroundColor(Color.Transparent));

        var fontName = this._options.FontFamilies[random.Next(0, this._options.FontFamilies.Length)];
        var font = SystemFonts.CreateFont(fontName, this._options.FontSize, this._options.FontStyle);

        foreach (var c in stringText)
        {
            var location = new PointF(startWith + position, random.Next(6, 13));
            imgText.Mutate(ctx => ctx.DrawText(c.ToString(), font, this._options.TextColor[random.Next(0, this._options.TextColor.Length)], location));
            position += TextMeasurer.Measure(c.ToString(), new TextOptions(font)).Width;
        }

        //add rotation
        var rotation = this.GetRotation();
        imgText.Mutate(ctx => ctx.Transform(rotation));

        // add the dynamic image to original image
        var size = (ushort)TextMeasurer.Measure(stringText, new TextOptions(font)).Width;
        var img = new Image<Rgba32>(size + 10 + 5, this._options.Height);
        img.Mutate(ctx => ctx.BackgroundColor(Color.White));

        Parallel.For(0, this._options.DrawLines, i =>
            {
                var x0 = random.Next(0, random.Next(0, 30));
                var y0 = random.Next(10, img.Height);
                var x1 = random.Next(70, img.Width);
                var y1 = random.Next(0, img.Height);
                img.Mutate(
                    ctx => ctx.DrawLines(
                        this._options.TextColor[random.Next(0, this._options.TextColor.Length)],
                        Extensions.GenerateNextFloat(this._options.MinLineThickness, this._options.MaxLineThickness),
                        new[] {new(x0, y0), new PointF(x1, y1)}));
            });

        img.Mutate(ctx => ctx.DrawImage(imgText, 0.80f));

        Parallel.For(0, this._options.NoiseRate, i =>
            {
                var x0 = random.Next(0, img.Width);
                var y0 = random.Next(0, img.Height);
                img.Mutate(
                    ctx => ctx
                        .DrawLines(this._options.NoiseRateColor[random.Next(0, this._options.NoiseRateColor.Length)],
                            Extensions.GenerateNextFloat(0.5, 1.5), new PointF[] { new Vector2(x0, y0), new Vector2(x0, y0) })
                );
            });

        img.Mutate(x =>
            {
                x.Resize(this._options.Width, this._options.Height);
            });

        using var ms = new MemoryStream();
        img.Save(ms, this._options.Encoder);
        result = ms.ToArray();

        return result;
    }

    private AffineTransformBuilder GetRotation()
    {
        var random = new Random();
        var builder = new AffineTransformBuilder();
        var width = random.Next(10, this._options.Width);
        var height = random.Next(10, this._options.Height);
        var pointF = new PointF(width, height);
        var rotationDegrees = random.Next(0, this._options.MaxRotationDegrees);
        var result = builder.PrependRotationDegrees(rotationDegrees, pointF);
        return result;
    }
}