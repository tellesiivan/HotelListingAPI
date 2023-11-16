using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListingAPI.Data;
using HotelListingAPI.Models.Country;
using HotelListingAPI.Services.Country;
using Microsoft.AspNetCore.Authorization;

namespace HotelListingAPI.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
            _mapper = mapper;
        }

        // GET: /Countries/all
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
          
          var countries = await _countriesRepository.GetAllAsync();
          var response = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(response);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDetailsDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetCountryDetails(id);

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
        [Authorize]
        public async Task<IActionResult> PutCountry(UpdateCountryDto updatedCountry)
        {

            var country = await _countriesRepository.GetAsync(updatedCountry.Id);

            if (country is null)
            {
                return NotFound();
            }

            // with Mapper, we use the values passed in(updatedCountry) to update the values of the matched country by id
            // ef automatically knows state has been modified
            _mapper.Map(updatedCountry, country);
            
            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await CountryExists(updatedCountry.Id))
                {
                    return NotFound();
                }
            }

            return Ok("Country was updated");
        }

        // POST: api/Countries/add
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("add")]
        [Authorize]
        public async Task<ActionResult<Country>> PostCountry(CountryDto newCountry)
        {
            // no id needed(auto generated), map over the new country to satisfy the Country model
            var country = _mapper.Map<Country>(newCountry);

            await _countriesRepository.AddAsync(country);

            return CreatedAtAction("GetCountry", new { id = country.Id }, country);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countriesRepository.GetAsync(id);

            if (country is null)
            {
                return NotFound();
            }

            await _countriesRepository.DeleteAsync(id);
                
            return Ok("Country was successfully deleted");
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.DoesExists(id);
        }
    }

