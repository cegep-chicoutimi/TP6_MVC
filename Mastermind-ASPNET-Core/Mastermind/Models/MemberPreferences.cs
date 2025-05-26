using System.ComponentModel.DataAnnotations;
using Mastermind.Resources;

namespace Mastermind.Models
{
    public class MemberPreferences
    {
        public int Id { get; set; }
        public int MemberId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, 10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidRange")]
        [Display(ResourceType = typeof(Resource), Name = "Colors")]
        public int NbColors { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, 10, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidRange")]
        [Display(ResourceType = typeof(Resource), Name = "Positions")]
        public int NbPositions { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Required")]
        [Range(1, 20, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidRange")]
        [Display(ResourceType = typeof(Resource), Name = "Attempts")]
        public int NbAttempts { get; set; }
    }
} 