namespace YAF.Core.Utilities.Captcha;

using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

public class SixLaborsCaptchaOptions
{
    /// <summary>
    /// Default fonts are  "Arial", "Verdana", "Times New Roman" in Windows
    /// Linux guys have to set tier own fonts :)
    /// </summary>
    public string[] FontFamilies { get; set; } = { "Arial", "Verdana", "Times New Roman" };
    public Color[] TextColor { get; set; } = new Color[] { Color.Blue, Color.Black, Color.Black, Color.Brown, Color.Gray, Color.Green };
    public float MinLineThickness { get; set; } = 0.7f;
    public float MaxLineThickness { get; set; } = 2.0f;
    public ushort Width { get; set; } = 180;
    public ushort Height { get; set; } = 50;
    public ushort NoiseRate { get; set; } = 800;
    public Color[] NoiseRateColor { get; set; } = new Color[] { Color.Gray };
    public byte FontSize { get; set; } = 29;
    public FontStyle FontStyle { get; set; } = FontStyle.Regular;
    public EncoderTypes EncoderType { get; set; } = EncoderTypes.Png;
    public IImageEncoder Encoder => Extensions.GetEncoder(this.EncoderType);
    public byte DrawLines { get; set; } = 5;
    public byte MaxRotationDegrees { get; set; } = 5;
}