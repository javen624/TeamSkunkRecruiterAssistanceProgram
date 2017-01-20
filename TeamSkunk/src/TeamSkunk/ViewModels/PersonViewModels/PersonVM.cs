using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamSkunk.Models;
using TeamSkunk.Services;

namespace TeamSkunk.ViewModels
{
    public class PersonVM
    {
        ///<summary>
        ///The ID of the Person
        ///</summary>
        public int PersonId { get; set; }

        ///<summary>
        ///The DiscordName of the Person
        ///</summary>
        public string DiscordName { get; set; }

        public virtual ICollection<Member> Members { get; set; }

        ///<summary>
        ///Number of Characters in Guilds
        ///</summary>
        public int NumberOfMembers { get; set; }
    }
}