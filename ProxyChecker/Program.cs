using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProxyChecker
{
    class Program
    {
        static async Task ScrapeProxyLinksAndSaveAsync(List<string> links, string outputFile)
        {
            List<string> proxies = new List<string>();
            int numThreads = 100;

            using (HttpClient httpClient = new HttpClient())
            {
                var tasks = new List<Task<List<string>>>();

                foreach (var link in links)
                {
                    tasks.Add(GetProxiesFromLinkAsync(httpClient, link));
                }

                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    proxies.AddRange(task.Result);
                }
            }

            using (StreamWriter fileWriter = new StreamWriter(outputFile))
            {
                foreach (var proxy in proxies)
                {
                    if (proxy.Contains(":") && !proxy.Any(char.IsLetter))
                    {
                        fileWriter.WriteLine(proxy);
                    }
                }
            }
            
        }
        static async Task<List<string>> ScrapeProxyLinksAsync(List<string> links)
        {
            List<string> proxies = new List<string>();
            int numThreads = 100;

            using (HttpClient httpClient = new HttpClient())
            {
                var tasks = new List<Task<List<string>>>();

                foreach (var link in links)
                {
                    tasks.Add(GetProxiesFromLinkAsync(httpClient, link));
                }

                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    proxies.AddRange(task.Result);
                }
            }
            return proxies;

        }
        static async Task<List<string>> GetProxiesFromLinkAsync(HttpClient httpClient, string link)
        {
            List<string> proxies = new List<string>();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(link);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    proxies.AddRange(content.Split('\n'));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching proxies from {link}: {ex.Message}");
            }

            return proxies;
        }
        private class WebClient : System.Net.WebClient
        {
            public int Timeout { get; set; } = 1000;

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest lWebRequest = base.GetWebRequest(uri);
                lWebRequest.Timeout = Timeout;
                if (lWebRequest is HttpWebRequest httpWebRequest)
                {
                    httpWebRequest.ReadWriteTimeout = Timeout;
                }
                return lWebRequest;
            }
        }

        public static bool ProxyCheck(string ipAddressport)
        {
            string[] data = ipAddressport.Split(':');
            int port = 0;
            try
            {
                port = int.Parse(data[1]);
            }
            catch
            {
                return false;
            }
            try
            {
                bool goog = false;
                bool ip = false;
                bool github = false;
                Task t1 = new Task(() => {
                    try
                    {
                        IWebProxy proxy = new WebProxy(data[0], port);
                        WebClient wc = new WebClient();
                        wc.Timeout = 3500;
                        wc.Proxy = proxy;
                        wc.Encoding = Encoding.UTF8;
                        wc.DownloadString("https://google.com");
                        goog = true;
                    }
                    catch (Exception ex)
                    {
                        goog = false;
                    }
                });
                Task t2 = new Task(() => {
                    try
                    {
                        IWebProxy proxy = new WebProxy(data[0], port);
                        WebClient wc = new WebClient();
                        wc.Timeout = 3500;
                        wc.Proxy = proxy;
                        wc.Encoding = Encoding.UTF8;
                        wc.DownloadString("http://ip-api.com/line/?fields=8192");
                        ip = true;
                    }
                    catch (Exception ex)
                    {
                        ip = false;
                    }
                });
                Task t3 = new Task(() =>
                {

                    try
                    {
                        IWebProxy proxy = new WebProxy(data[0], port);
                        WebClient wc = new WebClient();
                        wc.Timeout = 3500;
                        wc.Proxy = proxy;
                        wc.Encoding = Encoding.UTF8;
                        wc.DownloadString("https://raw.githubusercontent.com/Tuaxa/PSAC/main/ver");
                        github = true;
                    }
                    catch (Exception ex)
                    {
                        github = false;
                    }
                });
                //t1.Start();
                //t2.Start();
                //t3.Start();
                //t1.Wait();
                //t2.Wait();
                //t3.Wait();
                try
                {
                    IWebProxy proxy = new WebProxy(data[0], port);
                    WebClient wc = new WebClient();
                    wc.Timeout = 3500;
                    wc.Proxy = proxy;
                    wc.Encoding = Encoding.UTF8;
                    wc.DownloadString("http://ip-api.com/line/?fields=8192");
                    ip = true;
                    return true;
                }
                catch (Exception ex)
                {
                    ip = false;
                }
                bool aa = false;
                if(github || ip || goog) { aa = true; }
                return aa;
            }
            catch
            {
                return false;
            }
        }
        static async Task Main(string[] args)
        {
            List<string> httpLinks = new List<string>
        {
    "https://api.proxyscrape.com/?request=getproxies&proxytype=https&timeout=10000&country=all&ssl=all&anonymity=all",
    "https://api.proxyscrape.com/v2/?request=getproxies&protocol=http&timeout=10000&country=all&ssl=all&anonymity=all",
    "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/http.txt",
    "https://raw.githubusercontent.com/MuRongPIG/Proxy-Master/main/http.txt",
    "https://raw.githubusercontent.com/prxchk/proxy-list/main/http.txt",
    "https://raw.githubusercontent.com/saisuiu/Lionkings-Http-Proxys-Proxies/main/cnfree.txt",
    "https://raw.githubusercontent.com/Anonym0usWork1221/Free-Proxies/main/proxy_files/http_proxies.txt",
    "https://raw.githubusercontent.com/Anonym0usWork1221/Free-Proxies/main/proxy_files/https_proxies.txt",
    "https://raw.githubusercontent.com/roosterkid/openproxylist/main/HTTPS_RAW.txt",
    "https://raw.githubusercontent.com/officialputuid/KangProxy/KangProxy/https/https.txt",
    "https://raw.githubusercontent.com/officialputuid/KangProxy/KangProxy/http/http.txt"
        };

            List<string> socks4List = new List<string>
        {
    "https://api.proxyscrape.com/v2/?request=displayproxies&protocol=socks4",
    "https://api.proxyscrape.com/?request=displayproxies&proxytype=socks4&country=all",
    "https://api.openproxylist.xyz/socks4.txt",
    "https://proxyspace.pro/socks4.txt",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies/socks4.txt",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies_anonymous/socks4.txt",
    "https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies-socks4.txt",
    "https://raw.githubusercontent.com/ShiftyTR/Proxy-List/master/socks4.txt",
    "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks4.txt",
    "https://raw.githubusercontent.com/roosterkid/openproxylist/main/SOCKS4_RAW.txt",
    "https://proxyspace.pro/socks4.txt",
    "https://www.proxy-list.download/api/v1/get?type=socks4",
    "https://raw.githubusercontent.com/HyperBeats/proxy-list/main/socks4.txt",
    "https://raw.githubusercontent.com/mmpx12/proxy-list/master/socks4.txt",
    "https://raw.githubusercontent.com/saschazesiger/Free-Proxies/master/proxies/socks4.txt",
    "https://raw.githubusercontent.com/B4RC0DE-TM/proxy-list/main/SOCKS4.txt",
    "https://raw.githubusercontent.com/rdavydov/proxy-list/main/proxies/socks4.txt",
    "https://raw.githubusercontent.com/rdavydov/proxy-list/main/proxies_anonymous/socks4.txt",
    "https://raw.githubusercontent.com/zevtyardt/proxy-list/main/socks4.txt",
    "https://raw.githubusercontent.com/MuRongPIG/Proxy-Master/main/socks4.txt",
    "https://raw.githubusercontent.com/Zaeem20/FREE_PROXIES_LIST/master/socks4.txt",
    "https://raw.githubusercontent.com/prxchk/proxy-list/main/socks4.txt",
    "https://raw.githubusercontent.com/ALIILAPRO/Proxy/main/socks4.txt",
    "https://raw.githubusercontent.com/zloi-user/hideip.me/main/socks4.txt",
    "https://www.proxyscan.io/download?type=socks4,"
        };

            List<string> socks5List = new List<string>
        {
    "https://raw.githubusercontent.com/B4RC0DE-TM/proxy-list/main/SOCKS5.txt",
    "https://raw.githubusercontent.com/saschazesiger/Free-Proxies/master/proxies/socks5.txt",
    "https://raw.githubusercontent.com/mmpx12/proxy-list/master/socks5.txt",
    "https://raw.githubusercontent.com/HyperBeats/proxy-list/main/socks5.txt",
    "https://api.openproxylist.xyz/socks5.txt",
    "https://api.proxyscrape.com/?request=displayproxies&proxytype=socks5",
    "https://api.proxyscrape.com/v2/?request=displayproxies&protocol=socks5",
    "https://proxyspace.pro/socks5.txt",
    "https://raw.githubusercontent.com/manuGMG/proxy-365/main/SOCKS5.txt",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies/socks5.txt",
    "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies_anonymous/socks5.txt",
    "https://raw.githubusercontent.com/ShiftyTR/Proxy-List/master/socks5.txt",
    "https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies-socks5.txt",
    "https://raw.githubusercontent.com/roosterkid/openproxylist/main/SOCKS5_RAW.txt",
    "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks5.txt",
    "https://raw.githubusercontent.com/hookzof/socks5_list/master/proxy.txt",
    "https://raw.githubusercontent.com/rdavydov/proxy-list/main/proxies/socks5.txt",
    "https://raw.githubusercontent.com/rdavydov/proxy-list/main/proxies_anonymous/socks5.txt",
    "https://raw.githubusercontent.com/zevtyardt/proxy-list/main/socks5.txt",
    "https://raw.githubusercontent.com/MuRongPIG/Proxy-Master/main/socks5.txt",
    "https://raw.githubusercontent.com/Zaeem20/FREE_PROXIES_LIST/master/socks5.txt",
    "https://raw.githubusercontent.com/prxchk/proxy-list/main/socks5.txt",
    "https://raw.githubusercontent.com/ALIILAPRO/Proxy/main/socks5.txt",
    "https://spys.me/socks.txt",
    "https://raw.githubusercontent.com/zloi-user/hideip.me/main/socks5.txt"
        };
            string version = "0A";
            Console.Title = "Loading - Proxy Scraper & Checker - Made by tuaxa";
            Console.WriteLine("Made by tuaxa");
            Console.WriteLine("https://tuaxascripts.com");
            Console.WriteLine("Version : "+ version);
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Loading ...");
            //Check version
            bool vers = false;
            try
            {
                WebClient wc = new WebClient();
                if(wc.DownloadString("https://raw.githubusercontent.com/Tuaxa/PSAC/main/ver").Trim().ToLower().Replace(" ", "") == version.ToLower().Replace(" ",""))
                {
                    vers = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (vers)
            {
                restart:
                Console.Title = "Waiting - Proxy Scraper & Checker - Made by tuaxa";
                Console.Clear();
                Console.WriteLine("Made by tuaxa");
                Console.WriteLine("https://tuaxascripts.com");
                Console.WriteLine("Version : " + version);
                Console.WriteLine("----------------------------------------------------");
                Console.WriteLine("Choices :");
                Console.WriteLine("[1] Scrape & Check Proxy");
                Console.WriteLine("[2] Scrape Proxy");
                Console.WriteLine("[3] Check Proxy");
                ConsoleKeyInfo choice = Console.ReadKey();
                if(choice.KeyChar == '1')
                {
                    List<string> validHttp = await ScrapeProxyLinksAsync(httpLinks);
                    List<string> validSocks4 = await ScrapeProxyLinksAsync(socks4List);
                    List<string> validSocks5 = await ScrapeProxyLinksAsync(socks5List);

                    Directory.CreateDirectory("Results");
                    File.WriteAllText("Results/good_http.txt", "");
                    File.WriteAllText("Results/bad_http.txt", "");

                    File.WriteAllText("Results/good_socks4.txt", "");
                    File.WriteAllText("Results/bad_socks4.txt", "");

                    File.WriteAllText("Results/good_socks5.txt", "");
                    File.WriteAllText("Results/bad_socks5.txt", "");

                    //HTTP
                    int tb = 0;
                    int tg = 0;

                    int bad = 0;
                    int good = 0;
                    foreach (string proxy in validHttp)
                    {
                        try
                        {
                            if (ProxyCheck(proxy) == true)
                            {
                                using (StreamWriter sw = File.AppendText("Results/good_http.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                good++;
                            }
                            else
                            {
                                using (StreamWriter sw = File.AppendText("Results/bad_http.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                bad++;
                            }
             
                        Console.Clear();
                        Console.WriteLine("Made by tuaxa");
                        Console.WriteLine("https://tuaxascripts.com");
                        Console.WriteLine("Version : " + version);
                        Console.WriteLine("----------------------------------------------------");
                        Console.WriteLine(" ");
                        Console.WriteLine("Http");
                        Console.WriteLine(" ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Good: " + good);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Bad: " + bad);
                        Console.ForegroundColor = ConsoleColor.White;
                        }
                        catch (Exception ex) { }
                    }
                    tg += good;
                    tb += bad;
                    //SOCKS4
                    bad = 0;
                    good = 0;
                    foreach (string proxy in validSocks4)
                    {
                        try
                        {
                            if (ProxyCheck(proxy) == true)
                            {
                                using (StreamWriter sw = File.AppendText("Results/good_socks4.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                good++;
                            }
                            else
                            {
                                using (StreamWriter sw = File.AppendText("Results/bad_socks4.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                bad++;
                            }

                            Console.Clear();
                            Console.WriteLine("Made by tuaxa");
                            Console.WriteLine("https://tuaxascripts.com");
                            Console.WriteLine("Version : " + version);
                            Console.WriteLine("----------------------------------------------------");
                            Console.WriteLine(" ");
                            Console.WriteLine("Http");
                            Console.WriteLine(" ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Good: " + good);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Bad: " + bad);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        catch (Exception ex) { }
                    }
                    tg += good;
                    tb += bad;
                    //SOCKS5
                    bad = 0;
                    good = 0;
                    foreach (string proxy in validSocks5)
                    {
                        try
                        {
                            if (ProxyCheck(proxy) == true)
                            {
                                using (StreamWriter sw = File.AppendText("Results/good_socks5.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                good++;
                            }
                            else
                            {
                                using (StreamWriter sw = File.AppendText("Results/bad_socks5.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                bad++;
                            }

                            Console.Clear();
                            Console.WriteLine("Made by tuaxa");
                            Console.WriteLine("https://tuaxascripts.com");
                            Console.WriteLine("Version : " + version);
                            Console.WriteLine("----------------------------------------------------");
                            Console.WriteLine(" ");
                            Console.WriteLine("Http");
                            Console.WriteLine(" ");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Good: " + good);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Bad: " + bad);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        catch (Exception ex) { }
                    }
                    tg += good;
                    tb += bad;

                    Console.Clear();
                    Console.WriteLine("Made by tuaxa");
                    Console.WriteLine("https://tuaxascripts.com");
                    Console.WriteLine("Version : " + version);
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine(" ");
                    Console.WriteLine("Total :");
                    Console.WriteLine(" ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Good: " + tg);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bad: " + tb);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Saved to Results");
                    Thread.Sleep(3000);
                    goto restart;
                }
                if (choice.KeyChar == '2')
                {
                    Console.Title = "Scraping - Proxy Scraper & Checker - Made by tuaxa";
                    Console.Clear();
                    Console.WriteLine("Made by tuaxa");
                    Console.WriteLine("https://tuaxascripts.com");
                    Console.WriteLine("Version : " + version);
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine("Logs:\n");
                    Console.WriteLine("Scraping Http Proxies");
                    await ScrapeProxyLinksAndSaveAsync(httpLinks, "http_proxies.txt");
                    Console.WriteLine("Scraped And Saved Http Proxies\nSaved to " + Directory.GetCurrentDirectory()+ @"\http_proxies.txt" + "\n");
                    Console.WriteLine("Scraping Socks4 Proxies");
                    await ScrapeProxyLinksAndSaveAsync(socks4List, "socks4_proxies.txt");
                    Console.WriteLine("Scraped And Saved Socks4 Proxies\nSaved to "+Directory.GetCurrentDirectory()+ @"\socks4_proxies.txt" + "\n");
                    Console.WriteLine("Scraping Socks5 Proxies");
                    await ScrapeProxyLinksAndSaveAsync(socks5List, "socks5_proxies.txt");
                    Console.WriteLine("Scraped And Saved Socks5 Proxies\nSaved to " + Directory.GetCurrentDirectory()+ @"\socks5_proxies.txt" + "\n");
                    Console.WriteLine("Finished.");
                    Thread.Sleep(3000);
                    Console.Clear();
                    goto restart;
                }
                if (choice.KeyChar == '3')
                {
                    restart3:
                    Console.Title = "Checking - Proxy Scraper & Checker - Made by tuaxa";
                    Console.Clear();
                    Console.WriteLine("Made by tuaxa");
                    Console.WriteLine("https://tuaxascripts.com");
                    Console.WriteLine("Version : " + version);
                    Console.WriteLine("----------------------------------------------------");
                    Console.WriteLine("Please give proxy file :");
                    
                    string fn = Console.ReadLine();
                    if(File.Exists(fn)) { 
                    
                    } else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong file!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Thread.Sleep(3000);
                        goto restart3;
                    }
                    Console.Clear();
                    string[] proxies = File.ReadAllLines(fn);
                    if (File.Exists("good.txt")) { File.Delete("good.txt"); File.Create("bad.txt").Dispose(); } else { File.Create("bad.txt").Dispose(); ; }
                    if (File.Exists("bad.txt")) { File.Delete("bad.txt"); File.Create("bad.txt").Dispose(); } else { File.Create("bad.txt").Dispose(); }
                    int good = 0;
                    int bad = 0;
                    int thrs = 0;
                    foreach (string proxy in proxies)
                    {
                        try
                        {
                            if (ProxyCheck(proxy) == true)
                            {
                                using (StreamWriter sw = File.AppendText("good.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                good++;
                            }
                            else
                            {
                                using (StreamWriter sw = File.AppendText("bad.txt"))
                                {
                                    sw.WriteLine(proxy);
                                }
                                bad++;
                            }
                            Console.Clear();
                            Console.WriteLine("Made by tuaxa");
                            Console.WriteLine("https://tuaxascripts.com");
                            Console.WriteLine("Version : " + version);
                            Console.WriteLine("----------------------------------------------------");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Good: " + good);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Bad: " + bad);
                            Console.ForegroundColor = ConsoleColor.White;
                        }catch(Exception ex) { }
                    }
                    goto restart;
                }
            }
            else
            {

            }
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Key != ConsoleKey.Enter)
                keyInfo = Console.ReadKey();
        }
    }
}
