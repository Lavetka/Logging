using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using BrainstormSessions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BrainstormSessions.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBrainstormSessionRepository _sessionRepository;
        private readonly Serilog.ILogger _logger;


        public HomeController(IBrainstormSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
            _logger = Log.ForContext<HomeController>();
        }

        public async Task<IActionResult> Index()
        {
            Console.WriteLine("hLEB");
            _logger.Debug("Index starts");
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
                _logger.Debug("Index passed well");
                return View(model);
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occurred.");

                Console.WriteLine("hLEB");
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
                    _logger.Warning("ModelState is invalid: {@ModelState}", ModelState);
                    Console.WriteLine("hLEB");
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
                _logger.Information("Index passed well");
                return RedirectToAction(actionName: nameof(Index));
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error occurred.");
                throw;
            }
        }

    }
}
