using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using MyApi.Models;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController(MongoCollectionService<User> userService) : ControllerBase
    {
        private readonly IMongoCollection<User> _users = userService.Collection;

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _users.Find(u => true).ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var user = _users.Find(u => u.Id == id).FirstOrDefault();
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            _users.InsertOne(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, User updatedUser)
        {
            var result = _users.ReplaceOne(u => u.Id == id, updatedUser);
            if (result.MatchedCount == 0) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _users.DeleteOne(u => u.Id == id);
            if (result.DeletedCount == 0) return NotFound();
            return NoContent();
        }
    }
}
