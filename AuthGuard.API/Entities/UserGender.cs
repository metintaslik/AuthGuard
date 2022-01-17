using System.ComponentModel.DataAnnotations;

namespace AuthGuard.API.Entities
{
    public enum UserGender
    {
        [Display(Name = "DontWantToSpecify")]
        DontWantToSpecify = 0,

        [Display(Name = "Male")]
        Male = 1,

        [Display(Name = "Female")]
        Female = 2
    }
}