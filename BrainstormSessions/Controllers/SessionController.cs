using System;
using System.Threading.Tasks;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BrainstormSessions.Controllers
{
    public class SessionController : Controller
    {
        private readonly IBrainstormSessionRepository _sessionRepository;
        private ILogger<SessionController> _logger;

        public SessionController(IBrainstormSessionRepository sessionRepository, ILogger<SessionController> logger)
        {
            _sessionRepository = sessionRepository;
            _logger = logger;
        }

        /* private ILogger<SessionController> Logger
         {
             get => _logger ??= HttpContext?.RequestServices.GetService<ILogger<SessionController>>();
         }*/

        public async Task<IActionResult> Index(int? id)
        {
            try {
                if (!id.HasValue)
                {
                    _logger.LogError("All Bad");
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
                Logger.LogInformation("All good");
                return View(viewModel);
            }
            catch(Exception e)
            {
                Logger.LogCritical("Error");
                throw;
            }
        }
    }
}
