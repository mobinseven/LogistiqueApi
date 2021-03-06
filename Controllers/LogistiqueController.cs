﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Python.Runtime;

namespace LogistiqueApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogistiqueController : ControllerBase
    {
        private IMemoryCache _cache;

        public LogistiqueController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        [HttpPost("solve")]
        public ActionResult<string> Solve()
        {
            string module = System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory, Path.Combine("Resources", "rpc.py")));
            string problem = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            return AddProblem(problem, module);
        }
        [HttpPost("test")]
        public ActionResult Test()
        {
            string module = System.IO.File.ReadAllText(Path.Combine(Environment.CurrentDirectory, Path.Combine("Resources", "rpc_test.py")));
            string problem = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            return AddProblem(problem, module);
        }

        private ActionResult AddProblem(string problem, string module)
        {
            try
            {
                int hash = problem.GetHashCode() & 0x7FFFFFFF;
                if (Request.Headers.Keys.Contains("Origin"))
                {
                    StringValues values;
                    if (Request.Headers.TryGetValue("Origin", out values))
                    {
                        string host;
                        if (!_cache.TryGetValue(hash, out host))
                        {
                            _cache.Set(hash, values.First());
                            Console.WriteLine(hash);
                            Task.Factory.StartNew(async () =>
                            {

                                string fileName = Path.Combine(Environment.CurrentDirectory, $"Plans/Plan-{hash}-problem.json");
                                try
                                {
                                    string result;
                                    if (!Directory.Exists("Plans"))
                                        Directory.CreateDirectory("Plans");
                                    await System.IO.File.WriteAllTextAsync(fileName, problem);
                                    using (Py.GIL())
                                    {
                                        dynamic rpc = PythonEngine.ModuleFromString("rpc", module);
                                        dynamic Route = rpc.solver(fileName);
                                        result = Convert.ToString(Route);
                                    }
                                    using (WebClient wc = new WebClient())
                                    {
                                        string address;
                                        if (_cache.TryGetValue(hash, out address))
                                        {
                                            _cache.Remove(hash);
                                            wc.UploadString($"{address}/api/Plans/Result", "POST", result);
                                        }
                                    }

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    Console.WriteLine(e.InnerException?.Message);
                                    Console.WriteLine(e.StackTrace);
                                }
                                finally
                                {
                                    System.IO.File.Delete(fileName);
                                }
                            });
                        }
                        return Ok();
                    }
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException?.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCode(500);
            }
        }
    }
}