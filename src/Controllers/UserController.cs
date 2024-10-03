using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.src.Models;
using api.src.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.src.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        // Inyectamos el repositorio y el logger
        public UserController(IUserRepository userRepository, ILogger<BaseApiController> logger) 
            : base(logger)
        {
            _userRepository = userRepository;
        }

        // POST: Crear un usuario
        [HttpPost("")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            // Validación de los errores del modelo
            var validationError = HandleValidationErrors();
            if (validationError != null)
                return validationError;

            // Verificar si el RUT ya existe
            if (await _userRepository.UserExistsByRutAsync(user.Rut))
                return Conflict("El RUT ya existe.");

            return await SafeExecute(async () =>
            {
                var newUser = await _userRepository.AddUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
            });
        }

        // GET: Obtener todos los usuarios
        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            return await SafeExecute(async () =>
            {
                var users = await _userRepository.GetAllUsersAsync();
                return HandleResult(users);
            });
        }

        // GET: Obtener un usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            return await SafeExecute(async () =>
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                return HandleResult(user);
            });
        }

        // PUT: Actualizar un usuario existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            // Validación de los errores del modelo
            var validationError = HandleValidationErrors();
            if (validationError != null)
                return validationError;

            // Verificar si el ID coincide
            if (id != user.Id)
                return BadRequest("El ID del usuario no coincide.");

            // Verificar si el usuario existe
            if (!await _userRepository.UserExistsByRutAsync(user.Rut))
                return NotFound("Usuario no encontrado.");

            return await SafeExecute(async () =>
            {
                await _userRepository.UpdateUserAsync(user);
                return NoContent();  // 204 No Content, actualizaciones exitosas
            });
        }

        // DELETE: Eliminar un usuario por ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return await SafeExecute(async () =>
            {
                await _userRepository.DeleteUserAsync(id);
                return NoContent();  // 204 No Content, eliminación exitosa
            });
        }
    }
}
