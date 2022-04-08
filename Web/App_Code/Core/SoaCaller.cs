using System;
using System.Collections.Generic;
using System.Web;
using System.ServiceModel;

namespace Scms.Web.Core
{
  /// <summary>
  /// Summary description for SoaCaller
  /// </summary>
  public class SoaCaller
  {
    private ChannelFactory<ScmsSoaLibraryInterface.IScmsSoaLibrary> myChannelFactory;

    private static string sResult = null;

    private static bool IsSync = false;

    private volatile System.Threading.ManualResetEvent treSP;

    public SoaCaller()
    {
      //
      // TODO: Add constructor logic here
      //

      string baseaddr = "http://localhost:1234/scms/soap";

      UriBuilder uri = new UriBuilder(baseaddr);
      uri.Host = StaticObjects.SoaAddress;
      uri.Port = StaticObjects.SoaPort;

      baseaddr = uri.ToString();

      System.Xml.XmlDictionaryReaderQuotas readQuoatas = new System.Xml.XmlDictionaryReaderQuotas()
      {
        MaxArrayLength = int.MaxValue,
        MaxBytesPerRead = int.MaxValue,
        MaxDepth = int.MaxValue,
        MaxNameTableCharCount = int.MaxValue,
        MaxStringContentLength = int.MaxValue
      };

      // This code is written by an application developer.
      // Create a channel factory.
      BasicHttpBinding myBinding = new BasicHttpBinding()
      {
        MaxReceivedMessageSize = int.MaxValue,
        MaxBufferPoolSize = int.MaxValue,
        MaxBufferSize = int.MaxValue,
        ReaderQuotas = readQuoatas
      };

      EndpointAddress myEndpoint = new EndpointAddress(baseaddr);

      myChannelFactory = new ChannelFactory<ScmsSoaLibraryInterface.IScmsSoaLibrary>(myBinding, myEndpoint);
    }

    ~SoaCaller()
    {
      if (myChannelFactory != null)
      {
        try
        {
          myChannelFactory.Close();
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaCaller:~SoaCaller - ", ex.Message));
        }
      }
    }

    internal bool Open
    {
      get
      {
        bool bOk = false;

        try
        {
          switch(myChannelFactory.State )
          {
            case CommunicationState.Closed:
            case CommunicationState.Created:
            case CommunicationState.Faulted:
              myChannelFactory.Open();
              break;
          }

          bOk = true;
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaCaller:Open - ", ex.Message));
        }

        return bOk;
      }
    }

    internal ScmsSoaLibraryInterface.IScmsSoaLibrary SoaScms
    {
      get
      {
        ScmsSoaLibraryInterface.IScmsSoaLibrary soa = null;

        try
        {
          if (this.Open)
          {
            soa = myChannelFactory.CreateChannel();
          }
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaCaller:Open - ", ex.Message));
        }

        return soa;
      }
    }

    public string GlobalQueryService(int start, int limit, bool allQuery, string sort, string dir, string model, string[][] parameters)
    {
      ScmsSoaLibraryInterface.IScmsSoaLibrary soa = this.SoaScms;

      if (soa != null)
      {
        string result = null;

        try
        {
          result = soa.GlobalQueryService(start, limit, allQuery, sort, dir, model, parameters);

          /* 2022-01-04 */
          if (!string.IsNullOrEmpty(result) && model != "0257")
        //if (!string.IsNullOrEmpty(result))
          {
            result = result.Replace("\\'", "\"");
          }
        }
        catch (Exception ex)
        {
          result = string.Empty;

          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaCaller:GlobalQueryService - ", ex.Message));
        }

        return result;
      }

      return string.Empty;
    }

    private static void GenerateProcessingAsync(object state)
    {
      string Rg = (string)state;

      SoaCaller sc = new SoaCaller();

      sResult = sc.PostData(Rg);

      //Common.Functional.ShowMsg(sResult.Substring(4,40));
      
    }

    public string PostData(string data)
    {
      ScmsSoaLibraryInterface.IScmsSoaLibrary soa = this.SoaScms;

      if (soa != null)
      {
        string result = null;

        try
        {
          result = soa.PostData(data);
        }
        catch (Exception ex)
        {
          result = string.Empty;

          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaCaller:PostData - ", ex.Message));
        }
        finally
        {
          if (result == null)
          {
            result = string.Empty;
          }
        }

        return result;
      }

      return string.Empty;
    }

    public string PostData(string data, bool Sync)
    {
      bool isProcessing = false;

      if (Sync)
      {
        isProcessing = System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GenerateProcessingAsync), data);
      }
      else
      {
        sResult = PostData(data);
      }

      //System.Threading.WaitHandle[] waits = new System.Threading.WaitHandle[2];

