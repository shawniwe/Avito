using Avito2.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Avito2.Domains
{
    public class Advertisement : IEntity
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public virtual AdvertisementStatement Statement { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public virtual Category Category { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public DateTime PlacementDate { get; set; }
        public string CreationAuthorId { get; set; }
    }
}
