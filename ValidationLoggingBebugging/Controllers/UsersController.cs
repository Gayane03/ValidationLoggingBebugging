using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ValidationLoggingBebugging.Helpers;
using ValidationLoggingBebugging.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ValidationLoggingBebugging.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ILogger<UsersController> logger;
		public UsersController(ILogger<UsersController> logger)
		{
			this.logger = logger;	
		}

		[HttpGet]
		public ActionResult<IEnumerable<User>> Get()
		{
			logger.LogDebug("GET all users called at {Time}", DateTime.UtcNow);

			return Ok(UsersCollection.Users);
		}

		[HttpGet("{id:int}")]
		public ActionResult<User> Get(int id)
		{
			logger.LogDebug("GET user by ID called with id={Id}, at {Time} ", id, DateTime.UtcNow);

			var user = UsersCollection.Users.FirstOrDefault(u => u.Id == id);
			if (user is null)
			{
				logger.LogWarning("User with id={Id} not found", id);
				return NotFound();
			}

			logger.LogInformation("User with id={Id} retrieved successfully", id);
			return Ok(user);
		}

		[HttpPost]
		public ActionResult<User> Post([FromBody] UserRequest userRequest)
		{
			logger.LogDebug("POST create user called with username={Username} at {Time} ", userRequest.Username, DateTime.UtcNow);

			if (!ModelState.IsValid)
			{
				logger.LogWarning("Model validation failed for user: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
				return BadRequest(ModelState);
			}

			var createdUser = UsersCollection.Users.FirstOrDefault(u => u.Username == userRequest.Username);
			if (createdUser is not null)
			{				
				throw new DuplicateUsernameException();
			}

			var lastUser = UsersCollection.Users.OrderBy(u => u.Id).LastOrDefault();
			var newId = lastUser != null ? ++lastUser.Id : 1;

			var newUser = new User()
			{
				Id = newId,
				Username = userRequest.Username,
				Email = userRequest.Email,
				Password = userRequest.Password,
				DateOfBirth = userRequest.DateOfBirth,
				Quantity = userRequest.Quantity,
				Price = userRequest.Price,
				Amount = userRequest.Amount
			};

			UsersCollection.Users.Add(newUser);

			logger.LogInformation("User added: {@User}", userRequest);

			return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
		}

		[HttpPut("{id:int}")]
		public ActionResult Put(int id, [FromBody] string value)
		{
			logger.LogDebug("PUT called for user with id={Id}", id);

			if (!ModelState.IsValid)
			{
				logger.LogWarning("Model validation failed for PUT id={Id} at {Time}", id, DateTime.UtcNow);
				return BadRequest(ModelState);
			}

			logger.LogInformation("PUT successfully processed for id={Id}", id);
			return Ok();
		}

		[HttpPatch("{id:int}")]
		public ActionResult Patch(int id, [FromBody] JsonPatchDocument<UserRequest> patchDoc)
		{
			logger.LogDebug("PATCH called for user id={Id}", id);

			if (patchDoc == null)
			{
				logger.LogWarning("PATCH document is null for id={Id}", id);
				return BadRequest("Invalid patch document");
			}

			var userToPatch = UsersCollection.Users.FirstOrDefault(u => u.Id == id);
			if(userToPatch is null)
			{
				logger.LogWarning("User to patch not found with id={Id}", id);
				return NotFound();
			}
			
			patchDoc.ApplyTo(userToPatch, ModelState);

			if (!ModelState.IsValid)
			{
				logger.LogWarning("PATCH validation failed for id={Id}", id);
				return BadRequest(ModelState); 
			}

			logger.LogInformation("User with id={Id} patched successfully", id);
			return Ok();
		}
	}
}
