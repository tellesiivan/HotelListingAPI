using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListingAPI.Data;
using HotelListingAPI.Models.Hotel;
using HotelListingAPI.Services.Hotel;
using Microsoft.IdentityModel.Tokens;

namespace HotelListingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IMapper _mapper;

        public HotelsController(IHotelRepository hotelRepository, IMapper mapper)
        {
            _hotelRepository = hotelRepository;
            _mapper = mapper;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            var hotels = await _hotelRepository.GetAllAsync();
            return Ok(_mapper.Map<List<HotelDto>>(hotels));
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelRepository.GetAsync(id);
            return hotel is not null
                ? Ok(_mapper.Map<HotelDto>(hotel))
                : NotFound($"Hotel with id:{id.ToString()} was not found");
        }

        // PUT: api/Hotels/5
        [HttpPut("Update")]
        public async Task<IActionResult> PutHotel(HotelDto hotel)
        {
            try
            {
                await _hotelRepository.UpdateAsync(_mapper.Map<HotelDto, Hotel>(hotel));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(hotel.Id))
                {
                    return NotFound();
                }
            }

            return Ok($"Hotel with the id of ${hotel.Id} was successfully updated");
        }

        // POST: api/Hotels
        [HttpPost("add")]
        public async Task<ActionResult<Hotel>> PostHotel(BaseHotel hotel)
        {
            try
            {
                var hotelToAdd = _mapper.Map<Hotel>(hotel);
                await _hotelRepository.AddAsync(hotelToAdd);
                return CreatedAtAction("GetHotel", new { id = hotelToAdd.Id }, hotelToAdd);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // DELETE: api/Hotels/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                if (!await HotelExists(id))
                {
                    throw new Exception("The hotel does not exist");
                }

                await _hotelRepository.DeleteAsync(id);
                return Ok("Hotel was deleted");
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelRepository.DoesExists(id);
        }
    }
}