using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;


namespace Cryptograph
{

    internal static class ProtectorAES // Hash alhgoritm == SHA256 | Алгоритм шифрования - симметричный блочный AES
    {

        public static string PublicKey;

        private static readonly byte[] _salt = Encoding.Unicode.GetBytes("TadWhat"); // SALT

        private static readonly int _iterations = 2000;
         

        public static string Encrypt(string strForEncrypt, string password)
        {

            byte[] encryptedBytes;
            byte[] plainBytes = Encoding.Unicode.GetBytes(strForEncrypt);

            var aes = Aes.Create();

            var pbkdf2 = new Rfc2898DeriveBytes(password, _salt, _iterations);

            aes.Key = pbkdf2.GetBytes(32); //256 bit
            aes.IV = pbkdf2.GetBytes(16); // 128 bit

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {

                    cs.Write(plainBytes, 0, plainBytes.Length);
                }
                encryptedBytes = ms.ToArray();

            }
            return Convert.ToBase64String(encryptedBytes);
        }


        public static string Decrypt(string cryptoText, string password)
        {

            byte[] cryptoBytes = Convert.FromBase64String(cryptoText);
            byte[] plainBytes;

            var aes = Aes.Create();

            var pbkdf2 = new Rfc2898DeriveBytes(password, _salt, _iterations);

            aes.Key = pbkdf2.GetBytes(32); //256 bit
            aes.IV = pbkdf2.GetBytes(16); // 128 bit

            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {

                    cs.Write(cryptoBytes, 0, cryptoBytes.Length);
                }
                plainBytes = ms.ToArray();

            }
            return Encoding.Unicode.GetString(plainBytes);
        }


        public static User Register(string userName, string password)
        {

            //Генерация случайной соли
            var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            var saltText = Convert.ToBase64String(saltBytes);

            //генерация соленого и хешированого пароля
            var saltedHashPassword = SaltAndHashPassword(password, saltText);

            var user = new User()
            {
                Name = userName,
                Salt = saltText,
                SaltedHashedPassword = saltedHashPassword
            };
 
            return user;
        }


        public static bool CheckPassword(string password, User user)
        {
            
            // повторная генерация соленого и хешированого пароля
            var saltedHashPassword = SaltAndHashPassword(password, user.Salt);

            return saltedHashPassword == user.SaltedHashedPassword;

        }


        public static string GetSaltedHashPassword(string salt, string password) => SaltAndHashPassword(password, salt);
          

        private static string SaltAndHashPassword(string pwrd, string salt)
        {

            var sha = SHA256.Create();
            var saltedPassword = pwrd + salt;
            return Convert.ToBase64String(sha.ComputeHash(Encoding.Unicode.GetBytes(saltedPassword)));
        }


        //Подписывание цифровой подписи (открытый ключ)
        public static string ToXMLStringExt(this RSA rsa, bool includePrivateParameters)
        {

            var p = rsa.ExportParameters(includePrivateParameters);

            XElement xml;

            if (includePrivateParameters)
            {
                xml = new XElement("RSAKeyValue",
                    new XElement("Modulus", Convert.ToBase64String(p.Modulus)),
                    new XElement("Exponent", Convert.ToBase64String(p.Exponent)),
                    new XElement("P", Convert.ToBase64String(p.P)),
                    new XElement("Q", Convert.ToBase64String(p.Q)),
                    new XElement("DP", Convert.ToBase64String(p.DP)),
                    new XElement("InverseQ", Convert.ToBase64String(p.InverseQ))
                    );
            }
            else
            {
                xml = new XElement("RSAKeyValue",
                 new XElement("Modulus", Convert.ToBase64String(p.Modulus)),
                 new XElement("Exponent", Convert.ToBase64String(p.Exponent))
                 );
            }
            return xml?.ToString();
        }


        public static void FromXMLStringExt(this RSA rsa, string parametersAsXml)
        {

            var xml = XDocument.Parse(parametersAsXml);

            var root = xml.Element("RSAKeyValue");

            var p = new RSAParameters()
            {
                Modulus = Convert.FromBase64String(root.Element("Modulus").Value),
                Exponent = Convert.FromBase64String(root.Element("Exponent").Value),
            };


            if (root.Element("P") != null)
            {
                p.P = Convert.FromBase64String(root.Element("P").Value);
                p.Q = Convert.FromBase64String(root.Element("Q").Value);
                p.DP = Convert.FromBase64String(root.Element("DP").Value);
                p.DQ = Convert.FromBase64String(root.Element("DQ").Value);
                p.InverseQ = Convert.FromBase64String(root.Element("InverseQ").Value);
            }
            rsa.ImportParameters(p);
        }


        public static string GenerateSignature(string data)
        {

            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            var sha = SHA256.Create();
            var hashedData = sha.ComputeHash(dataBytes);
            var rsa = RSA.Create();

            PublicKey = rsa.ToXMLStringExt(false); // исключая приватный ключ

            return Convert.ToBase64String(rsa.SignHash(hashedData, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
        }


        public static bool ValidateSignature(string data, string signature)
        {

            byte[] dataBytes = Encoding.Unicode.GetBytes(data);
            var sha = SHA256.Create();

            var hashedData = sha.ComputeHash(dataBytes);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            var rsa = RSA.Create();

            rsa.FromXMLStringExt(PublicKey); // исключая приватный ключ

            return rsa.VerifyHash(hashedData, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}
