using System.Collections.Generic;

namespace ReversePhoneLookup.Models.Requests
{
    public class AddPhoneRequest
    {
        public string Phone { get; set; }
        public List<string> Names { get; set; }
    }
}
