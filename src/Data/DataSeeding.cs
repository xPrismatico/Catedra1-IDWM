using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using api.src.Models;
using Bogus;




namespace api.src.Data
{
    public class DataSeeder
    {


        // Me apoyo de Bogus para poder agregar todos estos datos a la base de datos generados random
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider; // provider que tiene bogus
                var context = services.GetRequiredService<ApplicationDBContext>(); //contexto que tiene .net lo conectamos con mi DBContext

                var existingRuts = new HashSet<string>();


                if (!context.Users.Any()){
                    var userFaker = new Faker<User>()
                        .RuleFor(u => u.Rut, f => GenerateUniqueRandomRut(existingRuts))
                        .RuleFor(u => u.Name, f => f.Person.FullName)
                        .RuleFor(u => u.Email, f => f.Person.Email)
                        .RuleFor(u => u.Genero, f => f.PickRandom("masculino", "femenino", "otro", "prefiero no decirlo"))
                        .RuleFor(u => u.FechaNacimiento, f => f.Date.Past(100, DateTime.Now)); // Generates a date in the past, up to 100 years ago
                    var users = userFaker.Generate(10);
                    context.Users.AddRange(users);
                    context.SaveChanges();
                }

                context.SaveChanges();
            }
        }


        private static string GenerateUniqueRandomRut(HashSet<string> existingRuts)
        {
            string rut;
            do{
                rut = GenerateRandomRut();
            } while (existingRuts.Contains(rut));


            existingRuts.Add(rut);
            return rut;
        }


        private static string GenerateRandomRut()
            {
                var random = new Random();
                var number = random.Next(10000000, 99999999); // Generate an 8-digit number
                var verifier = random.Next(0, 9); // Generate a single-digit verifier
                return $"{number}-{verifier}";
            }
    }
}
