using System.ComponentModel.DataAnnotations;

namespace Mastermind.ViewModels
{
    public class AuthLoginVM
    {
        //[Display(Name = "Username", ResourceType = typeof(Resource))]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ModelRequired", ErrorMessageResourceType = typeof(Resource))]
        //[StringLength(20, ErrorMessageResourceName = "ModelLengthLessThan", ErrorMessageResourceType = typeof(Resource))]
        public string Username { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        //[Display(Name = "Password", ResourceType = typeof(Resource))]
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ModelRequired", ErrorMessageResourceType = typeof(Resource))]
        //[StringLength(20, ErrorMessageResourceName = "ModelLengthLessThan", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; } = string.Empty;
    }
}
