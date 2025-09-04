using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Dynamic.DataLayer;

namespace CardReader_TCPComms
{
    //public class TCPComms
    //{
    //    // I DONT THINK THIS IS USED ANYMORE
    //    // DF 6TH MARCH 2019

    //    private SqlDataAccess da;

    //    private TagReaderLocations tagReaderLocations;
    //    private MachineSubIDs machineSubIDs;
    //    private Tags tags;
    //    private Operators operators;

    //    private int localPort = 23;

    //    public int LocalPort
    //    {
    //        get { return localPort; }
    //        set { localPort = value; }
    //    }

    //    private bool debugging = true;

    //    public bool Debugging
    //    {
    //        get { return debugging; }
    //        set
    //        {
    //            debugging = value;
    //            if (serverComms != null)
    //                serverComms.Debugging = value;
    //        }
    //    }

    //    private SynchronizationContext uiSyncContext;
    //    public System.Timers.Timer feedbackTimer;

    //    public CancellationTokenSource tokenSource;

    //    public ServerComms serverComms;

    //    public ConcurrentQueue<Telegram> RxQ;

    //    private CancellationToken cancellationToken;

    //    public CancellationToken CancellationToken
    //    {
    //        get { return cancellationToken; }
    //        set { cancellationToken = value; }
    //    }

    //    public TCPComms(CancellationToken token)
    //    {
    //        // I DONT THINK THIS IS USED ANYMORE
    //        // DF 6TH MARCH 2019

    //        da = SqlDataAccess.Singleton;
    //        da.SiteName = "dev";
    //        cancellationToken = token;
    //        feedbackTimer = new System.Timers.Timer();
    //        feedbackTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
    //        feedbackTimer.Interval = 100;
    //        feedbackTimer.Start(); 
    //        uiSyncContext = SynchronizationContext.Current;
    //        SetupCollections();
    //        SetupComms();
    //        Task MonitorTask;
    //        MonitorTask = new Task(new Action(StartServerComms));
    //        MonitorTask.ContinueWith(delegate
    //        {
    //            uiSyncContext.Post(delegate { TasksStopped(); }, null);
    //        });

    //        MonitorTask.Start();
    //    }

    //    public void Stop()
    //    {
    //        feedbackTimer.Stop();
    //    }

    //    private void TasksStopped()
    //    {
    //        feedbackTimer.Stop();
    //    }

    //    private void SetupCollections()
    //    {
    //        RxQ = new ConcurrentQueue<Telegram>();
    //        tagReaderLocations = new TagReaderLocations();
    //        tagReaderLocations = (TagReaderLocations)da.DBDataListSelect(tagReaderLocations, true, false);
    //        machineSubIDs = new MachineSubIDs();
    //        machineSubIDs = (MachineSubIDs)da.DBDataListSelect(machineSubIDs, true, false);
    //        tags = new Tags();
    //        tags = (Tags)da.DBDataListSelect(tags, true, false);
    //        operators = new Operators();
    //        operators = (Operators)da.DBDataListSelect(operators, true, false);
    //    }

    //    private void SetupComms()
    //    {
    //        serverComms = new ServerComms();
    //        serverComms.LocalPort = localPort;
    //        serverComms.LocalName = "Test";
    //        serverComms.TimeOut = 10;
    //        serverComms.RxQ = RxQ;
    //        serverComms.Debugging = debugging;
    //        serverComms.IsGo = true;
    //    }

    //    private void JGLoginOut(int machineID, int subid, int operatorid, bool isIn)
    //    {
    //        int state = 0;
    //        if (isIn)
    //            state = 1;
    //        JGLogData logData = new JGLogData();
    //        JGLogDataRec ldr = new JGLogDataRec();
    //        ldr.ForceNew = true;
    //        ldr.RecNum = -1;
    //        ldr.RemoteID = -1;
    //        ldr.CompanyID = 99;
    //        ldr.TimeStamp = DateTime.Now;
    //        ldr.MachineID = machineID;
    //        ldr.PositionID = -1;
    //        ldr.SubID = subid;
    //        ldr.SubIDName = string.Empty;
    //        ldr.RegType = 6;
    //        ldr.SubRegType = 0;
    //        ldr.SubRegTypeID = 1;
    //        ldr.State = state;
    //        ldr.MessageA = "Operator";
    //        ldr.MessageB = "Operator";
    //        ldr.BatchID = -1;
    //        ldr.SourceID = -1;
    //        ldr.ProcessCode = -1;
    //        ldr.ProcessName = string.Empty;
    //        ldr.CustomerID = -1;
    //        ldr.SortCategoryID = -1;
    //        ldr.ArticleID = -1;
    //        ldr.OperatorID = operatorid;
    //        ldr.Value = 0;
    //        ldr.Unit = -1;
    //        logData.Add(ldr);
    //        logData.UpdateToDB();
    //    }

