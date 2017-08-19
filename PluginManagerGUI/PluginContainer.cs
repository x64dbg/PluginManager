using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace PluginManagerGUI
{
    public class PluginContainer
    {
        [JsonProperty("meta")]
        public PluginMeta Meta;

        [JsonProperty("versions")]
        public PluginVersion[] Versions;
    }

    public class PluginMeta
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name;

        [JsonProperty("author", Required = Required.Always)]
        public string Author;

        [JsonProperty("description")]
        public string Description;
    }

    public class PluginVersion
    {
        [JsonProperty("version", Required = Required.Always)]
        public string Version;

        [JsonProperty("x32")]
        public PluginDelivery[] Delivery32;

        [JsonProperty("x64")]
        public PluginDelivery[] Delivery64;
    }

    public class PluginDelivery
    {
        [JsonProperty("archive", Required = Required.Always)]
        public bool IsArchive;

        [JsonProperty("download", Required = Required.Always)]
        public string Download;

        [JsonProperty("sha256")]
        public string Sha256;

        [JsonProperty("install")]
        public PluginAction[] Install;

        [JsonProperty("uninstall")]
        public PluginAction[] Uninstall;
    }

    public class PluginAction
    {
        public enum Operation
        {
            Invalid,
            Copy,
            CopyOverwrite,
            CopyRecursive,
            CopyRecursiveOverwrite,
            Delete,
            Execute
        }

        [JsonProperty("action")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Operation Op;

        [JsonProperty("src")]
        public string Src;

        [JsonProperty("dst")]
        public string Dst;

        public void Perform(string srcDir, string dstDir)
        {
            if (Src.Contains("..") || Dst.Contains(".."))
                throw new InvalidOperationException();
            var srcNorm = Src.Replace('/', '\\');
            var dstNorm = Dst.Replace('/', '\\');
            var srcFull = (srcDir + "\\" + srcNorm).Replace("\\\\", "\\");
            var dstFull = (dstDir + "\\" + dstNorm).Replace("\\\\", "\\");
            var dstFullDir = Path.GetDirectoryName(dstFull);
            switch (Op)
            {
                case Operation.Copy:
                    Directory.CreateDirectory(dstFullDir);
                    File.Copy(srcFull, dstFull);
                    break;
            }
        }
    }
}
