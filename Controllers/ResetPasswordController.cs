using FurniflexBE.Context;
using FurniflexBE.DTOModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace FurniflexBE.Controllers
{
    public class ResetPasswordController : ApiController
    {
        AppDbContext db;

        public ResetPasswordController()
        {
            db = new AppDbContext();
        }

        // PATCH: api/Auth/ForgotPassword
        [HttpPatch, Route("api/Auth/ForgotPassword")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordModel)
        {
            if (forgotPasswordModel == null || string.IsNullOrEmpty(forgotPasswordModel.Email))
            {
                return BadRequest("Email is required.");
            }

            var user = await db.users.FirstOrDefaultAsync(u => u.Email == forgotPasswordModel.Email);
            if (user == null)
            {
                return NotFound();
            }

            var otp = new Random().Next(100000, 999999).ToString();
            user.Key = otp;

            await db.SaveChangesAsync();

            try
            {
                SendOtpEmail(user.Email, otp);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error sending OTP email.", ex));
            }

            return Ok("OTP sent to your email.");
        }

        // PATCH: api/Auth/VerifyOtpAndResetPassword
        [HttpPatch, Route("api/Auth/VerifyOtpAndResetPassword")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> VerifyOtpAndResetPassword([FromBody] ResetPasswordDTO verifyOtpModel)
        {
            if (verifyOtpModel == null || string.IsNullOrEmpty(verifyOtpModel.Email) ||
                string.IsNullOrEmpty(verifyOtpModel.Otp) || string.IsNullOrEmpty(verifyOtpModel.NewPassword))
            {
                return BadRequest("Email, OTP, and new password are required.");
            }

            var user = await db.users.FirstOrDefaultAsync(u => u.Email == verifyOtpModel.Email);
            if (user == null)
            {
                return NotFound();
            }

            // Verify the OTP
            if (user.Key != verifyOtpModel.Otp)
            {
                return BadRequest("Invalid OTP.");
            }

            // Reset the password
            user.Password = BCrypt.Net.BCrypt.HashPassword(verifyOtpModel.NewPassword);
            user.Key = null;

            await db.SaveChangesAsync();

            return Ok("Password has been reset successfully.");
        }

        //later i will transfer it to tasdid's functions Folder
        private void SendOtpEmail(string email, string otp)
        {
            var fromAddress = new MailAddress("cartoonworld517@gmail.com", "FurniFlex");
            var toAddress = new MailAddress(email);
            const string subject = "Your OTP Code";
            string body = $"Your OTP code is: {otp}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("cartoonworld517@gmail.com", "jtkz enba evuv nouf")
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Email sending failed: " + ex.Message);
                }
            }
        }

    }
}
