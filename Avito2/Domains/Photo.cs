using Avito2.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Avito2.Domains
{
    public class Photo : IEntity
    {
        [Key]
        public long Id { get; set; }
        public string FilePath { get; set; }
        public virtual Advertisement Advertisement { get; set; }
    }
}
