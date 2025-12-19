namespace DoAnChamSocSucKhoe.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int TuVanSucKhoeId { get; set; }  // FK to TuVanSucKhoe (Conversation)
        public required string SenderId { get; set; }  // FK to NguoiDung
        public required string ReceiverId { get; set; }  // FK to NguoiDung
        public required string Content { get; set; }
        public DateTime SentTime { get; set; }
        public bool IsRead { get; set; }
        public string? MediaUrl { get; set; }
        public string? MediaType { get; set; }  // image, video, document

        public TuVanSucKhoe? TuVanSucKhoe { get; set; }
        public NguoiDung? Sender { get; set; }
        public NguoiDung? Receiver { get; set; }
    }
}
