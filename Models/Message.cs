namespace ASP.NET_OLX.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string SenderName { get; set; }      // Changed from SenderId
        public string ReceiverName { get; set; }    // Changed from ReceiverId
        public int ProductId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }


    public class MessageViewModel
    {
        public Product Product { get; set; }
        public Seller Seller { get; set; }
        public List<Message> Messages { get; set; }
    }

}

