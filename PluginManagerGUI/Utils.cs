using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Runtime.Serialization;

namespace PluginManagerGUI
{
    public static class Utils
    {
        public static string Serialize<T>(T data, bool indent = false)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    Formatting = indent ? Formatting.Indented : Formatting.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
        }

        //Taken from: https://stackoverflow.com/q/8437288
        public static string SignData(string message, RSAParameters privateKey)
        {
            // The array to store the signed message in bytes
            byte[] signedBytes;
            using (var rsa = new RSACryptoServiceProvider())
            {
                // Write the message to a byte array using UTF8 as the encoding.
                byte[] originalData = Encoding.UTF8.GetBytes(message);

                try
                {
                    // Import the private key used for signing the message
                    rsa.ImportParameters(privateKey);

                    // Sign the data, using SHA256 as the hashing algorithm
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA256"));
                }
                finally
                {
                    // Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }
            // Convert the a base64 string before returning
            return Convert.ToBase64String(signedBytes);
        }

        public static bool VerifySignature(string originalMessage, string signature, RSAParameters publicKey)
        {
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                byte[] bytesToVerify = Encoding.UTF8.GetBytes(originalMessage);
                byte[] signatureBytes = Convert.FromBase64String(signature);
                try
                {
                    rsa.ImportParameters(publicKey);
                    var Hash = new SHA256Managed();
                    success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA256"), signatureBytes);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static byte[] DownloadBytes(string url)
        {
            if (url.StartsWith("file://"))
                return File.ReadAllBytes(url.Substring(7).Replace('/', '\\'));
            else
                throw new NotImplementedException();
        }

        public static string DownloadString(string url)
        {
            return Encoding.UTF8.GetString(DownloadBytes(url));
        }
    }

    [Serializable]
    public class RSAParametersSerializable : ISerializable
    {
        private RSAParameters _rsaParameters;

        public RSAParameters RSAParameters
        {
            get
            {
                return _rsaParameters;
            }
        }

        public RSAParametersSerializable(RSAParameters rsaParameters)
        {
            _rsaParameters = rsaParameters;
        }

        private RSAParametersSerializable()
        {
        }

        public byte[] D { get { return _rsaParameters.D; } set { _rsaParameters.D = value; } }

        public byte[] DP { get { return _rsaParameters.DP; } set { _rsaParameters.DP = value; } }

        public byte[] DQ { get { return _rsaParameters.DQ; } set { _rsaParameters.DQ = value; } }

        public byte[] Exponent { get { return _rsaParameters.Exponent; } set { _rsaParameters.Exponent = value; } }

        public byte[] InverseQ { get { return _rsaParameters.InverseQ; } set { _rsaParameters.InverseQ = value; } }

        public byte[] Modulus { get { return _rsaParameters.Modulus; } set { _rsaParameters.Modulus = value; } }

        public byte[] P { get { return _rsaParameters.P; } set { _rsaParameters.P = value; } }

        public byte[] Q { get { return _rsaParameters.Q; } set { _rsaParameters.Q = value; } }

        public RSAParametersSerializable(SerializationInfo information, StreamingContext context)
        {
            _rsaParameters = new RSAParameters()
            {
                D = (byte[])information.GetValue("D", typeof(byte[])),
                DP = (byte[])information.GetValue("DP", typeof(byte[])),
                DQ = (byte[])information.GetValue("DQ", typeof(byte[])),
                Exponent = (byte[])information.GetValue("Exponent", typeof(byte[])),
                InverseQ = (byte[])information.GetValue("InverseQ", typeof(byte[])),
                Modulus = (byte[])information.GetValue("Modulus", typeof(byte[])),
                P = (byte[])information.GetValue("P", typeof(byte[])),
                Q = (byte[])information.GetValue("Q", typeof(byte[]))
            };
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("D", _rsaParameters.D);
            info.AddValue("DP", _rsaParameters.DP);
            info.AddValue("DQ", _rsaParameters.DQ);
            info.AddValue("Exponent", _rsaParameters.Exponent);
            info.AddValue("InverseQ", _rsaParameters.InverseQ);
            info.AddValue("Modulus", _rsaParameters.Modulus);
            info.AddValue("P", _rsaParameters.P);
            info.AddValue("Q", _rsaParameters.Q);
        }
    }
}
