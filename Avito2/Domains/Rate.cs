using Avito2.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Avito2.Domains
{
    public class Rate : IEntity
    {
        [Key]
        public long Id { get; set; }
        public string TargetUserId { get; set; }
        public string SoruceUserId { get; set; }
        [Range(1, 5, ErrorMessage = "Ошибка! Оценка должна быть не меньше 1-го и не больше 5-ти!")]
        public int Grade { get; set; }
    }
}
