using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.src.Models;
using api.src.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetAllUsers([FromQuery] string? sort=null,[FromQuery] string? gender=null)
        {
            return await SafeExecute(async () =>
            {
                // Empezamos obteniendo todos los usuarios como IQueryable para aplicar filtros y ordenación
                var query = _userRepository.GetAllUsersQuery();

                // Filtrar por género si se proporciona el parámetro
                if (!string.IsNullOrEmpty(gender))
                {
                    query = query.Where(u => u.Genero.ToLower() == gender.ToLower());
                }

                // Ordenar según el parámetro sort
                if (sort == "desc")
                {
                    query = query.OrderByDescending(u => u.Name);
                }
                else
                {
                    query = query.OrderBy(u => u.Name);
                }

                var users = await query.ToListAsync();
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
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                return NotFound("Usuario no encontrado.");

            // Verificar si el RUT ya está en uso por otro usuario
            if (existingUser.Rut != user.Rut && await _userRepository.UserExistsByRutAsync(user.Rut))
                return Conflict("El RUT ya está en uso por otro usuario.");

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
