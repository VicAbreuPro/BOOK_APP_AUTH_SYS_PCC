using Users.Models;
using Users.Data;
using Microsoft.EntityFrameworkCore;

namespace Users.Repository
{
    public class OtpUserRepository
    {
        private readonly DataContext _dataContext;

        public OtpUserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public int CreateOtpRequest(OtpUser otpRequest)
        {
            _dataContext.OtpRequest.Add(otpRequest);

            return _dataContext.SaveChanges();
        }

        public OtpUser? GetOtpRequest(string otp)
        {
            return _dataContext.OtpRequest.Where(otpRequest => otpRequest.Otp == otp).FirstOrDefault();
        }

        public int RemoveAllOtpRequests(string email)
        {
            return _dataContext.OtpRequest.Where(otpRequest => otpRequest.Email == email).ExecuteDelete();
        }
    }
}
