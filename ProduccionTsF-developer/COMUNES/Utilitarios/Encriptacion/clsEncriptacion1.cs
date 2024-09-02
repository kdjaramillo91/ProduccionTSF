using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilitarios.Encriptacion
{
    public static class clsEncriptacion1
    {
        
        public static class LeadnjirSimple
        {
            #region Encriptar
            /// <summary>
            /// Método para encriptar un texto plano usando el algoritmo (Leadnjir).
            /// Este es el mas simple posible, muchos de los datos necesarios los
            /// definimos como constantes.
            /// </summary>
            /// <param name="textoQueEncriptaremos">texto a encriptar</param>
            /// <returns>Texto encriptado</returns>
            public static string Encriptar(string textoQueEncriptaremos)
            {
                return Encriptar(textoQueEncriptaremos,
                  "pass75dc@avz10", "s@lAvz", "MD5", 1, "@1B2c3D4e5F6g7H8", 128);
            }
            /// <summary>
            /// Método para encriptar un texto plano usando el algoritmo (Leadnjir)
            /// </summary>
            /// <returns>Texto encriptado</returns>
            public static string Encriptar(string textoQueEncriptaremos,
              string passBase, string saltValue, string hashAlgorithm,
              int passwordIterations, string initVector, int keySize)
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(textoQueEncriptaremos);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passBase,
                  saltValueBytes, hashAlgorithm, passwordIterations);
                byte[] keyBytes = password.GetBytes(keySize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged()
                {
                    Mode = CipherMode.CBC
                };
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes,
                  initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor,
                 CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                string cipherText = Convert.ToBase64String(cipherTextBytes);
                return cipherText;
            }
            #endregion

            #region Desencriptar
            /// <summary>
            /// Método para desencriptar un texto encriptado.
            /// </summary>
            /// <returns>Texto desencriptado</returns>
            public static string Desencriptar(string textoEncriptado)
            {
                return Desencriptar(textoEncriptado.Trim(), "pass75dc@avz10", "s@lAvz", "MD5",
                  1, "@1B2c3D4e5F6g7H8", 128);
            }
            /// <summary>
            /// Método para desencriptar un texto encriptado (Leadnjir)
            /// </summary>
            /// <returns>Texto desencriptado</returns>
            public static string Desencriptar(string textoEncriptado, string passBase,
              string saltValue, string hashAlgorithm, int passwordIterations,
              string initVector, int keySize)
            {
                byte[] reducedArrayCipherTextBytes = new byte[16];
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] cipherTextBytes =   FromBase64String(textoEncriptado);
                if (cipherTextBytes.Length > 16)
                {
                    Array.Copy(cipherTextBytes, 0, reducedArrayCipherTextBytes, 0, 16);
                }
                else
                {
                    reducedArrayCipherTextBytes = cipherTextBytes;
                }
                    
                //byte[] cipherTextBytes = Convert.FromBase64String(textoEncriptado);
                PasswordDeriveBytes password = new PasswordDeriveBytes(passBase,
                  saltValueBytes, hashAlgorithm, passwordIterations);
                byte[] keyBytes = password.GetBytes(keySize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged()
                {
                    Mode = CipherMode.CBC
                };
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes,
                  initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(reducedArrayCipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor,
                  CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[reducedArrayCipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0,
                  plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                string plainText = Encoding.UTF8.GetString(plainTextBytes, 0,
                  decryptedByteCount);
                return plainText;
            }

            private static byte[] FromBase64String(string base64String)
            {
                string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
                List<byte> bytes = new List<byte>();

                int buffer = 0, bufferLength = 0;

                foreach (char c in base64String)
                {
                    if (base64Chars.IndexOf(c) == -1)
                        throw new FormatException("Input string is not a valid Base64 string.");

                    buffer = (buffer << 6) | base64Chars.IndexOf(c);
                    bufferLength += 6;

                    if (bufferLength >= 8)
                    {
                        bufferLength -= 8;
                        bytes.Add((byte)((buffer >> bufferLength) & 0xFF));
                    }
                }

                return bytes.ToArray();
            }
            #endregion
        }
    }
}
