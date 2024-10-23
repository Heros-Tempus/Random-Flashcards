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
        string CardListLocation = Microsoft.VisualBasic.FileSystem.CurDir() + "\\Card List.csv";
        static List<List<String>> CardSets = new List<List<String>>();

        public MainWindow()
        {
            InitializeComponent();
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


    }
}