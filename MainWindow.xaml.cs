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
        static List<List<String>> CardSets = new List<List<String>>();
        static DispatcherTimer dt = new DispatcherTimer();
        static int dt_counter = 75; 
        static Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            dt.Tick += new EventHandler(DT_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(33); //one tick per frame at a rate of 30fps

        }

        private void Tab_Control_SelectedIndexChanged(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
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
            dt_counter--;
        }

        private void Window_Initialized(object sender, EventArgs e)
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
                        foreach(string category in header)
                        {
                            CardSets.Add(new List<String> { category });

                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = category.Split(";")[0];
                            textBlock.Text = CardListLocation;
                            textBlock.Name = "item";
                            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                            textBlock.VerticalAlignment = VerticalAlignment.Center;
                            textBlock.Foreground = new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0});
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
                            case "Font_Size":
                                break;
                            case "Font_Color":
                                break;
                            case "Font_Gradient":
                                break;
                            case "Sound_Effects":
                                break;
                            case "Background_Color":
                                break;
                            case "Background_Image":
                                break;
                            default:
                                break;
                        }
                    }
                    catch { }
                }
            }

        }

        private void TabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Tab_Control.IsEnabled = false;
            dt_counter = 75;
            if (CardSets[Tab_Control.SelectedIndex].Count > 1)
            {
                dt.Start();
                try
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Microsoft.VisualBasic.FileSystem.CurDir() + "\\sound effect.wav");
                    player.Play();
                }
                catch { }
            }
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
            var a = (TextBlock)Tab_Control.SelectedContent;
            if (a != null)
            {
                a.Text = CardSets[Tab_Control.SelectedIndex][0].Split(";")[0];
                a.Foreground = new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0 });
            } 
        }
    }
}