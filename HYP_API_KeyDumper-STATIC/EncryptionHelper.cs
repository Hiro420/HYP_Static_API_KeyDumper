using System.Security.Cryptography;

namespace HYP_API_KeyDumper_STATIC;

public static class EncryptionHelper
{
    private static byte[] keyBytes;
    private static byte[] ivBytes;

    static EncryptionHelper()
    {
        keyBytes = Convert.FromBase64String("1qck4mmSyJ+YQ10PKzdZ6+J6AuvUAR8TS/7AiIDNyTA=");
        ivBytes = new byte[16];
    }

    public static string ByteArrayToHexString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");
    }

    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    public static string DecodeAndDecrypt(string cipherText)
    {
        string DecodeAndDecrypt = AesDecrypt(StringToByteArray(cipherText));
        return (DecodeAndDecrypt);
    }

    public static string AesDecrypt(Byte[] inputBytes)
    {
        Byte[] outputBytes = inputBytes;

        string plaintext = string.Empty;

        using (MemoryStream memoryStream = new MemoryStream(outputBytes))
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, GetCryptoAlgorithm().CreateDecryptor(keyBytes, ivBytes), CryptoStreamMode.Read))
            {
                using (StreamReader srDecrypt = new StreamReader(cryptoStream))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }
        }

        Console.WriteLine($"Succesfully decrypted app.conf.dat file!");
        return plaintext;
    }

    private static Aes GetCryptoAlgorithm()
    {
        Aes algorithm = Aes.Create();
        algorithm.Padding = PaddingMode.PKCS7;
        algorithm.Mode = CipherMode.CBC;
        algorithm.KeySize = 128;
        algorithm.BlockSize = 128;

        return algorithm;
    }

}