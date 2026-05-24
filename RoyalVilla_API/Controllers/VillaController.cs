using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVilla_API.Data;
using RoyalVilla_API.Models;
using RoyalVilla_API.Models.DTO;

namespace RoyalVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,customer")]
    public class VillaController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public VillaController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        //[HttpGet]
        //public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas()
        //{

        //    var villas = await _db.Villas.ToListAsync();
        //    var dtoResponseVilla = _mapper.Map<List<VillaDTO>>(villas);
        //    var response = APIResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVilla, "Villas reterived sucessfully");
        //    return Ok(response);
        //}
        [Authorize]
        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<VillaDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<IEnumerable<VillaDTO>>>> GetVillas()
        {
            try
            {
                var villas = await _db.Villas.ToListAsync();

                var dtoResponseVilla = _mapper.Map<List<VillaDTO>>(villas);

                var response = APIResponse<IEnumerable<VillaDTO>>.Ok(dtoResponseVilla,"Villas retrieved successfully" );

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = APIResponse<object>.Error(500,"An error occurred while retrieving villas",ex.Message );

                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> GetVillaById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound(APIResponse<object>.NotFound("Villa ID must be greater than 0")
);
                }
                var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);

                if (villa == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found"));
                }

               
                return Ok(APIResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(villa), "Recorded reterived sucessfully"));
            }
            catch (Exception ex)
            {

                var errorResponse = APIResponse<object>.Error(500, "An Error occurred while retrieving villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<VillaDTO>> CreateVilla(VillaCreateDTO villaDTO)
        public async Task<ActionResult<APIResponse<VillaDTO>>> CreateVilla(VillaCreateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                   
                    return BadRequest(APIResponse<object>.BadRequest("Villa is Required")
);
                }

                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaDTO.Name.ToLower());
                if (duplicateVilla != null)
                {
                    
                    return Conflict(APIResponse<object>.Conflict($"A villa with the name '{villaDTO.Name}' already exsist")
);
                }

                Villa villa = _mapper.Map<Villa>(villaDTO);

                await _db.Villas.AddAsync(villa);
                await _db.SaveChangesAsync();

              
               var response = APIResponse<VillaDTO>.CreatedAt(_mapper.Map<VillaDTO>(villa), "Villa created successfully");

                return CreatedAtAction(nameof(GetVillaById), new { id = villa.Id }, response);
            }
            catch (Exception ex)
            {

                
                var errorResponse = APIResponse<object>.Error(500," An Error occurred while Creating Villa", ex.Message);
                return StatusCode(500,errorResponse);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<VillaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<VillaDTO>>> UpdateVilla(int id, VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa data id required")
);
                }

                if (id != villaDTO.Id)
                {
                    return BadRequest(APIResponse<object>.BadRequest("Villa Id in URL doesnt match villa id request body")
);
                }

                var existingVilla = await _db.Villas
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (existingVilla == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found")
);
                }

                var duplicateVilla = await _db.Villas.FirstOrDefaultAsync(
                    u => u.Name.ToLower() == villaDTO.Name.ToLower()
                    && u.Id != id);

                if (duplicateVilla != null)
                {
                    return Conflict(APIResponse<object>.Conflict($"A villa with the name '{villaDTO.Name}' already exists")
);
                }

                _mapper.Map(villaDTO, existingVilla);

                existingVilla.UpdatedDate = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                var response = APIResponse<VillaDTO>.Ok(_mapper.Map<VillaDTO>(existingVilla),"Villa Updated Successfully");
                return Ok(response);
        
            }
            catch (Exception ex)
            {
               
                var errorResponse = APIResponse<object>.Error(500, "An Error occurred while updating villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse<object>>> DeleteVilla(int id)
        {
            try
            {
                var existingVilla = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);

                if (existingVilla == null)
                {
                    return NotFound(APIResponse<object>.NotFound($"Villa with ID {id} was not found")
);
                }

                _db.Villas.Remove(existingVilla);

                await _db.SaveChangesAsync();

                
                var response = APIResponse<object>.NoContent("Villa Deleted Sussefully..");
                return Ok(response);
            }
            catch (Exception ex)
            {
                
                var errorResponse = APIResponse<object>.Error(500, "An Error occurred while deleting villa", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}