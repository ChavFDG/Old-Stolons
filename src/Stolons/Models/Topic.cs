using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class Topic
    {
        public Guid TopicId { get; set; }
        [Required]
        public string Title { get; set; }
        public Guid SpeakerId { get; set; }
        public Speaker Speaker { get; set; }
    }
}
