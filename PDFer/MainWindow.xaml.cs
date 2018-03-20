using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

using PDFer.ViewModels;

namespace PDFer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGridEx_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
        }

        private void DataGridEx_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetFormats().Contains(DataFormats.FileDrop))
            {
                string[] droppedFiles = null;
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    droppedFiles = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                }

                if ((null == droppedFiles) || (!droppedFiles.Any())) { return; }

                var files = ((sender as DataGrid).ItemsSource) as ObservableCollection<PdfFile>;

                foreach (var filePath in droppedFiles)
                {
                    files.Add(new PdfFile(filePath));
                }
            }

        }
    }
}
