using System.ComponentModel.DataAnnotations;

namespace Mastermind.Areas.Admin.ViewModels
{
    public class ConfigEditVM
    {
        [Required(ErrorMessage = "Le nombre de couleurs est requis")]
        [Range(6, 8, ErrorMessage = "Vous devez choisir entre {1} et {2} couleurs.")]
        public int NbColors { get; set; }

        [Required(ErrorMessage = "Le nombre de positions est requis")]
        [Range(4, 5, ErrorMessage = "Vous devez choisir entre {1} et {2} positions.")]
        public int NbPositions { get; set; }

        [Required(ErrorMessage = "Le nombre de tentatives est requis")]
        [Range(6, 12, ErrorMessage = "Vous devez choisir entre {1} et {2} tentatives.")]
        public int NbAttempts { get; set; }

        // Constructeur vide requis pour la désérialisation
        public ConfigEditVM()
        {
        }

        public ConfigEditVM(int nbColors, int nbPositions, int nbAttempts)
        {
            NbColors = nbColors;
            NbPositions = nbPositions;
            NbAttempts = nbAttempts;
        }
    }
}
