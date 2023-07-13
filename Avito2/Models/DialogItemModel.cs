using Avito2.Domains;
using Avito2.Users;

namespace Avito2.Models
{
    public class DialogItemModel
    {
        public string InterlocutorId { get; set; }
        public string InterlocutorName { get; set; }
        public decimal AverageRate { get; set; }
        public Advertisement Advertisement { get; set; }
        public Message LastMessage { get; set; }
    }
}
