using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/user")] 

    public class UserController : ControllerBase
    {
        [HttpPost("")] // le indique que es un metodo get, cuando se haga un llamado a la URL
        
        public IActionResult CreateProduct()
	    {
            return Ok();
	    }

    }
}