    //    private void OnTimedEvent(object source, ElapsedEventArgs e)
    //    {
    //        if (RxQ.Count > 0)
    //        {
    //            Telegram telegram;
    //            bool gotTelegram = RxQ.TryDequeue(out telegram);
    //            if (gotTelegram) 
    //            {
    //                string tagNo = telegram.TelegramString;
    //                IPEndPoint remoteIpEndPoint = telegram.RemoteEndPoint as IPEndPoint;

    //                string ip = remoteIpEndPoint.Address.ToString();
    //                tagReaderLocations = (TagReaderLocations)da.DBDataListSelect(tagReaderLocations);
    //                TagReaderLocation trl = (TagReaderLocation)tagReaderLocations.GetByName(ip);
    //                Tag tag = (Tag)tags.GetByName("tblOperators" + tagNo);
    //                if (trl != null && tag != null)
    //                {
    //                    machineSubIDs = (MachineSubIDs)da.DBDataListSelect(machineSubIDs);
    //                    MachineSubID subid = (MachineSubID)machineSubIDs.GetByIDSubID(trl.MachineID, trl.SubID);
    //                    if (subid != null)
    //                    {
    //                        //if (subid.OperatorRemote > 0)
    //                        //    JGLoginOut(trl.MachineID, trl.SubID, subid.OperatorRemote, false);
    //                        if (subid.OperatorRemote == tag.ReferenceID)
    //                        {
    //                            JGLoginOut(trl.MachineID, trl.SubID, subid.OperatorRemote, false);
    //                            subid.OperatorRemote = 0;
    //                        }
    //                        else
    //                        {
    //                            subid.OperatorRemote = tag.ReferenceID;
    //                            JGLoginOut(trl.MachineID, trl.SubID, tag.ReferenceID, true);
    //                        }
    //                    }
    //                    else
    //                    {
    //                        subid = new MachineSubID();
    //                        subid.ForceNew = true;
    //                        subid.MachineID = trl.MachineID;
    //                        subid.SubID = trl.SubID;
    //                        subid.OperatorID = -1;
    //                        subid.CustomerID = -1;
    //                        subid.ArticleID = -1;
    //                        subid.OperatorRemote = tag.ReferenceID;
    //                        machineSubIDs.Add(subid);
    //                    }
    //                    machineSubIDs.UpdateToDB();
    //                }
    //            }
    //        }
    //    }

    //    public void StartServerComms()
    //    {
    //        Debug.WriteLine("StartServerComms");
    //        Task commsTaskRx = new Task(obj => ServerCommsMonitor((ServerComms)obj), serverComms, cancellationToken);
    //        commsTaskRx.Start();
    //        try
    //        {
    //            Task.WaitAll(commsTaskRx);
    //        }
    //        catch (AggregateException ex)
    //        {
    //            // ignore exceptions
    //            Debug.WriteLine(ex.Message);
    //        }
    //    }

    //    static private void ServerCommsMonitor(ServerComms sci)
    //    {
    //        Debug.WriteLine("ServerCommsMonitor");
    //        while (sci.IsGo)
    //        {
    //            Task aTask = new Task(obj => ServerListenerProcess((ServerComms)obj), sci, sci.CancellationToken);
    //            aTask.Start();
    //            try
    //            {
    //                Task.WaitAny(aTask);
    //            }
    //            catch (AggregateException ex)
    //            {
    //                Debug.WriteLine(ex.Message);
    //            }
    //        }
    //    }

    //    static void ServerListenerProcess(ServerComms sci)
    //    {
    //        Debug.WriteLine("ServerListenerProcess");
    //        TcpClient client = null;
    //        TcpListener tcpListener = new TcpListener(IPAddress.Any, sci.LocalPort);
    //        try
    //        {
    //            tcpListener.Start();
    //            sci.DebugMessage(DateTime.Now.ToString() + " : Listening for clients on " + tcpListener.LocalEndpoint.ToString());
    //            while (sci.IsGo)
    //            {

    //                if (tcpListener.Pending())
    //                {
    //                    client = tcpListener.AcceptTcpClient();
    //                    sci.DebugMessage(DateTime.Now.ToString() + " : Accepted Client connection from remote "
    //                        + client.Client.RemoteEndPoint + " local " + client.Client.LocalEndPoint);

    //                    ServerComms ci = sci.Copy();
    //                    ci.tcpClient = client;

