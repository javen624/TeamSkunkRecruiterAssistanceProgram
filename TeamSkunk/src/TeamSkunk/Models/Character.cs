using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamSkunk.Models
{
    public class Character
    {
        [Key]
        public int CharacterId { get; set; }

        [ForeignKey("Member")]
        public int MemberId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public int Gear { get; set; }

        public int Stars { get; set; }

        public int SevenStarG10 { get; set; }

        public virtual Member Member { get; set; }
    }
}