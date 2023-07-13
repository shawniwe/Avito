using Avito2.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Avito2.Domains
{
    public class AdvertisementStatement : IEntity
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
