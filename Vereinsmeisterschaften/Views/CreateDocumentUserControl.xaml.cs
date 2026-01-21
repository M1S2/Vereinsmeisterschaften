using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vereinsmeisterschaften.Core.Documents.DocumentStrategies;
using Vereinsmeisterschaften.Models;

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
        /// <see cref="DocumentCreationStatus"/> used for the document creation.
        /// </summary>
        public DocumentCreationStatus CreationStatus
        {
            get { return (DocumentCreationStatus)GetValue(CreationStatusProperty); }
            set { SetValue(CreationStatusProperty, value); }
        }
        public static readonly DependencyProperty CreationStatusProperty = DependencyProperty.Register(nameof(CreationStatus), typeof(DocumentCreationStatus), typeof(CreateDocumentUserControl));

        /// <summary>
        /// <see cref="DocumentCreationConfig"/> used for the document creation.
        /// </summary>
        public DocumentCreationConfig CreationConfig
        {
            get { return (DocumentCreationConfig)GetValue(CreationConfigProperty); }
            set { SetValue(CreationConfigProperty, value); }
        }
        public static readonly DependencyProperty CreationConfigProperty = DependencyProperty.Register(nameof(CreationConfig), typeof(DocumentCreationConfig), typeof(CreateDocumentUserControl));

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
        /// <see cref="DataTemplateSelector"/> used to decide which <see cref="DataTemplate"/> is used to edit the <see cref="ItemFilterParameter"/>
        /// </summary>
        public DataTemplateSelector FilterParameterEditorTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(FilterParameterEditorTemplateSelectorProperty); }
            set { SetValue(FilterParameterEditorTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty FilterParameterEditorTemplateSelectorProperty = DependencyProperty.Register(nameof(FilterParameterEditorTemplateSelector), typeof(DataTemplateSelector), typeof(CreateDocumentUserControl), new PropertyMetadata(null));


        private void btn_openDocument_Click(object sender, RoutedEventArgs e)
        {
            if(System.IO.File.Exists(CreationStatus.LastDocumentFilePath))
            {
                Core.Helpers.FilePathHelper.OpenWithDefaultProgram(CreationStatus.LastDocumentFilePath);
            }
        }
    }
}
