using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain.Models.Core
{
    public class BusinessNote : BaseModel, IEntity
    {
        public BusinessNote()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }

        public int NoteBy { get; set; }
        public string Token { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}
