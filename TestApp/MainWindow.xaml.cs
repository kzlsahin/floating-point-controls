using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FloatingPointControls;

namespace MarineParamCalculatorDataBindings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model? _contextModel = null;
        public Model? ContextModel
        {
            get
            {
                if(_contextModel is null)
                {
                    _contextModel = this.Resources["ContextModel"] as Model;
                }
                return _contextModel;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Model? model = ContextModel;
            if (model != null)
            {
                model.B += 1;
            }
        }

        private void WriteReultsToFile()
        {
            if(ContextModel is null)
            {
                return;
            }
            string path = "";
            SaveFileDialog filedialog = new SaveFileDialog();
            filedialog.Filter = "res | *.res";
            if (filedialog.ShowDialog() == true)
            {
                path = filedialog.FileName;
            }
            else
            {
                return;
            }            
            if (string.IsNullOrWhiteSpace(path))
                return;

            string output = ContextModel.ToString();
            File.WriteAllText(path, output);
        }

        private void ReadFile()
        {
            string path = "";
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "res | *.res";
            if (filedialog.ShowDialog() == true)
            {
                path = filedialog.FileName;
            }
            else
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(path))
                return;

            string data = File.ReadAllText(path);
            ContextModel?.Parse(data);
        }

        private void OpenCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ReadFile();
            e.Handled = true;
        }

        private void SaveasCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveasCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WriteReultsToFile();
            e.Handled = true;
        }
    }
}
