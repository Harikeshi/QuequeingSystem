using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Timers;

//todo  добыаить на форму максимальную длину очереди и время работы над заявками разного типа
//todo При запуске видим, что процесс 1 успел обработать 31 заявку, а процесс 2 только 11 добыпить зявке поле обслуж машины

namespace QueueingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //todo Ограничить очередь
        //todo перевести в часы понятные сутки
        double timertime=0.05;
        int time1 = 1000, time2 = 1000, time3 = 1000, time4 = 1000;
        ServiceMachineTypeFirst [] m1;
        ServiceMachineTypeSecond[] m2;
        ListRequest reqlist;//очередь событий
        ListRequest reqcomplete;//стек законченных
        ListRequest reqmissing;//стек пропущенных
        TimeSpan BeginTime=new TimeSpan(8, 0, 0);
        TimeSpan EndTime = new TimeSpan(16,30,0);
        Request request = new Request(-1);
        int MaxQueueLength = 20;//длина очереди
        TaskArray taskArray;//массив заявок перед работой
        int count;//общее количество заявок
        int current_count = 0;      
        bool Begin = true;
        int fcomplete = 0;
        int cardcomplete = 0;
        int credcomplete = 0;
        int xchgcomplete = 0;
        int acntcomplete = 0;
        TimeSpan interval;
        DispatcherTimer timer = new DispatcherTimer();
        private void Window_Loader(object sender, RoutedEventArgs e)
        {
            CurrentTime.Content = "00:00:00";
            QueueRequest.Content = 0;
            Begin = true;
            reqlist = new ListRequest();
            reqcomplete = new ListRequest();//стек законченных
            reqmissing = new ListRequest();//стек пропущенных
            m1 = new ServiceMachineTypeFirst[3];
            m2 = new ServiceMachineTypeSecond[2];
        }
        void init()
        {
            m1 = new ServiceMachineTypeFirst[3];
            m2 = new ServiceMachineTypeSecond[2];
            current_count = 0;
            reqlist = new ListRequest();
            reqcomplete = new ListRequest();
            reqmissing = new ListRequest();
            fcomplete = 0;
            cardcomplete = 0;
            credcomplete = 0;
            xchgcomplete = 0;
            acntcomplete = 0;
        }
        void ChangeColorTextBlock(TextBlock block)
        {
            block.Background = Brushes.Green;
            block.Text = "Open!";
        }
        //Проверка пустая ли машина   
        void CheckServiceMachineTypeFirst(ServiceMachineTypeFirst m,TextBlock block)
        {
            block.Background = Brushes.Green;
            block.Text = "Open!";
            Request temp = new Request(-1);
            int i = 0;
            if (i == 0)
            {
                temp = this.reqlist.Find(RequestType.XCHG);
                if (temp.ID != -1)
                {
                    i++;
                }
            }
            if (i == 0)
            {
                temp = this.reqlist.Find(RequestType.CARD);
                if (temp.ID != -1)
                {
                    i++;
                }
            }
            if (i == 1)
            {
                block.Text = temp.Type.ToString();
                block.Background = Brushes.Red;
                temp.TimeStarted = this.BeginTime;
                m.MethodOfTypeRequest(temp);               
                if (temp.Type == RequestType.XCHG)
                {
                temp.TimeFinalized = this.BeginTime+TimeSpan.FromMinutes(Convert.ToInt32(int.Parse(timeXCHG.Text)));
                    this.xchgcomplete++;
                }
                if (temp.Type == RequestType.CARD)
                {
                    temp.TimeFinalized = this.BeginTime + TimeSpan.FromMinutes(Convert.ToInt32(int.Parse(timeCARD.Text)));
                    this.cardcomplete++;
                }
                temp.RequestState = RequestStateType.Finalized;
                temp.MachineID = m.ID;
                this.reqcomplete.Add(temp);
                this.fcomplete++;
            }
        }
        void CheckServiceMachineTypeSecond(ServiceMachineTypeSecond m,TextBlock block)
        {
            block.Background = Brushes.Green;
            block.Text = "Open!";
            Request temp = new Request(-1);
            int i = 0;
            if (i == 0)
            {
                temp = this.reqlist.Find(RequestType.CRED);
                if (temp.ID != -1)
                {
                    i++;
                }
            }
            if (i == 0)
            {
                temp = this.reqlist.Find(RequestType.ACNT);
                if (temp.ID != -1)
                {
                    i++;
                }
            }
            if (i == 1)
            {
                block.Text = temp.Type.ToString();
                block.Background = Brushes.Red;
                temp.TimeStarted = this.BeginTime;
                m.MethodOfTypeRequest(temp);
                if (temp.Type == RequestType.CRED)
                {
                    temp.TimeFinalized = this.BeginTime + TimeSpan.FromMinutes(Convert.ToInt32(int.Parse(timeCRED.Text)));
                    this.credcomplete++;
                }
                if (temp.Type == RequestType.ACNT)
                {
                    temp.TimeFinalized = this.BeginTime + TimeSpan.FromMinutes(Convert.ToInt32(int.Parse(timeACNT.Text)));
                    this.acntcomplete++;
                }
                temp.MachineID = m.ID;
                temp.RequestState = RequestStateType.Finalized;
                this.reqcomplete.Add(temp);
                this.fcomplete++;
            }
        }
        void CheckMachinesEmpty(ServiceMachineTypeFirst[] m1, ServiceMachineTypeSecond[] m2)
        {
            if (m1[0].serviceMachineState == ServiceMachineState.Empty)
            {
                CheckServiceMachineTypeFirst(m1[0], Machine1);
            }
            if (m1[1].serviceMachineState == ServiceMachineState.Empty)
            {
                CheckServiceMachineTypeFirst(m1[1], Machine2);
            }
            if (m1[2].serviceMachineState == ServiceMachineState.Empty)
            {
                CheckServiceMachineTypeFirst(m1[2], Machine3);
            }
            if (m2[0].serviceMachineState == ServiceMachineState.Empty)
            {
                CheckServiceMachineTypeSecond(m2[0], Machine4);
            }
            if (m2[1].serviceMachineState == ServiceMachineState.Empty)
            {
                CheckServiceMachineTypeSecond(m2[1], Machine5);
            }
        }
        bool MachineEmpty()
        {
            if(m1[0].serviceMachineState==ServiceMachineState.Empty&& m1[1].serviceMachineState == ServiceMachineState.Empty
                && m1[2].serviceMachineState == ServiceMachineState.Empty
                && m2[0].serviceMachineState == ServiceMachineState.Empty&& m2[1].serviceMachineState == ServiceMachineState.Empty)
            {
                ChangeColorTextBlock(Machine1);
                ChangeColorTextBlock(Machine2);
                ChangeColorTextBlock(Machine3);
                ChangeColorTextBlock(Machine4);
                ChangeColorTextBlock(Machine5);
                return true;
            }
            return false;
        }
        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {       
            ServicesGenerate();
            if (Quantity.Text!="")
            if (Begin == true)
            {
                count = Convert.ToInt32(Quantity.Text);
                Begin = false;
            }           
            timer.Interval = TimeSpan.FromSeconds(timertime);
            timer.Tick += dtTicker;
            timer.Start();            
        }
        ///todo если не пустая, то добывляем событие в кассу, касса запломбирована завкой, по окончанию работы заявка отправляется в очередь
        ///todo законченных, а касса идет искать заявку в очередь, если в очереди нет заявки такого типа, то ждет тик.
        ///todo Если очередб переполнена, то заявка попадает в массив пропущенных заявок
        void ServicesGenerate()
        {           
            timertime = double.Parse(TimerTime.Text);
            double instance = 1000 * timertime / int.Parse(TimeInterval.Text);
            time1 = Convert.ToInt32(int.Parse(timeXCHG.Text) * instance);
            time2 = Convert.ToInt32(int.Parse(timeCARD.Text) * instance);
            time3 = Convert.ToInt32(int.Parse(timeCRED.Text) * instance);
            time4 = Convert.ToInt32(int.Parse(timeACNT.Text) * instance);

            m1[0] = new ServiceMachineTypeFirst(1, time1, time2);
            m1[1] = new ServiceMachineTypeFirst(2, time1, time2);
            m1[2] = new ServiceMachineTypeFirst(3, time1, time2);           
            m2[0] = new ServiceMachineTypeSecond(4, time3, time4);
            m2[1] = new ServiceMachineTypeSecond(5, time3, time4);
            interval = new TimeSpan(0, int.Parse(TimeInterval.Text), 0);
        }
        private void dtTicker(object sender, EventArgs e)
        {
            //TODO Событие на таймере
            BeginTime += interval;
            CurrentTime.Content = BeginTime.ToString();
            if (Quantity.Text != "")//проверка на пустую стороку
                if (count <= this.current_count || BeginTime >= EndTime)
                {
                    QueueRequest.Content = reqlist.Length.ToString();
                    CheckMachinesEmpty(m1, m2);
                    if (reqlist.Length == 0 && MachineEmpty())
                    {
                        QueueRequest.Content = reqlist.Length.ToString();
                        timer.Stop();
                    }
                }
                else
                {
                    taskArray.Requests[this.current_count].TimeQueued = BeginTime;
                    request = taskArray.Requests[this.current_count];
                }
                    if (reqlist.Length < MaxQueueLength&&this.current_count<count)//Длина очереди
                {
                    //
                    AllComplete.Text = fcomplete.ToString();
                    XchgComplete.Text = xchgcomplete.ToString();
                    CardComplete.Text = cardcomplete.ToString();
                    CredComplete.Text = credcomplete.ToString();
                    AcntComplete.Text = acntcomplete.ToString();
                    request.RequestState = RequestStateType.Queued;
                    request.TimeQueued = BeginTime;
                    this.current_count++;
                    TaskText.Text += request.ID + ". " + request.RequestState + "\n";
                    reqlist.Add(request);//добавляем завку в очередь
                    QueueRequest.Content = reqlist.Length.ToString();
                }
                else if(reqlist.Length>=MaxQueueLength)
                {
                    request.RequestState = RequestStateType.Missed;
                    request.TimeMissed = BeginTime;
                    TaskText.Text += request.ID + ". " + request.RequestState + "\n";
                    reqmissing.Add(request);
                    this.current_count++;
                }
            ///todo здесь только проверка на пустую кассу, за полчаса до окончания рабочего дня проверка останавливается.! еще один if
            CheckMachinesEmpty(m1, m2);
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {            
            timer.Stop();   
        }

        private void Button_Click_Generate(object sender, RoutedEventArgs e)
        {
            init();
            BeginTime = new TimeSpan(8,0,0);
            CurrentTime.Content= BeginTime.ToString();
            current_count = 0;
            TaskText.Text = "";
            if (Quantity.Text != "")
            {
                taskArray = new TaskArray(Convert.ToInt32(Quantity.Text),BeginTime);
            }
            for(int i=0;i< Convert.ToInt32(Quantity.Text); i++)
            {
                TaskText.Text += taskArray.Requests[i].ID+". Type: "+ taskArray.Requests[i].Type +", State: "+ taskArray.Requests[i].RequestState+", Generate Time: "+ taskArray.Requests[i].TimeStarted +";\n";
            }
        }

        private void Button_Click_Clear(object sender, RoutedEventArgs e)
        {
            TaskText.Text = "";
        }

        private void Button_Click_Miss(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < reqmissing.Length; i++)
            {
                TaskText.Text += reqmissing.Requests[i].ID + ". "+ reqmissing.Requests[i].Type+" "+ reqmissing.Requests[i].RequestState + reqmissing.Requests[i].TimeStarted + " " + reqmissing.Requests[i].TimeMissed + "\n";
            }
        }

        private void Button_Click_Complete(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < reqcomplete.Length; i++)
            {
                TaskText.Text += reqcomplete.Requests[i].ID + "." + "Тип: "+reqcomplete.Requests[i].Type + ", Касса:" +reqcomplete.Requests[i].MachineID+", В очередь: "+ reqcomplete.Requests[i].TimeQueued + ", Старт: " + reqcomplete.Requests[i].TimeStarted + ", Завершена:"+ reqcomplete.Requests[i].TimeFinalized + ";\n";
            }
        }
    }
}
