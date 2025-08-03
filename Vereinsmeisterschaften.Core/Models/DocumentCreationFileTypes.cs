using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Different file types of documents that can be created
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DocumentCreationFileTypes
    {
        /// <summary>
        /// Only DOCX
        /// </summary>
        DOCX,

        /// <summary>
        /// Only PDF
        /// </summary>
        PDF,

        /// <summary>
        /// DOCX and PDF
        /// </summary>
        DOCX_AND_PDF
    }
}
