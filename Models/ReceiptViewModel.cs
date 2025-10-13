using KiCData.Models;

namespace KiCWeb.Models;

public class ReceiptViewModel
{
    public List<Attendee> Attendees { get; set; }
    public Order Order { get; set; }
    public string PaymentMethod { get; set; }
}