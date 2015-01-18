using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using throne_configurator.Properties;

namespace throne_configurator
{
    public partial class Form1 : Form
    {

        private static readonly string ConfigPath = Directory.GetCurrentDirectory() +  @"\system\config\",
            LoggingPath = ConfigPath + "Logging.cfg",
            PersistencePath = ConfigPath + "Persistence.cfg",
            ServicesPath = ConfigPath + "Services.cfg",
            LoginPath = ConfigPath + @"Login\",
            LoginNetworkPath = LoginPath + "Network.cfg",
            GamePath =
                Directory.GetDirectories(ConfigPath).First(x => x != LoginPath) == string.Empty
                    ? Directory.GetDirectories(ConfigPath).First(x => x != LoginPath)
                    : ConfigPath + @"Test\",
            WorldFolderPath = GamePath + @"world\",
            WorldFilePath = GamePath + "world.cfg",
            GameNetworkPath = GamePath + "Network.cfg",
            CommandsPath = WorldFolderPath + "commands.cfg",
            miscPath = WorldFolderPath + "miscellaneous.cfg";

        private static SettingFile Logging, Persistance, Services, LoginNetwork, World, GameNetwork, Commands, Miscellaneous;

        public Form1()
        {
            InitializeComponent();
            InitializeConfigFiles();
            InitializeGUI();
        }

        /// <summary>
        /// this method is used to initialize the gui textbox variables with the current settings
        /// </summary>
        private void InitializeGUI()
        {
            //it made more sense with keyvalue pair as in x.key and expression.value but i kinda want to try tuples
            //Persistence
            dbhosttb.Text = Persistance.Settings.First(x => x.Item1 == "host").Item2;
            dbnametb.Text = Persistance.Settings.First(x => x.Item1 == "database").Item2;
            dbpasstb.Text = Persistance.Settings.First(x => x.Item1 == "password").Item2;
            dbusername.Text = Persistance.Settings.First(x => x.Item1 == "username").Item2;
            dbtypetb.Text = Persistance.Settings.First(x => x.Item1 == "database_type").Item2;
            //LoginNetwork
            LoginHosttb.Text = LoginNetwork.Settings.First(x => x.Item1 == "host").Item2;
            LoginPorttb.Text = LoginNetwork.Settings.First(x => x.Item1 == "port").Item2;
            LoginBacklogtb.Text = LoginNetwork.Settings.First(x => x.Item1 == "backlog").Item2;
            LoginGame_hosttb.Text = LoginNetwork.Settings.First(x => x.Item1 == "game_host").Item2;
            LoginGame_porttb.Text = LoginNetwork.Settings.First(x => x.Item1 == "game_port").Item2;
            LoginFirewallresettb.Text = LoginNetwork.Settings.First(x => x.Item1 == "firewall_reset_seconds").Item2;
            Loginfirewallmaxtb.Text = LoginNetwork.Settings.First(x => x.Item1 == "firewall_max_connections").Item2;
            loginnonageltb.Text = LoginNetwork.Settings.First(x => x.Item1 == "no_nagel").Item2;
            loginkeepalivetb.Text = LoginNetwork.Settings.First(x => x.Item1 == "keep_alive").Item2;
            logingracefultb.Text = LoginNetwork.Settings.First(x => x.Item1 == "graceful").Item2;
            loginfragmenttb.Text = LoginNetwork.Settings.First(x => x.Item1 == "no_fragment").Item2;
            loginreusetb.Text = LoginNetwork.Settings.First(x => x.Item1 == "reuse_endpoint").Item2;
            //GameNetwork
            gamehosttb.Text = GameNetwork.Settings.First(x => x.Item1 == "host").Item2;
            gameporttb.Text = GameNetwork.Settings.First(x => x.Item1 == "port").Item2;
            gamebacklogtb.Text = GameNetwork.Settings.First(x => x.Item1 == "backlog").Item2;
            outgoingfootertb.Text = GameNetwork.Settings.First(x => x.Item1 == "outgoing_packet_footer").Item2;
            incomingfootertb.Text = GameNetwork.Settings.First(x => x.Item1 == "incoming_packet_footer").Item2;
            cast5keytb.Text = GameNetwork.Settings.First(x => x.Item1 == "cast5_standard").Item2;
            gamefirewallresettb.Text = GameNetwork.Settings.First(x => x.Item1 == "firewall_reset_seconds").Item2;
            gamefirewallmaxtb.Text = GameNetwork.Settings.First(x => x.Item1 == "firewall_max_connections").Item2;
            gamenonangeltb.Text = GameNetwork.Settings.First(x => x.Item1 == "no_nagel").Item2;
            gamekeepalivetb.Text = GameNetwork.Settings.First(x => x.Item1 == "keep_alive").Item2;
            gamegracefultb.Text = GameNetwork.Settings.First(x => x.Item1 == "graceful").Item2;
            gamefragmenttb.Text = GameNetwork.Settings.First(x => x.Item1 == "no_fragment").Item2;
            gamereusetb.Text = GameNetwork.Settings.First(x => x.Item1 == "reuse_endpoint").Item2;
            //World
            if (World.Settings.Any()) requirelb.Items.Clear();
            foreach (var item in World.Settings)
            {
                requirelb.Items.Add(item.Item2);
            }
            //Services
            accounturitb.Text = Services.Settings.First(x => x.Item1 == "account_uri").Item2;
            //Logging
            hidelogleveltb.Text = Logging.Settings.First(x => x.Item1 == "hide_log_levels").Item2;
            clcolorstb.Text = Logging.Settings.First(x => x.Item1 == "cl_colors").Item2;
            cltimestampstb.Text = Logging.Settings.First(x => x.Item1 == "cl_timestamps").Item2;
            fileloggingtb.Text = Logging.Settings.First(x => x.Item1 == "file_logging").Item2;
            logincomingtb.Text = Logging.Settings.First(x => x.Item1 == "log_incoming_packets").Item2;
            archievelogstb.Text = Logging.Settings.First(x => x.Item1 == "archive_log_files").Item2;
            //Commands
            commandprefixtb.Text = Commands.Settings.First(x => x.Item1 == "command_prefix").Item2;
            //Miscellaneous
            jumprangetb.Text = Miscellaneous.Settings.First(x => x.Item1 == "max_jump_range").Item2;
            screenrangetb.Text = Miscellaneous.Settings.First(x => x.Item1 == "player_screen_range").Item2;
        }

