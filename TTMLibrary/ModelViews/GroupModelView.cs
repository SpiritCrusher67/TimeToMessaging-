using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TTMLibrary.ModelViews
{
    public class GroupModelView : IEntityModelView
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Название должено содержать от 6 до 50 символов", MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public string CreatorLogin { get; set; }
        public ICollection<UserModelView> Users { get; set; }

        public GroupModelView()
        {
            Users = new List<UserModelView>();
        }
    }
}
