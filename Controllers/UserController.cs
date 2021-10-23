using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SimpleApp.Dtos;
using SimpleApp.Models;
using SimpleApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController
            (
                IUserRepository userRepository,
                IMapper mapper
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        // Get all users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var usersFromRepo = await _userRepository.GetUsers();
            var usersDto = _mapper.Map<ICollection<User>>(usersFromRepo);

            return Ok(usersDto);
        }


        // Get user by email
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUser
            (
                [FromRoute] string email
            )
        {
            if (!await _userRepository.IsUserExist(email))
            {
                return BadRequest($"User was not found with the email {email}");
            }

            var userFromRepo = await _userRepository.GetUserByEmail(email);
            var userDto = _mapper.Map<UserDto>(userFromRepo);

            return Ok(userDto);
        }

        // Post a user information and save to the JSON file
        [HttpPost]
        public async Task<IActionResult> PostUser
            (
                [FromBody] UserPostDto userPostDto
            )
        {

            if (userPostDto == null)
            {
                return BadRequest("Please specify the User FirstName and LastName");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _userRepository.IsUserExist(userPostDto.Email))
            {
                return BadRequest($"{userPostDto.Email} has already been in use");
            }

            var userToSave = _mapper.Map<User>(userPostDto);
            await _userRepository.SaveUserToFile(userToSave);

            return NoContent();
        }

        // Partially update a user with email
        [HttpPatch("{email}")]
        public async Task<IActionResult> PartiallyUpdateUserByEmail
            (
                [FromRoute]string email,
                [FromBody]JsonPatchDocument<UserUpdateDto> jsonPatchDocument
            )
        {
            if (!await _userRepository.IsUserExist(email))
            {
                return BadRequest("User was not found");
            }

            var userFromRepo = await _userRepository.GetUserByEmail(email);
            var userToPatch = _mapper.Map<UserUpdateDto>(userFromRepo);
            jsonPatchDocument.ApplyTo(userToPatch, ModelState);

            if (!TryValidateModel(userToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(userToPatch, userFromRepo);

            // Simulate the saveChanges in DB
            var oldUserToDelete = await _userRepository.GetUserByEmail(email);
            var updatedUserToSave = _mapper.Map<User>(userToPatch);

            await _userRepository.DeleteUser(oldUserToDelete);
            await _userRepository.SaveUserToFile(updatedUserToSave);

            return NoContent();
        }



        // Delete a user
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser
            (
                [FromRoute]string email
            )
        {
            if (!await _userRepository.IsUserExist(email))
            {
                return BadRequest($"The user was not found by email {email}");
            }

            var userFromRepo = await _userRepository.GetUserByEmail(email);
            await _userRepository.DeleteUser(userFromRepo);

            return NoContent();
        }
    }
}
