using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace PluginManagerGUI
{
    public abstract class Signable
    {
        [JsonProperty("signature", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Signature;

        private string GetMessage()
        {
            var oldSignature = Signature;
            try
            {
                Signature = "SIGNSIGNSIGN";
                return Utils.Serialize(this);
            }
            finally
            {
                Signature = oldSignature;
            }
        }

        public string Sign(RSAParameters privateKey)
        {
            return Utils.SignData(GetMessage(), privateKey);
        }

        public bool Verify(string signature, RSAParameters publicKey)
        {
            return Utils.VerifySignature(GetMessage(), signature, publicKey);
        }
    }

    public class PluginRoot : Signable
    {
        [JsonProperty("plugins")]
        public PluginRootMeta[] Plugins;

        [JsonProperty("authors")]
        public PluginAuthor[] Authors;
    }

    public class PluginRootMeta
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name;

        [JsonProperty("author", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Author;

        [JsonProperty("container", Required = Required.Always)]
        public string Container;

        [JsonProperty("trust-signatures", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, string> TrustSignatures; //version -> signature

        public override string ToString()
        {
            return Name;
        }
    }

    public class PluginAuthor
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name;

        [JsonProperty("pubkey", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Pubkey;

        [JsonProperty("trust-signature", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TrustSignature;
    }
}
