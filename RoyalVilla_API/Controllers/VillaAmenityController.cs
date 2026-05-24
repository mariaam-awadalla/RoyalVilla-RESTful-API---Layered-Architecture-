using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class VillaAmenityController : ControllerBase
    {
        
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaAmenityController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaAmenityDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaAmenityDTO>>>> GetVillaAmenities()
        {
            try
            {
                var villaAmenities = await _db.VillaAmenities.Include(u => u.Villa).ToListAsync();

                var dtovillaAmenities = _mapper.Map<List<VillaAmenityDTO>>(villaAmenities);

                var response = APIResponse<IEnumerable<VillaAmenityDTO>>.Ok(dtovillaAmenities, "Villa Amenities retrieved successfully");

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, "An error occurred while retrieving Villa Amenities", ex.Message);

                return StatusCode(500, errorResponse);
            }
        }


        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaAmenityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenityDTO>>> GetVillaAmnetiesById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa Amenity ID must be greater than 0"));
                }
                //var villaAmenity = await _db.VillaAmenities.FirstOrDefaultAsync(u => u.Id == id);
                var villaAmenity = await _db.VillaAmenities.Include(u => u.Villa).FirstOrDefaultAsync(u => u.Id == id);

                if (villaAmenity == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa Amenity with ID {id} was not found"));
                }


                return Ok(APIResponse<VillaAmenityDTO>.Ok(_mapper.Map<VillaAmenityDTO>(villaAmenity), "Recorded reterived sucessfully"));
            }
            catch (Exception ex)
            {

                var errorResponse = APIResponse<object>.Error(500, "An Error occurred while retrieving villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<VillaAmenityDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
     
        public async Task<ActionResult<APIResponse<VillaAmenityDTO>>> CreateVillaAmenity(VillaAmenityCreateDTO villaAmenityCreateDTO)
        {
            try
            {
              
                if (villaAmenityCreateDTO == null)
                {

                    return BadRequest(APIResponse<object>.BadRequest("Villa Amenity is Required")
);
                }
                // Check if the Villa exists before assigning amenities to it.
                // We cannot create an amenity for a Villa that does not exist.
                var villaExists = await _db.Villas.FirstOrDefaultAsync(v => v.Id == villaAmenityCreateDTO.VillaId);
                if (villaExists == null)
                {
                    return NotFound(APIResponse<object>.NotFound("Villa you need post amneties is not exist"));

                }

                VillaAmenity villaAmenity = _mapper.Map<VillaAmenity>(villaAmenityCreateDTO);//from .. to as i define in auto mapper in program.cs
                villaAmenity.CreatedDate = DateTime.Now; //Server-Controlled Data as this is not  frontend respobsiblty
                await _db.VillaAmenities.AddAsync(villaAmenity);
                await _db.SaveChangesAsync();//INSERT INTO VillaAmenities

                //Reading / Retrieval
                // Reload entity with Navigation Property
                var createdAmenity = await _db.VillaAmenities
                  .Include(v => v.Villa)
                  .FirstOrDefaultAsync(v => v.Id == villaAmenity.Id);


                var response = APIResponse<VillaAmenityDTO>.CreatedAt(_mapper.Map<VillaAmenityDTO>(createdAmenity), "Villa Amenity created successfully");
                
                return CreatedAtAction(nameof(GetVillaAmnetiesById), new { id = villaAmenity.Id }, response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500, " An Error occurred while Creating Villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaAmenityDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaAmenityDTO>>> UpdateVillaAmenity(int id, VillaAmenityUpdateDTO villaAmenityUpdateDTO)
        {
            try
            {

                // Validate request body
                if (villaAmenityUpdateDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa Amenity data is required")
);
                }
                // Validate URL id matches Body id
                if (id != villaAmenityUpdateDTO.Id)
                { 
                    return NotFound(APIResponse<object>.NotFound("Villa Amenity ID in URL does not match request body"));

                }

                // Check if Villa exists
                var villaExists = await _db.Villas.FirstOrDefaultAsync( v => v.Id == villaAmenityUpdateDTO.VillaId);

                if (villaExists == null)
                {
                    return NotFound(
                        APIResponse<object>.NotFound(
                            "Villa does not exist"
                        )
                    );
                }

                // Retrieve existing amenity
                var existingAmenity = await _db.VillaAmenities
                    .FirstOrDefaultAsync(
                        v => v.Id == id
                    );

                if (existingAmenity == null)
                {
                    return NotFound(
                        APIResponse<object>.NotFound(
                            $"Villa Amenity with ID {id} was not found"
                        )
                    );
                }
                // Update existing entity using AutoMapper
                _mapper.Map(villaAmenityUpdateDTO, existingAmenity);

                // Server controlled field
                existingAmenity.UpdatedDate = DateTime.Now;

                // Save changes
                await _db.SaveChangesAsync();

                // Reload with Navigation Property
                var updatedAmenity = await _db.VillaAmenities
                    .Include(v => v.Villa)
                    .FirstOrDefaultAsync(v => v.Id == id);

                // Response DTO
                var response =
                    APIResponse<VillaAmenityDTO>.Ok(
                        _mapper.Map<VillaAmenityDTO>(updatedAmenity),
                        "Villa Amenity updated successfully"
                    );

                return Ok(response);
            }
            catch (Exception ex)
            {

                var errorResponse = APIResponse<object>.Error(500, "An Error occurred while updating villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

    }
}
