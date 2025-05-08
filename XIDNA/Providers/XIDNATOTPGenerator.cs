using OtpNet;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace XIDNA.Providers
{
    public class XIDNATOTPGenerator 
    {
        private static int period= 60;  // Minitues for code expiration
        private static int totpSize = 8;  // totp size by default 6
        private static int QRPixelsPerModule = 2; // Number of pixels per QR Module (2 pixels give ~ 100x100px QRCode)

        public static Totp GenerateOTP(string base32String)
        {
            var base32Bytes = Base32Encoding.ToBytes(base32String); //base32String
            return new Totp(base32Bytes, step: period, totpSize: totpSize);
        }


        //Generate SecurityKey we can map this security key to user for security purpose you can encrypt and save it
        public static string generateKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20);

            return Base32Encoding.ToString(key);
        }



        public static bool VerifyOTP(string base32String,string otp)
        {
            long timeWindowUsed;
            var base32Bytes = Base32Encoding.ToBytes(base32String); //base32String
            var totp= new Totp(base32Bytes);
            var status= totp.VerifyTotp(otp, out timeWindowUsed, VerificationWindow.RfcSpecifiedNetworkDelay);
            if (status)
            {
                return status;
            }
            return status;
        }


        public static TOTPAccountDto GenerateURI(string secrectKey,string userIdentifier,string issuer= "XIDNA")
        {

           var accountTitleNoSpaces = RemoveWhitespace(userIdentifier);

            var base32Bytes = Base32Encoding.ToBytes(secrectKey); //base32String
            string encodedSecretKey = Base32Encoding.ToString(base32Bytes);

            var provisionUrl = new OtpUri(OtpType.Totp,secrectKey,accountTitleNoSpaces, issuer, OtpHashMode.Sha256,totpSize, period).ToString();
            using (var qrGen = new QRCodeGenerator())
            using (var qrCode = qrGen.CreateQrCode(provisionUrl, QRCodeGenerator.ECCLevel.Q))
            using (var qrBmp = new BitmapByteQRCode(qrCode))
            {
                string qrCodeUrl = $"data:image/png;base64,{Convert.ToBase64String(qrBmp.GetGraphic(QRPixelsPerModule))}";

                return new TOTPAccountDto()
                {
                    QRCodeUrl = qrCodeUrl,
                    ManualAccountKey = encodedSecretKey,
                };
            }
        }

        private static string RemoveWhitespace(string str)
        {
            return new string(str.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
    }
}