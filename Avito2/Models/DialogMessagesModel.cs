using Avito2.Domains;
using Avito2.Users;
using System.Collections;
using System.Collections.Generic;

namespace Avito2.Models
{
    public class DialogMessagesModel
    {
        public string InterlocutorId { get; set; }
        public string InterlocutorName { get; set; }
        public Advertisement Advertisement { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public string NewMessage { get; set; }
        public float AverageGrade { get; set; }
    }
}
