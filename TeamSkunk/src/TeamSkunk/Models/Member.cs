using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamSkunk.Models
{
    public class Member
    {
        ///<summary>
        ///The Member Id
        ///</summary>
        [Key]
        public int MemberId { get; set; }

        ///<summary>
        ///The Person Id of the Member
        ///</summary>
        [ForeignKey("Person")]
        public int PersonId { get; set; }

        ///<summary>
        ///The Person Id of the Member
        ///</summary>
        [ForeignKey("Guild")]
        public int GuildId { get; set; }

        ///<summary>
        ///The Name of the Member
        ///</summary>
        [Required]
        public string Name { get; set; }

        ///<summary>
        ///The Member's Join Date
        ///</summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime? JoinDate { get; set; }

        ///<summary>
        ///Is this member an Officer?
        ///</summary>
        public bool isOfficer { get; set; }

        ///<summary>
        ///Is this member a Recruiter?
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
        /// RecruitedById
        ///</summary>
        public int recruitedById { get; set; }

        ///<summary>
        /// RecruitedByName
        ///</summary>
        public string recruitedByName { get; set; }

        ///<summary>
        /// TimezoneId
        ///</summary>
        public string TimezoneId { get; set; }

        ///<summary>
        /// TimezoneName
        ///</summary>
        public string TimezoneName { get; set; }

        ///<summary>
        ///Person Nav Property
        ///</summary>
        public virtual Person Person { get; set; }

        ///<summary>
        ///The Guild Nav Property
        ///</summary>
        public virtual Guild Guild { get; set; }

        ///<summary>
        ///Collection of this Member's Characters
        ///</summary>
        public virtual ICollection<Character> Characters { get; set; }
    }
}