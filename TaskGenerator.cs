using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueingSystem
{
    public class RandomGenerator
    {
        static ulong x = (ulong)DateTime.Now.Ticks;
        public ulong Rnd()
        {
            ulong a = 6364136223846793005;
            ulong c = 1442695040888963407;
            ulong m = 1844674407370955161;

            x = (x * a + c) % m;
            return x;
        }
    }
    public class TaskArray
    {
        public int QuantityXCHG = 0;
        public int QuantityCARD = 0;
        public int QuantityCRED = 0;
        public int QuantityACNT = 0;
        public Request[] Requests;
        public TaskArray(int c,TimeSpan time)
        {
            Requests = new Request[c];
            TaskGenerator tskgen = new TaskGenerator();
            for (int i = 0; i < c; i++)
            {             
            Request tsk = tskgen.GenTask(i,time);
                if (tsk.Type == RequestType.XCHG)
                {
                    QuantityXCHG++;
                }
                else if (tsk.Type == RequestType.CARD)
                {
                    QuantityCARD++;
                }
                else if (tsk.Type == RequestType.CRED)
                {
                    QuantityCRED++;
                }
                else
                {
                    QuantityACNT++;
                }
                Requests[i] = tsk;
            }
        }
    }

    public class TaskGenerator
    {
        //должен дать нумерацию таскам и присвоить им название XCHG,...
        
        public RandomGenerator rand=new RandomGenerator();
        public Request GenTask(int c, TimeSpan time)//
        {
            ulong x = rand.Rnd();
            while (x % 10 > 3)
            {
                x = rand.Rnd();
            }
            if (x % 10 == 0)
            {
                Request rq = new Request(c, RequestType.XCHG);
                rq.TimeStarted = time;
                return rq;
            }
            else if (x % 10 == 1)
            {
                Request rq = new Request(c, RequestType.CARD);
                rq.TimeStarted = time;
                return rq;
            }
            else if (x % 10 == 2)
            {
                Request rq = new Request(c, RequestType.CRED);
                rq.TimeStarted = time;
                return rq;
            }
            else 
            {
                Request rq = new Request(c, RequestType.ACNT);
                rq.TimeStarted = time;
                return rq;
            }
        }
    }
}
