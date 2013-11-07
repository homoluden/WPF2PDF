using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO.Packaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Windows.Xps.Serialization;

namespace Solution
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream("doc.xps", FileMode.Create))
            {
                using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (var xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                    {
                        var rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                        var paginator = DocViewer.Document.DocumentPaginator;
                        rsm.SaveAsXaml(paginator);
                        rsm.Commit();
                    }
                }
                stream.Position = 0;
            
                var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(stream);
                PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, "doc.pdf", 0);
            }
            
        }
    }
}
