using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using static System.Linq.Enumerable;
using NHotkey.Wpf;
using NHotkey;
using Path = System.IO.Path;

namespace Random_Flashcards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static Random rnd = new Random();
        static string CardListLocation = Microsoft.VisualBasic.FileSystem.CurDir() + "\\Card List.csv";
        static string SettingsLocation = Microsoft.VisualBasic.FileSystem.CurDir() + "\\Settings.txt";
        static string SoundLocation = Microsoft.VisualBasic.FileSystem.CurDir() + "\\Sound.wav";
        static List<List<String>> CardSets = new List<List<String>>();
        static List<List<String>> BackupCards = new List<List<String>>();
        static DispatcherTimer dt = new DispatcherTimer();
        static int dt_counter = 75; 
        static Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            dt.Tick += new EventHandler(DT_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(33); //one tick per frame at a rate of 30fps

        }
        private void DT_Tick(object? sender, EventArgs e)
        {
            var a = CardSets[Tab_Control.SelectedIndex];
            var random_index = random.Next(1, a.Count);

            var b = (TextBlock)Tab_Control.SelectedContent;
            b.Text = a[random_index];
            b.Foreground = Paint_The_Rainbow();

            if (dt_counter <= 0)
            {
                dt.Stop();
                Tab_Control.IsEnabled = true;
                if (CardSets[Tab_Control.SelectedIndex].Count > 2)
                {
                    try
                    {
                        if (CardSets[Tab_Control.SelectedIndex][0].Split(";")[1] == "delete")
                        {
                            CardSets[Tab_Control.SelectedIndex].RemoveAt(random_index);
                        }
                    }
                    catch
                    {
                    //nothing needs to be done in this block
                    //it's just here to keep the program from craching if the selected category can't be split on a ';'
                    }
                }
            }
            dt_counter--;
        }
        private void Window_Initialized(object sender, EventArgs e)
        {

            //Set the keyboard dictionaries
            Dictionary<string, ModifierKeys> mod_keys = new Dictionary<string, ModifierKeys>();
            {
                mod_keys.Add("ctrl", ModifierKeys.Control);
                mod_keys.Add("alt", ModifierKeys.Alt);
                mod_keys.Add("shift", ModifierKeys.Shift);
                mod_keys.Add("win", ModifierKeys.Windows);
                mod_keys.Add("None", ModifierKeys.None);
            }
            Dictionary<string, Key> keyboard = new Dictionary<string, Key>();
            {
                keyboard.Add("+", Key.Add);
                keyboard.Add("-", Key.Subtract);
                keyboard.Add("space", Key.Space);
                keyboard.Add("a", Key.A);
                keyboard.Add("b", Key.B);
                keyboard.Add("c", Key.C);
                keyboard.Add("d", Key.D);
                keyboard.Add("e", Key.E);
                keyboard.Add("f", Key.F);
                keyboard.Add("g", Key.G);
                keyboard.Add("h", Key.H);
                keyboard.Add("i", Key.I);
                keyboard.Add("j", Key.J);
                keyboard.Add("k", Key.K);
                keyboard.Add("l", Key.L);
                keyboard.Add("m", Key.M);
                keyboard.Add("n", Key.N);
                keyboard.Add("o", Key.O);
                keyboard.Add("p", Key.P);
                keyboard.Add("q", Key.Q);
                keyboard.Add("r", Key.R);
                keyboard.Add("s", Key.S);
                keyboard.Add("t", Key.T);
                keyboard.Add("u", Key.U);
                keyboard.Add("v", Key.V);
                keyboard.Add("w", Key.W);
                keyboard.Add("x", Key.X);
                keyboard.Add("y", Key.Y);
                keyboard.Add("z", Key.Z);
                keyboard.Add("0", Key.D0);
                keyboard.Add("1", Key.D1);
                keyboard.Add("2", Key.D2);
                keyboard.Add("3", Key.D3);
                keyboard.Add("4", Key.D4);
                keyboard.Add("5", Key.D5);
                keyboard.Add("6", Key.D6);
                keyboard.Add("7", Key.D7);
                keyboard.Add("8", Key.D8);
                keyboard.Add("9", Key.D9);
                keyboard.Add("f1", Key.F1);
                keyboard.Add("f2", Key.F2);
                keyboard.Add("f3", Key.F3);
                keyboard.Add("f4", Key.F4);
                keyboard.Add("f5", Key.F5);
                keyboard.Add("f6", Key.F6);
                keyboard.Add("f7", Key.F7);
                keyboard.Add("f8", Key.F8);
                keyboard.Add("f9", Key.F9);
                keyboard.Add("f10", Key.F10);
                keyboard.Add("f11", Key.F11);
                keyboard.Add("f12", Key.F12);
                keyboard.Add("f13", Key.F13);
                keyboard.Add("f14", Key.F14);
                keyboard.Add("f15", Key.F15);
                keyboard.Add("f16", Key.F16);
                keyboard.Add("f17", Key.F17);
                keyboard.Add("f18", Key.F18);
                keyboard.Add("f19", Key.F19);
                keyboard.Add("f20", Key.F20);
                keyboard.Add("f21", Key.F21);
                keyboard.Add("f22", Key.F22);
                keyboard.Add("f23", Key.F23);
                keyboard.Add("f24", Key.F24);
                keyboard.Add("backspace", Key.Back);
                keyboard.Add("capslock", Key.CapsLock);
                keyboard.Add("delete", Key.Delete);
                keyboard.Add("down", Key.Down);
                keyboard.Add("end", Key.End);
                keyboard.Add("enter", Key.Enter);
                keyboard.Add("home", Key.Home);
                keyboard.Add("insert", Key.Insert);
                keyboard.Add("left", Key.Left);
                keyboard.Add("numlock", Key.NumLock);
                keyboard.Add("pagedown", Key.PageDown);
                keyboard.Add("pageup", Key.PageUp);
                keyboard.Add("printscreen", Key.PrintScreen);
                keyboard.Add("right", Key.Right);
                keyboard.Add("up", Key.Up);
            }

            bool Build_CSV_From_TXTs = false;
            string ?Font = null;
            int ?Font_Size = null;
            bool Bold = false;
            bool Italic = false;
            bool Underline = false;
            string ?Background_Color = null;
            string ?Background_Image = null;
            bool Reload_On_Tab_Change = false;

            //Load the settings
            {
                if (File.Exists(SettingsLocation))
                {
                    var settings = File.ReadAllLines(SettingsLocation);
                    foreach (var line in settings)
                    {
                        try
                        {
                            var x = line;
                            switch (x.Split(";")[0])
                            {
                                case "Build_CSV_From_TXTs":
                                    if (x.Split(";")[1].ToLower() == "true")
                                    {
                                        Build_CSV_From_TXTs = true;
                                    }
                                    break;
                                case "Font":
                                     Font = x.Split(";")[1];
                                    break;
                                case "Font_Size":
                                    int y = 0;
                                    if (int.TryParse(x.Split(";")[1], out y))
                                    {
                                        FontSize = y;
                                    }
                                    break;
                                case "Bold":
                                    if (x.Split(';')[1].ToLower() == "true")
                                    {
                                            Bold = true;
                                    }

                                    break;
                                case "Italic":
                                    if (x.Split(';')[1].ToLower() == "true")
                                    {
                                        Italic = true;
                                    }
                                    break;
                                case "Underline":
                                    if (x.Split(';')[1].ToLower() == "true")
                                    {
                                        Underline = true;
                                    }
                                    break;
                                case "Sound_Effects":
                                    SoundLocation = x.Split(";")[1];
                                    break;
                                case "Background_Color":
                                    Background_Color = x.Split(";")[1];
                                    break;
                                case "Background_Image":
                                    Background_Image = x.Split(";")[1];
                                    break;
                                case "Hotkeys":
                                    //Hotkeys are set up in the format [action name]:[optional modkeys]&[key]
                                    //Example: TabLeft:ctrl&alt&-,TabRight:ctrl&alt&+,Roll:f4
                                    foreach (var k in x.Split(";")[1].Split(","))
                                    {
                                        if (k != "None")
                                        {
                                            var key = keyboard[k.Split(":")[1].Split("&").Last().ToLower()];
                                            var modkeys = mod_keys["None"];
                                            if (k.Count(c => c == '&') > 0)
                                                modkeys = k.Split(":")[1].Split("&").Take(k.Split("&").Length - 1).Select(s => mod_keys[s.ToLower()]).Aggregate((a, b) => a | b);
                                            switch (k.Split(":")[0])
                                            {
                                                case "TabLeft":
                                                    HotkeyManager.Current.AddOrReplace("Decrement", key, modkeys, OnDecrement);
                                                    break;
                                                case "TabRight":
                                                    HotkeyManager.Current.AddOrReplace("Increment", key, modkeys, OnIncrement);
                                                    break;
                                                case "Roll":
                                                    HotkeyManager.Current.AddOrReplace("Roll", key, modkeys, OnRoll);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                case "Reload_On_Tab_Change":
                                    if (x.Split(";")[1].ToLower() == "true")
                                    {
                                        Reload_On_Tab_Change = true;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch { }
                    }
                }
            }

            //Building the CSV from the text files has to be done before loading the card list
            if (Build_CSV_From_TXTs)
            {
                CombineTxtFilesToCsv(Microsoft.VisualBasic.FileSystem.CurDir() + "\\Lists", CardListLocation);
            }
            //Load the card list
            {
                if (File.Exists(CardListLocation))
                {
                    using (TextFieldParser parser = new TextFieldParser(CardListLocation))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");

                        string[]? header = parser.ReadFields();
                        if (header != null && header.Length > 0)
                        {
                            foreach (string category in header)
                            {
                                CardSets.Add(new List<String> { category });

                                TextBlock textBlock = new TextBlock();
                                textBlock.Text = category.Split(";")[0];
                                textBlock.Text = CardListLocation;
                                textBlock.Name = "item";
                                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                                textBlock.VerticalAlignment = VerticalAlignment.Center;
                                textBlock.Foreground = new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0 });
                                textBlock.FontSize = 24;
                                textBlock.TextWrapping = TextWrapping.WrapWithOverflow;

                                TabItem item = new TabItem();
                                item.Header = category.Split(";")[0];
                                item.Content = textBlock;


                                Tab_Control.Items.Add(item);

                            }
                            while (!parser.EndOfData)
                            {
                                string[]? fields = parser.ReadFields();

                                if (fields != null && fields.Length > 0)
                                {
                                    foreach (var i in Range(0, header.Length))
                                    {
                                        if (fields[i] != "")
                                        {
                                            CardSets[i].Add(fields[i]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            //yell at user for making an empty csv
                            MessageBox.Show("The CSV file is empty. Please add some data to the file and restart the program.");
                        }
                    }
                }
                else
                {
                    //yell at user for deleting the file
                    MessageBox.Show("The CSV file is missing. Please add the file and restart the program.");
                }
            }

            //The remaining settings have to be applied after loading the card list
            foreach (TabItem tab in Tab_Control.Items)
            {
                if (Font != null)
                {
                    ((TextBlock)tab.Content).FontFamily = new FontFamily(Font);
                }
                if (Font_Size != null)
                {
                    ((TextBlock)tab.Content).FontSize = (int)Font_Size;
                }
                if (Bold)
                {
                    ((TextBlock)tab.Content).FontWeight = FontWeights.Bold;
                }
                if (Italic)
                {
                    ((TextBlock)tab.Content).FontStyle = FontStyles.Italic;
                }
                if (Underline)
                {
                    ((TextBlock)tab.Content).TextDecorations = TextDecorations.Underline;
                }
                if (Background_Color != null)
                {
                    try
                    {
                        var color = (Color)ColorConverter.ConvertFromString(Background_Color);
                        ((TextBlock)tab.Content).Background = new SolidColorBrush(color);
                        Tab_Control.Background = new SolidColorBrush(color);
                    }
                    catch
                    {
                        //If the color is invalid, set the background to transparent
                        ((TextBlock)tab.Content).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
                    }
                }
                if (Background_Image != null)
                {
                    try
                    {
                        var image = new BitmapImage(new Uri(Background_Image));
                        ((TextBlock)tab.Content).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
                        Tab_Control.Background = new ImageBrush(image);
                    }
                    catch
                    {
                        //If the image is invalid, set the background to transparent
                        ((TextBlock)tab.Content).Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 0, 0, 0));
                    }
                }
            }

            if (Reload_On_Tab_Change)
            {
                BackupCards = CardSets.Select(innerList => innerList.ToList()).ToList();
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HotkeyManager.Current.Remove("Increment");
            HotkeyManager.Current.Remove("Decrement");
            HotkeyManager.Current.Remove("Roll");
            
        }
        private void OnIncrement(object ?sender, HotkeyEventArgs ?e)
        {
            Tab_Control.SelectedIndex++;
            if(e != null)
                e.Handled = true;
        }
        private void OnDecrement(object ?sender, HotkeyEventArgs ?e)
        {
            if (Tab_Control.SelectedIndex > 0)
                Tab_Control.SelectedIndex--;
            if (e != null)
                e.Handled = true;
        }
        private void OnRoll(object ?sender, HotkeyEventArgs ?e)
        {
            if (CardSets[Tab_Control.SelectedIndex].Count > 1)
            {
                Tab_Control.IsEnabled = false;
                dt_counter = 75;
                dt.Start();
                try
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(SoundLocation);
                    player.Play();
                }
                catch { }
            }
            if (e != null)
                e.Handled = true;
        }
        private void TabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnRoll(sender, null);
        }
        private LinearGradientBrush Paint_The_Rainbow()
        {
                LinearGradientBrush myBrush = new LinearGradientBrush();
                myBrush.StartPoint = new Point(0, 0);
                myBrush.EndPoint = new Point(1, 1);
                var c = (dt_counter % 11);
                myBrush.GradientStops.Add(new GradientStop(Colors.Red, ((0.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.OrangeRed, ((1.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.Orange, ((2.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.Yellow, ((3.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.YellowGreen, ((4.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.GreenYellow, ((5.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.Green, ((6.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.Blue, ((7.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.Indigo, ((8.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.BlueViolet, ((9.0 + c) % 11) * (1.0 / 10.0)));
                myBrush.GradientStops.Add(new GradientStop(Colors.Violet, ((10.0 + c) % 11) * (1.0 / 10.0)));
                return myBrush;
        }
        private void Tab_Control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BackupCards.Count > 0)
            {
                CardSets = BackupCards.Select(innerList => innerList.ToList()).ToList();
            }
            var a = (TextBlock)Tab_Control.SelectedContent;
            if (a != null)
            {
                a.Text = CardSets[Tab_Control.SelectedIndex][0].Split(";")[0];
                a.Foreground = new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0 });
            } 
        }
        private void CombineTxtFilesToCsv(string inputFolder, string outputCsvFile)
        {
            string[] txtFiles = Directory.GetFiles(inputFolder, "*.txt");

            Dictionary<string, List<string>> fileData = new Dictionary<string, List<string>>();

            foreach (string file in txtFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                List<string> lines = File.ReadAllLines(file).ToList();
                fileData[fileName] = lines;
            }

            int maxLines = fileData.Values.Any() ? fileData.Values.Max(lines => lines.Count) : 0;

            StringBuilder csvContent = new StringBuilder();

            csvContent.AppendLine(string.Join(",", fileData.Keys));

            for (int i = 0; i < maxLines; i++)
            {
                List<string> rowItems = new List<string>();

                foreach (var kvp in fileData)
                {
                    if (i < kvp.Value.Count)
                    {
                        string cell = kvp.Value[i].Replace("\"", "\"\"");
                        rowItems.Add($"\"{cell}\"");
                    }
                    else
                    {
                        rowItems.Add("");
                    }
                }

                csvContent.AppendLine(string.Join(",", rowItems));
            }

            File.WriteAllText(outputCsvFile, csvContent.ToString());
        }

    }
}