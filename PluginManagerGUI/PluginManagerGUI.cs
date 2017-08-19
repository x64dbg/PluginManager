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
        private PluginRoot _root;
        private Dictionary<string, PluginContainer> _containerCache = new Dictionary<string, PluginContainer>();

        private PluginContainer DownloadContainer(string url, bool invalidateCache = false)
        {
            if (_containerCache.ContainsKey(url))
                return _containerCache[url];
            //TODO: sort versions
            return _containerCache[url] = Utils.Deserialize<PluginContainer>(Utils.DownloadString(url));
        }

        private Dictionary<string, byte[]> _downloadCache = new Dictionary<string, byte[]>();

        private byte[] DownloadData(string url, bool invalidateCache = false)
        {
            if (_downloadCache.ContainsKey(url))
                return _downloadCache[url];
            return _downloadCache[url] = Utils.DownloadBytes(url);
        }

        private string GetPluginHtml(PluginContainer plugin)
        {
            return $@"<h1>{plugin.Meta.Name}</h1>
<h3>Author: {plugin.Meta.Author}</h3>
<h3>Latest version: {plugin?.Versions?.LastOrDefault()?.Version}</h3>
<p>{plugin.Meta.Description}</p>";
        }

        public PluginManagerGUI()
        {
            InitializeComponent();
            listBoxPlugins.SelectedIndexChanged += listBoxPlugins_SelectedIndexChanged;
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        void listBoxPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            webBrowserDescription.DocumentText = GetPluginHtml(DownloadContainer(_root.Plugins[listBoxPlugins.SelectedIndex].Container));
        }

        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            listBoxPlugins.Focus();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            var rootJsonUrl = "https://github.com/x64dbg/PluginManager/raw/master/plugins/root.json";
            var rootJson = Utils.DownloadString(rootJsonUrl);

            /*var rootTest = new PluginRoot
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
            };*/
            RSAParameters rsaParams;
            if (File.Exists("privatekey.json"))
            {
                rsaParams = Utils.Deserialize<RSAParametersSerializable>(File.ReadAllText("privatekey.json")).RSAParameters;
            }
            else
            {
                var rsa = new RSACryptoServiceProvider(4096);
                rsaParams = rsa.ExportParameters(true);
                File.WriteAllText("privatekey.json", Utils.Serialize(new RSAParametersSerializable(rsaParams), true));
            }
            /*var pubKeyStr = "";
            var pubKey = new RSAParameters
            {
                Exponent = Convert.FromBase64String("AQAB"),
                Modulus = Convert.FromBase64String(pubKeyStr)
            };*/
            _root = Utils.Deserialize<PluginRoot>(rootJson);
            //var signature = root.Sign(rsaParams);
            //MessageBox.Show(signature, "Signature");
            //MessageBox.Show(root.Verify(signature, rsaParams) ? "yay" : "nay");

            listBoxPlugins.DataSource = _root.Plugins;
        }

        private void PerformInstall(PluginDelivery delivery, string dstDir)
        {
            Func<string, string> getUrlFileName = url =>
            {
                var urla = url.Replace('/', '\\');
                var idx = urla.LastIndexOf('\\');
                if (idx != -1)
                    return urla.Substring(idx + 1);
                return urla;
            };
            var tempMain = Path.GetTempPath();
            var fileName = getUrlFileName(delivery.Download);
            var tempDir = tempMain + fileName;
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
            Directory.CreateDirectory(tempDir);
            try
            {
                var data = DownloadData(delivery.Download);
                if (delivery.Sha256 != null)
                {
                    if (delivery.Sha256 != Utils.Sha256(data))
                        throw new InvalidDataException();
                }
                var downloadFile = tempDir + "\\" + fileName;
                File.WriteAllBytes(downloadFile, data);
                if (delivery.IsArchive) //TODO: rar/7z support?
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(downloadFile, tempDir);
                    File.Delete(downloadFile);
                }
                foreach (var action in delivery.Install)
                {
                    action.Perform(tempDir, dstDir);
                }
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            if (listBoxPlugins.SelectedIndex < 0)
                return;
            var pluginMeta = _root.Plugins[listBoxPlugins.SelectedIndex];
            var plugin = DownloadContainer(pluginMeta.Container);
            if (plugin.Meta.Author != pluginMeta.Author || plugin.Meta.Name != pluginMeta.Name)
                throw new InvalidDataException();
            //TODO: sort versions
            var version = plugin.Versions.Last();
            var dstDir = AppDomain.CurrentDomain.BaseDirectory + "\\x32";
            Directory.CreateDirectory(dstDir);
            foreach (var delivery in version.Delivery32)
                PerformInstall(delivery, dstDir);
            dstDir = AppDomain.CurrentDomain.BaseDirectory + "\\x64";
            Directory.CreateDirectory(dstDir);
            foreach (var delivery in version.Delivery64)
                PerformInstall(delivery, dstDir);
            MessageBox.Show("Done!");
        }

        private void linkLabelIcon_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://icons8.com/icon/5578/Puzzle");
        }
    }
}
