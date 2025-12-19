using System;
using System.Collections.Generic;

namespace DoAnChamSocSucKhoe.Areas.Admin.Models
{
    public class FeedbackListViewModel
    {
        public required List<FeedbackViewModel> Feedbacks { get; set; }
        public required string SearchTerm { get; set; }
        public required string StatusFilter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class FeedbackViewModel
    {
        public int Id { get; set; }
        public required string SenderName { get; set; }
        public required string SenderEmail { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Status { get; set; }
        public required string StatusClass { get; set; }
    }
}
