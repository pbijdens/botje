using Botje.Messaging.Models;
using System;
using System.Collections.Generic;

namespace Botje.DB
{
    public class ClientUpdateQueueEntry : IAtom
    {
        public ClientUpdateQueueEntry()
        {
            Errors = new List<string>();
            NumberOfFailures = 0;
        }

        public Guid UniqueID { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public GetUpdatesResult Update { get; set; }

        public int NumberOfFailures { get; set; }

        public List<string> Errors { get; set; }

        public bool Failed { get; set; }
    }
}
