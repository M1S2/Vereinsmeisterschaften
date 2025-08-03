using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vereinsmeisterschaften.ViewModels
{
    public enum WorkspaceSettingEditorTypes
    {
        Numeric,
        String,
        FileRelative,
        FileAbsolute,
        FileDocxRelative,
        FileDocxAbsolute,
        FolderRelative,
        FolderAbsolute,
        Boolean,
        Enum
    }
}
