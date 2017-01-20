using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TeamSkunk.Models;
using TeamSkunk.Services;

namespace TeamSkunk.ViewModels
{
    public class GuildVM
    {
        ///<summary>
        ///The ID of the Guild
        ///</summary>
        public int GuildId { get; set; }

        ///<summary>
        ///The Name of the Guild
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///The swgoh.gg Link of the Guild
        ///</summary>
        public string swgohLink { get; set; }

        ///<summary>
        ///The Tier of the Guild
        ///</summary>
        public SelectionVM Tier { get; set; }

        ///<summary>
        ///Number of Guild Members
        ///</summary>
        public int MemberCount { get; set; }

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
        public int Order { get; set; }

        ///<summary>
        ///Transfer Trump Bool
        ///</summary>
        public bool TransferTrump { get; set; }

        ///<summary>
        ///String of recruiters
        ///</summary>
        public string Recruiters { get; set; }

        ///<summary>
        ///String of non-recruiting leaders
        ///</summary>
        public string Officers { get; set; }


        ///<summary>
        ///List of Members
        ///</summary>
        public ICollection<Member> Members { get; set; }
    }
}