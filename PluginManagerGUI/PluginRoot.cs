using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace PluginManagerGUI
{
    public abstract class Signable
    {
        [DataMember(Name = "signature", EmitDefaultValue = false)]
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
        [DataMember(Name = "plugins")]
        public PluginRootMeta[] Plugins;

        [DataMember(Name = "authors")]
        public PluginAuthor[] Authors;
    }

    public class PluginRootMeta
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Name;

        [DataMember(Name = "author", EmitDefaultValue = false)]
        public string Author;

        [DataMember(Name = "container", IsRequired = true)]
        public string Container;

        [DataMember(Name = "trust-signatures", EmitDefaultValue = false)]
        public Dictionary<string, string> TrustSignatures; //version -> signature
    }

    public class PluginAuthor
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Name;

        [DataMember(Name = "pubkey", IsRequired = false, EmitDefaultValue = false)]
        public string Pubkey;

        [DataMember(Name = "trust-signature", EmitDefaultValue = false)]
        public string TrustSignature;
    }
}
