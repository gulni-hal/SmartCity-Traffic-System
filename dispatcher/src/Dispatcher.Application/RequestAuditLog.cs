using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatcher.Application;

public class RequestAuditLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string TargetService { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

