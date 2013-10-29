using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RaginRovers
{
    class LidgrenWorkThread
    {
        ServerNetworking Communication = new ServerNetworking();
        bool Receiver = true;
        long whatever = 0;
        public void SyncComps()
        {
            while (!_shouldStop)
            {
                Communication.Receive();
                whatever++;

                Thread.Sleep(1);
            }
        }
        
        public void RequestStop()
        {
            _shouldStop = true;
        }

        private volatile bool _shouldStop;
    }
}