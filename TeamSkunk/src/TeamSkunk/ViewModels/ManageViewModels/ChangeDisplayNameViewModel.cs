using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TeamSkunk.ViewModels.ManageViewModels
{
    public class ChangeDisplayNameViewModel
    {

        [Required]
        [Display(Name = "New Display Name")]
        public string NewDisplayName { get; set; }

    }
}
