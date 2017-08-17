using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PluginManagerGUI
{
    public class PluginContainer
    {
        [DataMember(Name = "meta")]
        public PluginMeta Meta;

        [DataMember(Name = "versions")]
        public PluginVersion[] Versions;
    }

    public class PluginMeta
    {
        [DataMember(Name = "name", IsRequired = true)]
        public string Name;

        [DataMember(Name = "author", IsRequired = true)]
        public string Author;

        [DataMember(Name = "description")]
        public string Description;
    }

    public class PluginVersion
    {
        [DataMember(Name = "version", IsRequired = true)]
        public string Version;

        [DataMember(Name = "x32")]
        public PluginDelivery[] Delivery32;

        [DataMember(Name = "x64")]
        public PluginDelivery[] Delivery64;
    }

    public class PluginDelivery
    {
        [DataMember(Name = "archive", IsRequired = true)]
        public bool IsArchive;

        [DataMember(Name = "download", IsRequired = true)]
        public string Download;

        [DataMember(Name = "sha256")]
        public string Sha256;

        [DataMember(Name = "install")]
        public PluginAction[] Install;

        [DataMember(Name = "uninstall")]
        public PluginAction[] Uninstall;
    }

    public class PluginAction
    {
        public enum Operation
        {
            Copy,
            CopyOverwrite,
            CopyRecursive,
            CopyRecursiveOverwrite,
            Delete,
            Execute
        }

        [DataMember(Name = "action")]
        public Operation Op;

        [DataMember(Name = "src")]
        public string Src;

        [DataMember(Name = "dst")]
        public string Dst;
    }
}
