using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ValidationLoggingBebugging.Models;
using ValidationLoggingBebugging.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ValidationLoggingBebugging.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ILogger<UsersController> logger;
		private readonly IUserService userService;

		public UsersController(ILogger<UsersController> logger, IUserService userService)
		{
			this.logger = logger;
			this.userService = userService;
		}

		[HttpGet]
		public ActionResult<IEnumerable<User>> Get()
		{
			logger.LogDebug("GET all users called at {Time}", DateTime.UtcNow);
			return Ok(userService.GetUsers());
		}

		[HttpGet("{id:int}")]
		public ActionResult<User> Get(int id)
		{
			logger.LogDebug("GET user by ID called with id={Id} at {Time}", id, DateTime.UtcNow);

			var user = userService.GetUserById(id);
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
			logger.LogDebug("POST create user called with username={Username} at {Time}", userRequest.Username, DateTime.UtcNow);

			if (!ModelState.IsValid)
			{
				logger.LogWarning("Model validation failed for user: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
				return BadRequest(ModelState);
			}

			var newUser = userService.CreateUser(userRequest);
			return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
		}

		[HttpPut("{id:int}")]
		public ActionResult Put(int id, [FromBody] UserRequest userRequest)
		{
			logger.LogDebug("PUT called for user with id={Id}", id);

			if (!ModelState.IsValid)
			{
				logger.LogWarning("Model validation failed for PUT id={Id} at {Time}", id, DateTime.UtcNow);
				return BadRequest(ModelState);
			}

			try
			{
				userService.UpdateUser(id, userRequest);
				logger.LogInformation("PUT successfully processed for id={Id}", id);
				return Ok();
			}
			catch (KeyNotFoundException)
			{
				logger.LogWarning("User with id={Id} not found for PUT", id);
				return NotFound();
			}		
		}

		[HttpPatch("{id:int}")]
		public ActionResult Patch(int id, [FromBody] JsonPatchDocument<UserRequest> patchDoc)
		{
			logger.LogDebug("PATCH called for user id={Id}", id);

			if (patchDoc == null)
			{
				logger.LogWarning("PATCH document is null for id={Id}", id);
				return BadRequest("Invalid patch document.");
			}

			if (!ModelState.IsValid)
			{
				logger.LogWarning("PATCH validation failed for id={Id}", id);
				return BadRequest(ModelState);
			}

			try
			{
				userService.PatchUser(id, patchDoc);
				logger.LogInformation("User with id={Id} patched successfully", id);
				return Ok();
			}
			catch (KeyNotFoundException)
			{
				logger.LogWarning("User with id={Id} not found for PATCH", id);
				return NotFound();
			}
		}
	}
}
