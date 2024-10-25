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
using System.Windows.Threading;

namespace Random_Flashcards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Random rnd = new Random();
        static string CardListLocation = Microsoft.VisualBasic.FileSystem.CurDir() + "\\Card List.csv";
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

        private void DT_Tick(object? sender, EventArgs e)
        {
            var a = CardSets[Tab_Control.SelectedIndex];
            var random_index = random.Next(1, a.Count);

            var b = (TextBlock)Tab_Control.SelectedContent;
            b.Text = a[random_index];
            {
                LinearGradientBrush myBrush = new LinearGradientBrush();
                var c = (dt_counter % 10);
                myBrush.GradientStops.Add(new GradientStop(Colors.Red, c / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.OrangeRed, (c + 1) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.Orange, (c + 2) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.Yellow, (c + 3) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.YellowGreen, (c + 4) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.GreenYellow, (c + 5) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.Green, (c + 6) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.Blue, (c + 7) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.Indigo, (c + 8) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.BlueViolet, (c + 9) / 10));
                myBrush.GradientStops.Add(new GradientStop(Colors.Violet, (c + 10) / 10));

                b.Foreground = myBrush;
            }
            
            if (dt_counter <= 0)
            {
                dt.Stop();
                try
                {
                    b.Foreground = new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0 });
                    if (CardSets[Tab_Control.SelectedIndex][0].Split(";")[1] == "delete")
                    {
                        CardSets[Tab_Control.SelectedIndex].RemoveAt(random_index);
                    }
                }
                catch
                {

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

                            //todo: create tab control for the category
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = category.Split(";")[0];
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
                    }
                }
            }
            else
            {
                //yell at user for deleting the file
            }

        }

        private void TabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dt_counter = 75;
            if (CardSets[Tab_Control.SelectedIndex].Count > 1)
            {
                dt.Start(); 
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Microsoft.VisualBasic.FileSystem.CurDir() + "\\sound effect.wav");
                player.Play();
            }
        }

    }
}