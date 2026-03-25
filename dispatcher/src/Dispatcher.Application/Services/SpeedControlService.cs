using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatcher.Application.Services
{
    public class SpeedControlService
    {

        public bool IsSpeedLimitExceeded(int speed)
        {
            return speed > 90;
        }
    }
}
