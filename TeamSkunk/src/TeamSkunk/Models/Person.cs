using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TeamSkunk.Models
{
    public class Person
    {
        ///<summary>
        ///The ID of the Person
        ///</summary>
        [Key]
        public int PersonId { get; set; }

        ///<summary>
        ///The DiscordName of the Person
        ///</summary>
        [Required]
        public string DiscordName { get; set; }

        public virtual ICollection<Member> Members { get; set; }

    }
}
