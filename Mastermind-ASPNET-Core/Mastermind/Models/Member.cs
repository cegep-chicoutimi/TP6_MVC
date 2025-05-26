using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Mastermind.Resources;

namespace Mastermind.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Resource), Name = "FullName")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Resource), Name = "Email")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(20, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 20 caractères")]
        [Remote(action: "IsUsernameAvailable", controller: "Membres", 
            AdditionalFields = "Id", ErrorMessage = "Ce nom d'utilisateur est déjà utilisé")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Resource), Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le rôle est requis")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "ImagePath")]
        public string ImagePath { get; set; } = string.Empty;

        [Display(ResourceType = typeof(Resource), Name = "RegistrationDate")]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        public Member() { }

        public Member(int id, string fullName, string email, string username, string password, string role, string imagePath, DateTime registrationDate)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            Username = username;
            Password = password;
            Role = role;
            ImagePath = imagePath;
            RegistrationDate = registrationDate;
        }
    }

    public static class MemberRoles
    {
        public const string Admin = "Admin";
        public const string Standard = "Standard";

        public static readonly string[] AllRoles = new[] { Admin, Standard };
    }
}
