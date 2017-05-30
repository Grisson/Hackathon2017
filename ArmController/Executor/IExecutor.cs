using ArmController.Models.Command;

namespace ArmController.Executor
{
    interface IExecutor
    {
        void Execute(BaseCommand command);
    }
}
