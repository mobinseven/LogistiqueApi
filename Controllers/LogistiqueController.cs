using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Python.Runtime;

namespace LogistiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogistiqueController : ControllerBase
    {
        [HttpPost("solve")]
        public async Task<ActionResult<string>> Solve()
        {
            string problem = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string result;
            using (Py.GIL())
            {
                dynamic rpc = PythonEngine.ModuleFromString("rpc", Properties.Resources.rpc);
                dynamic Route = rpc.solver(problem);
                result = Convert.ToString(Route);
            }
            return result;
        }
        [HttpPost("test")]
        public async Task<ActionResult<string>> Test()
        {
            string problem = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            string result;
            using (Py.GIL())
            {
                dynamic rpc = PythonEngine.ModuleFromString("rpc", Properties.Resources.rpc_testing);
                dynamic Route = rpc.solver(problem);
                result = Convert.ToString(Route);
            }
            return result;
        }
    }
}