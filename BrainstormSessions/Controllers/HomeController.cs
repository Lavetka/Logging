using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using BrainstormSessions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace BrainstormSessions.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBrainstormSessionRepository _sessionRepository;
        private ILogger<HomeController> _logger;
        public HomeController(IBrainstormSessionRepository sessionRepository, ILogger<HomeController> logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var sessionList = await _sessionRepository.ListAsync();

                var model = sessionList.Select(session => new StormSessionViewModel()
                {
                    Id = session.Id,
                    DateCreated = session.DateCreated,
                    Name = session.Name,
                    IdeaCount = session.Ideas.Count
                });
                _logger.LogInformation("All good");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error: ", ex);
                throw;
            }
        }

        public class NewSessionModel
        {
            [Required]
            public string SessionName { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Index(NewSessionModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Error appears");
                    Console.WriteLine("asdasdasdasdasdasdasdasdas");
                    return BadRequest(ModelState);
                }
                else
                {
                    await _sessionRepository.AddAsync(new BrainstormSession()
                    {
                        DateCreated = DateTimeOffset.Now,
                        Name = model.SessionName
                    });
                }
                _logger.LogInformation("All good");
                return RedirectToAction(actionName: nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Issues appears", ex);
                throw;
            }
        }
    }
}
