using CommunityToolkit.Mvvm.ComponentModel;

namespace Vereinsmeisterschaften.Models
{
    /// <summary>
    /// Class combining informations used while document creation.
    /// </summary>
    public partial class DocumentCreationStatus : ObservableObject
    {
        /// <summary>
        /// State of whether a document creation process is currently running.
        /// </summary>
        [ObservableProperty]
        private bool _isDocumentCreationRunning = false;

        /// <summary>
        /// State of whether a document creation process was successful.
        /// </summary>
        [ObservableProperty]
        private bool _isDocumentCreationSuccessful = false;

        /// <summary>
        /// State of whether data is available for document creation.
        /// </summary>
        [ObservableProperty]
        private bool _isDocumentDataAvailable = false;

        /// <summary>
        /// State of whether the template file is available for document creation.
        /// </summary>
        [ObservableProperty]
        private bool _isDocumentTemplateAvailable = false;

        /// <summary>
        /// File path of the last created document.
        /// </summary>
        [ObservableProperty]
        private string _lastDocumentFilePath = string.Empty;
    }
}
