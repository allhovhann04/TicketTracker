namespace TicketTracker.Api.Options;

public class SmtpOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public required string FromAddress { get; set; }
    public string FromName { get; set; } = "TicketTracker";
    public bool UseSsl { get; set; } = true;
}
