using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to get and store a list of Person objects
    /// </summary>
    public class PersonService : IPersonService
    {
        /// <summary>
        /// Event that is raised when the file operation progress changes
        /// </summary>
        public event ProgressDelegate OnFileProgress;

        /// <summary>
        /// Event that is raised when the file operation is finished.
        /// </summary>
        public event EventHandler OnFileFinished;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Path to the person file
        /// </summary>
        public string PersonFilePath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all people
        /// </summary>
        private ObservableCollection<Person> _personList { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;

        /// <summary>
        /// Constructor
        /// </summary>
        public PersonService(IFileService fileService)
        {
            _personList = new ObservableCollection<Person>();
            _fileService = fileService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load a list of Persons to the <see cref="_personList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="_personList"/> is cleared and the functions returns loading success.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        public async Task<bool> LoadFromFile(CancellationToken cancellationToken)
        {
            bool importingResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    
                    if (!File.Exists(PersonFilePath))
                    {
                        OnFileProgress?.Invoke(this, 0);
                        _personList.Clear();
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        List<Person> list = _fileService.LoadFromCsv<Person>(PersonFilePath, cancellationToken, Person.SetPropertyFromString, OnFileProgress);
                        _personList = new ObservableCollection<Person>(list);
                    }
                    
                    importingResult = true;
                }
                catch (OperationCanceledException)
                {
                    importingResult = false;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return importingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Save the list of Persons to a file
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> SaveToFile(CancellationToken cancellationToken)
        {
            bool saveResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    _fileService.SaveToCsv(PersonFilePath, _personList.ToList(), cancellationToken, OnFileProgress, (data) =>
                    {
                        if (data is bool dataBool)
                        {
                            return dataBool ? "X" : "";
                        }
                        else
                        {
                            return data.ToString();
                        }
                    });

                    saveResult = true;
                }
                catch (OperationCanceledException)
                {
                    saveResult = false;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return saveResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Return all available Persons
        /// </summary>
        /// <returns>List of <see cref="Person"/> objects</returns>
        public ObservableCollection<Person> GetPersons() => _personList;

        /// <summary>
        /// Clear all Persons
        /// </summary>
        public void ClearAll()
        {
            if (_personList == null) { _personList = new ObservableCollection<Person>(); }
            _personList.Clear();
        }

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        public void AddPerson(Person person)
        {
            if(_personList == null) { _personList = new ObservableCollection<Person>(); }
            _personList.Add(person);
        }

        /// <summary>
        /// Return the number of <see cref="Person"/>
        /// </summary>
        public int PersonCount => _personList?.Count ?? 0;

        /// <summary>
        /// Return the total number of starts of all persons
        /// </summary>
        public int PersonStarts => _personList.Sum(p => p.Starts.Where(s => s.Value != null).Count());

        /// <summary>
        /// Find all duplicate <see cref="Person"/> objects.
        /// </summary>
        /// <returns>List with duplicate <see cref="Person"/></returns>
        public List<Person> CheckForDuplicatePerson()
        {
            List<Person> tmpPersonList = new List<Person>();
            List<Person> duplicates = new List<Person>();
            foreach (Person person in _personList)
            {
                if(!tmpPersonList.Contains(person))
                {
                    tmpPersonList.Add(person);
                }
                else
                {
                    duplicates.Add(person);
                }
            }
            return duplicates.Distinct().ToList();
        }
    }
}
