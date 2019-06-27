using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueingSystem
{
    /// <summary>
    /// todo: Поступление заявок прирывается за полчаса до конца рабочего дня, в течение этого получаса заявки из очереди принимаются 
    /// </summary>
    public enum RequestStateType
    {
        Undefined,Queued,Started,Finalized,Interrupted,Continued,NotProcessed,Rejected,Missed
    }
    public enum RequestType
    {
        XCHG,CARD,CRED,ACNT
    }
    public class CommonRequest
    {
        public int ID;//id заявки
        public RequestType Type;//название задания (XCHG,CARD,CRED,ACNT)
        public RequestStateType RequestState { get; set; }
    }
    public delegate void EventDelegate();
    public class Request : CommonRequest
    {
        public int MachineID;
        public TimeSpan TimeQueued;//Время постановки в очередь(+)
        public TimeSpan TimeStarted;//Время начала обработки(+)
        public TimeSpan TimeFinalized;//Время конца обработки 
        public TimeSpan TimeOnQueueing;//Время проведенное в очереди
        public TimeSpan TimeInterruption;//Время прерывания
        public TimeSpan TimeResume;//Время возобновления
        public TimeSpan TimeMissed;
        public event EventDelegate RequestEvent = null;//событие
        public void InvokeEvent()
        {
            RequestEvent.Invoke();
        }
        public Request(int id,RequestType type)
        {
            ID = id;
            Type = type;
            RequestState = RequestStateType.Undefined;//Состояние заявки неопределено    
        }
        public Request(int id)
        {
            ID = id;
        }
    }
}