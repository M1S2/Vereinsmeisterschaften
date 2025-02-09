using System;
using System.Collections.Generic;
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
        private List<Person> _personList { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Constructor
        /// </summary>
        public PersonService()
        {
            _personList = new List<Person>();

            //#warning TESTDATA!!!!!!!!!!!!!!!!!!!!!!!!
            //AddPerson(new Person() { FirstName = "Max", Name = "Mustermann", BirthYear = 2000, Gender = Genders.Male, Breaststroke = true, Freestyle = true });
            //AddPerson(new Person() { FirstName = "Eva", Name = "Musterfrau", BirthYear = 1900, Gender = Genders.Female, Breaststroke = true, Backstroke = true, Butterfly = true, Medley = true });
            //AddPerson(new Person() { FirstName = "Tim", Name = "Mustersohn", BirthYear = 2010, Gender = Genders.Male, WaterFlea = true });
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
                    OnFileProgress?.Invoke(this, 0);
                    if (!File.Exists(PersonFilePath))
                    {
                        _personList.Clear();
                    }
                    else
                    {   
                        _personList.Clear();
                        List<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(Person)).OfType<PropertyDescriptor>().ToList();

                        List<string> lines = File.ReadAllLines(PersonFilePath).ToList();
                        if (lines.Count >= 2)
                        {
                            char delimiter = ';';
                            List<string> headers = lines.First().Split(delimiter).ToList();
                            lines.RemoveAt(0);

                            int processedElementsCnt = 0;
                            foreach (string line in lines)
                            {
                                List<string> lineParts = line.Split(delimiter).ToList();
                                Person person = new Person();
                                for (int i = 0; i < Math.Min(headers.Count, lineParts.Count); i++)
                                {
                                    person.SetPropertyFromString(headers[i], lineParts[i]);
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                                _personList.Add(person);
                                OnFileProgress?.Invoke(this, processedElementsCnt++ / (float)lines.Count);
                            }
                        }
                    }
                    OnFileProgress?.Invoke(this, 100);
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
                    OnFileProgress?.Invoke(this, 0);
                    // https://stackoverflow.com/questions/25683161/fastest-way-to-convert-a-list-of-objects-to-csv-with-each-object-values-in-a-new
                    List<string> lines = new List<string>();
                    List<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(Person)).OfType<PropertyDescriptor>().ToList();
                    char delimiter = ';';
                    List<string> headers = props.Select(x => x.Name).ToList();
                    lines.Add(string.Join(delimiter.ToString(), headers));
                    int processedElementsCnt = 0;
                    foreach (Person person in _personList)
                    {
                        string line = string.Empty;
                        foreach(string header in headers)
                        {
                            object data = typeof(Person).GetProperty(header)?.GetValue(person, null);
                            if (data is bool dataBool)
                            {
                                line += (dataBool ? "X" : "");
                            }
                            else
                            {
                                line += data;
                            }
                            line += delimiter;
                        }
                        line = line.Trim(delimiter);
                        lines.Add(line);
                        OnFileProgress?.Invoke(this, processedElementsCnt++/(float)_personList.Count);
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    File.WriteAllLines(PersonFilePath, lines.ToArray());
                    OnFileProgress?.Invoke(this, 100);
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
        /// Return all available Persons.
        /// </summary>
        /// <returns>List of <see cref="Person"/> objects</returns>
        public List<Person> GetPersons() => _personList;

        /// <summary>
        /// Clear all Persons.
        /// </summary>
        public void ClearPersons()
        {
            if (_personList == null) { _personList = new List<Person>(); }
            _personList.Clear();
        }

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        public void AddPerson(Person person)
        {
            if(_personList == null) { _personList = new List<Person>(); }
            person.PersonID = _personList.Count;
            _personList.Add(person);
        }

        /// <summary>
        /// Return the number of <see cref="Person"/>
        /// </summary>
        public int PersonCount => _personList?.Count ?? 0;
    }
}
