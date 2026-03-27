namespace Fine.Application.DTOs;

public class CreateFineRequest
{
    public string LicensePlate { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}