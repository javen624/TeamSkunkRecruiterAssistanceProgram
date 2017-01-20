using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TeamSkunk.ViewModels.CharacterViewModels
{
    public class CharacterIndexVM
    {
        public ApplicationUser Account { get; set; }

        public IList<CharacterVM> Characters { get; set; }

    }
}
