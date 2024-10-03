using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Models
{
    public class User
    {
        // Atributos
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El RUT es obligatorio")]
        [RegularExpression(@"\d{7,8}-[\dkK]", ErrorMessage = "RUT inválido")]
        public string Rut { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El género es obligatorio")]
        [RegularExpression("^(masculino|femenino|otro|prefiero no decirlo)$", ErrorMessage = "Género inválido")]
        public string Genero { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(User), "ValidateBirthDate")]
        public DateTime FechaNacimiento { get; set; }

        // Método de validación personalizado para la fecha de nacimiento
        public static ValidationResult? ValidateBirthDate(DateTime fechaNacimiento, ValidationContext context)
        {
            if (fechaNacimiento >= DateTime.Now)
            {
                return new ValidationResult("La fecha de nacimiento debe ser anterior a la fecha actual.");
            }
            return ValidationResult.Success;
        }
    }
}