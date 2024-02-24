using Users.Dtos;
using Microsoft.AspNetCore.Mvc;
using Users.Models;
using Users.Repository;
using Users.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text;
using OtpNet;

namespace Users.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly OtpUserRepository _otpRepository;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;

        public UserController(UserRepository userRepository, OtpUserRepository otpRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _otpRepository = otpRepository;

            _httpClient = new()
            {
                BaseAddress = new Uri("http://host.docker.internal:8083/Email/")
            };
        }

        [Route("register")]
        [HttpPost]
        public async Task<ActionResult> Register([FromBody] CreateUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var checkUsernameAndEmail = _userRepository.CheckUsernameAndEmail(user.Email, user.Username);

            if(checkUsernameAndEmail == 1)
            {
                return BadRequest("Username already registered");
            }

            if (checkUsernameAndEmail == 2)
            {
                return BadRequest("Email already registered");
            }

            var affectedRows = _userRepository.CreateUser(new User
            {
                UserName = user.Username,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                FullName = user.FullName,
                Country = user.Country,
                Role = user.Role,
            });

            if (affectedRows == 0)
            {
                return StatusCode(500, "Unexpected Error");
            }

            var resultMessage = "User Registered!";

            try
            {
                
                var mailObject = new
                {
                    To = user.Email,
                    Subject = "Welcome to BookApp Review",
                    Message = "Welcome user" + user.Username
                };

                var jsonToSend = JsonConvert.SerializeObject(mailObject);
                var content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("send", content);

                if (!response.IsSuccessStatusCode)
                {
                    resultMessage += " Email confirmation not sent! (Service Unavailable, internal error)";
                }
            }

            catch (Exception ex)
            {
                resultMessage += " EmailService Unavailable due the error: " + ex.Message;
            }

            return StatusCode(201, resultMessage);
        }

        [Route("login")]
        [HttpPost]
        public ActionResult LoginAsync([FromBody] Login loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int removeAttempts = 0;
            var checkAttempts = _userRepository.GetLoginAttempts(loginRequest.Email);

            if(checkAttempts > 5)
            {
                DateTime currentTime = DateTime.Now;

                var attempt = _userRepository.GetLastLoginAttempt(loginRequest.Email);

                if(attempt != null)
                {
                    TimeSpan dif = currentTime - attempt.CreatedAt;

                    Console.WriteLine("Attempt time: " + attempt.CreatedAt.ToString());
                    Console.WriteLine("Current time: " + currentTime.ToString());
                    Console.WriteLine("Dif: " + dif.ToString());


                    if(dif.TotalMinutes <= 5)
                    {
                        return BadRequest("To Many attempts, try again later!");
                    }

                    if (dif.TotalMinutes >= 5)
                    {
                        removeAttempts = _userRepository.RemoveAllLoginAttempts(loginRequest.Email);

                        if (removeAttempts == 0)
                        {
                            return StatusCode(500, "Unexpected Error! Remove Attempt Login error");
                        }
                    }
                }

                if(attempt == null)
                {
                    return StatusCode(500, "Unexpected Error! Attempt Login error");
                }
            }

            DateTime currentTimeAux = DateTime.Now;

            var loginAttemptInsert = _userRepository.CreateLoginAttempt(new LoginAttempts
            {
                email = loginRequest.Email,
                CreatedAt = currentTimeAux
            });

            if(loginAttemptInsert == 0)
            {
                return StatusCode(500, "Unexpected Error!Insert Attempt Login error");
            }

            var user = _userRepository.GetUserByEmail(loginRequest.Email);
            var token = "";

            if(user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
                {
                    return BadRequest("Invalid Credentials");
                }

                try
                {
                    TokenService tokenconfig;

                    tokenconfig = new TokenService(_configuration);

                    token = tokenconfig.GenerateToken(user);

                    removeAttempts = _userRepository.RemoveAllLoginAttempts(loginRequest.Email);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }

            if (user == null)
            {
                return BadRequest("Invalid Credentials");
            }

            return Ok(token);
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword changePasswordRequest)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var token = "";
                var resultMessage = "Password Changed Succesfully";

                TokenService ts;

                ts = new TokenService(_configuration);

                if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    token = authHeader.ToString().Replace("Bearer ", "");
                }

                if (!HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderAux))
                {
                    return BadRequest("Invalid Token");
                }

                // Get userId from token
                int userId = ts.GetUserIdFromJwtToken(token);

                var user = _userRepository.GetUserById(userId);

                if(user != null)
                {
                    if (!BCrypt.Net.BCrypt.Verify(changePasswordRequest.OldPassword, user.Password))
                    {
                        return BadRequest("Invalid OldPassword");
                    }
                }

                if(user == null)
                {
                    return NotFound("Invalid User");
                }

                var affectedRows = _userRepository.UpdatePassword(user, changePasswordRequest.NewPassword);

                if (affectedRows == 0)
                {
                    return StatusCode(500, "Unexpected error!");
                }

                try
                {
                    var mailObject = new
                    {
                        To = user.Email,
                        Subject = "Password Changed",
                        Message = "You Password has been changed"
                    };

                    var jsonToSend = JsonConvert.SerializeObject(mailObject);
                    var content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("send", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        resultMessage += " Email confirmation not sent! (Service Unavailable, internal error)";
                    }
                }

                catch (Exception ex)
                {
                    resultMessage += "EmailService Unavailable due the error: " + ex.Message;
                }

                return Ok("Password Changed!");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPassword forgotPasswordRequest)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string? secretKey   = _configuration.GetSection("AppSettings:TOTP_Key").Value;

            DateTime currentTime = DateTime.UtcNow;

            // Create a TOTP generator with a 30-second time step and a 6-digit code length
            var totp = new Totp(Base32Encoding.ToBytes(secretKey), step: 520);

            // Calculate the TOTP code for the current time
            string otp = totp.ComputeTotp(currentTime);

            try
            {
                _otpRepository.CreateOtpRequest(new OtpUser
                {
                    Email = forgotPasswordRequest.Email,
                    Otp = otp
                });

                var mailObject = new
                {
                    To = forgotPasswordRequest.Email,
                    Subject = "Forgot Password",
                    Message = "A request to change your password has been issued. Please to reset your password, insert the following  otp code " + otp
                };

                var jsonToSend = JsonConvert.SerializeObject(mailObject);

                var content = new StringContent(jsonToSend, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("send", content);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(500, " Email confirmation not sent! (Service Unavailable, internal error");
                }

                return Ok("If exists an account registered with the email: " + forgotPasswordRequest.Email + " , then a message will be sent.");
            }

            catch (Exception ex)
            {
                return StatusCode(500,ex.Message);
            }
        }

        [HttpPut("ResetPassword")]
        public ActionResult ResetPassword([FromBody] string newPassword, [FromQuery] string totpReceived)
        {

            if (newPassword == null || newPassword == string.Empty || newPassword.Length < 4)
            {
                return BadRequest("Password is required");
            }

            if (newPassword.Length < 4)
            {
                return BadRequest("Password too short");
            }

            string? secretKey = _configuration.GetSection("AppSettings:TOTP_Key").Value;

            var totp = new Totp(Base32Encoding.ToBytes(secretKey), step: 520);

            bool result = totp.VerifyTotp(totpReceived, out _, window: null);

            if(result != true)
            {
                return BadRequest("Invalid TOTP");
            }

            OtpUser? otpRequest = _otpRepository.GetOtpRequest(totpReceived);

            if (otpRequest == null)
            {
                return StatusCode(500, "Internal server error. The password cannot be updated, please contact the support");
            }

            User? user = _userRepository.GetUserByEmail(otpRequest.Email);

            if (user == null)
            {
                return StatusCode(500, "Internal server error. The password cannot be updated, please contact the support");
            }

            var affectedRows = _userRepository.UpdatePassword(user, newPassword);

            if(affectedRows == 0)
            {
                return StatusCode(500, "Internal server error. The password cannot be updated, please contact the support");
            }


            _otpRepository.RemoveAllOtpRequests(user.Email);

            return Ok("Passwor updated!");

        }
    }
}
