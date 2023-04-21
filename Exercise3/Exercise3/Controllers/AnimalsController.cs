using Exercise3.Models;
using Exercise3.Models.DTOs;
using Exercise3.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace Exercise3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalsRepository _animalsRepository;
        public AnimalsController(IAnimalsRepository animalsRepository)
        {
            _animalsRepository = animalsRepository; 
        }

        [HttpGet]
        public async Task<IActionResult> GetAnimals(string? orderBy) 
        {
            if (orderBy is null) { orderBy = "name"; }

            var animals = await _animalsRepository.GetAnimals(orderBy);

            return Ok(animals);
        }

        [HttpGet("{ID}")]
        public async Task<IActionResult> GetAnimalById(int ID)
        {
            var animal = await _animalsRepository.GetAnimalById(ID);
            Console.WriteLine(animal);
            if (animal == null) {
                return NotFound("Animal with id " + ID + " doesn't exists!");
            }

            return Ok(animal);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnimal(AnimalPOST animalPOST)
        {
            if (await _animalsRepository.DoesAnimalExists(animalPOST.ID))
            {
                return Conflict("Animal exists!");
            }
            
            await _animalsRepository.AddAnimal(animalPOST);

            return Created("api/animals", animalPOST);
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdateAnimal(int ID, AnimalUpdate animal)
        {
            if (_animalsRepository.GetAnimalById(ID) == null)
            {
                return NotFound();
            }

            return Ok(await _animalsRepository.UpdateAnimal(ID, animal));
        }

        [HttpDelete("{ID}")]
        public async Task<IActionResult> DeleteAnimal(int ID) 
        {
            if (!await _animalsRepository.DoesAnimalExists(ID)) 
            {
                return NotFound("Animal with id " + ID + " doesn't exists!");
            }

            _animalsRepository.DeleteAnimal(ID);
            return Ok("Object has been deleted succesfully.");
        }
    }
}
