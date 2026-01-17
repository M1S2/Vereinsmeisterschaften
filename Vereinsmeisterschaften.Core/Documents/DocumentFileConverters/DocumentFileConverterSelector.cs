namespace Vereinsmeisterschaften.Core.Documents.DocumentFileConverters
{
    /// <summary>
    /// Class used to select the correct <see cref="IDocumentFileConverter"/>
    /// </summary>
    public sealed class DocumentFileConverterSelector : IDocumentFileConverterSelector
    {
        private readonly IReadOnlyList<IDocumentFileConverter> _converters;

        /// <summary>
        /// Constructor for the <see cref="DocumentFileConverterSelector"/>
        /// </summary>
        /// <param name="converters">List with all available <see cref="IDocumentFileConverter"/> objects</param>
        public DocumentFileConverterSelector(IEnumerable<IDocumentFileConverter> converters)
        {
            _converters = converters.ToList();
        }

        /// <inheritdoc/>
        public IDocumentFileConverter GetConverter(string docxFile, List<IDocumentFileConverter> ignoreConverters)
        {
            // Create a list with all useable converters (not in ignore list and available)
            List<IDocumentFileConverter> useableConverterList = _converters.Except(ignoreConverters).Where(c => c.IsAvailable).ToList();

            // Choose the preferred converter (available and the first one that states that the document was created with this converter)
            IDocumentFileConverter preferred = useableConverterList.FirstOrDefault(c => c.IsDocxCreateWithThisConverter(docxFile));
            if (preferred != null)
            {
                return preferred;
            }

            // Fallback: use first available converter
            IDocumentFileConverter fallback = useableConverterList.FirstOrDefault();
            if (fallback != null)
            {
                return fallback;
            }
            return null;
        }
    }
}
