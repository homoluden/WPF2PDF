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
using DotLiquid;
using dotTemplate = DotLiquid.Template;
using System.Xml;
using System.Windows.Markup;

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
                        var paginator = ((IDocumentPaginatorSource)DocViewer.Document).DocumentPaginator;
                        rsm.SaveAsXaml(paginator);
                        rsm.Commit();
                    }
                }
                stream.Position = 0;
            
                var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(stream);
                PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, "doc.pdf", 0);
            }
            
        }

        private void ParseButton_OnClick(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream("Templates\\report1.lqd", FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    var templateString = reader.ReadToEnd();
                    var template = dotTemplate.Parse(templateString);
                    var docContext = CreateDocumentContext();
                    var docString = template.Render(docContext);
                    DocViewer.Document = (FlowDocument) XamlReader.Parse(docString);
                }
            }
        }

        private DotLiquid.Hash CreateDocumentContext()
        {
            var context = new
            {
                Title = "Hello, Habrahabr!",
                Subtitle = "Experimenting with dotLiquid, FlowDocument and PDFSharp",
                Steps = new List<dynamic>{
                    new { Title = "Document Context", Description = "Create data source for dotLiquid Template"},
                    new { Title = "Rendering", Description = "Load template string and render it into FlowDocument markup with Document Context given"},
                    new { Title = "Parse markup", Description = "Use XAML Parser to prepare FlowDocument instance"},
                    new { Title = "Save to XPS", Description = "Save prepared FlowDocument into XPS format"},
                    new { Title = "Convert XPS to PDF", Description = "Convert XPS to WPF using PDFSharp"},
                }
            };
            
            return DotLiquid.Hash.FromAnonymousObject(context);
        }
    }
}
