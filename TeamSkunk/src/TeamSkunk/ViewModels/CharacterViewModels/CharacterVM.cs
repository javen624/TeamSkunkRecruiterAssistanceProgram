using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TeamSkunk.ViewModels.CharacterViewModels
{
    public class CharacterVM
    {
        public int CharacterId { get; set; }

        public int MemberId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public int Gear { get; set; }

        public int Stars { get; set; }

        public int SevenStarG10 { get; set; }

    }
}
