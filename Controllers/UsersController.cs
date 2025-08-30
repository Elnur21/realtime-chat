using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RealTimeChat.Models;
using MediatR;
using RealTimeChat.Handlers.Queries;
using RealTimeChat.Handlers.Commands;

namespace RealTimeChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllUsersQuery();
            var response = await _mediator.Send(query);
            
            if (response.Success)
            {
                return Ok(response.Users);
            }
            
            return BadRequest(response.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var response = await _mediator.Send(query);
            
            if (response.Success)
            {
                return Ok(response.User);
            }
            
            return NotFound(response.Message);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            var command = new CreateUserCommand
            {
                Name = user.Name,
                Email = user.Email
            };
            
            var response = await _mediator.Send(command);
            
            if (response.Success)
            {
                return CreatedAtAction(nameof(GetById), new { id = response.User?.Id }, response.User);
            }
            
            return BadRequest(response.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] User updatedUser)
        {
            var command = new UpdateUserCommand
            {
                Id = id,
                Name = updatedUser.Name,
                Email = updatedUser.Email
            };
            
            var response = await _mediator.Send(command);
            
            if (response.Success)
            {
                return NoContent();
            }
            
            return NotFound(response.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var command = new DeleteUserCommand { Id = id };
            var response = await _mediator.Send(command);
            
            if (response.Success)
            {
                return NoContent();
            }
            
            return NotFound(response.Message);
        }
    }
}
