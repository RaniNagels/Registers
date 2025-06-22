using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registers.ViewModels.Templates
{
    public interface IFormViewModel<T>
    {
        event Action<T>? Saved;
        event Action<Guid>? Removed;
        event Action? Canceled;
    }
}
