using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vereinsmeisterschaften.ViewModels
{
    public class WorkspaceSettingViewConfig
    {
        public string Label { get; set; }
        public string Tooltip { get; set; }
        public string Icon { get; set; }
        public WorkspaceSettingEditorTypes Editor { get; set; }
        public bool SupportResetToDefault { get; set; } = true;
    }
}
