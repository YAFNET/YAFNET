namespace YAF.Controls
{
    using System;

    public class RecaptchaResponse
    {
        private string errorCode;
        public static readonly RecaptchaResponse InvalidSolution = new RecaptchaResponse(false, "incorrect-captcha-sol");
        private bool isValid;
        public static readonly RecaptchaResponse RecaptchaNotReachable = new RecaptchaResponse(false, "recaptcha-not-reachable");
        public static readonly RecaptchaResponse Valid = new RecaptchaResponse(true, string.Empty);

        internal RecaptchaResponse(bool isValid, string errorCode)
        {
            this.isValid = isValid;
            this.errorCode = errorCode;
        }

        public override bool Equals(object obj)
        {
            RecaptchaResponse response = (RecaptchaResponse) obj;
            if (response == null)
            {
                return false;
            }
            return ((response.IsValid == this.IsValid) && (response.ErrorCode == this.ErrorCode));
        }

        public override int GetHashCode()
        {
            return (this.IsValid.GetHashCode() ^ this.ErrorCode.GetHashCode());
        }

        public string ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        public bool IsValid
        {
            get
            {
                return this.isValid;
            }
        }
    }
}

