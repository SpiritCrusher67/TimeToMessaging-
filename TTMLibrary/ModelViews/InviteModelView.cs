using System;
using System.ComponentModel.DataAnnotations;

namespace TTMLibrary.ModelViews
{
    public class InviteModelView : IEntityModelView
    {
        public Guid Id { get; set; }
        [Required]
        public string SenderLogin { get; set; }
        [Required]
        public string ReceiverLogin { get; set; }
        public Guid? GroupId { get; set; }
    }
}