        /// <summary>
        /// this method is used to load all the configuration files, you can access all properties within the settings list
        /// </summary>
        private static void InitializeConfigFiles()
        {
            Logging = new SettingFile(LoggingPath, "Logging");
            Persistance = new SettingFile(PersistencePath, "Persistence");
            Services = new SettingFile(ServicesPath, "Services");
            LoginNetwork = new SettingFile(LoginNetworkPath, "Network configuration for Login");
            World = new SettingFile(WorldFilePath, "World configuration for Test");
            GameNetwork = new SettingFile(GameNetworkPath, "Network configuration for Test");
            Commands = new SettingFile(CommandsPath, "World/commands");
            Miscellaneous = new SettingFile(miscPath, "World/miscellaneous");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(requiretb.Text)) requirelb.Items.Add(requiretb.Text);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if(requirelb.Items.Contains(requiretb.Text)) requirelb.Items.Remove(requiretb.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //forgot that keyvalue pair and tuples are immutable so i've had to recreate the lists >.<
            //Persistence
            Persistance.Settings.Clear();
            Persistance.Settings.Add(Tuple.Create("host", dbhosttb.Text));
            Persistance.Settings.Add(Tuple.Create("database", dbnametb.Text));
            Persistance.Settings.Add(Tuple.Create("password", dbpasstb.Text));
            Persistance.Settings.Add(Tuple.Create("username", dbusername.Text));
            Persistance.Settings.Add(Tuple.Create("database_type", dbtypetb.Text));
            Persistance.SaveFile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //LoginNetwork
            LoginNetwork.Settings.Clear();
            LoginNetwork.Settings.Add(Tuple.Create("host", LoginHosttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("port", LoginPorttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("backlog", LoginBacklogtb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("game_host", LoginGame_hosttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("game_port", LoginGame_porttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("firewall_reset_seconds", LoginFirewallresettb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("firewall_max_connections", Loginfirewallmaxtb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("no_nagel", loginnonageltb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("keep_alive", loginkeepalivetb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("graceful", logingracefultb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("no_fragment", loginfragmenttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("reuse_endpoint", loginreusetb.Text));
            LoginNetwork.SaveFile();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //GameNetwork
            GameNetwork.Settings.Clear();
            GameNetwork.Settings.Add(Tuple.Create("host", gamehosttb.Text));
            GameNetwork.Settings.Add(Tuple.Create("port", gameporttb.Text));
            GameNetwork.Settings.Add(Tuple.Create("backlog", gamebacklogtb.Text));
            GameNetwork.Settings.Add(Tuple.Create("outgoing_packet_footer", outgoingfootertb.Text));
            GameNetwork.Settings.Add(Tuple.Create("incoming_packet_footer", incomingfootertb.Text));
            GameNetwork.Settings.Add(Tuple.Create("cast5_standard", cast5keytb.Text));
            GameNetwork.Settings.Add(Tuple.Create("firewall_reset_seconds", gamefirewallresettb.Text));
            GameNetwork.Settings.Add(Tuple.Create("firewall_max_connections", gamefirewallmaxtb.Text));
            GameNetwork.Settings.Add(Tuple.Create("no_nagel", gamenonangeltb.Text));
            GameNetwork.Settings.Add(Tuple.Create("keep_alive", gamekeepalivetb.Text));
            GameNetwork.Settings.Add(Tuple.Create("graceful", gamegracefultb.Text));
            GameNetwork.Settings.Add(Tuple.Create("no_fragment", gamefragmenttb.Text));
            GameNetwork.Settings.Add(Tuple.Create("reuse_endpoint", gamereusetb.Text));
            GameNetwork.SaveFile();
        }

        private void requirelb_SelectedIndexChanged(object sender, EventArgs e)
        {
            requiretb.Text = requirelb.Items[requirelb.SelectedIndex].ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //World
            World.Settings.Clear();
            foreach (var item in requirelb.Items)
            {
                World.Settings.Add(Tuple.Create("require",item.ToString()));
            }
            World.SaveFile();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Services
            Services.Settings.Clear();
            Services.Settings.Add(Tuple.Create("account_uri", accounturitb.Text));
            Services.SaveFile();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Logging
            Logging.Settings.Clear();
            Logging.Settings.Add(Tuple.Create("hide_log_levels", hidelogleveltb.Text));
            Logging.Settings.Add(Tuple.Create("cl_colors", clcolorstb.Text));
            Logging.Settings.Add(Tuple.Create("cl_timestamps", cltimestampstb.Text));
            Logging.Settings.Add(Tuple.Create("file_logging", fileloggingtb.Text));
            Logging.Settings.Add(Tuple.Create("log_incoming_packets", logincomingtb.Text));
            Logging.Settings.Add(Tuple.Create("archive_log_files", archievelogstb.Text));
            Logging.SaveFile();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Commands
            Commands.Settings.Clear();
            Commands.Settings.Add(Tuple.Create("command_prefix", commandprefixtb.Text));
            Commands.SaveFile();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Miscellaneous
            Miscellaneous.Settings.Clear();
            Miscellaneous.Settings.Add(Tuple.Create("max_jump_range", jumprangetb.Text));
            Miscellaneous.Settings.Add(Tuple.Create("player_screen_range", screenrangetb.Text));
            Miscellaneous.SaveFile();
        }

        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }

        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private void button1_Click(object sender, EventArgs e)
        {
            //forgot that keyvalue pair and tuples are immutable so i've had to recreate the lists >.<
            //Persistence
            Persistance.Settings.Clear();
            Persistance.Settings.Add(Tuple.Create("host", dbhosttb.Text));
            Persistance.Settings.Add(Tuple.Create("database", dbnametb.Text));
            Persistance.Settings.Add(Tuple.Create("password", dbpasstb.Text));
            Persistance.Settings.Add(Tuple.Create("username", dbusername.Text));
            Persistance.Settings.Add(Tuple.Create("database_type", dbtypetb.Text));
            Persistance.SaveFile();
            //LoginNetwork
            LoginNetwork.Settings.Clear();
            LoginNetwork.Settings.Add(Tuple.Create("host", LoginHosttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("port", LoginPorttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("backlog", LoginBacklogtb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("game_host", LoginGame_hosttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("game_port", LoginGame_porttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("firewall_reset_seconds", LoginFirewallresettb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("firewall_max_connections", Loginfirewallmaxtb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("no_nagel", loginnonageltb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("keep_alive", loginkeepalivetb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("graceful", logingracefultb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("no_fragment", loginfragmenttb.Text));
            LoginNetwork.Settings.Add(Tuple.Create("reuse_endpoint", loginreusetb.Text));
            LoginNetwork.SaveFile();
            //GameNetwork
            GameNetwork.Settings.Clear();
            GameNetwork.Settings.Add(Tuple.Create("host", gamehosttb.Text));
            GameNetwork.Settings.Add(Tuple.Create("port", gameporttb.Text));
            GameNetwork.Settings.Add(Tuple.Create("backlog", gamebacklogtb.Text));
            GameNetwork.Settings.Add(Tuple.Create("outgoing_packet_footer", outgoingfootertb.Text));
            GameNetwork.Settings.Add(Tuple.Create("incoming_packet_footer", incomingfootertb.Text));
            GameNetwork.Settings.Add(Tuple.Create("cast5_standard", cast5keytb.Text));
            GameNetwork.Settings.Add(Tuple.Create("firewall_reset_seconds", gamefirewallresettb.Text));
            GameNetwork.Settings.Add(Tuple.Create("firewall_max_connections", gamefirewallmaxtb.Text));
            GameNetwork.Settings.Add(Tuple.Create("no_nagel", gamenonangeltb.Text));
            GameNetwork.Settings.Add(Tuple.Create("keep_alive", gamekeepalivetb.Text));
            GameNetwork.Settings.Add(Tuple.Create("graceful", gamegracefultb.Text));
            GameNetwork.Settings.Add(Tuple.Create("no_fragment", gamefragmenttb.Text));
            GameNetwork.Settings.Add(Tuple.Create("reuse_endpoint", gamereusetb.Text));
            GameNetwork.SaveFile();
            //World
            World.Settings.Clear();
            foreach (var item in requirelb.Items)
            {
                World.Settings.Add(Tuple.Create("require", item.ToString()));
            }
            World.SaveFile();
            //Services
            Services.Settings.Clear();
            Services.Settings.Add(Tuple.Create("account_uri", accounturitb.Text));
            Services.SaveFile();
            //Logging
            Logging.Settings.Clear();
            Logging.Settings.Add(Tuple.Create("hide_log_levels", hidelogleveltb.Text));
            Logging.Settings.Add(Tuple.Create("cl_colors", clcolorstb.Text));
            Logging.Settings.Add(Tuple.Create("cl_timestamps", cltimestampstb.Text));
            Logging.Settings.Add(Tuple.Create("file_logging", fileloggingtb.Text));
            Logging.Settings.Add(Tuple.Create("log_incoming_packets", logincomingtb.Text));
            Logging.Settings.Add(Tuple.Create("archive_log_files", archievelogstb.Text));
            Logging.SaveFile();
            //Commands
            Commands.Settings.Clear();
            Commands.Settings.Add(Tuple.Create("command_prefix", commandprefixtb.Text));
            Commands.SaveFile();
            //Miscellaneous
            Miscellaneous.Settings.Clear();
            Miscellaneous.Settings.Add(Tuple.Create("max_jump_range", jumprangetb.Text));
            Miscellaneous.Settings.Add(Tuple.Create("player_screen_range", screenrangetb.Text));
            Miscellaneous.SaveFile();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            InitializeConfigFiles();
            InitializeGUI();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string selectedfile = string.Empty;
            ShowConsoleWindow();
            Console.ForegroundColor = ConsoleColor.Green;
            this.Hide();
            Console.WriteLine(Resources.helptext);
            Console.WriteLine(Environment.NewLine);
            while (true)
            {
                if(!string.IsNullOrEmpty(selectedfile))
                    Console.Write(selectedfile + ">");
                string[] cmd = Console.ReadLine().Split(' ');
                Console.WriteLine(Environment.NewLine);
                switch (cmd[0].ToLower())
                {
                    case "showfiles":
                    {
                        Console.WriteLine(Resources.show_file_msg);
                        selectedfile = string.Empty;
                        break;
                    }
                    case "selectfile":
                    {
                        if (cmd[1] == "logging" || cmd[1] == "persistance" || cmd[1] == "services" ||
                            cmd[1] == "loginnetwork" || cmd[1] == "world" || cmd[1] == "gamenetwork" ||
                            cmd[1] == "commands" || cmd[1] == "miscellaneous")
                            selectedfile = cmd[1];
                        Console.WriteLine("{0} have been selected",selectedfile);
                        break;
                    }
                    case "showproperties":
                    {
                        Console.Write("{0} Properties are ",selectedfile);
                        switch (selectedfile.ToLower())
                        {
                            case "logging":
                                foreach (var setting in Logging.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                            case "persistance":
                                foreach (var setting in Persistance.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                            case "services":
                                foreach (var setting in Services.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                            case "loginnetwork":
                                foreach (var setting in LoginNetwork.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                            case "world":
                                foreach (var setting in World.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                            case "gamenetwork":
                                foreach (var setting in GameNetwork.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                            case "commands":
                                foreach (var setting in Commands.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                            case "miscellaneous":
                                foreach (var setting in Miscellaneous.Settings)
                                {
                                    Console.Write(setting.Item1 + " ");
                                }
                                break;
                        }
                        Console.WriteLine();
                        break;
                    }
                    case "editproperty":
                    {
                        switch (selectedfile.ToLower())
                        {
                            case "logging":
                                foreach (var setting in Logging.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    Logging.Settings.Remove(setting);
                                    Logging.Settings.Add(Tuple.Create(cmd[1],cmd[2]));
                                }
                                break;
                            case "persistance":
                                foreach (var setting in Persistance.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    Persistance.Settings.Remove(setting);
                                    Persistance.Settings.Add(Tuple.Create(cmd[1], cmd[2]));
                                }
                                break;
                            case "services":
                                foreach (var setting in Services.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    Services.Settings.Remove(setting);
                                    Services.Settings.Add(Tuple.Create(cmd[1], cmd[2]));
                                }
                                break;
                            case "loginnetwork":
                                foreach (var setting in LoginNetwork.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    LoginNetwork.Settings.Remove(setting);
                                    LoginNetwork.Settings.Add(Tuple.Create(cmd[1], cmd[2]));
                                }
                                break;
                            case "world":
                                foreach (var setting in World.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    World.Settings.Remove(setting);
                                    World.Settings.Add(Tuple.Create(cmd[1], cmd[2]));
                                }
                                break;
                            case "gamenetwork":
                                foreach (var setting in GameNetwork.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    GameNetwork.Settings.Remove(setting);
                                    GameNetwork.Settings.Add(Tuple.Create(cmd[1], cmd[2]));
                                }
                                break;
                            case "commands":
                                foreach (var setting in Commands.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    Commands.Settings.Remove(setting);
                                    Commands.Settings.Add(Tuple.Create(cmd[1], cmd[2]));
                                }
                                break;
                            case "miscellaneous":
                                foreach (var setting in Miscellaneous.Settings.Where(setting => setting.Item1 == cmd[1].ToLower()))
                                {
                                    Miscellaneous.Settings.Remove(setting);
                                    Miscellaneous.Settings.Add(Tuple.Create(cmd[1], cmd[2]));
                                }
                                break;
                        }
                        break;
                    }
                    case "save":
                    {
                        Persistance.SaveFile();
                        LoginNetwork.SaveFile();
                        GameNetwork.SaveFile();
                        World.SaveFile();
                        Services.SaveFile();
                        Logging.SaveFile();
                        Commands.SaveFile();
                        Miscellaneous.SaveFile();
                        break;
                    }
                    case "switch":
                    {
                        this.Show();
                        HideConsoleWindow();
                        return;
                    }
                    default:
                    {
                        Console.WriteLine(Resources.helptext);
                        selectedfile = string.Empty;
                        break;
                    }
                }
                Console.WriteLine(Environment.NewLine);
            }
        }
        
    }
}
