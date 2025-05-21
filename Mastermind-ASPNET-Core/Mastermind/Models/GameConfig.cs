using System.ComponentModel.DataAnnotations;

namespace Mastermind.Models
{
    public class GameConfig
    {
        [Required(ErrorMessage = "Le nombre maximum de tentatives est requis")]
        [Range(1, 20, ErrorMessage = "Le nombre de tentatives doit être entre 1 et 20")]
        public int MaxAttempts { get; set; }

        [Required(ErrorMessage = "La longueur du code est requise")]
        [Range(4, 8, ErrorMessage = "La longueur du code doit être entre 4 et 8")]
        public int CodeLength { get; set; }

        [Required(ErrorMessage = "Le nombre de couleurs disponibles est requis")]
        [Range(4, 10, ErrorMessage = "Le nombre de couleurs doit être entre 4 et 10")]
        public int AvailableColors { get; set; }
    }
} 