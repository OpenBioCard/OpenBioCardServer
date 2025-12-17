namespace OpenBioCardServer.Models;

public class ContactItem
{
    public string Type { get; set; } = string.Empty;
    public string? Text { get; set; } // If Image != null, ignore this
    public Asset? Image { get; set; }
    
    public bool IsImage() => Image != null;
}