using Avito2.Abstract;
using System;
using System.ComponentModel.DataAnnotations;

namespace Avito2.Domains
{
    public class Message : IEntity
    {
        [Key]
        public long Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public virtual Advertisement Advertisement { get; set; }
        public DateTime Date { get; set; }
    }
}
