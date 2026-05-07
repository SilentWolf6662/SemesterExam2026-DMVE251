using System;
using System.Collections.Generic;
using System.Text;

namespace BookRight.Domain.ValueObjects
{
    public record Address  //Patient og klinik skal have adresse
    {
        public string StreetName { get; private set; }
        public int Zipcode { get; private set; }
    }
}
