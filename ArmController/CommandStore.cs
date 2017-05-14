using ArmController.lib.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmController
{
    public class CommandStore
    {
        private readonly ConcurrentQueue<BaseCommand> _commands = new ConcurrentQueue<BaseCommand>(); // from cloud or human inputs

        public static readonly CommandStore SharedInstance = new CommandStore();

        public int Count
        {
            get
            {
                if (this._commands != null)
                {
                    return this._commands.Count;
                }

                return 0;
            }
        }
        private CommandStore()
        {
        }

        public bool TryDequeue(out BaseCommand command)
        {
            return _commands.TryDequeue(out command);
        }

        public void Enqueue(BaseCommand command)
        {
            _commands.Enqueue(command);
        }

    }
}
