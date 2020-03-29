using App.Domain.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace App.Web.Models.ViewModels.Identity
{
    public class RoleForm
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [UIHint("TextEditor")]
        public string Description { get; set; }
    }

    public class RoleViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
        [UIHint("TextEditor")]
        public string Description { get; set; }
    }

    public static class RoleExtension
    {
        public static void Update(this ApplicationRole role, RoleForm model)
        {
            role.Name = model.Name;
            role.Description = model.Description;
        }
    }
}
