using BPMSystem.DB;

namespace BPMSystem.Base.Services
{
    public class BaseService
    {
        public BaseService(BPMContext bpmContext)
        {
            BPMContext = bpmContext;
        }
        public readonly BPMContext BPMContext;
    }
}
