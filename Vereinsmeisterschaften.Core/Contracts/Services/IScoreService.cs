using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to calculate the scores for all persons
    /// </summary>
    public interface IScoreService
    {
        void UpdateScoresForPerson(Person person, ushort competitionYear);
        void UpdateScoresForAllPersons(ushort competitionYear);
    }
}
