namespace SİberGüvenlikTool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;

    internal class Program
    {
        static void Main(string[] args)
        {
        // Beyaz listeye alınmış BSSID'ler
        HashSet<string> whiteListedBSSIDs = new HashSet<string>
        {
            "00:11:22:33:44:55", // Beyaz listedeki BSSID örneği
            "66:77:88:99:AA:BB"  // Beyaz listedeki başka bir BSSID örneği
        };

        // Mevcut ağları tarama
        List<WiFiNetwork> networks = ScanWiFiNetworks();

        // Aynı SSID'ye sahip ağları kontrol et
        var duplicateSSIDs = networks.GroupBy(n => n.SSID)
                                     .Where(g => g.Count() > 1)
                                     .Select(g => new
                                     {
                                         SSID = g.Key,
                                         Networks = g.Where(n => !whiteListedBSSIDs.Contains(n.BSSID))
                                     })
                                     .Where(g => g.Networks.Count() > 1);

        // Sahte ağları tespit et ve bildir
        if (duplicateSSIDs.Any())
            if (duplicateSSIDs.Any())
            {
                Console.WriteLine("Sahte ağlar tespit edildi:");
                foreach (var group in duplicateSSIDs)
                {
                    Console.WriteLine($"Sahte SSID: {group.SSID}");
                    foreach (var network in group.Networks)
                    {
                        Console.WriteLine($"- BSSID: {network.BSSID}, Signal: {network.SignalStrength}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Herhangi bir sahte ağ tespit edilmedi.");
            }
    }

    static List<WiFiNetwork> ScanWiFiNetworks()
    {
        List<WiFiNetwork> networks = new List<WiFiNetwork>();

        // Komut satırı üzerinden ağları tarama (Windows için)
        Process process = new Process();
            process.StartInfo.FileName = "netsh";
            process.StartInfo.Arguments = "wlan show networks mode=bssid";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

        // Çıktıyı işleme ve ağları listeye ekleme
        var lines = output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        WiFiNetwork currentNetwork = null;

        foreach (var line in lines)
        {
            if (line.Contains("SSID"))
            {
                if (currentNetwork != null)
                {
                    networks.Add(currentNetwork);
                }

                currentNetwork = new WiFiNetwork();
                currentNetwork.SSID = line.Split(':')[1].Trim();
            }

            if (line.Contains("BSSID"))
            {
                if (currentNetwork != null)
                {
                    currentNetwork.BSSID = line.Split(':')[1].Trim();
                }
            }

            if (line.Contains("Sinyal"))
            {
                if (currentNetwork != null)
                {
                    currentNetwork.SignalStrength = line.Split(':')[1].Trim();
                }
            }
        }

        if (currentNetwork != null)
        {
            networks.Add(currentNetwork);
        }

        return networks;
    }
}
class WiFiNetwork
    {
        public string SSID { get; set; }
        public string BSSID { get; set; }
        public string SignalStrength { get; set; }
    }