    //                    Task aTask = new Task(obj => ServerCommsProcess((ServerComms)obj), ci, ci.CancellationToken);
    //                    aTask.Start();
    //                }
    //                sci.CancellationToken.WaitHandle.WaitOne(10);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            sci.DebugMessage(ex.Message);
    //        }
    //        finally
    //        {
    //            if (client != null)
    //            {
    //                client.Close();
    //            }
    //            tcpListener.Stop();
    //            tcpListener = null;
    //            sci.DebugMessage("Stopped listening for clients");
    //            sci.CancellationToken.WaitHandle.WaitOne(100);
    //        }
    //    }

    //    static private void ServerCommsProcess(ServerComms sci)
    //    {
    //        Debug.WriteLine("ServerCommsProcess");
    //        bool Error = false;
    //        int counter = 1;
    //        int errorCount = 0;
    //        bool rxError = false;
    //        DateTime lastRx = DateTime.Now;
    //        DateTime lastTx = DateTime.Now;
    //        Socket socket = sci.tcpClient.Client;

    //        sci.DebugMessage("Connected to client : " + sci.tcpClient.Client.RemoteEndPoint.ToString() + " with "
    //            + sci.tcpClient.Client.LocalEndPoint.ToString());
    //        FeedbackMessage fb = new FeedbackMessage(socket.LocalEndPoint, "Connected");
    //        //sci.StatusQ.Enqueue(fb);

    //        byte[] message = new byte[4096];

    //        try
    //        {

    //            while (sci.IsGo && !Error)
    //            {
    //                NetworkStream clientStream = sci.tcpClient.GetStream();
    //                // Check the stream for read data
    //                if (clientStream.CanRead && clientStream.DataAvailable)
    //                {
    //                    Thread.Sleep(50);
    //                    rxError = !ServerReceive(socket, ref counter, sci);
    //                    lastRx = DateTime.Now;
    //                    if (rxError)
    //                    {
    //                        sci.DebugMessage("Error in receive [" + socket.RemoteEndPoint + "]");
    //                        errorCount++;
    //                    }
    //                }
    //                Error = false;
    //                // Check that we have received a message within the last timeout period
    //                if ((DateTime.Now > lastRx.Add(new TimeSpan(0, 0, sci.TimeOut))) && sci.TimeOut > 0)
    //                {
    //                    Error = true;
    //                    sci.DebugMessage("Exit due to no message received in time [" + socket.RemoteEndPoint + "]");
    //                }
    //                if (errorCount > 10)
    //                {
    //                    sci.DebugMessage("Exit due to 10 consecutive errors [" + socket.RemoteEndPoint + "]");
    //                    Error = true;
    //                }
    //                sci.CancellationToken.WaitHandle.WaitOne(10);
    //            }
    //            sci.DebugMessage("Exit from comms to [" + socket.RemoteEndPoint + "]");
    //        }
    //        catch (Exception ex)
    //        {
    //            sci.DebugMessage("Server exception: " + ex.Message);
    //        }

    //        fb = new FeedbackMessage(socket.LocalEndPoint, "Disconnected");
    //        //sci.StatusQ.Enqueue(fb);

    //        sci.tcpClient.Close();                          // close the client connection
    //        sci.tcpClient = null;
    //    }

    //    static private bool ServerReceive(Socket socket, ref int counter, ServerComms serverComms)
    //    {
    //        bool retValue = true;
    //        bool decodeResult = false;
    //        byte[] buffer = new byte[4096];
    //        int startTickCount = Environment.TickCount;
    //        int offset = 0;
    //        int size = socket.Available;
    //        int received = 0;  // how many bytes is already received
    //        if (socket.Available == 0)
    //        {
    //            return true;
    //        }
    //        try
    //        {
    //            do
    //            {
    //                try
    //                {
    //                    received += socket.Receive(buffer, offset + received, size - received, SocketFlags.None);
    //                }
    //                catch (SocketException ex)
    //                {
    //                    if (ex.SocketErrorCode == SocketError.WouldBlock ||
    //                        ex.SocketErrorCode == SocketError.IOPending ||
    //                        ex.SocketErrorCode == SocketError.NoBufferSpaceAvailable)
    //                    {
    //                        // socket buffer is probably empty, wait and try again
    //                        Thread.Sleep(30);
    //                        Debug.WriteLine("sleep30");
    //                    }
    //                    else
    //                    {
    //                        //Serious error
    //                        retValue = false;
    //                        Debug.WriteLine("error");
    //                    }
    //                }
    //            }
    //            while (received < size);

