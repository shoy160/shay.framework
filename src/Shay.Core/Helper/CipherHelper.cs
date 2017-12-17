using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shay.Core.Helper
{
    /// <summary>
    /// 加密和解密
    /// </summary>
    public static class CipherHelper
    {
        /// <summary>
        /// Hash 加密采用的算法
        /// </summary>
        public enum HashFormat
        {
            MD516,
            MD532,
            //RIPEMD160,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        /// <summary>
        /// 基于密钥的 Hash 加密采用的算法
        /// </summary>
        public enum HmacFormat
        {
            HMACMD5,
            //HMACRIPEMD160,
            HMACSHA1,
            HMACSHA256,
            HMACSHA384,
            HMACSHA512
        }

        /// <summary>
        /// 对称加密采用的算法
        /// </summary>
        public enum SymmetricFormat
        {
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:8 Max:8 Skip:0），IV（8）
            /// </summary>
            DES,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:16 Max:24 Skip:8），IV（8）
            /// </summary>
            TripleDES,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:5 Max:16 Skip:1），IV（8）
            /// </summary>
            RC2,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:16 Max:32 Skip:8），IV（16）
            /// </summary>
            Rijndael,
            /// <summary>
            /// 有效的 KEY 与 IV 长度，以英文字符为单位： KEY（Min:16 Max:32 Skip:8），IV（16）
            /// </summary>
            AES
        }

        /// <summary>
        /// 随机生成一个 8 位密码，包括大小写字母，数字和特殊符号。
        /// </summary>
        public static string CreateRandom()
        {
            return CreateRandom(8);
        }

        /// <summary>
        /// 随机生成一个指定长度的密码，包括大小写字母，数字和特殊符号。
        /// </summary>
        public static string CreateRandom(int len)
        {
            string str = @"ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnpqrstuvwxyz123456789!@#$%^&*";

            return CreateRandom(str, len);
        }

        /// <summary>
        /// 随机生成一个指定长度的密码，仅包括指定的字符。
        /// </summary>
        public static string CreateRandom(string str, int len)
        {
            if (string.IsNullOrEmpty(str)) throw new ArgumentNullException("str");

            if (len < 1) throw new ArgumentOutOfRangeException("len");

            Random random = new Random(Guid.NewGuid().GetHashCode());

            int strLen = str.Length;

            StringBuilder pass = new StringBuilder(len);

            for (int i = 0; i < len; i++)
            {
                pass.Append(str.Substring(random.Next(strLen), 1));
            }

            return pass.ToString();
        }

        /// <summary>
        /// 对字符串进行 Hash 加密
        /// </summary>
        public static string Hash(string inputString, HashFormat hashFormat = HashFormat.SHA1)
        {
            HashAlgorithm algorithm = GetHashAlgorithm(hashFormat);

            algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            if (hashFormat == HashFormat.MD516) return BitConverter.ToString(algorithm.Hash).Replace("-", "").Substring(8, 16).ToUpper();

            return BitConverter.ToString(algorithm.Hash).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// 对字符串进行基于密钥的 Hash 加密
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="key">密钥的长度不限，建议的密钥长度为 64 个英文字符。</param>
        /// <param name="hashFormat"></param>
        /// <returns></returns>
        public static string Hmac(string inputString, string key, HmacFormat hashFormat = HmacFormat.HMACSHA1)
        {
            HMAC algorithm = GetHmac(hashFormat, Encoding.ASCII.GetBytes(key));

            algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            return BitConverter.ToString(algorithm.Hash).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// 对字符串进行对称加密
        /// </summary>
        public static string SymmetricEncrypt(string inputString, SymmetricFormat symmetricFormat, string key, string iv)
        {
            SymmetricAlgorithm algorithm = GetSymmetricAlgorithm(symmetricFormat);

            byte[] desString = Encoding.UTF8.GetBytes(inputString);

            byte[] desKey = Encoding.ASCII.GetBytes(key);

            byte[] desIV = Encoding.ASCII.GetBytes(iv);

            if (!algorithm.ValidKeySize(desKey.Length * 8)) throw new ArgumentOutOfRangeException("key");

            if (algorithm.IV.Length != desIV.Length) throw new ArgumentOutOfRangeException("iv");

            MemoryStream mStream = new MemoryStream();

            CryptoStream cStream = new CryptoStream(mStream, algorithm.CreateEncryptor(desKey, desIV), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// 对字符串进行对称解密
        /// </summary>
        public static string SymmetricDecrypt(string inputString, SymmetricFormat symmetricFormat, string key, string iv)
        {
            SymmetricAlgorithm algorithm = GetSymmetricAlgorithm(symmetricFormat);

            byte[] desString = Convert.FromBase64String(inputString);

            byte[] desKey = Encoding.ASCII.GetBytes(key);

            byte[] desIV = Encoding.ASCII.GetBytes(iv);

            MemoryStream mStream = new MemoryStream();

            CryptoStream cStream = new CryptoStream(mStream, algorithm.CreateDecryptor(desKey, desIV), CryptoStreamMode.Write);

            cStream.Write(desString, 0, desString.Length);

            cStream.FlushFinalBlock();

            cStream.Close();

            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        /// <summary>
        /// 使用 RSA 公钥加密
        /// </summary>
        public static string RsaEncrypt(string message, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                byte[] resultBytes = rsa.Encrypt(messageBytes, false);

                return Convert.ToBase64String(resultBytes);
            }
        }

        /// <summary>
        /// 使用 RSA 私钥解密
        /// </summary>
        public static string RsaDecrypt(string message, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);

                byte[] messageBytes = Convert.FromBase64String(message);

                byte[] resultBytes = rsa.Decrypt(messageBytes, false);

                return Encoding.UTF8.GetString(resultBytes);
            }
        }

        /// <summary>
        /// 使用 RSA 私钥签名
        /// </summary>
        public static string RsaSignature(string message, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                byte[] resultBytes = rsa.SignData(messageBytes, "SHA1");

                return Convert.ToBase64String(resultBytes);
            }
        }

        /// <summary>
        /// 使用 RSA 公钥验证签名
        /// </summary>
        public static bool RsaVerifySign(string message, string signature, string publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                byte[] messageBytes = Encoding.UTF8.GetBytes(message);

                byte[] signatureBytes = Convert.FromBase64String(signature);

                return rsa.VerifyData(messageBytes, "SHA1", signatureBytes);
            }
        }

        /// <summary>
        /// 为 RSA 非对称加密生成密钥对，并存储到文件
        /// </summary>
        public static void CreateKeyFileForAsymmetricAlgorithm(string publicFileName, string privateFileName)
        {
            if (string.IsNullOrEmpty(publicFileName)) throw new ArgumentNullException("publicFileName");

            if (string.IsNullOrEmpty(privateFileName)) throw new ArgumentNullException("privateFileName");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                File.WriteAllText(publicFileName, rsa.ToXmlString(false));
                File.WriteAllText(privateFileName, rsa.ToXmlString(true));
            }
        }

        /// <summary>
        /// 为非对称加密从文件读取密钥
        /// </summary>
        public static string GetKeyFromFileForAsymmetricAlgorithm(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            return (File.ReadAllText(fileName));
        }

        /// <summary>
        /// 获取 Hash 加密方法
        /// </summary>
        private static HashAlgorithm GetHashAlgorithm(HashFormat hashFormat)
        {
            HashAlgorithm algorithm = null;

            switch (hashFormat)
            {
                case HashFormat.MD516:
                    algorithm = MD5.Create();
                    break;
                case HashFormat.MD532:
                    algorithm = MD5.Create();
                    break;
                //case HashFormat.RIPEMD160:
                //    algorithm = RIPEMD160.Create();
                //    break;
                case HashFormat.SHA1:
                    algorithm = SHA1.Create();
                    break;
                case HashFormat.SHA256:
                    algorithm = SHA256.Create();
                    break;
                case HashFormat.SHA384:
                    algorithm = SHA384.Create();
                    break;
                case HashFormat.SHA512:
                    algorithm = SHA512.Create();
                    break;
            }

            return algorithm;
        }

        /// <summary>
        /// 获取基于密钥的 Hash 加密方法
        /// </summary>
        private static HMAC GetHmac(HmacFormat hmacFormat, byte[] key)
        {
            HMAC hmac = null;

            switch (hmacFormat)
            {
                case HmacFormat.HMACMD5:
                    hmac = new HMACMD5(key);
                    break;
                //case HmacFormat.HMACRIPEMD160:
                //    hmac = new HMACRIPEMD160(key);
                //    break;
                case HmacFormat.HMACSHA1:
                    hmac = new HMACSHA1(key);
                    break;
                case HmacFormat.HMACSHA256:
                    hmac = new HMACSHA256(key);
                    break;
                case HmacFormat.HMACSHA384:
                    hmac = new HMACSHA384(key);
                    break;
                case HmacFormat.HMACSHA512:
                    hmac = new HMACSHA512(key);
                    break;
            }

            return hmac;
        }

        /// <summary>
        /// 获取对称加密方法
        /// </summary>
        private static SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricFormat symmetricFormat)
        {
            SymmetricAlgorithm algorithm = null;

            switch (symmetricFormat)
            {
                case SymmetricFormat.DES:
                    algorithm = DES.Create();
                    break;
                case SymmetricFormat.TripleDES:
                    algorithm = TripleDES.Create();
                    break;
                case SymmetricFormat.RC2:
                    algorithm = RC2.Create();
                    break;
                case SymmetricFormat.Rijndael:
                    algorithm = Rijndael.Create();
                    break;
                case SymmetricFormat.AES:
                    algorithm = Aes.Create();
                    break;
            }

            return algorithm;
        }
    }
}