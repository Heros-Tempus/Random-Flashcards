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
        string CardListLocation = Microsoft.VisualBasic.FileSystem.CurDir() + "\\Card List.csv";
        static List<List<String>> CardSets = new List<List<String>>();
        static DispatcherTimer dt = new DispatcherTimer();
        static int dt_counter = 90;

        public MainWindow()
        {
            InitializeComponent();

            dt.Tick += new EventHandler(DT_Tick);
            dt.Interval = TimeSpan.FromMilliseconds(33); //one tick per frame at a rate of 30fps
        }

        private void DT_Tick(object? sender, EventArgs e)
        {
            if (dt_counter % 3 == 0) 
            {
                //pick random element
            }
            if (dt_counter <= 0)
            {
                dt.Stop();
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
                            textBlock.Text = "fff" + category;
                            textBlock.Name = "item";
                            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                            textBlock.VerticalAlignment = VerticalAlignment.Center;
                            textBlock.Foreground = new SolidColorBrush(new Color() { A = 255, R = 100, G = 50, B = 0});
                            textBlock.FontSize = 50;

                            TabItem item = new TabItem();
                            item.Header = category;
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
            dt_counter = 100;
            dt.Start();
        }

    }
}