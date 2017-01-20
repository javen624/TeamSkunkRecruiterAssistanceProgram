using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamSkunk.Services
{
    public enum GuildTiers
    {
        Heroic = 8,
        Tier7 = 7,
        Tier6 = 6,
        Tier5 = 5,
        Tier4 = 4,
        Tier3 = 3,
        Tier2 = 2,
        Tier1 = 1
    }

    public enum Roles
    {
        Member = 0,
        Recruiter = 1,
        Officer = 2,
        Admin = 3
    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
