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
        public static readonly CommandStore SharedInstance = new CommandStore();

        public PosePosition CurrentPosePosition { get; set; }


        private ConcurrentQueue<BaseCommand> _commands = new ConcurrentQueue<BaseCommand>(); // from cloud or human inputs

        public BaseCommand CurrentCommand { get; set; }

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
            if(CurrentCommand != null)
            {
                command = null;
                return false;
            }

            var result = _commands.TryDequeue(out command);
            CurrentCommand = command;

            return result;
        }

        public void Enqueue(BaseCommand command)
        {
            _commands.Enqueue(command);
        }

        public void DeleteAll()
        {
            BaseCommand item;
            while (_commands.TryDequeue(out item))
            {
                // spit all command
            }
        }
    }
}
