using System;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BrainstormSessions.Controllers
{
    public class SessionController : Controller
    {
        private readonly IBrainstormSessionRepository _sessionRepository;
        private ILogger _logger;

        public SessionController(IBrainstormSessionRepository sessionRepository, ILogger logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? id)
        {
            try {
                if (!id.HasValue)
                {
                    _logger.Error("All Bad");
                    return RedirectToAction(actionName: nameof(Index),
                    controllerName: "Home");
                }

                var session = await _sessionRepository.GetByIdAsync(id.Value);
                if (session == null)
                {
                    return Content("Session not found.");
                }

                var viewModel = new StormSessionViewModel()
                {
                    DateCreated = session.DateCreated,
                    Name = session.Name,
                    Id = session.Id
                };
                _logger.Information("All good");
                return View(viewModel);
            }
            catch(Exception e)
            {
                _logger.Fatal("Error");
                throw;
            }
        }
    }
}
