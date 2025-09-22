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
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views
{
    /// <summary>
    /// Interaktionslogik für CreateDocumentUserControl.xaml
    /// </summary>
    public partial class CreateDocumentUserControl : UserControl
    {
        public CreateDocumentUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Command to create a document. The type of document is defined by the <see cref="DocumentType"/> property.
        /// </summary>
        public ICommand CreateDocumentCommand
        {
            get => (ICommand)GetValue(CreateDocumentCommandProperty);
            set => SetValue(CreateDocumentCommandProperty, value);
        }
        public static readonly DependencyProperty CreateDocumentCommandProperty = DependencyProperty.Register(nameof(CreateDocumentCommand), typeof(ICommand), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Type of document to create.
        /// </summary>
        public DocumentCreationTypes DocumentType
        {
            get => (DocumentCreationTypes)GetValue(DocumentTypeProperty);
            set => SetValue(DocumentTypeProperty, value);
        }
        public static readonly DependencyProperty DocumentTypeProperty = DependencyProperty.Register(nameof(DocumentType), typeof(DocumentCreationTypes), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Text to display on the button that triggers document creation.
        /// </summary>
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Glyph for the button icon, typically a font icon glyph.
        /// </summary>
        public string ButtonIconGlyph
        {
            get => (string)GetValue(IconGlyphProperty);
            set => SetValue(IconGlyphProperty, value);
        }
        public static readonly DependencyProperty IconGlyphProperty = DependencyProperty.Register(nameof(ButtonIconGlyph), typeof(string), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Indicates whether the document creation is currently running.
        /// </summary>
        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }
        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(nameof(IsRunning), typeof(bool), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Indicates whether the document creation was successful.
        /// </summary>
        public bool IsSuccessful
        {
            get => (bool)GetValue(IsSuccessfulProperty);
            set => SetValue(IsSuccessfulProperty, value);
        }
        public static readonly DependencyProperty IsSuccessfulProperty = DependencyProperty.Register(nameof(IsSuccessful), typeof(bool), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Indicates whether data is available for the document creation.
        /// </summary>
        public bool IsDataAvailable
        {
            get => (bool)GetValue(IsDataAvailableProperty);
            set => SetValue(IsDataAvailableProperty, value);
        }
        public static readonly DependencyProperty IsDataAvailableProperty = DependencyProperty.Register(nameof(IsDataAvailable), typeof(bool), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Indicates whether the template is available for the document creation.
        /// </summary>
        public bool IsTemplateAvailable
        {
            get => (bool)GetValue(IsTemplateAvailableProperty);
            set => SetValue(IsTemplateAvailableProperty, value);
        }
        public static readonly DependencyProperty IsTemplateAvailableProperty = DependencyProperty.Register(nameof(IsTemplateAvailable), typeof(bool), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Text to display when the document creation is successful.
        /// </summary>
        public string SuccessText
        {
            get => (string)GetValue(SuccessTextProperty);
            set => SetValue(SuccessTextProperty, value);
        }
        public static readonly DependencyProperty SuccessTextProperty = DependencyProperty.Register(nameof(SuccessText), typeof(string), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Number of created documents.
        /// Use -1 to hide the number of created documents text.
        /// </summary>
        public int NumberCreatedDocuments
        {
            get => (int)GetValue(NumberCreatedDocumentsProperty);
            set => SetValue(NumberCreatedDocumentsProperty, value);
        }
        public static readonly DependencyProperty NumberCreatedDocumentsProperty = DependencyProperty.Register(nameof(NumberCreatedDocuments), typeof(int), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Path to the last created document. If <see cref="string.Empty"/>, no document was created.
        /// Only shown when <see cref="IsSuccessful"/> is <see langword="true"/>
        /// </summary>
        public string LastCreatedDocumentPath
        {
            get => (string)GetValue(LastCreatedDocumentPathProperty);
            set => SetValue(LastCreatedDocumentPathProperty, value);
        }
        public static readonly DependencyProperty LastCreatedDocumentPathProperty = DependencyProperty.Register(nameof(LastCreatedDocumentPath), typeof(string), typeof(CreateDocumentUserControl));

        /// <summary>
        /// Array with all available orderings for the items. If no ordering is supported, this will be <see langword="null"/>.
        /// </summary>
        public IEnumerable<Enum> AvailableItemOrderings
        {
            get { return (IEnumerable<Enum>)GetValue(AvailableItemOrderingsProperty); }
            set { SetValue(AvailableItemOrderingsProperty, value); }
        }
        public static readonly DependencyProperty AvailableItemOrderingsProperty = DependencyProperty.Register(nameof(AvailableItemOrderings), typeof(IEnumerable<Enum>), typeof(CreateDocumentUserControl), new PropertyMetadata(null));

        /// <summary>
        /// Current ordering for the items. If no ordering is supported, this will be <see langword="null"/>.
        /// </summary>
        public Enum ItemOrdering
        {
            get { return (Enum)GetValue(ItemOrderingProperty); }
            set { SetValue(ItemOrderingProperty, value); }
        }
        public static readonly DependencyProperty ItemOrderingProperty = DependencyProperty.Register(nameof(ItemOrdering), typeof(Enum), typeof(CreateDocumentUserControl), new PropertyMetadata(null));


        private void btn_openDocument_Click(object sender, RoutedEventArgs e)
        {
            if(System.IO.File.Exists(LastCreatedDocumentPath))
            {
                Core.Helpers.FilePathHelper.OpenWithDefaultProgram(LastCreatedDocumentPath);
            }
        }
    }
}
