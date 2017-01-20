using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeamSkunk.Services;

namespace TeamSkunk.Models
{
    public class Guild
    {
        ///<summary>
        ///The ID of the Guild
        ///</summary>
        [Key]
        public int GuildId { get; set; }

        ///<summary>
        ///The Name of the Guild
        ///</summary>
        [Required]
        public string Name { get; set; }

        ///<summary>
        ///The swgoh.gg URL link of the Guild
        ///</summary>
        //[Required]
        public string swgohLink { get; set; }

        ///<summary>
        ///The Tier of the Guild
        ///</summary>
        public GuildTiers Tier { get; set; }

        /// <summary>
        /// List of this Guild's Members
        /// </summary>
        public virtual ICollection<Member> Members { get; set; }

        ///<summary>
        ///Guild Reset Time in CST
        ///</summary>
        public DateTime? ResetTime { get; set; }

        ///<summary>
        ///List of Guild Raid Times in CST
        ///</summary>
        //public List<DateTime?> RaidTimes { get; set; }
        public string RaidTimes { get; set; } //TODO make this better

        ///<summary>
        ///Remarks
        ///</summary>
        public string Remarks { get; set; }

        ///<summary>
        ///Recruitiing Queue Order
        ///</summary>
        [Required]
        [Range(0, 100, ErrorMessage = "Can only be between 0 .. 100")]
        public int Order { get; set; }

        ///<summary>
        ///Transfer Trump Order?
        ///</summary>
        public bool TransferTrump {get; set;}

    }
}
