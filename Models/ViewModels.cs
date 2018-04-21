using System;

namespace native_api_proxy_module.Models
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        
    }

    public class UserModel
    {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Email { get; set; }
      public DateTime Birthdate { get; set; }
    }

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}