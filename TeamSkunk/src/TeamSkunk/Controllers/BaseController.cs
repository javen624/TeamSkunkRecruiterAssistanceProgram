using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamSkunk.Services;

namespace TeamSkunk.Controllers
{
    public abstract class BaseController : Controller
    {
        public IUnitOfWork work;
    }
}
