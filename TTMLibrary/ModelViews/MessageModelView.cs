using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TTMLibrary.ModelViews
{
    public class MessageModelView : IEntityModelView
    {
        public Guid Id { get; set; }
        [Required]
        public string SenderLogin { get; set; }
        [Required]
        public Guid GroupId { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string AttachedFileName { get; set; }

    }
}
