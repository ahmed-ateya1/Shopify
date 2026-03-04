using E_Shop.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace E_Shop.Application.Dtos.AccountDtos
{
    public class RegisterDto 
    {
        public string FullName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Remote(action: "UniqueEmail", controller: "Account", ErrorMessage = "Account Already used")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public UserOption UserOption { get; set; } = UserOption.USER;
    }
}
