using Akka.Actor;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineBankingActorSystem;
using OnlineBankingActorSystem.Messagess.ProfileMessages;
using OnlineBankingWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace OnlineBankingWebApi.Controllers
{
	[Route("api/profile")]
	[ApiController]
	public class ProfileController : ControllerBase
	{

		private readonly ILoggerManager _logger;
		private readonly IActorRef _profileActor;
		private readonly IIncrement _profileIncrementor;
		private readonly IWebHostEnvironment _hostEnvironment;


		public ProfileController(ILoggerManager logger, ProfileActorProvider profileActorProvider, IIncrement incrementor, IWebHostEnvironment hostEnvironment)
		{
			_logger = logger;
			_profileActor = profileActorProvider();
			_profileIncrementor = incrementor;
			_hostEnvironment = hostEnvironment;
		}

		[HttpPost("editUser")]
		public async Task<IActionResult> EditUser(EditUserModel editUserModal) {
			_logger.LogInfo($"{nameof(EditUser)}, editing of user information {editUserModal.Address ?? editUserModal.Email ?? editUserModal.Mobile ?? editUserModal.UserName} where user has token {editUserModal.UserToken} ");
			
			object result;
			if (!string.IsNullOrEmpty(editUserModal.Address))
			{
				result = await _profileActor.Ask(new EditUserAddress(_profileIncrementor.Increment(nameof(EditUser)), editUserModal.UserToken, editUserModal.Address));
			}
			else if (!string.IsNullOrEmpty(editUserModal.Email))
			{
				result = await _profileActor.Ask(new EditUserEmail(_profileIncrementor.Increment(nameof(EditUser)), editUserModal.UserToken, editUserModal.Email));
			}
			else if (!string.IsNullOrEmpty(editUserModal.Mobile))
			{
				result = await _profileActor.Ask(new EditUserMobile(_profileIncrementor.Increment(nameof(EditUser)), editUserModal.UserToken, editUserModal.Mobile));
			}
			else if (!string.IsNullOrEmpty(editUserModal.UserName))
			{
				result = await _profileActor.Ask(new EditUserName(_profileIncrementor.Increment(nameof(EditUser)), editUserModal.UserToken, editUserModal.UserName));
			}
			else
			{
				return BadRequest("Invalid data sent to be updated");
			}

			return Ok(result);
		}

		
		[HttpPost("editLogo")]
		public async Task<IActionResult> EditLogo([FromForm] EditUserLogoModel editUserLogoModel) {
			_logger.LogInfo($"{nameof(EditUser)}, editing of user logo of user with token {editUserLogoModel.UserToken} ");
			var rootPath = _hostEnvironment.ContentRootPath;
			var result = await _profileActor.Ask(new EditUserLogo(_profileIncrementor.Increment(nameof(EditLogo)),editUserLogoModel.UserToken, rootPath, editUserLogoModel.LogoName, editUserLogoModel.LogoFile));
			return Ok(result);
		}

		[HttpGet("getUserProfile/{userToken}")]
		public async Task<ActionResult<GetUserDataResponseModel>> GetUserProfileDate(string userToken)
		{
			_logger.LogInfo($"{nameof(GetUserProfileDate)}, geting user profile data; user has user token: {userToken}");
			var result = await _profileActor.Ask(new GetUserData(_profileIncrementor.Increment(nameof(GetUserProfileDate)),userToken));
			var userData = (UserDataRetrieved)result;
			var imageName = userData.ProfileImagePath?.Split('\\').Last();
			var response = new GetUserDataResponseModel
			{
				UserToken = userData.UserToken,
				HomeAddress = userData.Address,
				Email = userData.Email,
				MobileNumber = userData.Mobile,
				UserName = userData.UserName,
				Logo =userData.ProfileImagePath !=null ?  $"{Request.Scheme}://{Request.Host}{Request.PathBase}/ProfileImages/{imageName}" : null,
				LastLoginDate = userData.LastLoginDate,
				RegistrationDate = userData.RegistrationDate
			};

			return Ok(response);
		}
	}
}
