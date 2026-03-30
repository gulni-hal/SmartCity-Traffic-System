using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatcher.Application;

public interface IAuthValidationService
{
    Task<AuthValidationResult> ValidateAsync(string token);
}