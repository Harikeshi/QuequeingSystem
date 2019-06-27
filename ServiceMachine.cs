using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace QueueingSystem
{
    public enum ServiceMachineState
    {
        Empty, Busy, Wait
    }
    public class ServiceMachineTypeFirst
    {
        public int ID;
        public RequestType[] ReqTypes;
        int timeXCHG;
        int timeCARD;
        Thread thread;
        object locker = new object();
        public ServiceMachineState serviceMachineState;
        public ServiceMachineTypeFirst(int id, int timeA, int timeB)
        {
            timeXCHG = timeA;
            timeCARD = timeB;
            ReqTypes = new RequestType[2];
            ReqTypes[0] = RequestType.XCHG;
            ReqTypes[1] = RequestType.CARD;
            ID = id;
            serviceMachineState = ServiceMachineState.Empty;
        }
        public void MethodXCHG()
        {
            try
            {
                Monitor.Enter(locker);
                this.serviceMachineState = ServiceMachineState.Busy;
                Thread.Sleep(timeXCHG);

            }
            finally
            {
                Monitor.Exit(locker);
                this.serviceMachineState = ServiceMachineState.Empty;
            }
        }
        public void MethodCARD()
        {
            try
            {
                Monitor.Enter(locker);
                this.serviceMachineState = ServiceMachineState.Busy;
                Thread.Sleep(timeCARD);

            }
            finally
            {
                Monitor.Exit(locker);
                this.serviceMachineState = ServiceMachineState.Empty;

            }
        }
        public async void MethodOfTypeRequest(Request req)
        {
            if (req.Type == RequestType.XCHG)
            {
                //this.thread = new Thread(this.MethodXCHG);
                //this.thread.Start();
                Task task = new Task(MethodXCHG);
                task.Start();
                await task;
            }
            if (req.Type == RequestType.CARD)
            {
                //this.thread = new Thread(this.MethodCARD);
                //this.thread.Start();
                Task task = new Task(MethodCARD);
                task.Start();
                await task;
            }
        }
    }
    public class ServiceMachineTypeSecond
    {
        public int ID;
        public RequestType[] ReqTypes;
        public ServiceMachineState serviceMachineState;
        int timeCRED;
        int timeACNT;
        object locker = new object();
        Thread thread;
        public ServiceMachineTypeSecond(int id, int timeA, int timeB)
        {
            timeCRED = timeA;
            timeACNT = timeB;
            ReqTypes = new RequestType[2];
            ReqTypes[0] = RequestType.CRED;
            ReqTypes[1] = RequestType.ACNT;
            ID = id;
            serviceMachineState = ServiceMachineState.Empty;
        }
        public void MethodCRED()
        {
            try
            {
                Monitor.Enter(locker);
                this.serviceMachineState = ServiceMachineState.Busy;
                Thread.Sleep(timeCRED);

            }
            finally
            {
                Monitor.Exit(locker);
                this.serviceMachineState = ServiceMachineState.Empty;
            }
        }
        public void MethodACNT()
        {
            try
            {
                Monitor.Enter(locker);
                this.serviceMachineState = ServiceMachineState.Busy;
                Thread.Sleep(timeACNT);
            }
            finally
            {
                Monitor.Exit(locker);
                this.serviceMachineState = ServiceMachineState.Empty;
            }
        }
        public async void MethodOfTypeRequest(Request req)
        {
            if (req.Type == RequestType.CRED)
            {
                //this.serviceMachineState = ServiceMachineState.Busy;
                //this.thread = new Thread(this.MethodCRED);
                //this.thread.Start();
                Task task = new Task(MethodCRED);
                task.Start();
                await task;
            }
            if (req.Type == RequestType.ACNT)
            {
                //this.serviceMachineState = ServiceMachineState.Busy;
                //this.thread = new Thread(this.MethodACNT);
                //this.thread.Start();
                Task task = new Task(MethodACNT);
                task.Start();
                await task;

            }
        }
    }
}   
        //TODO на кассах два события свободно и занято при занятой спит,при свободной кассе идет в очередь за заявками, если заявок данного типа нет, ждет тик и по тику снова проверка
