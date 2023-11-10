using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListingAPI.Data;
using HotelListingAPI.Models.Country;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        // represents a copy of our HotelListingDbContext
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public CountriesController(HotelListingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: /Countries/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
          if (_context.Countries.IsNullOrEmpty())
          {
              return NotFound();
          }
          Console.WriteLine("BEFORE RESPONSE");

          
          var countries = await _context.Countries.ToListAsync();
          var response = _mapper.Map<List<GetCountryDto>>(countries);
          
          Console.WriteLine("AFTER RESPONSE");
              
            return Ok(response);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDetailsDto>> GetCountry(int id)
        {
          if (_context.Countries.IsNullOrEmpty())
          {
              return NotFound();
          }
            var country = await _context.Countries.Include(country => country.Hotels)
                .FirstOrDefaultAsync(country => country.Id == id);

            if (country is null)
            {
                return NotFound();
            }

            var response = _mapper.Map<CountryDetailsDto>(country);

            return Ok(response);
        }

        // PUT: api/Countries/5
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update")]
        public async Task<IActionResult> PutCountry(UpdateCountryDto updatedCountry)
        {
       
            var country = await _context.Countries.FindAsync(updatedCountry.Id);

            if (country is null)
            {
                return NotFound();
            }

            // with Mapper, we use the values passed in(updatedCountry) to update the values of the matched country by id
            // ef automatically knows state has been modified
            _mapper.Map(updatedCountry, country);
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(updatedCountry.Id))
                {
                    return NotFound();
                }
            }

            return Ok("Country was updated");
        }

        // POST: api/Countries/add
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add")]
        public async Task<ActionResult<Country>> PostCountry(CountryDto newCountry)
        {
            // no id needed(auto generated)
            var country = _mapper.Map<Country>(newCountry);
       
            
          if (_context.Countries == null)
          {
              return Problem("Entity set 'HotelListingDbContext.Countries'  is null.");
          }
            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

