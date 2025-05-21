using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Mastermind.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom complet est requis")]
        [StringLength(20, ErrorMessage = "Le nom complet ne peut pas dépasser 20 caractères")]
        [Display(Name = "FullName")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "L'email n'est pas valide")]
        [StringLength(50, ErrorMessage = "L'email ne peut pas dépasser 50 caractères")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom d'utilisateur est requis")]
        [StringLength(20, ErrorMessage = "Le nom d'utilisateur ne peut pas dépasser 20 caractères")]
        [Remote(action: "IsUsernameAvailable", controller: "Membres", 
            AdditionalFields = "Id", ErrorMessage = "Ce nom d'utilisateur est déjà utilisé")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, ErrorMessage = "Le mot de passe ne peut pas dépasser 100 caractères")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le rôle est requis")]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "ImagePath")]
        public string ImagePath { get; set; } = string.Empty;

        public Member() { }

        public Member(int id, string fullName, string email, string username, string password, string role, string imagePath)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            Username = username;
            Password = password;
            Role = role;
            ImagePath = imagePath;
        }
    }

    public static class MemberRoles
    {
        public const string Admin = "Admin";
        public const string Standard = "Standard";

        public static readonly string[] AllRoles = new[] { Admin, Standard };
    }
}
