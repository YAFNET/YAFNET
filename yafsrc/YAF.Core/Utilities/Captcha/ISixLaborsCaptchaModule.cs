namespace YAF.Core.Utilities.Captcha;

public interface ISixLaborsCaptchaModule
{
    byte[] Generate(string stringText);
}