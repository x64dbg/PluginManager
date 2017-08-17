using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace PluginManagerGUI
{
    public partial class PluginManagerGUI : Form
    {
        public PluginManagerGUI()
        {
            InitializeComponent();
        }

        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            listViewPlugins.Focus();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            var rootJsonUrl = @"file://c:\CodeBlocks\plugins\root.json";
            var rootJson = Utils.DownloadString(rootJsonUrl);

            var rootTest = new PluginRoot
            {
                Plugins = new PluginRootMeta[]
                {
                    new PluginRootMeta
                    {
                        Author = "mrexodia",
                        Name = "Plugin One",
                        Container = @"file://c:\CodeBlocks\plugins\plugin1.json"
                    }
                },
                Authors = new PluginAuthor[]
                {
                    new PluginAuthor
                    {
                        Name = "mrexodia"
                    }
                },
                Signature = "signaturehax"
            };
            RSAParameters rsaParams;
            if (File.Exists("privatekey.json"))
            {
                rsaParams = Utils.Deserialize<RSAParametersSerializable>(File.ReadAllText("privatekey.json")).RSAParameters;
            }
            else
            {
                var rsa = new RSACryptoServiceProvider(4096);
                rsaParams = rsa.ExportParameters(true);
                File.WriteAllText("privatekey.json", Utils.Serialize(new RSAParametersSerializable(rsaParams)));
            }
            var root = Utils.Deserialize<PluginRoot>(rootJson);
            var signature = root.Sign(rsaParams);
            MessageBox.Show(signature, "Signature");
            MessageBox.Show(root.Verify(signature, rsaParams) ? "yay" : "nay");
        }
    }
}
