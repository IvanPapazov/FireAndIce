using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FireAndIce.Models
{
    public class Query
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Discription { get; set; }
        public string Address { get; set; }
        public string Immage { get; set; }
        public Status Status { get; set; }
        public DateTime? DateVisit { get; set; }
        public string TechId { get; set; }
        public virtual AppUser Tech { get; set; }
        public string OwnerId { get; set; }
        public virtual AppUser Owner { get; set; }
    }
    public enum Status
    {
        Waiting,
        ExpectingAVisit,
        InProgress,
        Completed
    }
}
