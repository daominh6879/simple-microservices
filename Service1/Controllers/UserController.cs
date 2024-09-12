using Microsoft.AspNetCore.Mvc;
using SharedModels;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private static readonly List<User> Users = new List<User>
    {
        new User { Id = 1, Name = "Alice", Age = 30 },
        new User { Id = 2, Name = "Bob", Age = 25 }
    };

    [HttpGet]
    public IEnumerable<User> Get()
    {
        return Users;
    }

    [HttpGet("{id}")]
    public ActionResult<User> Get(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost]
    public ActionResult<User> Post([FromBody] User user)
    {
        user.Id = Users.Max(u => u.Id) + 1;
        Users.Add(user);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] User user)
    {
        var existingUser = Users.FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
        {
            return NotFound();
        }
        existingUser.Name = user.Name;
        existingUser.Age = user.Age;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        Users.Remove(user);
        return NoContent();
    }
}