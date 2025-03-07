using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to manage a workspace
    /// </summary>
    public interface IWorkspaceService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// If true, a workspace is loaded; if false, not workspace is loaded
        /// </summary>
        bool IsWorkspaceOpen { get; set; }

        /// <summary>
        /// Settings for the current workspace
        /// </summary>
        public WorkspaceSettings Settings { get; set; }

        /// <summary>
        /// Close the current workspace (set the current path to <see cref="string.Empty"/> and the <see cref="Settings"/> to <see langword="null"/>)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="save">If true, <see cref="Save(CancellationToken, string)"/> is called before</param>
        /// <returns>true if saving during close succeeded; false if saving failed (e.g. canceled)</returns>
        Task<bool> CloseWorkspace(CancellationToken cancellationToken, bool save = true);
    }
}