      //try
      //{
      //  treSP = new System.Threading.ManualResetEvent(false);
      //  waits[0] = treSP;
      //  System.Threading.ThreadPool.QueueUserWorkItem(GenerateProcessingAsync, data);

      //  treSP = new System.Threading.ManualResetEvent(false);
      //  waits[0] = treSP;
      //  System.Threading.ThreadPool.QueueUserWorkItem(GenerateProcessingAsync, data);

      //  System.Threading.WaitHandle.WaitAll(waits);
      //}
      //catch (Exception ex)
      //{
      //  sResult = string.Empty;

      //  System.Diagnostics.Debug.WriteLine(
      //    string.Concat("Scms.Web.Core.SoaCaller:PostData - ", ex.Message));
      //}
      //finally
      //{
        
      //}

      //isProcessing = System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(GenerateProcessingAsync), data);

      return sResult;
    }

    public string TestPostData(string data)
    {
      ScmsSoaLibraryInterface.IScmsSoaLibrary soa = this.SoaScms;

      if (soa != null)
      {
        string result = null;

        try
        {
          result = soa.TestPostData(data);
        }
        catch (Exception ex)
        {
          result = string.Empty;

          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaCaller:TestPostData - ", ex.Message));
        }
        finally
        {
          if (result == null)
          {
            result = string.Empty;
          }
        }

        return result;
      }

      return string.Empty;
    }

    public string GlobalUploadData(string model, string origName, byte[] data, string[][] parameters)
    {
      ScmsSoaLibraryInterface.IScmsSoaLibrary soa = this.SoaScms;

      if (soa != null)
      {
        string result = null;

        try
        {
          result = soa.GlobalPostUploadedData(model, origName, data, parameters);
        }
        catch (Exception ex)
        {
          result = string.Empty;

          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaCaller:UploadData - ", ex.Message));
        }
        finally
        {
          if (result == null)
          {
            result = string.Empty;
          }
        }

        return result;
      }

      return string.Empty;
    }
  }

  /// <summary>
  /// Summary description for SoaCaller
  /// </summary>
  public class SoaReportCaller
  {
    private ChannelFactory<ScmsSoaLibraryInterface.IScmsSoaReporting> myChannelFactory;

    public SoaReportCaller()
    {
      //
      // TODO: Add constructor logic here
      //

      string baseaddr = "http://localhost:1234/scms/report/soap";

      UriBuilder uri = new UriBuilder(baseaddr);
      uri.Host = StaticObjects.SoaAddress;
      uri.Port = StaticObjects.SoaPort;

      baseaddr = uri.ToString();

      // This code is written by an application developer.
      // Create a channel factory.
      BasicHttpBinding myBinding = new BasicHttpBinding();

      EndpointAddress myEndpoint = new EndpointAddress(baseaddr);

      myChannelFactory = new ChannelFactory<ScmsSoaLibraryInterface.IScmsSoaReporting>(myBinding, myEndpoint);
    }

    ~SoaReportCaller()
    {
      if (myChannelFactory != null)
      {
        try
        {
          myChannelFactory.Close();
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaReportCaller:~SoaReportCaller - ", ex.Message));
        }
      }
    }

    internal bool Open
    {
      get
      {
        bool bOk = false;

        try
        {
          switch (myChannelFactory.State)
          {
            case CommunicationState.Closed:
            case CommunicationState.Created:
            case CommunicationState.Faulted:
              myChannelFactory.Open();
              break;
          }

          bOk = true;
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaReportCaller:Open - ", ex.Message));
        }

        return bOk;
      }
    }

    internal ScmsSoaLibraryInterface.IScmsSoaReporting SoaReportScms
    {
      get
      {
        ScmsSoaLibraryInterface.IScmsSoaReporting soa = null;

        try
        {
          if (this.Open)
          {
            soa = myChannelFactory.CreateChannel();
          }
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaReportCaller:Open - ", ex.Message));
        }

        return soa;
      }
    }

    public string GeneratorReport(string config)
    {
      return GeneratorReport(false, config);
    }
    
    public string GeneratorReport(bool async, string config)
    {
      ScmsSoaLibraryInterface.IScmsSoaReporting soa = this.SoaReportScms;

      if (soa != null)
      {
        string result = null;

        try
        {
          result = soa.GeneratorReport(async, config);
        }
        catch (Exception ex)
        {
          result = string.Empty;

          System.Diagnostics.Debug.WriteLine(
            string.Concat("Scms.Web.Core.SoaReportCaller:GeneratorReport - ", ex.Message));
        }
        finally
        {
          if (result == null)
          {
            result = string.Empty;
          }
        }

        return result;
      }

      return string.Empty;
    }
  }
}