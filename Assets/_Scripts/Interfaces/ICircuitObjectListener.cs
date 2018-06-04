using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coop
{
  public interface ICircuitObjectListener
  {
    void OnStateChangePositive(CircuitObject source);
    void OnStateChangeNegative(CircuitObject source);
    void OnStateChangeOff(CircuitObject source);
  }
}
