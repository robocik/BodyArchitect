using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BodyArchitect.Shared;

namespace BodyArchitect.Service.Model
{
    public enum MapperEntryOperation
    {
        OnlyReMap,
        ReMapAndDeleteExercise
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MapperResult
    {
        public MapperResult(int rowAffected)
        {
            EntriesAffected = rowAffected;
        }

        public int EntriesAffected { get; private set; }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MapperData
    {
        public MapperData()
        {
            Entries=new List<MapperEntry>();
        }

        [DataMember]
        public List<MapperEntry> Entries { get; set; }

        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        public List<MapperEntry> Validate()
        {
            Dictionary<Guid,MapperEntry> dist = new Dictionary<Guid, MapperEntry>();
            List<MapperEntry> wrongEntries = new List<MapperEntry>();

            foreach (var entry in Entries)
            {
                if(dist.ContainsKey(entry.FromExerciseId))
                {
                    if (!wrongEntries.Contains(entry))
                    {
                        wrongEntries.Add(entry);
                    }
                    if (!wrongEntries.Contains(dist[entry.FromExerciseId]))
                    {
                        wrongEntries.Add(dist[entry.FromExerciseId]);
                    }
                }
                else
                {
                    dist.Add(entry.FromExerciseId,entry);
                }
            }
            return wrongEntries;
        }
    }

    [DataContract(Namespace = Const.ServiceNamespace)]
    public class MapperEntry
    {
        public MapperEntry()
        {
        }

        public MapperEntry(Guid fromId, Guid toId)
        {
            this.FromExerciseId = fromId;
            this.ToExerciseId = toId;
        }

        public MapperEntry(Guid fromId, Guid toId,MapperEntryOperation operation)
        {
            this.FromExerciseId = fromId;
            this.ToExerciseId = toId;
            this.Operation = operation;
        }

        [DataMember]
        public MapperEntryOperation Operation { get; set; }

        [DataMember]
        public Guid FromExerciseId { get; set; }

        [DataMember]
        public Guid ToExerciseId { get; set; }
    }
}
