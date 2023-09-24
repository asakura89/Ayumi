using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using ConfigurationManager = AppSea.ConfigurationManager;

namespace Serena {
    public static class SecurityExt {
        static RijndaelManaged CreateRijndaelAlgorithm(String securityKey, String securitySalt) {
            Byte[] saltBytes = Encoding.UTF8.GetBytes(securityKey + securitySalt);
            var randByte = new Rfc2898DeriveBytes(securityKey, saltBytes, 12000);

            const Int32 MaxOutSize = 256;
            const Int32 MaxOutSizeInBytes = MaxOutSize / 8;
            return new RijndaelManaged {
                BlockSize = MaxOutSize,
                Key = randByte.GetBytes(MaxOutSizeInBytes),
                IV = randByte.GetBytes(MaxOutSizeInBytes),
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };
        }

        public static String Encrypt(this String plainText) => Encrypt(plainText, GetKey(), GetSalt());

        public static String Encrypt(this String plainText, String securityKey, String securitySalt) {
            using (RijndaelManaged algorithm = CreateRijndaelAlgorithm(securityKey, securitySalt)) {
                Byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                Byte[] cipherBytes = null;
                using (var stream = new MemoryStream()) {
                    using (var cryptoStream = new CryptoStream(stream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);

                    cipherBytes = stream.ToArray();
                }

                return EncodeBase64UrlFromBytes(cipherBytes);
            }
        }

        public static String Decrypt(this String chiperText) => Decrypt(chiperText, GetKey(), GetSalt());

        public static String Decrypt(this String chiperText, String securityKey, String securitySalt) {
            using (RijndaelManaged algorithm = CreateRijndaelAlgorithm(securityKey, securitySalt)) {
                Byte[] cipherBytes = DecodeBase64UrlToBytes(chiperText);
                Byte[] plainBytes = null;
                using (var encstream = new MemoryStream(cipherBytes)) {
                    using (var decstream = new MemoryStream()) {
                        using (var cryptoStream = new CryptoStream(encstream, algorithm.CreateDecryptor(), CryptoStreamMode.Read)) {
                            Int32 data;
                            while ((data = cryptoStream.ReadByte()) != -1)
                                decstream.WriteByte((Byte) data);

                            decstream.Position = 0;
                            plainBytes = decstream.ToArray();
                        }
                    }
                }

                return Encoding.UTF8.GetString(plainBytes);
            }
        }

        public const String SecurityKeyAppSettingsKey = "Security.Key";

        static String GetKey() {
            String key = ConfigurationManager.GetAppSetting(SecurityKeyAppSettingsKey)?.Value;
            if (String.IsNullOrEmpty(key))
                return String.Empty;

            return EncodeBase64Url(key);
        }

        public const String SecuritySaltAppSettingsKey = "Security.Salt";

        static String GetSalt() {
            String salt = ConfigurationManager.GetAppSetting(SecuritySaltAppSettingsKey)?.Value;
            if (String.IsNullOrEmpty(salt))
                return String.Empty;

            return EncodeBase64Url(salt);
        }

        const String Base64Plus = "+";
        const String Base64Slash = "/";
        const String Base64Underscore = "_";
        const String Base64Minus = "-";
        const String Base64Equal = "=";
        const String Base64DoubleEqual = "==";
        const Char Base64EqualChar = '=';

        public static String EncodeBase64Url(String plain) =>
            EncodeBase64UrlFromBytes(
                Encoding.UTF8.GetBytes(plain)
            );

        static String EncodeBase64UrlFromBytes(Byte[] bytes) =>
            Convert
                .ToBase64String(bytes)
                .TrimEnd(Base64EqualChar)
                .Replace(Base64Plus, Base64Minus)
                .Replace(Base64Slash, Base64Underscore);

        public static String DecodeBase64Url(String base64Url) =>
            Encoding.UTF8.GetString(
                DecodeBase64UrlToBytes(base64Url)
            );

        static Byte[] DecodeBase64UrlToBytes(String base64Url) {
            String halfProcessed = base64Url
                .Replace(Base64Minus, Base64Plus)
                .Replace(Base64Underscore, Base64Slash);

            String base64 = halfProcessed;
            if (halfProcessed.Length % 4 == 2)
                base64 = halfProcessed + Base64DoubleEqual;
            else if (halfProcessed.Length % 4 == 3)
                base64 = halfProcessed + Base64Equal;

            return Convert.FromBase64String(base64);
        }

        public static SecureString AsSecureString(this String plain) {
            if (String.IsNullOrEmpty(plain) || String.IsNullOrWhiteSpace(plain))
                throw new ArgumentNullException(nameof(plain));

            return new NetworkCredential(String.Empty, plain).SecurePassword;
        }

        public static String AsPlainString(this SecureString secure) {
            if (secure == null)
                throw new ArgumentNullException(nameof(secure));

            return new NetworkCredential(String.Empty, secure).Password;
        }

        const String Alphanumeric = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z a b c d e f g h i j k l m n o p q r s t u v w x y z 1 2 3 4 5 6 7 8 9 0 ~ ! @ # $ % ^ & * _ - + = ` | \\ ( ) { } [ ] : ; < > . ? /";

        // https://en.wikipedia.org/wiki/Feigenbaum_constants
        const Int32 Feigenbaum = 46692;

        static Int32 Ryandomize(this Int32 lowerBound, Int32 upperBound) {
            Int32 seed = Guid.NewGuid().GetHashCode() % Feigenbaum;
            var rnd = new Random(seed);
            return rnd.Next(lowerBound, upperBound);
        }

        public static Int32 Ryandomize(this Int32 upperBound) => 0.Ryandomize(upperBound);

        static String GetRandomAlphanumeric(Int32 length) {
            String[] charCombination = Alphanumeric.Split(' ');
            var output = new StringBuilder();
            for (Int32 ctr = 0; ctr < length; ctr++) {
                Int32 randomIdx = (charCombination.Length -1).Ryandomize();
                output.Append(charCombination[randomIdx]);
            }

            return output.ToString();
        }

        public static String GenerateKey() => EncodeBase64Url(GetRandomAlphanumeric(64));

        public static String GenerateSalt() => EncodeBase64Url(GetRandomAlphanumeric(128));
    }
}