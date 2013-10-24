using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace RaginRovers
{

#if WINDOWS || XBOX
    static class Program
    {
        static LidgrenWorkThread workerObject;
        static Thread workerThread;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "server")
                {
                    return;
                }
            }

            //Create Thread
            workerObject = new LidgrenWorkThread();
            workerThread = new Thread(workerObject.SyncComps);
            //Start Thread
            workerThread.Start();

            #region use elsewhere
            //Loop until worker thread active
            //while (!workerThread.IsAlive) ;

            //put the main thread to sleep to allow worker thread to do some work
            //Thread.Sleep(1);

            //stop worker thread
            //workerObject.RequestStop();

            //use join method
            //workerThread.Join();
            #endregion

            Game1 game = new Game1();
            game.Exiting += new EventHandler<EventArgs>(game_Exiting);
            game.Run();
        }

        static void game_Exiting(object sender, EventArgs e)
        {
            workerObject.RequestStop();
            workerThread.Abort();
        }
    }
#endif
}

