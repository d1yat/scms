using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using ScmsMailLibrary.Global;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsMailLibrary
{
  class Pop3MailerReader
  {
    internal class TaskInfo
    {
      public TaskInfo()
      {
        this.OtherInfo = "default";
        this.Handle = null;
      }

      public RegisteredWaitHandle Handle { get; set; }
      public string OtherInfo { get; set; }
    }

    //SmtPop.POP3Client pop3;
    private OpenPop.Pop3.Pop3Client pop3;

    private volatile string serverPop3;
    private volatile int portPop3;
    private volatile bool useSsl;

    private bool isStart;

    private AutoResetEvent areEvent;
    private AutoResetEvent areThreadStop;
    private ScmsSoaLibrary.Commons.Config config;
    private volatile bool isRunning;
    private volatile string emailName;
    private volatile string passEmail;

    const int RTO = (5 * 60000);

    public Pop3MailerReader(string serverPop3, int portPop3, ScmsSoaLibrary.Commons.Config config)
    {
      pop3 = new OpenPop.Pop3.Pop3Client();

      isStart = false;

      this.serverPop3 = serverPop3;
      this.portPop3 = portPop3;
      this.config = config;

      if ((config == null) || string.IsNullOrEmpty(config.PathTempExtract))
      {
        string tmp = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        tmp = (tmp.EndsWith("\\") ? tmp : string.Concat(tmp, "\\"));

        this.TempPath = tmp;
      }
      else 
      {
        this.TempPath = config.PathTempExtract;
      }
      if ((config == null) || config.TimerPeriodicMailer.Equals(TimeSpan.MinValue))
      {
        this.TimePeriodic = new TimeSpan(0, 3, 0);
      }
      else
      {
        this.TimePeriodic = config.TimerPeriodicMailer;
      }

      areEvent = new AutoResetEvent(false);
    }

    ~Pop3MailerReader()
    {
      if (pop3 != null)
      {
        //pop3.Authentified -= new SmtPop.AuthentifiedEventHandler(pop3_Authentified);
        //pop3.Connected -= new SmtPop.ConnectEventHandler(pop3_Connected);
        //pop3.Received -= new SmtPop.ReceivedEventHandler(pop3_Received);
        //pop3.SendedCommand -= new SmtPop.ClientCommandEventHandler(pop3_SendedCommand);
        //pop3.ServerAnswer -= new SmtPop.ServerAnswerEventHandler(pop3_ServerAnswer);

        try
        {
          pop3.Dispose();
        }
        catch { ;}
      }
    }

    #region Delegate & Event

    public delegate void ClassNotifyEventHandler(object sender, ClassNotifyEventArgs e);
    public delegate void Pop3MailMessageEventHandler(object sender, Pop3MailMessageEventArgs e);

    public event Pop3MailMessageEventHandler Pop3MailMessage;
    public event ClassNotifyEventHandler ClassNotify;

    #endregion
    
    #region Private

    private void StartingEventPop3()
    {
      TaskInfo ti = new TaskInfo();

      ti.Handle = System.Threading.ThreadPool.RegisterWaitForSingleObject(areEvent, 
        new WaitOrTimerCallback(WoTCallback), ti,
        this.TimePeriodic, false);
    }

    private void CheckingEmail()
    {
      if (!isStart)
      {
        this.TriggerClassNotify("Pop3 belum aktif.", ClassNotifyEventArgs.TypeEnum.IsNotify);

        return;
      }

      int nLoop = 0,
        nLen = 0,
        nLoopC = 0,
        nLenC = 0,
        nMsgId = 0;

      OpenPop.Mime.Message msg = null;
      List<OpenPop.Mime.MessagePart> lstMsgParts = null;
      OpenPop.Mime.MessagePart msgPart = null;
      Dictionary<string, byte[]> dicAttach = new Dictionary<string, byte[]>();

      //// retrieve messages list from pop server
      //SmtPop.POPMessageId[] messages = pop3 // .GetMailList();
      nLen = pop3.GetMessageCount();

      Encoding utf8 = Encoding.UTF8;
      string bodyPlain = null,
        bodyHtml = null;

      if (nLen > 0)
      {
        for (nLoop = 0, nMsgId = 1; nLoop < nLen; nLoop++, nMsgId++)
        {
          msg = pop3.GetMessage(nMsgId);

          try
          {
            #region Body

            lstMsgParts = msg.FindAllTextVersions();

            if ((lstMsgParts != null) && (lstMsgParts.Count > 0))
            {
              for (nLoopC = 0, nLenC = lstMsgParts.Count; nLoopC < nLenC; nLoopC++)
              {
                msgPart = lstMsgParts[nLoopC];
                if (msgPart.ContentTransferEncoding == OpenPop.Mime.Header.ContentTransferEncoding.SevenBit)
                {
                  bodyPlain = utf8.GetString(msgPart.Body);
                }
                else if (msgPart.ContentTransferEncoding == OpenPop.Mime.Header.ContentTransferEncoding.QuotedPrintable)
                {
                  bodyHtml = utf8.GetString(msgPart.Body);
                }
              }
            }

            lstMsgParts.Clear();

            #endregion

            #region Attachment

            lstMsgParts = msg.FindAllAttachments();

            if ((lstMsgParts != null) && (lstMsgParts.Count > 0))
            {
              for (nLoopC = 0, nLenC = lstMsgParts.Count; nLoopC < nLenC; nLoopC++)
              {
                msgPart = lstMsgParts[nLoopC];
                if ((!string.IsNullOrEmpty(msgPart.FileName)) && msgPart.IsAttachment)
                {
                  if (dicAttach.ContainsKey(msgPart.FileName))
                  {
                    dicAttach[msgPart.FileName] = msgPart.Body;
                  }
                  else
                  {
                    dicAttach.Add(msgPart.FileName, msgPart.Body);
                  }
                }
              }
            }

            lstMsgParts.Clear();

            #endregion

            if(TriggerPop3Mail(msg.Headers.From.Address, msg.Headers.Subject, bodyPlain, bodyHtml, dicAttach, utf8.GetString(msg.RawMessage), msg))
            {
              pop3.DeleteMessage(nMsgId);
            }
            //TriggerPop3Mail(msg.Headers.From, msg.Headers.Subject, null, null, null, null, null);
            //bodyPlain, bodyHtml, dicAttach, utf8.GetString(msg.RawMessage), msg
          }
          catch (Exception ex)
          {
            this.TriggerClassNotify(
              string.Format("Pop3Mailer.CheckingEmail - {0}", ex.Message),
              ClassNotifyEventArgs.TypeEnum.IsError);

          }
          finally
          {
            if (dicAttach != null)
            {
              dicAttach.Clear();
            }
          }
        }

        //pop3.Reset();
      }
    }

    #region Trigger Event

    private void TriggerClassNotify(string msg, ClassNotifyEventArgs.TypeEnum type)
    {
      if (ClassNotify != null)
      {
        ClassNotify(this, new ClassNotifyEventArgs(msg, type));
      }
    }

    private bool TriggerPop3Mail(string from, string subject, string body, string bodyHtml, Dictionary<string, byte[]> dic, string rawData, OpenPop.Mime.Message msg)
    {
      bool bOk = false;

      if (Pop3MailMessage != null)
      {
        Pop3MailMessageEventArgs ppm = new Pop3MailMessageEventArgs(from, subject, body, bodyHtml, dic, rawData, msg);

        Pop3MailMessage(this, ppm);

        bOk = ppm.Delete;
      }

      return bOk;
    }

    #endregion

    #endregion

    #region Callback

    private void WoTCallback(object state, bool timeOut)
    {
      if (isRunning)
      {
        return;
      }

      isRunning = true;

      TaskInfo ti = state as TaskInfo;

      try
      {
        if (ti != null)
        {
          if (isStart)
          {
            if (timeOut)
            {
              if (!pop3.Connected)
              {
                pop3.Connect(this.serverPop3, this.portPop3, useSsl, Pop3MailerReader.RTO, Pop3MailerReader.RTO, new RemoteCertificateValidationCallback(SslValidation));
              }

              if (pop3.Connected)
              {
                pop3.Authenticate(this.emailName, this.passEmail, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);

                this.CheckingEmail();
              }

              pop3.Disconnect();
            }
          }
          else
          {
            if (ti.Handle != null)
            {
              //ti.Handle.Unregister(areEvent);
              ti.Handle.Unregister(null);

              if (areThreadStop != null)
              {
                areThreadStop.Set();
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.TriggerClassNotify(
          string.Format("Pop3Mailer.WoTCallback - {0}", ex.Message),
          ClassNotifyEventArgs.TypeEnum.IsError);

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      isRunning = false;
    }

    #endregion

    #region Pop3 Event

    private bool SslValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      bool bOk = false;

      bOk = true;

      return bOk;
    }

    #endregion

    public string TempPath
    { get; private set; }

    public bool Start(string email, string password, bool useSsl)
    {
      if (isStart)
      {
        this.TriggerClassNotify("Pop3 sudah aktif.", ClassNotifyEventArgs.TypeEnum.IsNotify);

        return false;
      }

      try
      {
        pop3.Connect(serverPop3, portPop3, useSsl, Pop3MailerReader.RTO, Pop3MailerReader.RTO, new RemoteCertificateValidationCallback(SslValidation));

        if (pop3.Connected)
        {
          //resl = pop3.Open(this.serverPop3, this.portPop3, email, password);
          pop3.Authenticate(email, password, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);

          this.useSsl = useSsl;
          this.emailName = email;
          this.passEmail = password;

          pop3.Disconnect();

          StartingEventPop3();
          
          isStart = true;
        }
      }
      catch (Exception ex)
      {
        this.TriggerClassNotify(
          string.Format("Pop3Mailer.Start - {0}", ex.Message), 
          ClassNotifyEventArgs.TypeEnum.IsError);
      }

      return isStart;
    }

    public void Stop()
    {
      if (!isStart)
      {
        this.TriggerClassNotify("Pop3 belum aktif.", ClassNotifyEventArgs.TypeEnum.IsNotify);

        return;
      }

      isStart = false;

      try
      {
        areThreadStop = new AutoResetEvent(false);

        if (areEvent != null)
        {
          areEvent.Set();
        }

        areThreadStop.WaitOne();

        areThreadStop.Close();

        areThreadStop = null;

        areEvent.Reset();

        if (pop3 == null)
        {
          this.TriggerClassNotify("Pop3 object tidak terbuat.", ClassNotifyEventArgs.TypeEnum.IsNotify);
        }
        else
        {
          try
          {
            pop3.Disconnect();
          }
          catch { ;}
        }
      }
      catch (Exception ex)
      {
        this.TriggerClassNotify(
          string.Format("Pop3Mailer.Stop - {0}", ex.Message),
          ClassNotifyEventArgs.TypeEnum.IsError);
      }

    }

    public TimeSpan TimePeriodic
    { get; private set; }
  }

  class Pop3MailMessageEventArgs : EventArgs
  {
    internal Pop3MailMessageEventArgs(string fromMail, string subjectData, string bodyData, string bodyDataHtml, Dictionary<string, byte[]> dic, string rawData, OpenPop.Mime.Message msg)
    {
      this.From = fromMail;
      this.Subject = subjectData;
      this.Body = bodyData;
      this.BodyHtml = bodyDataHtml;
      this.Attachments = dic;
      this.Raw = rawData;
      this.MailMessage = msg;
      this.HasAttachment = ((dic != null) || (dic.Count > 0) ? true : false);
    }

    public string From
    { get; private set; }

    public string Subject
    { get; private set; }

    public string Body
    { get; private set; }

    public string BodyHtml
    { get; private set; }

    public Dictionary<string, byte[]> Attachments
    { get; private set; }

    public OpenPop.Mime.Message MailMessage
    { get; private set; }

    public string Raw
    { get; private set; }

    public bool Delete
    { get; set; }

    public bool Reset
    { get; set; }

    public bool HasAttachment
    { get; private set; }
  }
}
