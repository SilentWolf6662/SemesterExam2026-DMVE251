using System;
using System.Collections.Generic;
using System.Text;
using BookRight.Domain.Enums;

namespace BookRight.Domain.Entities
{
    public class TreatmentType : AggregateRoot
    {
        public string Name { get; private set; }

        public AuthorizationType AuthorizationType { get; private set; }

        public int? MaxParticipants { get; private set; }

        private TreatmentType() { } // EF-Core

        private TreatmentType(string name, AuthorizationType authorizationType, int? maxParticipants)
        {
            Name = name;
            AuthorizationType = authorizationType;
            MaxParticipants = maxParticipants;
        }

        public static TreatmentType Create(string name, AuthorizationType authorizationType, int? maxParticipants)
        {
            return new TreatmentType(name, authorizationType, maxParticipants);
        }
    }
}
