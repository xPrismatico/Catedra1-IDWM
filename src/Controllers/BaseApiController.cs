using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected readonly ILogger<BaseApiController> _logger;

        public BaseApiController(ILogger<BaseApiController> logger)
        {
            _logger = logger;
        }

        // Método genérico para manejar las respuestas
        protected ActionResult HandleResult<T>(T result)
        {
            if (result == null)
            {
                _logger.LogWarning("Recurso no encontrado.");
                return NotFound(new { message = "Recurso no encontrado" });
            }

            return Ok(result);
        }

        // Método para manejar errores de validación
        protected ActionResult HandleValidationErrors()
        {
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var state in ModelState.Values)
                {
                    foreach (var error in state.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                return BadRequest(new { message = "Errores de validación", errors });
            }

            return null;
        }

        // Método para manejar excepciones
        protected async Task<ActionResult> SafeExecute(Func<Task<ActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error inesperado.");
                return StatusCode(500, new { message = "Ocurrió un error inesperado, por favor intente más tarde." });
            }
        }
    }
}
