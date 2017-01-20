using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamSkunk.Models;

namespace TeamSkunk.ViewModels
{
    public class MemberVM
    {
        ///<summary>
        ///The Member Id
        ///</summary>
        public int MemberId { get; set; }

        ///<summary>
        ///The Person Id of the Member
        ///</summary>
        public int PersonId { get; set; }

        ///<summary>
        ///The Person Id of the Member
        ///</summary>
        public int GuildId { get; set; }

        ///<summary>
        ///The Name of the Member
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///The Member's Join Date
        ///</summary>
        public DateTime? JoinDate { get; set; }

        ///<summary>
        ///Is this member an Officer?
        ///</summary>
        public bool isOfficer { get; set; }

        ///<summary>
        ///Is this member a Recuiter?
        ///</summary>
        public bool isRecruiter { get; set; }

        ///<summary>
        ///Is this member a Guild Leader?
        ///</summary>
        public bool isLeader { get; set; }

        ///<summary>
        /// SWGOH.GG URL
        ///</summary>
        public string swgohURL { get; set; }

        ///<summary>
        ///RecruitedBy Nav Property
        ///</summary>
        public SelectionVM RecruitedBy { get; set; }

        ///<summary>
        /// Timezone
        ///</summary>
        public SelectionStringVM Timezone { get; set; }

        ///<summary>
        ///Person 
        ///</summary>
        public SelectionVM  Person { get; set; } 

        ///<summary>
        ///List of persons
        ///</summary>
        public List<SelectionVM> Persons { get; set; }

        ///<summary>
        ///The Guild 
        ///</summary>
        public SelectionVM Guild { get; set; }

        ///<summary>
        ///List of guilds
        ///</summary>
        public List<SelectionVM> Guilds { get; set; }

        ///<summary>
        ///Collection of this Member's Characters
        ///</summary>
        public virtual ICollection<Character> Characters { get; set; }
    }
}