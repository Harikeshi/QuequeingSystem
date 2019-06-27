using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueingSystem
{
    public class ListRequest
    {
        public List<Request> Requests = new List<Request>();
        public int Length = 0;
        public void Add(Request request)
        {
            Requests.Add(request);
            Length++;
        }
        public void Remove(Request request)
        {
            if (Length > 0)
            {
                Requests.Remove(request);
                Length--;
            }
            else
            {
            }
        }
        public Request Find(RequestType type)
        {
            Request temp = new Request(-1);
            for (int i = 0; i < Length; i++)
            {
                if (Requests[i].Type == type)
                {
                    temp = Requests[i];
                    Requests.Remove(Requests[i]);
                    Length--;
                    return temp;
                }
            }
            return temp;
        }
    }
}
