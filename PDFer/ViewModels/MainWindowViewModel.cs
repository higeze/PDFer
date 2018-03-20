using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Input;
using Microsoft.Win32;

using iTextSharp.text;
using iTextSharp.text.pdf;

using HiLib;
using Prism.Mvvm;
using Prism.Commands;

namespace PDFer.ViewModels
{
    public class PdfFile:BindableBase
    {
        public PdfFile(string path)
        {
            Path = path;
        }

        private static IconCache _iconCache = new IconCache();

        public string FileName
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(Path))
                {
                    return null;
                }
                else
                {
                    return System.IO.Path.GetFileName(Path);
                }
            }
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                SetProperty(ref _path, value);
                RaisePropertyChanged("FileName");
                RaisePropertyChanged("Icon");
                RaisePropertyChanged("Pages");
            }
        }

        private ImageSource _icon = null;
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                {
                    if (string.IsNullOrWhiteSpace(Path))
                    {
                    }
                    else
                    {
                        _icon = _iconCache.GetIconByPath(this.Path);
                    }
                }
                return _icon;
            }
        }

        public string Pages
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(Path))
                {
                    return "N/A";
                }
                else
                {
                    var reader = new PdfReader(Path);
                    return reader.NumberOfPages.ToString();
                }

            }
        }

    }

    public class MainWindowViewModel:BindableBase
    {
        private ObservableCollection<PdfFile> _splitFiles = new ObservableCollection<PdfFile>();
        public ObservableCollection<PdfFile> SplitFiles 
        {
            get { return _splitFiles; }
            set { SetProperty(ref _splitFiles, value); }
        }

        private ObservableCollection<PdfFile> _mergeFiles = new ObservableCollection<PdfFile>();
        public ObservableCollection<PdfFile> MergeFiles
        {
            get { return _mergeFiles; }
            set { SetProperty(ref _mergeFiles, value); }
        }

        private DelegateCommand _splitFileCommand;
        public ICommand SplitFileCommand
        {
            get
            {
                if (_splitFileCommand == null)
                {
                    _splitFileCommand = new DelegateCommand(SplitFile);
                }
                return _splitFileCommand;
            }
        }
        private void SplitFile()
        {
            PdfReader.unethicalreading = true; 
            foreach (var file in SplitFiles)
            {
                using (PdfReader reader = new PdfReader(file.Path))
                {
                    PdfReader.unethicalreading = true;
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        Document sourceDocument = new Document(reader.GetPageSizeWithRotation(i));
                        string outPath = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(file.Path),
                            System.IO.Path.GetFileNameWithoutExtension(file.Path) + "_" + i.ToString("000") + System.IO.Path.GetExtension(file.Path));
                        PdfCopy pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outPath, System.IO.FileMode.Create));

                        sourceDocument.Open();

                        PdfImportedPage importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                        pdfCopyProvider.AddPage(importedPage);

                        sourceDocument.Close();
                    }
                    reader.Close();
                }

            }
        }


        private DelegateCommand _mergeFileCommand;
        public ICommand MergeFileCommand
        {
            get
            {
                if (_mergeFileCommand == null)
                {
                    _mergeFileCommand = new DelegateCommand(MergeFile);
                }
                return _mergeFileCommand;
            }
        }
        private void MergeFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.Filter = "Pdf file(.pdf)|*.pdf|All Files (*.*)|*.*";
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                PdfReader.unethicalreading = true;
                Document document = new Document();
                PdfCopy pdfCopy = new PdfCopy(document, new System.IO.FileStream(saveFileDialog.FileName, System.IO.FileMode.Create));
                document.Open();

                foreach (var file in MergeFiles)
                {
                    using (PdfReader reader = new PdfReader(file.Path))
                    {
                        reader.ConsolidateNamedDestinations();

                        for (var i = 1; i <= reader.NumberOfPages; i++)
                        {
                            PdfImportedPage importedPage = pdfCopy.GetImportedPage(reader, i);
                            pdfCopy.AddPage(importedPage);
                        }
                        reader.Close();
                    }

                }
                document.Close();
            }
        }
    }
}