    //            decodeResult = retValue;
    //            if (retValue == true)
    //            {
    //                Telegram telegram = new Telegram(counter, buffer, socket.RemoteEndPoint);
    //                Debug.WriteLine(telegram.TelegramString);
    //                decodeResult = telegram.IsValid;
    //                if (decodeResult)
    //                {
    //                    counter = telegram.Counter;
    //                    serverComms.RxQ.Enqueue(telegram);
    //                    serverComms.DebugMessage(DateTime.Now.ToLongTimeString() + ", Rx " + telegram.TelegramString + ", " + telegram.Counter.ToString()
    //                                + " at " + socket.LocalEndPoint + " from " + socket.RemoteEndPoint);
    //                    counter++;
    //                }
    //                else
    //                {
    //                    serverComms.DebugMessage(DateTime.Now.ToLongTimeString() + ", " + telegram.TelegramString + ", Invalid ");
    //                }
    //            }
    //            else
    //                Debug.WriteLine("Bad retval");
    //        }
    //        catch (Exception ex)
    //        {
    //            serverComms.DebugMessage("Receive Exception: " + ex.Message);
    //            decodeResult = false;
    //        }
    //        return decodeResult;
    //    }
    //}

    //public class Telegram
    //{

    //    //private int counter;

    //    //public int Counter
    //    //{
    //    //    get { return counter; }
    //    //    set { counter = value; }
    //    //}

    //    //protected byte[] buffer;

    //    //public byte[] Buffer
    //    //{
    //    //    get { return buffer; }
    //    //    set
    //    //    {
    //    //        buffer = value;
    //    //        isValid = IsMessageValid();
    //    //    }
    //    //}
        
    //    //public Telegram(int count, byte[] rxBuffer, EndPoint endPoint)
    //    //{
    //    //    counter = count;
    //    //    buffer = new byte[rxBuffer.Length];
    //    //    rxBuffer.CopyTo(buffer, 0);
    //    //    remoteEndPoint = endPoint;
    //    //    telegramString = GetString(0, buffer.Length - 1);
    //    //    isValid = IsMessageValid();
    //    //}

    //    //private DateTime timeStamp;

    //    //public DateTime TimeStamp
    //    //{
    //    //    get { return timeStamp; }
    //    //    set { timeStamp = value; }
    //    //}

    //    //private EndPoint remoteEndPoint;

    //    //public EndPoint RemoteEndPoint
    //    //{
    //    //    get { return remoteEndPoint; }
    //    //    set { remoteEndPoint = value; }
    //    //}

    //    //private string telegramString;

    //    //public string TelegramString
    //    //{
    //    //    get { return telegramString; }
    //    //}

    //    //private bool isValid;

    //    //public bool IsValid
    //    //{
    //    //    get { return isValid; }
    //    //    set { isValid = value; }
    //    //}

    //    //public bool IsMessageValid()
    //    //{
    //    //    return telegramString != string.Empty;
    //    //}


    //    //protected string GetString(int start, int end)
    //    //{
    //    //    string aString = string.Empty;
    //    //    if (buffer != null)
    //    //    {
    //    //        if ((start <= end) && (end < buffer.Length))
    //    //        {
    //    //            for (int i = start; i <= end; i++)
    //    //            {
    //    //                if ((buffer[i] >= 32) && (buffer[i] < 127))
    //    //                    aString += (char)buffer[i];
    //    //                else
    //    //                    i = end;
    //    //            }
    //    //        }
    //    //    }
    //    //    return aString;
    //    //}
    //}

    //public class FeedbackMessage
    //{
    //    public EndPoint endPoint;
    //    public String message;

    //    public string RemoteIP
    //    {
    //        get { return ((IPEndPoint)endPoint).Address.ToString(); }
    //    }

    //    public FeedbackMessage(EndPoint ep, string msg)
    //    {
    //        endPoint = ep;
    //        message = msg;
    //    }
    //}

    //public class ClientServerComms
    //{
    //    public bool isGo = false;
    //    public int TimeOut;
    //    public ConcurrentQueue<Telegram> RxQ;
        
    //    private CancellationToken cancellationToken;
    //    public CancellationToken CancellationToken
    //    {
    //        get { return cancellationToken; }
    //        set { cancellationToken = value; }
    //    }
    //    public bool IsGo
    //    {
    //        get { return isGo; }
    //        set { isGo = value; }
    //    }

    //    public bool Debugging = true;

    //    public void DebugMessage(string aString)
    //    {
    //        if (Debugging)
    //        {
    //            Debug.WriteLine(aString);
    //        }
    //    }

    //}

    //public class ServerComms : ClientServerComms
    //{
    //    public string LocalName = string.Empty;
    //    public int LocalPort = 23;
    //    public TcpClient tcpClient;

    //    public ServerComms Copy()
    //    {
    //        ServerComms sci = new ServerComms();
    //        sci.LocalName = LocalName;
    //        sci.LocalPort = LocalPort;
    //        sci.TimeOut = TimeOut;
    //        sci.IsGo = isGo;
    //        sci.Debugging = Debugging;
    //        sci.RxQ = RxQ;
    //        return sci;
    //    }
    //}
}
