using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibrary.Core;
//using ScmsSoaLibrary.Core.Converter;
//using ScmsSoaLibrary.Core.JSONP;
using ScmsSoaLibrary.Services;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Core;
using ScmsSoaLibraryInterface.Core.Converter;
//using ScmsSoaLibraryInterface.Core.CustomEncoder;do
using ScmsSoaLibraryInterface.Core.CustomMessageEncoder;
//using ScmsSoaLibraryInterface.Core.JSONP;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using ScmsModel;
using ScmsModel.Core;
using System.Linq;
using ScmsMailLibrary;
using ScmsSoaLibraryInterface.Commons;
using System.Runtime.InteropServices;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;

namespace ScmsSoaTester
{
  class Program
  {
    [DllImport("Winspool.drv")]
    public static extern bool SetDefaultPrinter(string printerName);

    internal class PrincipalEmail
    {
      public string PrincipalName { get; set; }
      public string PrincipalCode { get; set; }
      public string[] Emails { get; set; }
    }

    internal class DO_PO_Link
    {
      public string PO { get; set; }
      public string Item { get; set; }
      public decimal Sisa { get; set; }
    }

    internal class DO_PI_Header_Check
    {
      public string Principal { get; set; }
      public string DO { get; set; }
    }

    internal class DO_PI_Detail_Check
    {
      public string Principal { get; set; }
      public string DO { get; set; }
      public string PO { get; set; }
      public string Item { get; set; }
      public string Batch { get; set; }
    }

    internal class DO_PI_Result
    {
      public string Principal { get; set; }
      public string DO { get; set; }
      public string PO { get; set; }
      public string Item { get; set; }
      public string Batch { get; set; }
    }

    internal class POHeaderSenderFormat
    {
      public string C_CORNO { get; set; }
      public DateTime D_CORDA { get; set; }
      public string C_KOMEN1 { get; set; }
      public string C_KOMEN2 { get; set; }
      public bool L_LOAD { get; set; }
      public string C_KDDEPO { get; set; }
      public string C_KDCAB { get; set; }
      public string C_NMCAB { get; set; }
    }

    internal class PODetailSenderFormat
    {
      public string c_corno { get; set; }
      public string c_iteno { get; set; }
      public string c_itenopri { get; set; }
      public string c_itnam { get; set; }
      public string c_undes { get; set; }
      public decimal n_qty { get; set; }
      public decimal n_salpri { get; set; }
      public string c_nosp { get; set; }
      public string c_via { get; set; }
    }

    internal class SuplierInformation
    {
      public string KodeSuplier { get; set; }
      public string NamaSuplier { get; set; }
      public string Alamat1 { get; set; }
      public string Alamat2 { get; set; }
      public string[] Emails { get; set; }
    }

    const string DEFAULT_NAME_FILE_HEADER = "DOHEADER.DBF";
    const string DEFAULT_NAME_FILE_DETAIL = "DODETAIL.DBF";

    const string DEFAULT_NAME_TABEL_HEADER = "DOHEADER";
    const string DEFAULT_NAME_TABEL_DETAIL = "DODETAIL";

    const string DEFAULT_NAME_TABEL_PO_PRINCIPAL = "POPRINCIPAL";
    const string DEFAULT_NAME_TABEL_PO_LIST = "POLIST";
    const string DEFAULT_NAME_TABEL_PO_HEADER = "POHEADER";
    const string DEFAULT_NAME_TABEL_PO_DETAIL = "PODETAIL";

    static ServiceHost StartingSoaLibrary(Config config, bool isJsonActive, string url)
    {
      Service srvc = new Service();

      Service.IsJsonPaddingActive = isJsonActive;

      //string baseaddr = "http://localhost:8080/Aig";
      Uri baseAddress = new Uri(url);

      ServiceHost host = new ServiceHost(typeof(Service), baseAddress);

      // Enable metadata publishing.
      ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
      smb.HttpGetEnabled = true;
      smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
      smb.HttpGetUrl = baseAddress;
      //smb.MetadataExporter = new Soap11ConformantWsdlExporter();
      host.Description.Behaviors.Add(smb);

      ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>() as ServiceDebugBehavior;
      if (debug == null)
      {
        debug = new ServiceDebugBehavior();

        debug.IncludeExceptionDetailInFaults = true;

        host.Description.Behaviors.Add(debug);
      }
      else
      {
        debug.IncludeExceptionDetailInFaults = true;
      }

      ServiceThrottlingBehavior throttle = host.Description.Behaviors.Find<ServiceThrottlingBehavior>() as ServiceThrottlingBehavior;
      if (throttle == null)
      {
        throttle = new ServiceThrottlingBehavior()
        {
          MaxConcurrentCalls = 12,
          MaxConcurrentInstances = 56,
          MaxConcurrentSessions = 34
        };

        host.Description.Behaviors.Add(throttle);
      }
      else
      {
        throttle.MaxConcurrentCalls = 12;
        throttle.MaxConcurrentInstances = 56;
        throttle.MaxConcurrentSessions = 34;
      }

      ServiceEndpoint endPoint = null;
      CustomBinding binding = new CustomBinding(),
        dcBinding = new CustomBinding();

      CustomMessageBindingElement custMsgBE = new CustomMessageBindingElement();
      WebMessageEncodingBindingElement wme = new WebMessageEncodingBindingElement();
      binding.Elements.Add(custMsgBE);
      

      //buat dc
      
      dcBinding.Elements.Add(wme);
      

      HttpTransportBindingElement bindingelemt = new HttpTransportBindingElement();
      bindingelemt.ManualAddressing = true;
      binding.Elements.Add(bindingelemt);

      //buat dc
      dcBinding.Elements.Add(bindingelemt);


      endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), binding, "httpcustom");
      endPoint.Behaviors.Add(new WebQueryStringBehaviour()
      {
        DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json,
        DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare
      });

      if (isJsonActive)
      {
        //binding = new CustomBinding();

        //bindingelemt = new HttpTransportBindingElement();
        //bindingelemt.ManualAddressing = true;
        //binding.Elements.Add(bindingelemt);

        endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), binding, "WebJsonP");

        endPoint.Behaviors.Add(new WebQueryStringBehaviour()
        {
          DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json,
          DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare
        });

        Console.WriteLine("Activate JSON Padding...");
      }

      endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), dcBinding, "WebDist");

      endPoint.Behaviors.Add(new WebQueryStringBehaviour()
      {
        DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json,
        DefaultBodyStyle = System.ServiceModel.Web.WebMessageBodyStyle.Bare
      });

      System.Xml.XmlDictionaryReaderQuotas readQuoatas = new System.Xml.XmlDictionaryReaderQuotas()
      {
        MaxArrayLength = int.MaxValue,
        MaxBytesPerRead = int.MaxValue,
        MaxDepth = int.MaxValue,
        MaxNameTableCharCount = int.MaxValue,
        MaxStringContentLength = int.MaxValue
      };

      Binding bind = new BasicHttpBinding()
        {
          MaxReceivedMessageSize = int.MaxValue,
          ReaderQuotas = readQuoatas,
        };
      endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), bind, "Soap");

      endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), new WebHttpBinding()
        {
          MaxReceivedMessageSize = int.MaxValue,
          //MaxBufferPoolSize = int.MaxValue,
          //MaxBufferSize = int.MaxValue,
          ReaderQuotas = readQuoatas,
        }, "Web");
      
      endPoint.Behaviors.Add(new WebQueryStringBehaviour());

      //endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), new WSHttpBinding()
      //  {
      //    MaxReceivedMessageSize = int.MaxValue,
      //    //MaxBufferPoolSize = int.MaxValue,
      //    ReaderQuotas = readQuoatas
      //  }, "Ws");

      try
      {
        //for some reason a default endpoint does not get created here
        host.Open();

        Console.WriteLine("The service soa is ready at {0}", baseAddress);

        Console.WriteLine();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      return host;
    }

    static ServiceHost StartingSoaReporting(Config config, string url)
    {
      Reporting srvc = new Reporting();

      //string baseaddr = "http://localhost:8080/Aig";
      Uri baseAddress = new Uri(url);

      ServiceHost host = new ServiceHost(typeof(Reporting), baseAddress);

      // Enable metadata publishing.
      ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
      smb.HttpGetEnabled = true;
      smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy12;
      smb.HttpGetUrl = baseAddress;
      //smb.MetadataExporter = new Soap11ConformantWsdlExporter();
      host.Description.Behaviors.Add(smb);

      ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>() as ServiceDebugBehavior;
      if (debug == null)
      {
        debug = new ServiceDebugBehavior();

        debug.IncludeExceptionDetailInFaults = true;

        host.Description.Behaviors.Add(debug);
      }
      else
      {
        debug.IncludeExceptionDetailInFaults = true;
      }

      ServiceThrottlingBehavior throttle = host.Description.Behaviors.Find<ServiceThrottlingBehavior>() as ServiceThrottlingBehavior;
      if (throttle == null)
      {
        throttle = new ServiceThrottlingBehavior()
        {
          MaxConcurrentCalls = 12,
          MaxConcurrentInstances = 56,
          MaxConcurrentSessions = 34
        };

        host.Description.Behaviors.Add(throttle);
      }
      else
      {
        throttle.MaxConcurrentCalls = 12;
        throttle.MaxConcurrentInstances = 56;
        throttle.MaxConcurrentSessions = 34;
      }

      ServiceEndpoint endPoint = null;

      System.Xml.XmlDictionaryReaderQuotas readQuoatas = new System.Xml.XmlDictionaryReaderQuotas()
      {
        MaxArrayLength = int.MaxValue,
        MaxBytesPerRead = int.MaxValue,
        MaxDepth = int.MaxValue,
        MaxNameTableCharCount = int.MaxValue,
        MaxStringContentLength = int.MaxValue
      };

      Binding bind = new BasicHttpBinding()
        {
          MaxReceivedMessageSize = int.MaxValue,
          //MaxBufferPoolSize = int.MaxValue,
          //MaxBufferSize = int.MaxValue,
          ReaderQuotas = readQuoatas
        };
      endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaReporting), bind, "Soap");

      endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaReporting), new WebHttpBinding()
        {
          MaxReceivedMessageSize = int.MaxValue,
          //MaxBufferPoolSize = int.MaxValue,
          //MaxBufferSize = int.MaxValue,
          ReaderQuotas = readQuoatas
        }, "Web");

      endPoint.Behaviors.Add(new WebQueryStringBehaviour());

      endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaReporting), new WSHttpBinding()
        {
          MaxReceivedMessageSize = int.MaxValue,
          //MaxBufferPoolSize = int.MaxValue,
          ReaderQuotas = readQuoatas
        }, "Ws");

      try
      {
        //for some reason a default endpoint does not get created here
        host.Open();

        Console.WriteLine("The service reporting is ready at {0}", baseAddress);

        Console.WriteLine();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      return host;
    }

    static DOPharosReader DOPharosMailer(Config config)
    {
      DOPharosReader dopr = new DOPharosReader(config);

      if (!dopr.Start("scms.dop@ams.co.id", "scmsdop"))
      {
        Console.WriteLine("Failed to run POP3 DO Prinsipal Client.");
      }

      return dopr;
    }

    static DOPharmanetReader DOPharmanetMailer(Config config)
    {
        DOPharmanetReader dophar = new DOPharmanetReader(config);
        //if (!dophar.Start("scms.sph@ams.co.id", "scms"))
        if (!dophar.Start("scms.dophar@ams.co.id", "scmsdophar"))
        {
            Console.WriteLine("Failed to run POP3 DO Prinsipal Client.");
        }

        return dophar;
    }

    static DOPharmanetReaderPerItem DOPharmanetMailerPerItem(Config config)
    {
        DOPharmanetReaderPerItem dopharItem = new DOPharmanetReaderPerItem(config);
        if (!dopharItem.Start("scms.sph@ams.co.id", "scms"))
        {
            Console.WriteLine("Failed to run POP3 DO Prinsipal Client.");
        }

        return dopharItem;
    }

    static SPCabangReader SPCabMailer(Config Config)
    {

      SPCabangReader spcab = new SPCabangReader(Config);

      if (!spcab.Start("scms.sp@ams.co.id", "scmssp"))
      {
        Console.WriteLine("Failed to run POP3 SP Cabang Client.");
      }

      return spcab;
    }

    static SPPharmanetReader SPPharMailer(Config Config)
    {

        SPPharmanetReader spphar = new SPPharmanetReader(Config);

        if (!spphar.Start("scms.sph@ams.co.id", "scms"))
        {
            Console.WriteLine("Failed to run POP3 SP Cabang Client.");
        }

        return spphar;
    }
    //static SendPOAuto POSendingMailer(Config config)
    //{
    //  SendPOAuto spoa = new SendPOAuto(config);

    //  if (!spoa.Start())
    //  {
    //    Console.WriteLine("Failed to run SenderPOAuto.");
    //  }

    //  return spoa;
    //}

    static SendRCAuto RCSendingMailer(Config config)
    {
        SendRCAuto srca = new SendRCAuto(config);

        if (!srca.Start())
        {
            Console.WriteLine("Failed to run SenderRCAuto.");
        }

        return srca;
    }

    //static ScmsSoaLibrary.Core.Schedule.AutoRunning ScheduleAutoRunning(Config Config)
    //{
    //  ScmsSoaLibrary.Core.Schedule.AutoRunning sar = new ScmsSoaLibrary.Core.Schedule.AutoRunning(Config);

    //  sar.Start();

    //  if (!sar.IsStart)
    //  {
    //    Console.WriteLine("Failed to run auto schedule.");
    //  }

    //  return sar;
    //}

    static SPCabangReaderBackup SPCabNonMail(Config config)
    {
      SPCabangReaderBackup spcb = new SPCabangReaderBackup(config);

      return spcb;
    }

    static string GenerateNumbering<T>(ORMDataContext db, string headerCode, char portalKode, string tipeKode, DateTime tanggalAktif, string fieldCondition)
    {
      string result = string.Empty;

      //db.GetTable<T>().Where(

      int nCount = 0,
        nValue = 0;
      string tmpNum = null,
        hdrValue = null,
        tVal = null;
      char chr = char.MinValue;

      SysNo sysNum = (from q in db.SysNos
                      where q.c_portal == portalKode && q.c_type == tipeKode
                        && q.s_tahun == tanggalAktif.Year
                      select q).SingleOrDefault();

      if (sysNum != null)
      {
        switch (tanggalAktif.Month)
        {
          case 1: tmpNum = (sysNum.c_bln01 ?? string.Empty); break;
          case 2: tmpNum = (sysNum.c_bln02 ?? string.Empty); break;
          case 3: tmpNum = (sysNum.c_bln03 ?? string.Empty); break;
          case 4: tmpNum = (sysNum.c_bln04 ?? string.Empty); break;
          case 5: tmpNum = (sysNum.c_bln05 ?? string.Empty); break;
          case 6: tmpNum = (sysNum.c_bln06 ?? string.Empty); break;
          case 7: tmpNum = (sysNum.c_bln07 ?? string.Empty); break;
          case 8: tmpNum = (sysNum.c_bln08 ?? string.Empty); break;
          case 9: tmpNum = (sysNum.c_bln09 ?? string.Empty); break;
          case 10: tmpNum = (sysNum.c_bln10 ?? string.Empty); break;
          case 11: tmpNum = (sysNum.c_bln11 ?? string.Empty); break;
          case 12: tmpNum = (sysNum.c_bln12 ?? string.Empty); break;
          default: tmpNum = null; break;
        }

        if (!string.IsNullOrEmpty(tmpNum))
        {
          tmpNum = (string.IsNullOrEmpty(tmpNum) ? string.Empty : tmpNum.Trim());

          hdrValue = (string.IsNullOrEmpty(headerCode) ? "__" :
            ((headerCode.Length > 1) ? headerCode.PadLeft(2, '_') : headerCode.Substring(0, 2)));

          do
          {
            if (tmpNum.Length > 1)
            {
              if (!char.IsNumber(tmpNum, 0))
              {
                tVal = tmpNum.Substring(1);

                if (!int.TryParse(tVal, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 999)
                {
                  chr = tmpNum[0];
                  chr++;

                  if (chr > 0x60)
                  {
                    chr = (char)0x7b;
                  }

                  tmpNum = string.Concat(chr, "001");
                }
                else
                {
                  tmpNum = string.Concat(tmpNum[0], nValue.ToString().PadLeft(3, '0'));
                }
              }
              else
              {
                if (!int.TryParse(tmpNum, out nValue))
                {
                  result = string.Empty;

                  goto endLogic;
                }

                nValue++;

                if (nValue > 9999)
                {
                  tmpNum = "A001";
                }
                else
                {
                  tmpNum = nValue.ToString().PadLeft(4, '0');
                }
              }
            }
            else
            {
              tmpNum = "0001";
            }

            result = string.Concat(hdrValue, tanggalAktif.ToString("yyMM"), tmpNum);

            nCount = db.GetTable(typeof(T)).Where(string.Concat(fieldCondition, " = @0"), result).OfType<T>().Count();

          } while (nCount != 0);


          switch (tanggalAktif.Month)
          {
            case 1: sysNum.c_bln01 = tmpNum; break;
            case 2: sysNum.c_bln02 = tmpNum; break;
            case 3: sysNum.c_bln03 = tmpNum; break;
            case 4: sysNum.c_bln04 = tmpNum; break;
            case 5: sysNum.c_bln05 = tmpNum; break;
            case 6: sysNum.c_bln06 = tmpNum; break;
            case 7: sysNum.c_bln07 = tmpNum; break;
            case 8: sysNum.c_bln08 = tmpNum; break;
            case 9: sysNum.c_bln09 = tmpNum; break;
            case 10: sysNum.c_bln10 = tmpNum; break;
            case 11: sysNum.c_bln11 = tmpNum; break;
            case 12: sysNum.c_bln12 = tmpNum; break;
          }

          db.SubmitChanges();

        endLogic:
          ;
        }
      }

      return result;
    }

    static void Testing()
    {
      Uri uri = new Uri("http://10.100.11.12/dist_core/?m=com.ams.json.ds&action=form&f=Business&open=trx_pb");

      Dictionary<string, string> param = new Dictionary<string,string>();
      param.Add("C_PBNO", "PBR-JK1-2011-10-0001");
      param.Add("C_KODECABOLD", "F1");

      Dictionary<string, string> header = new Dictionary<string,string>();
      header.Add("X-Requested-With", "XMLHttpRequest");

      Encoding utf8 = Encoding.UTF8;
      
      bool getSuccess = false;

      string result = null;
      Dictionary<string, object> dicHeader = null;
      Dictionary<string, object> dicDetail = null;
      List<Dictionary<string, string>> list = null;
      Dictionary<string, string> dataRow = null;

      ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

      pdc.Referer = "http://10.100.11.12/dist_core/?m=com.ams.trx.pbbpbr";
      pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      if (pdc.PostGetData(uri, param, header))
      {
        result = utf8.GetString(pdc.Result);

        dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

        if ((dicHeader != null) && dicHeader.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS))
        {
          getSuccess = (bool)dicHeader[Constant.DEFAULT_NAMING_SUCCESS];

          if (getSuccess)
          {
            param.Remove("C_KODECABOLD");

            list = dicHeader[Constant.DEFAULT_NAMING_RECORDS] as List<Dictionary<string, string>>;

            if (list.Count > 0)
            {
              dataRow = list[0];

              if (dataRow.ContainsKey("C_KODECAB"))
              {
                param.Add("C_KODECAB", dataRow["C_KODECAB"]);

                uri = new Uri("http://10.100.11.12/dist_core/?m=com.ams.json.ds&action=form&f=Business&open=trx_pb_dt");
                if (pdc.PostGetData(uri, param, header))
                {
                  result = utf8.GetString(pdc.Result);

                  dicDetail = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

                  if ((dicHeader != null) && dicHeader.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS))
                  {
                    getSuccess = (bool)dicDetail[Constant.DEFAULT_NAMING_SUCCESS];
                    
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(dicDetail, Newtonsoft.Json.Formatting.None);
                  }
                }
                else
                {
                  result = pdc.ErrorMessage;
                }
              }
            }
          }
        }
      }
      else
      {
        result = pdc.ErrorMessage;
      }

      Console.WriteLine(result);
    }

    static void TestingJSonRpc()
    {
      //Jayrock.JsonRpc.JsonRpcClient client = new Jayrock.JsonRpc.JsonRpcClient();
      //client.Url = "http://www.raboof.com/projects/jayrock/demo.ashx";
      ////client.Url = "http://10.100.11.25/devel/distcore_service_bus/?Business=" + business;
      //Console.WriteLine(client.Invoke("system.about"));
      //Console.WriteLine(client.Invoke("system.version"));
      //Console.WriteLine(string.Join(Environment.NewLine, (string[])(new ArrayList((ICollection)client.Invoke("system.listMethods"))).ToArray(typeof(string))));
      //Console.WriteLine(client.Invoke("now"));
      //Console.WriteLine(((DateTime)client.Invoke(typeof(DateTime), "now")).ToString("r"));
      //Console.WriteLine(client.InvokeVargs("sum", 123, 456));
      //Console.WriteLine(client.Invoke("sum", new Jayrock.Json.JsonObject { { "a", 123 }, { "b", 456 } }));
      //Console.WriteLine(client.InvokeVargs("total", new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
      //client.CookieContainer = new System.Net.CookieContainer();
      //Console.WriteLine(client.Invoke("counter"));
      //Console.WriteLine(client.Invoke("counter"));
    }

    static void TestingKirim()
    {
      Uri uri = new Uri("http://10.100.11.12/dist_core/?m=com.ams.json.ds&action=form&f=Submit&_q=trx_rc_trigger");

      string dikirim = @"{
      'ID':'PBRXXXX', 'RC':'',
      'Fields':
      [
        { 'Item':'1212', 'Batch':'-', 'DO':'DO11011011', 'QTY':25, 'Acc':25, 'Dtry'=0, 'Type':'01' },
        { 'Item':'1213', 'Batch':'-', 'DO':'DO11011012', 'QTY':20, 'Acc':10, 'Dtry'=10, 'Type':'02' }
      ]
      }";

      Dictionary<string, string> param = new Dictionary<string, string>();
      param.Add("params", dikirim);

      Dictionary<string, string> header = new Dictionary<string, string>();
      header.Add("X-Requested-With", "XMLHttpRequest");

      Encoding utf8 = Encoding.UTF8;

      bool getSuccess = false;

      string result = null;
      Dictionary<string, object> dicHeader = null;
      //Dictionary<string, object> dicDetail = null;
      //List<Dictionary<string, string>> list = null;
      //Dictionary<string, string> dataRow = null;

      ScmsSoaLibrary.Parser.ParserDisCore pdc = new ScmsSoaLibrary.Parser.ParserDisCore();

      pdc.Referer = "http://10.100.11.12/dist_core/?m=com.ams.trx.pbbpbr";
      pdc.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
      if (pdc.PostGetData(uri, param, header))
      {
        result = utf8.GetString(pdc.Result);

        dicHeader = ScmsSoaLibrary.Parser.ParserDisCore.ParsingFromDisCore(result);

        if ((dicHeader != null) && dicHeader.ContainsKey(Constant.DEFAULT_NAMING_SUCCESS))
        {
          getSuccess = (bool)dicHeader[Constant.DEFAULT_NAMING_SUCCESS];

          if (getSuccess)
          {
            Console.WriteLine(result);
          }
        }
      }
      else
      {
        result = pdc.ErrorMessage;
      }
      
      Console.WriteLine(result);
    }

    static void ConvertClassToJson()
    {
      ScmsSoaLibrary.Parser.Class.ReturCustomerResponse strt = null;
      List<ScmsSoaLibrary.Parser.Class.ReturCustomerStructureField> list = new List<ScmsSoaLibrary.Parser.Class.ReturCustomerStructureField>();

      list.Add(new ScmsSoaLibrary.Parser.Class.ReturCustomerStructureField()
      {
        Acceptance = 25,
        Batch = "BATCHXX",
        NoDO = "DOXX",
        Item = "1561",
        //QuantityReceive = 25,
        //QuantityReceive = 25,
        Destroy = 0,
        Type = ""
      });
      list.Add(new ScmsSoaLibrary.Parser.Class.ReturCustomerStructureField()
      {
        Acceptance = 15,
        Batch = "BATCHXX",
        NoDO = "DOXX",
        //Dtry = 10,
        Item = "1562",
        //QuantityReceive = 25,
        //QuantityReceive = 25,
        Destroy = 0,
        Type = ""
      });

      strt = new ScmsSoaLibrary.Parser.Class.ReturCustomerResponse()
      {
        ID = "PBRXXX",
        RC = "",
        Fields = list.ToArray()
      };

      list.Clear();

      string result = ScmsSoaLibrary.Parser.Class.ReturCustomerResponse.Serialize(strt);

      Console.WriteLine(result);
    }

    static void PostKirim(object param)
    {
      string par = param as string;

      // Logic Kirim

      int nLoop = 5;

      while (nLoop > 0)
      {
        System.Threading.Thread.Sleep(1000);

        Console.WriteLine();
        Console.WriteLine();

        Console.WriteLine(par);

        nLoop--;
      }

      Console.WriteLine("Proses selesai");
    }

    static void MultiPostKirim()
    {
      System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(PostKirim), "patra monyong"); 
    }

    static void TestingSendMail()
    {
      //System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage("m.rudi@yangganteng.org", "patra@ams.co.id");
      //mm.Subject = "Test";
      //mm.Body = "Patra eek banget...";

      //System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient("10.100.10.9", 25);
      //sc.Send(mm);
    }

    static void TestingCodeGenerated()
    {
      ORMDataContext db = new ORMDataContext();

      DateTime dateF = new DateTime(2011, 5, 02);

      string tmp = GenerateNumbering<LG_FJRH>(db, "JR", '3', "18", dateF, "c_fjno");

      Console.WriteLine(tmp);
    }

    static void TestingContains()
    {
      //ORMDataContext db = new ORMDataContext();

      //var itm = db.FA_MasItms.AsQueryable().ToList().Where(
    }

    #region Dbf Pop3

    //static void TestingGetMail()
    //{
    //  string srvPop3 = "10.100.10.9";
    //  int srvPop3Port = 110;
    //  string userMail = "scms@ams.co.id";
    //  string userMailPwd = "scms";

    //  SmtPop.POP3Client pop = new SmtPop.POP3Client();
    //  pop.ReceiveTimeout = 3 * 60000; // Set the timeout to 3 minutes
      
    //  int nResult = pop.Open(srvPop3, srvPop3Port, userMail, userMailPwd);

    //  // retrieve messages list from pop server
    //  SmtPop.POPMessageId[] messages = pop.GetMailList();

    //  if (messages != null)
    //  {
    //    // run through available messages in POP3 server
    //    foreach (SmtPop.POPMessageId id in messages)
    //    {
    //      SmtPop.POPReader reader = pop.GetMailReader(id); //pop.GetMailReader(id.Id);
    //      SmtPop.MimeMessage msg = new SmtPop.MimeMessage();

    //      // read the message
    //      msg.Read(reader);
    //      if (msg.Attachments != null)
    //      {
    //        // retrieve attachements
    //        foreach (SmtPop.MimeAttachment attach in msg.Attachments)
    //        {
    //          if (attach.Filename != "")
    //          {
    //            // read data from attachment
    //            byte[] b = Convert.FromBase64String(attach.Body);

    //            //// save attachment to disk
    //            //System.IO.MemoryStream mem = new System.IO.MemoryStream(b, false);
    //            //System.IO.FileStream outStream = System.IO.File.OpenWrite(attach.Filename);
    //            //mem.WriteTo(outStream);
    //            //mem.Close();
    //            //outStream.Flush();
    //            //outStream.Close();
    //          }
    //        }
    //      }

    //      reader.Close();
    //      reader.Dispose();

    //      ////delete message
    //      //pop.Dele(id.Id);
    //    }
    //  }

    //  pop.Quit();

    //}

    static void TestingMailer()
    {
      Config cfg = new Config();

      string srvPop3 = "10.100.10.9";
      int srvPop3Port = 110;
      string userMail = "scms@ams.co.id";
      string userMailPwd = "scms";

      Pop3Mailer pop = new Pop3Mailer(srvPop3, srvPop3Port, cfg);

      pop.Pop3MailMessage += new Pop3Mailer.Pop3MailMessageEventHandler(pop_Pop3MailMessage);

      pop.Start(userMail, userMailPwd, false);

      //System.Threading.Thread.Sleep(3000);

      pop.CheckNow();

      pop.Stop();
    }

    static void pop_Pop3MailMessage(object sender, Pop3MailMessageEventArgs e)
    {
      //throw new NotImplementedException();

      System.Data.DataSet dateset = null;
      Pop3Mailer pop = sender as Pop3Mailer;

      try
      {
        //pop.TempPath
        if (!System.IO.Directory.Exists(pop.TempPath))
        {
          System.IO.Directory.CreateDirectory(pop.TempPath);
        }

        //SaveToTemplateAndExtract(pop.TempPath, 
        //pathFile = System.IO.Path.Combine(pop.TempPath, e.Attachments);
        foreach (KeyValuePair<string, byte[]> kvp in e.Attachments)
        {
          dateset = SaveToTemplateAndExtract(pop.TempPath, kvp.Value);
        }

      }
      catch (Exception ex)
      {
        dateset = null;

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
    }

    static System.Data.DataSet SaveToTemplateAndExtract(string pathFolder, byte[] datas)
    {
      if (string.IsNullOrEmpty(pathFolder) || (datas == null) ||(!System.IO.Directory.Exists(pathFolder)))
      {
        return null;
      }

      const int MAXIMUM_BUFFER = 4096;

      System.Data.DataSet dataSet = new System.Data.DataSet();
      System.Data.DataTable tableHeader = null;
      System.Data.DataTable tableDetail = null;

      int nExtract = 0;

      #region Zip Operation

      System.IO.FileStream fs = null;
      System.IO.MemoryStream ms = null;
      ICSharpCode.SharpZipLib.Zip.ZipFile zip = null;
      System.IO.Stream zipStream = null;
      string fName = null;
      byte[] buff = null;
      BufferManager bufMan = null;
      bool isOkToExtract = false;
      
      ms = new System.IO.MemoryStream();

      ms.Write(datas, 0, datas.Length);

      zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(ms);

      bufMan = BufferManager.CreateBufferManager(long.MaxValue, 1024);

      foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry ze in zip)
      {
        if (ze.IsFile)
        {
          try
          {
            isOkToExtract = false;

            if (ze.Name.Equals(DEFAULT_NAME_FILE_DETAIL, StringComparison.OrdinalIgnoreCase))
            {
              nExtract++;

              isOkToExtract = true;
            }
            else if (ze.Name.Equals(DEFAULT_NAME_FILE_HEADER, StringComparison.OrdinalIgnoreCase))
            {
              nExtract++;

              isOkToExtract = true;
            }

            if (isOkToExtract)
            {
              //buff = new byte[MAXIMUM_BUFFER];

              buff = bufMan.TakeBuffer(MAXIMUM_BUFFER);

              zipStream = zip.GetInputStream(ze);

              fName = System.IO.Path.Combine(pathFolder, ze.Name);

              if (System.IO.File.Exists(fName))
              {
                System.IO.File.Delete(fName);
              }

              fs = new System.IO.FileStream(fName, System.IO.FileMode.Create);

              ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(zipStream, fs, buff);
            }
          }
          catch (Exception ex)
          {
            Logger.WriteLine(ex.Message);
            Logger.WriteLine(ex.StackTrace);
          }
          finally
          {
            if (fs != null)
            {
              fs.Close();
              fs.Dispose();
            }

            if (zipStream != null)
            {
              zipStream.Close();
              zipStream.Dispose();
            }

            if (buff != null)
            {
              //Array.Clear(buff, 0, MAXIMUM_BUFFER);
              bufMan.ReturnBuffer(buff);
              buff = null;
            }
          }
        }
      }

      bufMan.Clear();

      zip.Close();
      
      ms.Close();
      ms.Dispose();

      #endregion

      if (nExtract == 2)
      {
        fName = System.IO.Path.Combine(pathFolder, DEFAULT_NAME_FILE_HEADER);
        tableHeader = ReadDbfDatabase(fName, DEFAULT_NAME_TABEL_HEADER);

        fName = System.IO.Path.Combine(pathFolder, DEFAULT_NAME_FILE_DETAIL);
        tableDetail = ReadDbfDatabase(fName, DEFAULT_NAME_TABEL_DETAIL);

        dataSet.Tables.Add(tableHeader);
        dataSet.Tables.Add(tableDetail);
      }

      return dataSet;
    }

    static void ReadDbf(string dbfFile)
    {
      string fileName = System.IO.Path.GetFileName(dbfFile);
      string pathName = System.IO.Path.GetDirectoryName(dbfFile);
      string dbName = System.IO.Path.GetFileNameWithoutExtension(dbfFile);

      System.Data.DataTable dt1 = null;

      pathName = (pathName.EndsWith("\\") ? pathName : pathName + "\\");

      //System.Data.OleDb.OleDbConnection ccc = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathName.Substring(0, pathName.LastIndexOf("\\")) + ";Extended Properties=dBASE IV;");
      System.Data.OleDb.OleDbConnection ccc = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbfFile + ";Extended Properties=dBASE IV;");
      ccc.Open();

      System.Data.OleDb.OleDbCommand cmd1 = new System.Data.OleDb.OleDbCommand("Select * From " + fileName, ccc);
      System.Data.OleDb.OleDbDataReader dr1 = cmd1.ExecuteReader();
      if (dr1.HasRows)
      {
        dt1 = new System.Data.DataTable();
        dt1.Load(dr1);

        dr1.Close();
        dr1.Dispose();
      }

      ccc.Close();
      ccc.Dispose();

      System.Data.DataRow row = dt1.Rows[0];

      
    }

    static System.Data.DataTable ReadDbfDatabase(string dbfFile, string tahleName)
    {
      if (!System.IO.File.Exists(dbfFile))
      {
        return null;
      }

      System.Data.DataTable table = null;

      string fileName = System.IO.Path.GetFileName(dbfFile);
      string pathName = System.IO.Path.GetDirectoryName(dbfFile);
      string pathNameFull = (pathName.EndsWith("\\") ? pathName : string.Concat(pathName, "\\"));

      System.Data.OleDb.OleDbConnection con = null;
      System.Data.OleDb.OleDbCommand cmd = null;
      System.Data.OleDb.OleDbDataReader odbReader = null;

      try
      {
        con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties=dBASE IV;", pathName));
        con.Open();

        if (con.State == System.Data.ConnectionState.Open)
        {
          cmd = con.CreateCommand();
          cmd.CommandText = string.Format("Select * From [{0}]", fileName);

          odbReader = cmd.ExecuteReader();

          if ((odbReader != null) && (odbReader.HasRows) && (!odbReader.IsClosed))
          {
            table = new System.Data.DataTable(tahleName);
            table.Load(odbReader);
          }
        }
      }
      catch (Exception ex)
      {
        table = null;

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        if (odbReader != null)
        {
          odbReader.Close();
          odbReader.Dispose();
        }

        if (con != null)
        {
          if (con.State == System.Data.ConnectionState.Open)
          {
            con.Close();
          }
          con.Dispose();
        }
      }

      return table;
    }

    static void PopulateDoHeaderDetailPI(System.Data.DataSet dataset, Dictionary<string, string> dicMappingPrinsipal, ScmsModel.ORMDataContext db)
    {
      if ((dataset == null) || (dataset.Tables.Count != 2) || (dicMappingPrinsipal == null))
      {
        return;
      }

      const string PENDING_PO_NAME = "(PENDING)";

      List<LG_DOPH> lstDOPH = null;
      LG_DOPH doph = null;
      List<LG_DOPD> lstDOPD = null;
      LG_DOPD dopd = null;
      List<LG_DOPH> lstDOPHRem = null;

      System.Data.DataTable table = null,
        tableDetail = null;
      string namaPrinsipal = null,
        tmp = null,
        kodePrinsipal = null,
        poData = null;
      System.Data.DataRow row = null;
      System.Data.DataRow[] rowCols = null;
      DateTime date = DateTime.MinValue;

      string[] poList = null;
      DateTime dateNow = DateTime.Now;
      bool isPending = false;

      int nLoop = 0,
        nLen = 0,
        nLoopC = 0,
        nLenC = 0,
        nLoopCi = 0,
        nLenCi = 0;

      decimal nPoQtySisa = 0,
        nQtyDO = 0;

      List<DO_PO_Link> lstPoLink = null;
      DO_PO_Link dol = default(DO_PO_Link);

      List<DO_PI_Header_Check> lstDOPHeaderCheck = null;
      List<DO_PI_Detail_Check> lstDOPDetailCheck = null;
      List<DO_PI_Result> lstDOPResult = null;
      DO_PI_Result dopic = null;

      if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_HEADER))
      {
        table = dataset.Tables[DEFAULT_NAME_TABEL_HEADER];

        if (dataset.Tables.Contains(DEFAULT_NAME_TABEL_DETAIL))
        {
          tableDetail = dataset.Tables[DEFAULT_NAME_TABEL_DETAIL];
        }

        lstDOPH = new List<LG_DOPH>();
        lstDOPD = new List<LG_DOPD>();

        #region Populate Data

        for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
        {
          row = table.Rows[nLoop];

          namaPrinsipal = row.GetValue<string>("C_PT", string.Empty);

          if (dicMappingPrinsipal.ContainsKey(namaPrinsipal))
          {
            kodePrinsipal = dicMappingPrinsipal[namaPrinsipal];
          }
          else
          {
            kodePrinsipal = "00001";
          }

          #region DOPH

          doph = new LG_DOPH()
          {
            c_nosup = kodePrinsipal,
            c_dono = row.GetValue<string>("C_NODO", string.Empty),
            d_dodate = null,
            d_fjno = row.GetValue<string>("C_EXNOINV", string.Empty),
            d_fjdate = null,
            l_status = false,
            c_cab = row.GetValue<string>("C_KDCAB", string.Empty),
            c_via = null,
            c_taxno = row.GetValue<string>("C_NOTAX", string.Empty),
            d_entry = dateNow
          };

          #endregion

          if (tableDetail != null)
          {
            rowCols = tableDetail.Select(string.Format("C_PT = '{0}' And C_NODO = '{1}'",
              namaPrinsipal, doph.c_dono), "C_ITNAM");

            if ((rowCols != null) && (rowCols.Length > 0))
            {
              #region Lanjutan DOPH

              date = row.GetValue<DateTime>("D_TGLDO", DateTime.MinValue);
              if (date.Equals(DateTime.MinValue))
              {
                tmp = row.GetValue<string>("D_TGLDO", string.Empty);
                if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                {
                  date = Functionals.StandardSqlDateTime;
                }
              }
              doph.d_dodate = date;

              date = row.GetValue<DateTime>("D_JTH", DateTime.MinValue);
              if (date.Equals(DateTime.MinValue))
              {
                tmp = row.GetValue<string>("D_JTH", string.Empty);
                if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                {
                  date = Functionals.StandardSqlDateTime;
                }
              }
              doph.d_fjdate = date;

              tmp = row.GetValue<string>("C_VIA", string.Empty).Trim();
              if (tmp == null)
              {
                tmp = "00";
              }
              else if (tmp.Equals("D", StringComparison.OrdinalIgnoreCase))
              {
                tmp = "02";
              }
              else if (tmp.Equals("U", StringComparison.OrdinalIgnoreCase))
              {
                tmp = "01";
              }
              else
              {
                tmp = "03";
              }
              doph.c_via = tmp;

              #endregion

              for (nLoopC = 0, nLenC = rowCols.Length; nLoopC < nLenC; nLoopC++)
              {
                row = rowCols[nLoopC];

                poData = row.GetValue<string>("C_NOSP", string.Empty).Replace(" ", "").Trim();
                nQtyDO = row.GetValue<decimal>("N_QTYRCV");

                #region Default Detail

                dopd = new LG_DOPD()
                {
                  c_nosup = kodePrinsipal,
                  c_dono = row.GetValue<string>("C_NODO", string.Empty),
                  c_iteno = row.GetValue<string>("C_ITENO", string.Empty),
                  c_itenopri = row.GetValue<string>("C_ITENOPRI", string.Empty),
                  v_itnam = null,
                  v_undes = null,
                  n_qty = nQtyDO,
                  n_qty_sisa = nQtyDO,
                  c_batch = row.GetValue<string>("C_BATCH", string.Empty),
                  d_expired = null,
                  c_pono = null,
                  n_disc = row.GetValue<decimal>("N_DISC"),
                  l_claim = row.GetValue<bool>("L_CLAIMBNS"),
                  c_type = "01",
                  d_entry = dateNow,
                  l_done = false,
                  l_pending = false
                };

                #endregion

                date = row.GetValue<DateTime>("D_EXPIRED", DateTime.MinValue);
                if (date.Equals(DateTime.MinValue))
                {
                  tmp = row.GetValue<string>("D_EXPIRED", string.Empty);
                  if (!Functionals.DateParser(tmp, "M/d/yyyy HH:mm:ss", out date))
                  {
                    date = Functionals.StandardSqlDateTime;
                  }
                }
                dopd.d_expired = date;

                if (poData.Contains(","))
                {
                  #region Multiple PO

                  poList = poData.Split(',');

                  if (poData.IndexOf(PENDING_PO_NAME, StringComparison.OrdinalIgnoreCase) == -1)
                  {
                    isPending = true;
                  }
                  else
                  {
                    isPending = false;
                  }

                  lstPoLink = (from q in db.LG_POD1s
                               where poList.Contains(q.c_pono) && (q.c_iteno == dopd.c_iteno)
                               select new DO_PO_Link()
                               {
                                 PO = q.c_pono,
                                 Item = dopd.c_iteno,
                                 Sisa = (q.n_sisa.HasValue ? q.n_sisa.Value : 0)
                               }).Distinct().ToList();

                  for (nLoopCi = 0, nLenCi = poList.Length; nLoopCi < nLenCi; nLoopCi++)
                  {
                    tmp = poList[nLoopCi].Trim();

                    dol = lstPoLink.Find(delegate(DO_PO_Link dopol)
                    {
                      return (string.IsNullOrEmpty(dopol.PO) ? false : true);
                    });

                    if (dol == null)
                    {
                      nPoQtySisa = (nQtyDO > dol.Sisa ?
                        (dol.Sisa - nQtyDO) : nQtyDO);
                    }
                    else
                    {
                      nPoQtySisa = 0;
                    }

                    if (tmp.IndexOf(PENDING_PO_NAME, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                      isPending = false;
                    }
                    else
                    {
                      tmp = tmp.Replace(PENDING_PO_NAME, "").Trim();
                      isPending = true;
                    }

                    //nQtyDO = (lstPoLink.Count > nLoopCi

                    lstDOPD.Add(new LG_DOPD()
                    {
                      c_nosup = kodePrinsipal,
                      c_dono = dopd.c_dono,
                      c_iteno = dopd.c_iteno,
                      c_itenopri = dopd.c_itenopri,
                      v_itnam = dopd.v_itnam,
                      v_undes = dopd.v_undes,
                      n_qty = nPoQtySisa,
                      n_qty_sisa = nPoQtySisa,
                      c_batch = dopd.c_batch,
                      d_expired = dopd.d_expired,
                      c_pono = tmp,
                      n_disc = dopd.n_disc,
                      l_claim = dopd.l_claim,
                      c_type = dopd.c_type,
                      d_entry = dateNow,
                      l_done = false,
                      l_pending = isPending
                    });
                  }

                  lstPoLink.Clear();

                  #endregion
                }
                else
                {
                  #region Single PO

                  if (poData.IndexOf(PENDING_PO_NAME, StringComparison.OrdinalIgnoreCase) == -1)
                  {
                    isPending = false;
                  }
                  else
                  {
                    poData = poData.Replace(PENDING_PO_NAME, "").Trim();
                    isPending = true;
                  }

                  dopd.c_pono = string.Concat("PO", poData);
                  dopd.l_pending = isPending;

                  lstDOPD.Add(dopd);

                  #endregion
                }
              }
            }

            lstDOPH.Add(doph);
          }
        }

        #endregion

        #region Check Header

        lstDOPHeaderCheck = lstDOPH.GroupBy(x => new { x.c_nosup, x.c_dono })
          .Select(y => new DO_PI_Header_Check()
          {
            Principal = y.Key.c_nosup.Trim(),
            DO = y.Key.c_dono.Trim()
          }).ToList();

        if ((lstDOPHeaderCheck != null) && (lstDOPHeaderCheck.Count > 1))
        {
          tmp = (from q in db.LG_DOPHs
                 where lstDOPHeaderCheck.Contains(new DO_PI_Header_Check()
                 {
                   Principal = q.c_nosup,
                   DO = q.c_dono
                 })
                 select new DO_PI_Result()
                 {
                   Principal = q.c_nosup,
                   DO = q.c_dono
                 }).Distinct().Provider.ToString();

          lstDOPResult = (from q in db.LG_DOPHs
                          where lstDOPHeaderCheck.Contains(new DO_PI_Header_Check()
                          {
                            Principal = q.c_nosup,
                            DO = q.c_dono
                          })
                          select new DO_PI_Result()
                          {
                            Principal = q.c_nosup.Trim(),
                            DO = q.c_dono.Trim()
                          }).Distinct().ToList();

          if ((lstDOPResult != null) && (lstDOPResult.Count > 0))
          {
            lstDOPHRem = (from q in lstDOPH
                          join q1 in lstDOPResult on q.c_nosup equals q1.Principal
                          select q).ToList();

            if ((lstDOPHRem != null) && (lstDOPHRem.Count > 0))
            {
              for (nLoop = 0, nLen = lstDOPHRem.Count; nLoop < nLen; nLoop++)
              {
                lstDOPH.Remove(lstDOPHRem[nLoop]);
              }

              lstDOPHRem.Clear();
            }

            for (nLoop = 0, nLen = lstDOPResult.Count; nLoop < nLen; nLoop++)
            {
                dopic = lstDOPResult[nLoop];

                doph = lstDOPH.Find(delegate(LG_DOPH dopoh)
                {
                    return (dopoh.c_nosup.Equals(dopic.Principal, StringComparison.OrdinalIgnoreCase) &&
                      dopoh.c_dono.Equals(dopic.DO, StringComparison.OrdinalIgnoreCase));
                });

                if (doph != null)
                {
                    lstDOPH.Remove(doph);
                }
            }

            lstDOPResult.Clear();
          }

          lstDOPHeaderCheck.Clear();
        }

        #endregion

        #region Check Detail

        lstDOPDetailCheck = lstDOPD.GroupBy(x => new { x.c_nosup, x.c_dono, x.c_iteno, x.c_type, x.c_pono, x.c_batch })
          .Select(y => new DO_PI_Detail_Check()
          {
            Principal = y.Key.c_nosup.Trim(),
            DO = y.Key.c_dono.Trim(),
            Item = y.Key.c_iteno.Trim(),
            PO = y.Key.c_pono.Trim(),
            Batch = y.Key.c_batch.Trim()
          }).ToList();

        if ((lstDOPDetailCheck != null) && (lstDOPDetailCheck.Count > 1))
        {
          tmp = (from q in db.LG_DOPDs
                 where lstDOPDetailCheck.Contains(new DO_PI_Detail_Check()
                 {
                   Principal = q.c_nosup,
                   DO = q.c_dono,
                   Item = q.c_iteno,
                   PO = q.c_pono,
                   Batch = q.c_batch
                 })
                 select new DO_PI_Result()
                 {
                   Principal = q.c_nosup,
                   DO = q.c_dono,
                   Item = q.c_iteno,
                   PO = q.c_pono,
                   Batch = q.c_batch
                 }).Distinct().Provider.ToString();

          lstDOPResult = (from q in db.LG_DOPDs
                          where lstDOPDetailCheck.Contains(new DO_PI_Detail_Check()
                          {
                            Principal = q.c_nosup,
                            DO = q.c_dono,
                            Item = q.c_iteno,
                            PO = q.c_pono,
                            Batch = q.c_batch
                          })
                          select new DO_PI_Result()
                          {
                            Principal = q.c_nosup.Trim(),
                            DO = q.c_dono.Trim(),
                            Item = q.c_iteno.Trim(),
                            PO = q.c_pono.Trim(),
                            Batch = q.c_batch.Trim()
                          }).Distinct().ToList();

          if ((lstDOPResult != null) && (lstDOPResult.Count > 0))
          {
            lstDOPHRem = (from q in lstDOPH
                          join q1 in lstDOPResult on q.c_nosup equals q1.Principal
                          select q).ToList();

            if ((lstDOPHRem != null) && (lstDOPHRem.Count > 0))
            {
              for (nLoop = 0, nLen = lstDOPHRem.Count; nLoop < nLen; nLoop++)
              {
                lstDOPH.Remove(lstDOPHRem[nLoop]);
              }

              lstDOPHRem.Clear();
            }

            for (nLoop = 0, nLen = lstDOPResult.Count; nLoop < nLen; nLoop++)
            {
                dopic = lstDOPResult[nLoop];

                doph = lstDOPH.Find(delegate(LG_DOPH dopoh)
                {
                    return (dopoh.c_nosup.Equals(dopic.Principal, StringComparison.OrdinalIgnoreCase) &&
                      dopoh.c_dono.Equals(dopic.DO, StringComparison.OrdinalIgnoreCase));
                });

                if (doph != null)
                {
                    lstDOPH.Remove(doph);
                }
            }

            lstDOPResult.Clear();
          }

          lstDOPDetailCheck.Clear();
        }

        #endregion

        if (lstDOPH.Count > 0)
        {
          db.LG_DOPHs.InsertAllOnSubmit(lstDOPH.ToArray());
          lstDOPH.Clear();
        }

        if (lstDOPD.Count > 0)
        {
          db.LG_DOPDs.InsertAllOnSubmit(lstDOPD.ToArray());
          lstDOPD.Clear();
        }
      }
    }

    static void TestReadData()
    {
      string targetFile = @"D:\Rudi\SampelData\Data DO Pharos\20120416.ZIP";
      string folderTarget = @"D:\Tes";
      byte[] buf = null;
      int fileLen = 0;
      System.Data.DataSet dataset = null;

      BufferManager bufMan = BufferManager.CreateBufferManager(long.MaxValue, 1024);

      System.IO.FileStream fs = new System.IO.FileStream(targetFile, System.IO.FileMode.Open);

      fileLen = (int)fs.Length;

      buf = bufMan.TakeBuffer(fileLen);      

      fs.Read(buf, 0, fileLen);
     
      fs.Close();
      fs.Dispose();

      dataset = SaveToTemplateAndExtract(folderTarget, buf);
      
      bufMan.ReturnBuffer(buf);
      
      bufMan.Clear();

      Dictionary<string, string> dicMappingPrinsipal = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      dicMappingPrinsipal.Add("PT. NUTRISAINS", "00113");
      dicMappingPrinsipal.Add("PT. PRIMA MEDIKA LABORATORIES", "00117");
      dicMappingPrinsipal.Add("PT. NUTRINDO JAYA ABADI", "00112");
      dicMappingPrinsipal.Add("PT. PHAROS INDONESIA", "00001");
      dicMappingPrinsipal.Add("PT. APEX PHARMA INDONESIA", "00120");

      ORMDataContext db = new ORMDataContext();

      PopulateDoHeaderDetailPI(dataset, dicMappingPrinsipal, db);

      db.SubmitChanges();

      db.Dispose();

      GC.Collect();
    }

    #endregion

    #region DBF Send

    static System.Data.DataTable ReadAllActivePrincipal(ORMDataContext db)
    {
      System.Data.DataTable table = null;

      var qry = (from q in db.LG_DatsupEmails
                 join q1 in db.LG_DatSups on q.c_nosup equals q1.c_nosup
                 where (q.c_form == "02")
                 group new { q, q1 } by new { q.c_nosup, q1.v_nama, q1.v_alamat1, q1.v_alamat2 } into g                 
                 select new SuplierInformation()
                 {
                   KodeSuplier = g.Key.c_nosup,
                   NamaSuplier = g.Key.v_nama,
                   Alamat1 = g.Key.v_alamat1,
                   Alamat2 = g.Key.v_alamat2,
                   Emails = (from sq in db.LG_DatsupEmails
                             where (sq.c_nosup == g.Key.c_nosup)
                              && (sq.c_form == "02")
                             select sq.v_email).Distinct().ToArray()
                 }).Distinct().AsQueryable();

      table = qry.CopyToDataTableObject();

      return table;
    }

    static System.Data.DataSet CreateReaderSetPO(ORMDataContext db, string noSup, string PoNo)
    {
      System.Data.DataSet dataset = new System.Data.DataSet();
      System.Data.DataTable table = null;

      DateTime date = DateTime.Now;

      string groupPo = date.ToString("yyMMddHHmm");

      List<string> lstSentDO = null;
      List<POHeaderSenderFormat> lstHdr = null;
      List<PODetailSenderFormat> lstDtl = null;

      #region Header

      var qry = (from q in db.LG_POHs
                 join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
                 join q2 in db.LG_POD2s on new { q.c_gdg, q.c_pono } equals new { q2.c_gdg, q2.c_pono }
                 where (q.c_nosup == noSup)
                   //&& ((q.l_send.HasValue ? q.l_send.Value : false) == true)
                   //&& ((q.l_print.HasValue ? q.l_print.Value : false) == true)
                   //&& ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                   && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                   && ((string.IsNullOrEmpty(PoNo) ? q.c_pono : PoNo) == q.c_pono)
                   && (from sq in db.LG_ORHs
                       where (sq.c_gdg == q.c_gdg)
                          && ((sq.l_delete.HasValue ? sq.l_delete.Value : false) == false)
                        select sq.c_orno).Contains(q2.c_orno)
                 select new
                 {
                   q,
                   q1
                 }).Distinct().AsQueryable();

      #region Old Coded

      //var qry = (from q in db.LG_POHs
      //           join q1 in db.LG_POD1s on new { q.c_gdg, q.c_pono } equals new { q1.c_gdg, q1.c_pono }
      //           join q2 in db.LG_POD2s on new { q.c_gdg, q.c_pono } equals new { q2.c_gdg, q2.c_pono } into q_2
      //           from qPOD2 in q_2.DefaultIfEmpty()
      //           join q3 in db.LG_ORD2s on new { q.c_gdg, qPOD2.c_orno } equals new { q3.c_gdg, q3.c_orno } into q_3
      //           from qORD2 in q_3.DefaultIfEmpty()
      //           join q4 in db.LG_ORHs on new { qORD2.c_gdg, qORD2.c_orno } equals new { q4.c_gdg, q4.c_orno } into q_4
      //           from qORH in q_4.DefaultIfEmpty()
      //           join q5 in db.LG_SPHs on qORD2.c_spno equals q5.c_spno into q_5
      //           from qSPH in q_5.DefaultIfEmpty()
      //           join q6 in db.LG_Cusmas on qSPH.c_cusno equals q6.c_cusno into q_6
      //           from qCus in q_6.DefaultIfEmpty()
      //           where (q.c_nosup == noSup)
      //           && ((q.l_send.HasValue ? q.l_send.Value : false) == true)
      //           && ((q.l_print.HasValue ? q.l_print.Value : false) == true)
      //           && ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
      //           && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //           && ((string.IsNullOrEmpty(PoNo) ? q.c_pono : PoNo) == q.c_pono)
      //           && (((qORH != null) ?
      //           (qORH.l_delete.HasValue ? qORH.l_delete.Value : false) : false) == false)
      //             && (((qSPH != null) ?
      //             (qSPH.l_delete.HasValue ? qSPH.l_delete.Value : false) : false) == false)
      //               && (((qCus != null) ?
      //               (qCus.l_stscus.HasValue ? qCus.l_stscus.Value : false) : true) == true)
      //           select new
      //           {
      //             q,
      //             q1,
      //             qCus
      //           }).Distinct().AsQueryable();

      #endregion

      lstSentDO = (from q in qry
                   group q by new { q.q.c_pono } into g
                   select (g.Key.c_pono == null ? string.Empty : g.Key.c_pono)).Distinct().ToList();

      lstHdr = new List<POHeaderSenderFormat>();
      lstHdr.Add(new POHeaderSenderFormat()
      {
        C_CORNO = groupPo,
        D_CORDA = date,
        C_KOMEN1 = "Group PO",
        C_KOMEN2 = string.Empty,
        C_KDDEPO = string.Empty,
        L_LOAD = true,
        C_KDCAB = "HO",
        C_NMCAB = "Head Office"                  
      });

      //lstHdr = (from q in qry
      //          select new POHeaderSenderFormat()
      //          {
      //            //C_CORNO = (string.IsNullOrEmpty(q.q.c_pono) ? "" : q.q.c_pono.Substring(2, 8)),
      //            C_CORNO = groupPo,
      //            //D_CORDA = (q.q.d_podate.HasValue ? q.q.d_podate.Value : ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime),
      //            D_CORDA = date,
      //            C_KOMEN1 = "Group PO",
      //            C_KOMEN2 = string.Empty,
      //            C_KDDEPO = string.Empty,
      //            L_LOAD = true,
      //            C_KDCAB = "HO",
      //            C_NMCAB = "Head Office"
      //          }).Distinct().ToList();

      table = lstHdr.CopyToDataTableObject();
      table.TableName = DEFAULT_NAME_TABEL_PO_HEADER;

      dataset.Tables.Add(table);

      #endregion

      if (lstSentDO.Count > 0)
      {
        #region Detail

        var qryDtlSub = (from q in qry
                         join q7 in db.FA_MasItms on q.q1.c_iteno equals q7.c_iteno into q_7
                         from qItm in q_7.DefaultIfEmpty()
                         select new
                         {
                           q,
                           q.q1,
                           qItm
                         }).Distinct().AsQueryable();

        #region Old Coded

        //var qryDtlSub = (from q in qry
        //                 join q7 in db.FA_MasItms on q.q1.c_iteno equals q7.c_iteno into q_7
        //                 from qItm in q_7.DefaultIfEmpty()
        //                 join q8 in db.LG_Vias on new { q.qCus.c_cusno, qItm.c_iteno } equals new { q8.c_cusno, q8.c_iteno } into q_8
        //                 from qViaCus in q_8.DefaultIfEmpty()
        //                 join q9 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = qViaCus.c_via } equals new { q9.c_portal, q9.c_notrans, q9.c_type } into q_9
        //                 from qViaCusDesc in q_9.DefaultIfEmpty()
        //                 select new
        //                 {
        //                   q,
        //                   q.q1,
        //                   //q.qCus,
        //                   qItm,
        //                   //qViaCus,
        //                   qViaCusDesc
        //                 }).Distinct().AsQueryable();

        #endregion

        lstDtl = (from q in qryDtlSub
                  join q10 in db.MsTransDs on new { c_portal = '3', c_notrans = "02", c_type = q.qItm.c_via } equals new { q10.c_portal, q10.c_notrans, q10.c_type } into q_10
                  from qViaItmDesc in q_10.DefaultIfEmpty()
                  group q by new { q.q1.c_iteno, q.qItm.c_itenopri, q.qItm.v_itnam, q.qItm.v_undes, q.qItm.n_salpri, qViaItmDesc.v_ket } into g
                  select new PODetailSenderFormat()
                  {
                    //c_corno = (string.IsNullOrEmpty(q.q.q.c_pono) ? "" : q.q.q.c_pono.Substring(2, 8)),
                    c_corno = groupPo,
                    c_iteno = g.Key.c_iteno, //q.q1.c_iteno,
                    c_itenopri = g.Key.c_itenopri, //q.qItm.c_itenopri,
                    c_itnam = g.Key.v_itnam, //q.qItm.v_itnam,
                    c_nosp = string.Empty,
                    c_undes = g.Key.v_undes, //q.qItm.v_undes,
                    //c_via = ((qViaItmDesc != null) ? 
                    //           (string.IsNullOrEmpty(qViaItmDesc.v_ket) ? "Darat" : qViaItmDesc.v_ket) :"Darat").Substring(0, 1).ToUpper(),
                    c_via = (g.Key.v_ket == null ? "Darat" : g.Key.v_ket).Substring(0, 1).ToUpper(),
                    n_qty = g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)), //(q.q1.n_qty.HasValue ? q.q1.n_qty.Value : 0),
                    n_salpri = (g.Key.n_salpri.HasValue ? g.Key.n_salpri.Value : 0), //(q.qItm.n_salpri.HasValue ? q.qItm.n_salpri.Value : 0)
                  }).Distinct().ToList();

        table = lstDtl.CopyToDataTableObject();
        table.TableName = DEFAULT_NAME_TABEL_PO_DETAIL;

        dataset.Tables.Add(table);

        #endregion

        #region All PO Sended

        if (lstSentDO.Count > 0)
        {
          table = new System.Data.DataTable(DEFAULT_NAME_TABEL_PO_LIST);

          table.Columns.Add("PO", typeof(string));

          table.BeginLoadData();

          for (int nLoop = 0, nLen = lstSentDO.Count; nLoop < nLen; nLoop++)
          {
            table.LoadDataRow(new object[] { lstSentDO[nLoop] }, true);
          }

          table.EndLoadData();

          dataset.Tables.Add(table);
        }

        #region Old Coded

        //if ((lstHdr != null)  && (lstHdr.Count > 0))
        //{
        //  List<string> lstTotalPO = lstHdr.GroupBy(x => x.C_CORNO).Select(y => string.Concat("PO", y.Key)).ToList();

        //  if ((lstTotalPO != null) && (lstTotalPO.Count > 0))
        //  {
        //    table = new System.Data.DataTable(DEFAULT_NAME_TABEL_PO_LIST);

        //    table.Columns.Add("PO", typeof(string));

        //    table.BeginLoadData();

        //    for (int nLoop = 0, nLen = lstTotalPO.Count; nLoop < nLen; nLoop++)
        //    {
        //      table.LoadDataRow(new object[] { lstTotalPO[nLoop] }, true);
        //    }

        //    table.EndLoadData();

        //    dataset.Tables.Add(table);

        //    lstTotalPO.Clear();
        //  }
        //}

        #endregion

        #endregion

        lstSentDO.Clear();
        lstDtl.Clear();
        lstHdr.Clear();
      }
     
      return dataset;
    }

    static string DbfColumnParser(System.Data.DataColumn column)
    {
      string rets = null;

      if (column.DataType.Equals(typeof(DateTime)))
      {
        rets = string.Format("[{0}] datetime {1}", 
          column.ColumnName,
          (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }
      else if (column.DataType.Equals(typeof(float)) ||
        column.DataType.Equals(typeof(double)) ||
        column.DataType.Equals(typeof(decimal)))
      {
        rets = string.Format("[{0}] numeric (18,2) {1}",
          column.ColumnName,
          (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }
      else if (column.DataType.Equals(typeof(ushort)) ||
        column.DataType.Equals(typeof(short)) ||
        column.DataType.Equals(typeof(uint)) ||
        column.DataType.Equals(typeof(int)) ||
        column.DataType.Equals(typeof(ulong)) ||
        column.DataType.Equals(typeof(long)))
      {
        rets = string.Format("[{0}] int {1}",
          column.ColumnName,
          (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }
      else if (column.DataType.Equals(typeof(bool)))
      {
        rets = string.Format("[{0}] bit {1}",
          column.ColumnName,
          (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }
      //else if (column.DataType.Equals(typeof(byte)) ||
      //  column.DataType.Equals(typeof(sbyte)) ||
      //  column.DataType.Equals(typeof(char)))
      //{
      //  rets = string.Format("[{0}] char(254) {1}",
      //    column.ColumnName,
      //    (column.AllowDBNull ? "NULL" : "NOT NULL"));
      //}
      //else
      //{
      //  rets = string.Format("[{0}] text {1}",
      //    column.ColumnName,
      //    (column.AllowDBNull ? "NULL" : "NOT NULL"));
      //}
      else
      {
        rets = string.Format("[{0}] char(254) {1}",
          column.ColumnName,
          (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }

      return rets;
    }

    //static string CreateDBF(System.Data.DataTable table, string pathName)
    //{
    //  if (!System.IO.Directory.Exists(pathName) || (table == null))
    //  {
    //    return null;
    //  }

    //  string reslt = null;

    //  Random rnd = new Random((int)DateTime.Now.Ticks);

    //  string tableFileName = string.Empty,
    //    tmp = null;

    //  do
    //  {
    //    //tableFileName = string.Concat("T", rnd.Next(int.MinValue, int.MaxValue).ToString("X08"));
    //    tableFileName = string.Concat("T", rnd.Next(0, (int)short.MaxValue).ToString("X04"));
    //    tmp = System.IO.Path.Combine(pathName, tableFileName);
    //  } while (System.IO.File.Exists(tmp));

    //  StringBuilder sb = new StringBuilder();
    //  string pathNameFull = System.IO.Path.Combine(pathName, tableFileName);
    //  string fileDbfName = (tableFileName.EndsWith(".dbf", StringComparison.OrdinalIgnoreCase) ? tableFileName : string.Concat(tableFileName, ".dbf"));
    //  string pathNameFullDbf = (tableFileName.EndsWith(".dbf", StringComparison.OrdinalIgnoreCase) ? pathNameFull : string.Concat(pathNameFull, ".dbf"));

    //  System.Data.OleDb.OleDbConnection con = null;
    //  System.Data.OleDb.OleDbCommand cmd = null;
    //  System.Data.OleDb.OleDbDataReader odbReader = null;
    //  System.Data.OleDb.OleDbDataAdapter adapt = null;
    //  System.Data.OleDb.OleDbCommandBuilder odbCmdBuild = null;
    //  System.Data.DataColumn col = null;
    //  System.Data.DataRow row = null;

    //  int nLoop = 0,
    //    nLen = 0,
    //    nLoopC = 0,
    //    nLenC = 0;

    //  bool bData = false;

    //  try
    //  {
    //    if (System.IO.File.Exists(pathNameFull))
    //    {
    //      System.IO.File.Delete(pathNameFull);
    //    }
    //    else if (System.IO.File.Exists(pathNameFullDbf))
    //    {
    //      System.IO.File.Delete(pathNameFullDbf);
    //    }

    //    con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties=dBASE IV;", pathName));
    //    con.Open();

    //    if (con.State == System.Data.ConnectionState.Open)
    //    {
    //      #region Create Table

    //      cmd = con.CreateCommand();

    //      sb.AppendFormat("CREATE TABLE {0} (", tableFileName);

    //      for (nLoop = 0, nLen = table.Columns.Count; nLoop < nLen; nLoop++)
    //      {
    //        if ((nLoop + 1) >= nLen)
    //        {
    //          sb.AppendFormat(" {0}", DbfColumnParser(table.Columns[nLoop]));
    //        }
    //        else
    //        {
    //          sb.AppendFormat(" {0},", DbfColumnParser(table.Columns[nLoop]));
    //        }
    //      }          

    //      sb.Append(" )");

    //      cmd.CommandText = sb.ToString();
    //      cmd.ExecuteNonQuery();

    //      cmd.Dispose();

    //      sb.Remove(0, sb.Length);

    //      #endregion

    //      #region Populate Data

    //      cmd = con.CreateCommand();

    //      //cmd.CommandText = string.Format("SELECT * FROM {0}", fileDbfName);

    //      //adapt = new System.Data.OleDb.OleDbDataAdapter(cmd);

    //      //odbCmdBuild = new System.Data.OleDb.OleDbCommandBuilder(adapt);
          
    //      //adapt.Update(table);
          
    //      nLenC = table.Columns.Count;

    //      for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
    //      {
    //        col = table.Columns[nLoopC];

    //        reslt = string.Concat(reslt, ",", col.ColumnName);
    //      }

    //      reslt = (reslt.StartsWith(",", StringComparison.OrdinalIgnoreCase) ?
    //        reslt.Remove(0, 1) : reslt);

    //      for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
    //      {
    //        row = table.Rows[nLoop];

    //        sb.AppendFormat("Insert Into {0} ({1}) Values (", tableFileName, reslt);

    //        for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
    //        {
    //          col = table.Columns[nLoopC];

    //          if (col.DataType.Equals(typeof(DateTime)))
    //          {
    //            sb.AppendFormat("'{0}' ,", row.GetValue<DateTime>(col, ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime).ToString("yyyy-MM-dd"));
    //          }
    //          else if (col.DataType.Equals(typeof(float)) ||
    //            col.DataType.Equals(typeof(double)) ||
    //            col.DataType.Equals(typeof(decimal)))
    //          {
    //            sb.AppendFormat("{0} ,", row.GetValue<decimal>(col, 0));
    //          }
    //          else if (col.DataType.Equals(typeof(ushort)) ||
    //            col.DataType.Equals(typeof(short)) ||
    //            col.DataType.Equals(typeof(uint)) ||
    //            col.DataType.Equals(typeof(int)) ||
    //            col.DataType.Equals(typeof(ulong)) ||
    //            col.DataType.Equals(typeof(long)))
    //          {
    //            sb.AppendFormat("{0} ,", row.GetValue<decimal>(col, 0));
    //          }
    //          else if (col.DataType.Equals(typeof(bool)))
    //          {
    //            bData = row.GetValue<bool>(col, false);
    //            sb.AppendFormat("{0} ,", (bData ? 1 : 0));
    //          }
    //          //else if (col.DataType.Equals(typeof(byte)) ||
    //          //  col.DataType.Equals(typeof(sbyte)) ||
    //          //  col.DataType.Equals(typeof(char)))
    //          //{
    //          //  sb.AppendFormat("{0} ,", row.GetValue<decimal>(col, 0));
    //          //}
    //          else
    //          {
    //            sb.AppendFormat("'{0}' ,", row.GetValue<string>(col, string.Empty));
    //          }
    //        }
            
    //        sb.Remove(sb.Length - 1, 1);

    //        sb.AppendLine(" ) ");

    //        cmd.CommandText = sb.ToString();

    //        cmd.ExecuteNonQuery();

    //        sb.Remove(0, sb.Length);
    //      }

    //      ////sb.Append(";");

    //      //cmd.CommandText = sb.ToString();

    //      //cmd.ExecuteNonQuery();

    //      #endregion
          
    //      if (System.IO.File.Exists(pathNameFull))
    //      {
    //        reslt = pathNameFull;
    //      }
    //      else if (System.IO.File.Exists(pathNameFullDbf))
    //      {
    //        reslt = pathNameFullDbf;
    //      }
    //      else
    //      {
    //        reslt = string.Empty;
    //      }
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    reslt = null;

    //    table = null;

    //    Logger.WriteLine(ex.Message);
    //  }
    //  finally
    //  {
    //    if (odbCmdBuild != null)
    //    {
    //      odbCmdBuild.Dispose();
    //    }

    //    if (adapt != null)
    //    {
    //      adapt.Dispose();
    //    }

    //    if (odbReader != null)
    //    {
    //      odbReader.Close();
    //      odbReader.Dispose();
    //    }

    //    if (cmd != null)
    //    {
    //      con.Dispose();
    //    }

    //    if (con != null)
    //    {
    //      if (con.State == System.Data.ConnectionState.Open)
    //      {
    //        con.Close();
    //      }
    //      con.Dispose();
    //    }
    //  }

    //  return reslt;
    //}

    //static System.IO.MemoryStream CreateDBFStream(System.Data.DataTable table, string pathName)
    //{      
    //  string file = CreateDBF(table, pathName);

    //  System.IO.MemoryStream mem = null;
    //  System.IO.FileStream fs = null;
    //  bool isReaded = false;

    //  if (!string.IsNullOrEmpty(file))
    //  {
    //    if (System.IO.File.Exists(file))
    //    {
    //      try
    //      {
    //        fs = System.IO.File.OpenRead(file);

    //        mem = new System.IO.MemoryStream();

    //        mem.SetLength(fs.Length);
            
    //        fs.Read(mem.GetBuffer(), 0, (int)fs.Length);

    //        isReaded = true;

    //        if (fs != null)
    //        {
    //          fs.Close();
    //          fs.Dispose();
    //          fs = null;
    //        }

    //        System.IO.File.Delete(file);
    //      }
    //      catch (Exception ex)
    //      {
    //        Logger.WriteLine(ex.Message);
    //      }
    //      finally
    //      {
    //        if (fs != null)
    //        {
    //          fs.Close();
    //          fs.Dispose();
    //        }

    //        if ((!isReaded) && (mem != null))
    //        {
    //          mem.Close();
    //          mem.Dispose();
    //          mem = null;
    //        }
    //      }
    //    }
    //  }

    //  return mem;
    //}

    static string PadLeftRight(string dataString, int lenPad, char leftChar, char rightChar)
    {
      string result = null;
      int lenData = dataString.Length,
        subData = (lenData >= lenPad ? 0 : ((lenPad - lenData) / 2));

      result = string.Concat("".PadLeft(subData, leftChar), dataString, "".PadLeft(subData, rightChar));

      return result;
    }

    static Dictionary<string, string> PrintPage(System.Data.DataRow rowSupInfo, System.Data.DataTable tableHeader, System.Data.DataTable tableDetail)
    {
      if ((rowSupInfo == null) || (tableHeader == null) || (tableDetail == null))
      {
        return null;
      }

      StringBuilder sbHeader = new StringBuilder();
      StringBuilder sbHeaderData = new StringBuilder();
      StringBuilder sbHeaderColumn = new StringBuilder();
      StringBuilder sbListData = new StringBuilder();
      StringBuilder sbFooter = new StringBuilder();
      StringBuilder sbFooterData = new StringBuilder();
      StringBuilder sb = new StringBuilder();

      //List<string> listDataPrinted = new List<string>();
      Dictionary<string, string> dataPrinting = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      int widthPage = 80;

      int hdrPading = 40,
        colProd = 36,
        colKemasan = 13,
        colQty = 10,
        colTotal = 21,
        footApj = 28,
        footAcc = 26,
        footLog = 26;

      int nLoop = 0,
        nLen = 0,
        nLoopC = 0,
        nLenC = 0;

      System.Data.DataRow rowHdr = null,
        rowDtl = null;

      System.Data.DataView viewDtl = null;

      string tmp = null,
        primaryId = null;

      decimal decQty = 0, decHna = 0,
        calcQtyHna = 0,
        sumCalcQtyHna = 0;

      #region Header

      tmp = "PT ANTARMITRA SEMBADA (PUSAT)";
      sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Kepada Yth,"));
      tmp = "JL. POS PENGUMBEN RAYA NO.8";
      sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "NUTRINDO JAYA"));
      tmp = "KEBON JERUK, JAKARTA 11560";
      sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Ruko Taman Kebon Jeruk"));
      tmp = "JAKARTA";
      sbHeader.AppendLine(string.Concat(tmp.PadRight(hdrPading), "Meruya Utara,Kembangan"));

      sbHeader.AppendLine();

      sbHeader.AppendLine(PadLeftRight("P U R C H A S E  O R D E R", widthPage, ' ', ' '));

      #endregion

      #region Column Header

      tmp = "=";
      sbHeaderColumn.AppendLine(tmp.PadRight(widthPage, '='));
      tmp = "Product";
      sbHeaderColumn.Append(PadLeftRight(tmp, colProd, ' ', ' '));
      tmp = "Kemasan";
      sbHeaderColumn.Append(PadLeftRight(tmp, colKemasan, ' ', ' '));
      tmp = "Jumlah";
      sbHeaderColumn.Append(PadLeftRight(tmp, colQty, ' ', ' '));
      tmp = "Rp.";
      sbHeaderColumn.AppendLine(PadLeftRight(tmp, colTotal, ' ', ' '));
      tmp = "=";
      sbHeaderColumn.AppendLine(tmp.PadRight(widthPage, '='));

      #endregion

      #region Footer

      tmp = "A A Penanggung Jawab";
      sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));
      tmp = "Menyetujui";
      sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
      tmp = "LOGISTIK";
      sbFooter.AppendLine(PadLeftRight(tmp, footLog, ' ', ' '));

      sbFooter.AppendLine();
      sbFooter.AppendLine();
      sbFooter.AppendLine();

      tmp = "IIS SETIAWATI";
      sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));
      tmp = "TJETJENG HERJADI";
      sbFooter.Append(PadLeftRight(tmp, footAcc, ' ', ' '));
      tmp = "NIKEN PRISCILIA";
      sbFooter.AppendLine(PadLeftRight(tmp, footLog, ' ', ' '));

      tmp = "SIK KP.01.03.1.3.150";
      sbFooter.Append(PadLeftRight(tmp, footApj, ' ', ' '));

      #endregion

      for (nLoop = 0, nLen = tableHeader.Rows.Count; nLoop < nLen; nLoop++)
      {
        rowHdr = tableHeader.Rows[nLoop];

        #region Header Data

        primaryId = tmp = rowHdr.GetValue<string>("C_CORNO", string.Empty);
        sbHeaderData.Append(string.Format("NO : {0}", tmp.PadRight(16)));
        tmp = rowHdr.GetValue<DateTime>("D_CORDA", ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime).ToString("dd-MM-yyyy");
        sbHeaderData.AppendLine(string.Format("TGL : {0}", tmp));
        tmp = rowHdr.GetValue<string>("C_NMCAB", string.Empty);
        sbHeaderData.AppendLine((string.IsNullOrEmpty(tmp) ? null : string.Concat("PESANAN DI KIRIM KE ", tmp)));

        #endregion

        #region Populate Data

        sumCalcQtyHna = 0;

        viewDtl = new System.Data.DataView(tableDetail, string.Format("c_corno = '{0}'", primaryId), null, System.Data.DataViewRowState.CurrentRows);

        for (nLoopC = 0, nLenC = viewDtl.Count; nLoopC < nLenC; nLoopC++)
        {
          rowDtl = viewDtl[nLoopC].Row;

          tmp = rowDtl.GetValue<string>("c_itnam", string.Empty);
          sbListData.Append(tmp.PadRight(colProd));
          tmp = rowDtl.GetValue<string>("c_undes", string.Empty);
          sbListData.Append(tmp.PadRight(colKemasan));

          decQty = rowDtl.GetValue<decimal>("n_qty", 0);
          sbListData.Append(decQty.ToString("N2").PadLeft(colQty));

          decHna = rowDtl.GetValue<decimal>("n_salpri", 0);
          calcQtyHna = (decQty * decHna);
          sbListData.AppendLine(calcQtyHna.ToString("N2").PadLeft(colTotal));

          sumCalcQtyHna += calcQtyHna;
        }

        viewDtl.Dispose();

        #endregion

        #region Footer Data

        tmp = "=";
        sbFooterData.AppendLine(tmp.PadRight(widthPage, '='));
        tmp = "TOTAL : ";
        sbFooterData.Append(tmp.PadLeft(60));
        //tmp = "119,316,000";
        tmp = sumCalcQtyHna.ToString("N2");
        sbFooterData.AppendLine(tmp.PadLeft(20));
        tmp = "=";
        sbFooterData.AppendLine(tmp.PadRight(widthPage, '='));

        #endregion

        #region Combine

        sb.Append(sbHeader.ToString());

        sb.AppendLine();

        sb.Append(sbHeaderData.ToString());
        sb.Append(sbHeaderColumn.ToString());

        sb.AppendLine();

        sb.Append(sbListData.ToString());

        sb.AppendLine();

        sb.Append(sbFooterData.ToString());
        sb.Append(sbFooter.ToString());

        #endregion

        //listDataPrinted.Add(sb.ToString());
        if (!dataPrinting.ContainsKey(primaryId))
        {
          dataPrinting.Add(primaryId, sb.ToString());
        }

        #region Clear

        sbHeader.Remove(0, sbHeader.Length);
        sbHeaderColumn.Remove(0, sbHeaderColumn.Length);
        sbListData.Remove(0, sbListData.Length);
        sbFooter.Remove(0, sbFooter.Length);
        sbFooterData.Remove(0, sbFooterData.Length);
        sb.Remove(0, sb.Length);

        #endregion
      }

      return dataPrinting;
    }

    static bool SendMail(System.Data.DataRow rowSuplInfo, System.IO.MemoryStream msHeader, System.IO.MemoryStream msDetail, Dictionary<string, string> dicPrinting)
    {
      if ((rowSuplInfo == null) || (msHeader == null) || (msHeader == null) || (dicPrinting == null))
      {
        return false;
      }

      bool bOk = false;

      //System.IO.MemoryStream mem = null;
      System.Net.Mail.MailMessage mail = null;
      System.Net.Mail.SmtpClient smtp = null;

      Encoding utf8 = Encoding.UTF8;

      int nLoop= 0, nLen = 0;

      string namaSupl = rowSuplInfo.GetValue<string>("NamaSuplier", string.Empty),
        tmp = null;
      string[] emailUsers = rowSuplInfo.GetValue<string[]>("Emails", new string[0]);
      StringBuilder sb = new StringBuilder();
      
      //List<System.IO.MemoryStream> lstMemStream = new List<System.IO.MemoryStream>();

      ////byte[] byt = null;
      //System.Net.Mail.Attachment attach = null;
      //System.Net.Mime.ContentDisposition contDisp = null;
      //System.Net.Mime.ContentType contType = null;

      if ((emailUsers != null) && (emailUsers.Length > 0))
      {
        mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("scms@ams.co.id", "Supply Chain Management System");

        mail.Subject = string.Concat("Permintaan Untuk - ", namaSupl);

        for (nLoop = 0, nLen = emailUsers.Length; nLoop < nLen; nLoop++)
        {
          mail.To.Add(emailUsers[nLoop]);
        }

        for (nLoop = 0, nLen = 10; nLoop < nLen; nLoop++)
        {
          sb.Append("Dummy ");
        }

        sb.AppendLine();
        sb.AppendLine();

        sb.AppendLine("Pengiriman PO :");

        //nLen = (int)msHeader.Length;
        //attach = new System.Net.Mail.Attachment(msHeader, "header.poh");
        //contDisp = attach.ContentDisposition;
        //contDisp.CreationDate =
        //  contDisp.ModificationDate =
        //   contDisp.ReadDate = DateTime.Now;
        ////contDisp.Inline = true;
        //contDisp.DispositionType = System.Net.Mime.MediaTypeNames.Application.Octet;
        //contDisp.FileName = "header.poh";
        //contDisp.Size = nLen;
        //mail.Attachments.Add(attach);

        ////System.IO.FileStream fs = new System.IO.FileStream("C:\\Output.dbf", System.IO.FileMode.CreateNew);
        ////msHeader.WriteTo(fs);

        ////fs.Close();
        ////fs.Dispose();

        //contType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Octet);
        //contType.Name = "header.zip";
        //mail.Attachments.Add(new System.Net.Mail.Attachment(msHeader, contType));

        //mail.Attachments.Add(new System.Net.Mail.Attachment(msDetail, "detail.pod"));

        mail.Attachments.Add(new System.Net.Mail.Attachment(msHeader, "header.sp1",
          System.Net.Mime.MediaTypeNames.Application.Octet));
        mail.Attachments.Add(new System.Net.Mail.Attachment(msDetail, "detail.sp2",
          System.Net.Mime.MediaTypeNames.Application.Octet));

        nLoop = 1;

        foreach(KeyValuePair<string, string> kvp in dicPrinting)
        {
          tmp = kvp.Key;
          sb.AppendLine(string.Concat(nLoop, ". ", tmp));

          #region Old Coded

          //nLen = kvp.Value.Length;
          //byt = new byte[nLen];

          //byt = utf8.GetBytes(kvp.Value);

          //mem = new System.IO.MemoryStream();
          //mem.Write(byt, 0, kvp.Value.Length);
          //mem.Position = 0;

          //mail.Attachments.Add(new System.Net.Mail.Attachment(mem, string.Concat(tmp, ".txt")));

          #endregion

          mail.Attachments.Add(System.Net.Mail.Attachment.CreateAttachmentFromString(kvp.Value, string.Concat(tmp, ".txt")));

          nLoop++;
        }
        
        sb.AppendLine();
        sb.AppendLine();

        sb.AppendLine("Mohon untuk diperiksa dan diproses.");

        mail.Body = sb.ToString();

        smtp = new System.Net.Mail.SmtpClient("10.100.10.9", 25);

        try
        {
          smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
          smtp.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
          smtp.Send(mail);

          bOk = true;
        }
        catch (Exception ex)
        {
          Logger.WriteLine(ex.Message);
          Logger.WriteLine(ex.StackTrace);
        }

        //for (nLoop = 0, nLen = lstMemStream.Count; nLoop < nLen; nLoop++)
        //{
        //  mem = lstMemStream[nLoop];

        //  if (mem != null)
        //  {
        //    mem.Close();
        //    mem.Dispose();
        //  }
        //}
      }

      return bOk;
    }

    static void UpdateAllToTable(ORMDataContext db, System.Data.DataTable table)
    {
      int nLoop = 0,
        nLen = 0;

      System.Data.DataRow row = null;

      string poNumber = null;

      List<string> lstPO = new List<string>();

      List<LG_POH> lstPOH = null;
      
      for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
      {
        row = table.Rows[nLoop];

        poNumber = row.GetValue<string>("PO", string.Empty);

        if (!string.IsNullOrEmpty(poNumber))
        {
          if (!lstPO.Contains(poNumber))
          {
            lstPO.Add(poNumber);
          }
        }
      }

      if (lstPO.Count > 0)
      {
        lstPOH = (from q in db.LG_POHs
                  where lstPO.Contains(q.c_pono)
                    && ((q.l_sent.HasValue ? q.l_sent.Value : false) == false)
                  select q).Distinct().ToList();

        if (lstPOH != null)
        {
          try
          {
            for (nLoop = 0, nLen = lstPOH.Count; nLoop < nLen; nLoop++)
            {
              lstPOH[nLoop].l_sent = true;
            }

            db.SubmitChanges();
          }
          catch (Exception ex)
          {
            Logger.WriteLine(ex.Message);
            Logger.WriteLine(ex.StackTrace);
          }
          finally
          {
            lstPOH.Clear();
          }
        }

        lstPO.Clear();
      }
    }

    static void ReadingFromDB(string pathFolder)
    {
      ORMDataContext db = new ORMDataContext();

      System.Data.DataTable tableMailSuplier = null;

      tableMailSuplier = ReadAllActivePrincipal(db);

      System.Data.DataView view = new System.Data.DataView(tableMailSuplier, null, "KodeSuplier", System.Data.DataViewRowState.CurrentRows);
      
      System.Data.DataTable table = null,
        tableHeader = null, tableDetail = null;
      System.IO.MemoryStream msHeader = null,
        msDetail = null;

      System.Data.DataSet dataset = null;

      System.Data.DataRow row = null;

      int nLoop = 0,
        nLen = 0;

      string kodeSupl = null;

      Dictionary<string, string> dicPrinting = null;

      for (nLoop = 0, nLen = view.Count; nLoop < nLen; nLoop++)
      {
        row = view[nLoop].Row;

        kodeSupl = row.GetValue<string>("KodeSuplier", string.Empty);

        if (!string.IsNullOrEmpty(kodeSupl))
        {
          dataset = CreateReaderSetPO(db, kodeSupl, null);

          if (dataset != null)
          {
            if (dataset.Tables.Count == 3)
            {
              tableHeader = dataset.Tables[DEFAULT_NAME_TABEL_PO_HEADER];
              msHeader = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableHeader, pathFolder);

              tableDetail = dataset.Tables[DEFAULT_NAME_TABEL_PO_DETAIL];
              msDetail = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableDetail, pathFolder);
              
              dicPrinting = PrintPage(row, tableHeader, tableDetail);

              if (SendMail(row, msHeader, msDetail, dicPrinting))
              {
                table = dataset.Tables[DEFAULT_NAME_TABEL_PO_LIST];

                UpdateAllToTable(db, table);

                if (table != null)
                {
                  table.Clear();
                  table.Dispose();
                }
              }

              if (tableHeader != null)
              {
                tableHeader.Clear();
                tableHeader.Dispose();
              }

              if (tableDetail != null)
              {
                tableDetail.Clear();
                tableDetail.Dispose();
              }
            }

            dataset.Clear();
            dataset.Dispose();
          }
        }
      }

      if (view != null)
      {
        view.Dispose();
      }

      if (tableMailSuplier != null)
      {
        tableMailSuplier.Clear();
        tableMailSuplier.Dispose();
      }

      db.Dispose();
    }

    static void TestingCreateDBF()
    {
      string pathFolder = @"D:\Rudi\AmsSource\Scms\ScmsSoaTester\bin\Debug\Temp\extract";

      #region Old Coded

      //System.Data.DataTable table = new System.Data.DataTable("test");
      //table.Columns.Add("Col1", typeof(string));
      //table.Columns.Add("Col2", typeof(int));
      //table.Columns.Add("Col3", typeof(long));
      //table.Columns.Add("Col4", typeof(float));
      //table.Columns.Add("Col5", typeof(double));
      //table.Columns.Add("Col6", typeof(decimal));
      //table.Columns.Add("Col7", typeof(short));
      //table.Columns.Add("Col8", typeof(byte));
      //table.Columns.Add("Col9", typeof(ushort));
      //table.Columns.Add("Col10", typeof(uint));
      //table.Columns.Add("Col11", typeof(ulong));
      //table.Columns.Add("Col12", typeof(byte));
      //table.Columns.Add("Col13", typeof(bool));
      //table.Columns.Add("Col14", typeof(DateTime));

      //System.Data.DataRow row = null;
      //System.Data.DataColumn col = null;

      //Random rnd = new Random((int)DateTime.Today.Ticks);
      //byte[] buf = new byte[1];

      //for (int nLoop = 0, nLen = 100; nLoop < nLen; nLoop++)
      //{
      //  row = table.NewRow();

      //  for (int nLoopC = 0, nLenC = table.Columns.Count; nLoopC < nLenC; nLoopC++)
      //  {
      //    col = table.Columns[nLoopC];

      //    if (col.DataType.Equals(typeof(DateTime)))
      //    {
      //      row[col] = DateTime.Now;
      //    }
      //    else if (col.DataType.Equals(typeof(float)) ||
      //      col.DataType.Equals(typeof(double)) ||
      //      col.DataType.Equals(typeof(decimal)))
      //    {
      //      row[col] = rnd.NextDouble();
      //    }
      //    else if (col.DataType.Equals(typeof(ushort)) ||
      //      col.DataType.Equals(typeof(short)) ||
      //      col.DataType.Equals(typeof(uint)) ||
      //      col.DataType.Equals(typeof(int)) ||
      //      col.DataType.Equals(typeof(ulong)) ||
      //      col.DataType.Equals(typeof(long)))
      //    {
      //      row[col] = rnd.Next(0, 255);
      //    }
      //    else if (col.DataType.Equals(typeof(bool)))
      //    {
      //      row[col] = ((nLoop % 2) == 0);
      //    }
      //    else if (col.DataType.Equals(typeof(byte)) ||
      //      col.DataType.Equals(typeof(sbyte)) ||
      //      col.DataType.Equals(typeof(char)))
      //    {
      //      rnd.NextBytes(buf);
      //      row[col] = buf[0];
      //    }
      //    else
      //    {
      //      row[col] = string.Concat("Data : ", nLoop, ", Index : ", nLoopC);
      //    }
      //  }

      //  table.Rows.Add(row);
      //}

      #endregion

      ORMDataContext db = new ORMDataContext();

      System.Data.DataTable tableMailSuplier = null;

      tableMailSuplier = ReadAllActivePrincipal(db);

      System.Data.DataView view = new System.Data.DataView(tableMailSuplier, null, "KodeSuplier", System.Data.DataViewRowState.CurrentRows);

      System.Data.DataTable table = null,
        tableHeader = null, tableDetail = null;
      System.IO.MemoryStream msHeader = null,
        msDetail = null;

      System.Data.DataRow row = null;

      int nRow = view.Find("00001");
      if (nRow != -1)
      {
        row = view[nRow].Row;
      }

      System.Data.DataSet dataset = CreateReaderSetPO(db, "00001", null);

      tableHeader = dataset.Tables[DEFAULT_NAME_TABEL_PO_HEADER];
      msHeader = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableHeader, pathFolder);

      tableDetail =  dataset.Tables[DEFAULT_NAME_TABEL_PO_DETAIL];
      msDetail = ScmsMailLibrary.Core.Commons.CreateDBFStream(tableDetail, pathFolder);

      table = dataset.Tables[DEFAULT_NAME_TABEL_PO_LIST];

      Dictionary<string, string> dicPrinting = PrintPage(row, tableHeader, tableDetail);
      
      SendMail(row, msHeader, msDetail, dicPrinting);

      if (dicPrinting != null)
      {
        dicPrinting.Clear();
      }

      if (msHeader != null)
      {
        msHeader.Close();
        msHeader.Dispose();
      }

      if (msDetail != null)
      {
        msDetail.Close();
        msDetail.Dispose();
      }

      if (dataset != null)
      {
        dataset.Clear();
        dataset.Dispose();
      }
    }

    static void RunningCreateDBF()
    {
      string pathFolder = @"D:\Rudi\AmsSource\Scms\ScmsSoaTester\bin\Debug\Temp\extract";

      //ReadingFromDB(pathFolder);

      Config cfg = new Config();

      SendPOAuto spoa = new SendPOAuto(cfg);

      spoa.Testing();

      //if (spoa.Start())
      //{
      //  System.Threading.Thread.Sleep(1000000);
      //}

      //spoa.Stop();

      //spoa.Testing();

      //ORMDataContext db = new ORMDataContext();

      //CreateReaderSetPO(db, "00001", null);
    }

    #endregion

    #region Testing RPT

    static void CallRpt()
    {
      System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
      sw.Start();

      CRDatasetBind crdb = new CRDatasetBind();

      ORMDataContext db = new ORMDataContext();
      
      DateTime date1 = DateTime.Parse("2010-11-01"),
        date2 = DateTime.Parse("2012-11-30");

      System.Data.DataSet ds = crdb.ReportSales(db, date1, date2);

      LG_QuerySales lqs = new LG_QuerySales();
      
      lqs.SetDataSource(ds);

      lqs.RecordSelectionFormula = string.Empty;

      //lqs.VerifyDatabase();      

      lqs.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.Excel, "D:\\Eek.xls");

      lqs.Close();

      sw.Stop();
      Console.WriteLine(sw.Elapsed.TotalSeconds);
    }

    #endregion

    #region Testing Get Faktur Pajak

    static void GetFakturSP()
    {
      Service srvc = new Service();
      srvc.Testing();
    }

    #endregion

    #region Testing Post Data DO

    static void PostDataDO()
    {
      Service srvc = new Service();
      srvc.Testing();
    }

    #endregion

    #region Test Full Outer Join

    internal class TestFullOuterJoin
    {
      public string c_fjno { get; set; }
      public DateTime? d_fjdate { get; set; }
      public string c_dono { get; set; }
      public DateTime? d_dono { get; set; }
      public string c_cusno { get; set; }
      public string c_taxno { get; set; }
      public DateTime? d_taxdate { get; set; }
      public decimal? n_top { get; set; }
      public DateTime? d_top { get; set; }
      public decimal? n_toppjg { get; set; }
      public DateTime? d_toppjg { get; set; }
      public string c_kurs { get; set; }
      public decimal? n_kurs { get; set; }
      public string v_ket { get; set; }
      public decimal? n_gross { get; set; }
      public decimal? n_disc { get; set; }
      public decimal? n_tax { get; set; }
      public decimal? n_net { get; set; }
      public decimal? n_sisa { get; set; }
      public bool? l_cabang { get; set; }
      public bool? l_print { get; set; }
      public string c_entry { get; set; }
      public DateTime? d_entry { get; set; }
      public string c_update { get; set; }
      public DateTime? d_update { get; set; }
      public string c_iteno { get; set; }
      public decimal? n_qty { get; set; }
      public decimal? n_salpri { get; set; }
      public decimal? n_discon { get; set; }
    }

    static void TestingFullOuterJoin()
    {
      /*
       * SELECT a.c_fjno, a.d_fjdate, a.c_cusno, a.c_taxno, a.d_taxdate, a.n_top, a.d_top, a.n_toppjg, a.d_toppjg, a.c_kurs, a.n_kurs, a.v_ket, a.n_gross, 
          a.n_disc, a.n_tax, a.n_net, a.n_sisa, a.l_cabang, a.l_print, a.c_entry, a.d_entry, a.c_update, a.d_update, b.c_iteno, b.n_qty, b.n_salpri, 
          c.n_discon
         FROM dbo.LG_FJH AS a INNER JOIN
                      dbo.LG_FJD1 AS b ON a.c_fjno = b.c_fjno INNER JOIN
                      dbo.LG_FJD2 AS c ON b.c_fjno = c.c_fjno AND b.c_iteno = c.c_iteno
       */

      DateTime date1 = DateTime.Parse("2011-11-02"),
        date2 = DateTime.Parse("2011-11-02");

      ORMDataContext db = new ORMDataContext();

      var qqqKiri = (from q in db.LG_FJHs
                     join q1 in db.LG_FJD1s on q.c_fjno equals q1.c_fjno
                     join q2 in db.LG_FJD2s on new { q.c_fjno, q1.c_iteno } equals new { q2.c_fjno, q2.c_iteno }
                     where (q.d_fjdate >= date1) && (q.d_fjdate <= date2)
                      && (q.c_fjno == "FJ11110573")
                     select new TestFullOuterJoin()
                     {
                       c_fjno = q.c_fjno,
                       d_fjdate = q.d_fjdate,
                       c_dono = q.c_fjno,
                       d_dono = q.d_fjdate,
                       c_cusno = q.c_cusno,
                       c_taxno = q.c_taxno,
                       d_taxdate = q.d_taxdate,
                       n_top = q.n_top,
                       d_top = q.d_top,
                       n_toppjg = q.n_toppjg,
                       d_toppjg = q.d_toppjg,
                       c_kurs = q.c_kurs,
                       n_kurs = q.n_kurs,
                       v_ket = q.v_ket,
                       n_gross = q.n_gross,
                       n_disc = q.n_disc,
                       n_tax = q.n_tax,
                       n_net = q.n_net,
                       n_sisa = q.n_sisa,
                       l_cabang = q.l_cabang,
                       l_print = q.l_print,
                       c_entry = q.c_entry,
                       d_entry = q.d_entry,
                       c_update = q.c_update,
                       d_update = q.d_update,
                       c_iteno = q1.c_iteno,
                       n_qty = q1.n_qty,
                       n_salpri = q1.n_salpri,
                       n_discon = q2.n_discon
                     }).ToList();

      /*
       * SELECT     c.c_cusno, a.c_norcv, a.d_order, CASE WHEN substring(a.c_nodo, 3, 1) <> '0' THEN 'FJ' + SUBSTRING(a.c_nodo, 3, 8) 
                                       ELSE c_nodo END AS c_dono, a.d_tgldo, a.c_exnoinv, a.d_exinv, b.c_iteno, SUM(b.n_qtyrcv) AS n_qtyrcv, b.n_salpri, b.n_disc
                FROM         dbo.SD_OrderH AS a LEFT OUTER JOIN
                             dbo.SD_OrderD AS b ON a.c_cab = b.c_cab AND a.c_norcv = b.c_norcv INNER JOIN
                             dbo.LG_Cusmas AS c ON a.c_cab = c.c_cab
                GROUP BY c.c_cusno, a.c_norcv, a.d_order, CASE WHEN substring(a.c_nodo, 3, 1) <> '0' THEN 'FJ' + SUBSTRING(a.c_nodo, 3, 8) 
                                       ELSE c_nodo END, a.d_tgldo, a.c_exnoinv, a.d_exinv, b.c_iteno, b.n_salpri, b.n_disc
       */

      var qqqKanan = (from q in db.SD_OrderHs
                      join q1 in db.SD_OrderDs on new { q.c_cab, q.c_norcv } equals new { q1.c_cab, q1.c_norcv } into q_1
                      from qOrd in q_1.DefaultIfEmpty()
                      join q2 in db.LG_Cusmas on q.c_cab equals q2.c_cab
                      where (q.d_order >= date1) && (q.d_order <= date2)
                        && (q.c_nodo == "F611110573")
                      group new { q, qOrd, q2 } by new { q2.c_cusno, q.c_norcv, q.d_order, q.c_nodo, q.d_tgldo, q.c_exnoinv, q.d_exinv, qOrd.c_iteno, qOrd.n_salpri, qOrd.n_disc } into g                      
                      select new TestFullOuterJoin()
                      {
                        c_fjno = (g.Key.c_nodo.Substring(2, 1) == "0" ? g.Key.c_nodo : string.Concat("FJ", g.Key.c_nodo.Substring(2).Trim())),
                        d_fjdate = g.Key.d_tgldo,
                        c_dono = g.Key.c_norcv,
                        d_dono = g.Key.d_order,
                        c_cusno = g.Key.c_cusno,
                        c_taxno = g.Key.c_exnoinv,
                        d_taxdate = g.Key.d_exinv,
                        n_top = null,
                        d_top = null,
                        n_toppjg = null,
                        d_toppjg = null,
                        c_kurs = null,
                        n_kurs = null,
                        v_ket = null,
                        n_gross = null,
                        n_disc = null,
                        n_tax = null,
                        n_net = null,
                        n_sisa = null,
                        l_cabang = null,
                        l_print = null,
                        c_entry = null,
                        d_entry = null,
                        c_update = null,
                        d_update = null,
                        c_iteno = g.Key.c_iteno,
                        n_qty = 0,
                        n_salpri = g.Key.n_salpri,
                        n_discon = g.Key.n_disc
                      }).ToList();

      Func<TestFullOuterJoin, TestFullOuterJoin, bool> match =
        delegate(TestFullOuterJoin kiri, TestFullOuterJoin kanan)
        {
          //return ((kiri == null) || (kanan == null) ? false : 
          //  (ReferenceEquals(kiri.c_fjno, (string.IsNullOrEmpty(kanan.c_fjno) ? string.Empty : kanan.c_fjno.Trim())) &&
          //    ReferenceEquals(kiri.c_iteno, (string.IsNullOrEmpty(kanan.c_iteno) ? string.Empty : kanan.c_iteno.Trim())) &&
          //    ReferenceEquals(kiri.c_cusno, (string.IsNullOrEmpty(kanan.c_cusno) ? string.Empty : kanan.c_cusno.Trim()))));

          return ((kiri == null) || (kanan == null) ? false :
            (kiri.c_fjno.Trim().Equals((string.IsNullOrEmpty(kanan.c_fjno) ? string.Empty : kanan.c_fjno.Trim()), StringComparison.OrdinalIgnoreCase) &&
              kiri.c_iteno.Trim().Equals((string.IsNullOrEmpty(kanan.c_iteno) ? string.Empty : kanan.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
              kiri.c_cusno.Trim().Equals((string.IsNullOrEmpty(kanan.c_cusno) ? string.Empty : kanan.c_cusno.Trim()), StringComparison.OrdinalIgnoreCase)));
        };

      var qqq = qqqKiri.FullOuterJoin(qqqKanan, match).ToList();

      int nCount = qqq.Count();
    }

    #endregion

    static void ShowForm()
    {
      //Form1 f = new Form1();

      //f.BringToFront();

      //f.ShowDialog();
    }

    static void TestOCR()
    {
      List<ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField> listOCR = new List<ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField>();

      ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure ocr = new ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure();
      
      listOCR.Add(new ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField()
      {
        Item = "3666",
        Bonus = 0,
        Discount = 12,
        Salpri = 19250,
        NomorSP = "SP13077",
        DivisiAMS = "062",
        DivisiSupplier = "089",
        Name = "0",
        Quantity = 100
      });
      listOCR.Add(new ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField()
      {
        Item = "5079",
        Bonus = 0,
        Discount = 12,
        Salpri = 62000,
        NomorSP = "SP13077",
        DivisiAMS = "062",
        DivisiSupplier = "318",
        Name = "1",
        Quantity = 40
      });

      ocr.Method = "Submit";
      ocr.Name = "OrderCustomerReceiveStructure";
      ocr.Fields = new ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureFields()
      {
        Cabang = "A1",
        NoReceive = "PB37404",
        TanggalRN = "2011-09-01",
        TanggalRNDate = DateTime.Parse("2011-09-01"),
        Supplier = "00001",
        NomorInvoice = "108B461",
        TanggalInvoice = "2011-08-23",
        TanggalInvoiceDate = DateTime.Parse("2011-08-23"),
        NomorDO = "A11108B461",
        TanggalDO = "2011-08-23",
        TanggalDODate = DateTime.Parse("2011-08-23"),
        Keterangan = "Testing",
        Top = 20,
        Entry = "Rudi",
        Field = listOCR.ToArray()
      };

      listOCR.Clear();

      string str = ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure.Deserialize(ocr);
    }

    #region Testing Kartu Barang

    class LG_KB_PROCESS
    {
      public char c_gdg { get; set; }
      public string c_no { get; set; }
      public DateTime d_date { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_gqty { get; set; }
      public decimal n_bqty { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
      public string c_cusno { get; set; }
    }

    class CLASS_TEMP_AWAL
    {
      public char c_gdg { get; set; }
      public string c_iteno { get; set; }
      public string c_batch { get; set; }
      public decimal n_gawal { get; set; }
      public decimal n_bawal { get; set; }
    }

    static System.Data.DataSet ReportKartuBarang(ORMDataContext db, Dictionary<string, ScmsSoaLibrary.Commons.Functionals.ParameterParser> dic)
    {
      ScmsSoaLibrary.Commons.Functionals.ParameterParser pp = default(ScmsSoaLibrary.Commons.Functionals.ParameterParser);
      
      string[] supplier = null,
        items = null,
        divprin = null;
      int tahun = 0, bulan = 0;

      DateTime date1 = DateTime.Parse("2010-10-01"),
      date2 = DateTime.Parse("2010-10-31"),
      dateX = date1.AddMonths(-1),
      datePrev1 = new DateTime(dateX.Year, dateX.Month, 1),
      datePrev2 = date1.AddDays(-1);

      char gud = '1';
      string userId = "mpat_tra";

      string[] arrItems = new string[0];//{"4625","3434","3127"};
      string[] arrSupls = new string[0];//{"00117","00120","00001"};
      string[] arrDivAms = new string[0];
      string[] arrDivSupl = new string[0];
      List<string> lstAvailItems = null;

      System.Data.DataSet dataSet = null;
      System.Data.DataTable table = null;

      List<LG_KB_PROCESS> listStokAwal = new List<LG_KB_PROCESS>();
      List<LG_KB_PROCESS> listSATemp = null,
        listSAJalan2 = null, listSAProsesAwal = null, listSAProsesJalan = new List<LG_KB_PROCESS>(),
        listSAProsesJalan1 = null, listSAFinal = null, listSAJalan = null, listSAJalan3 = null;
      List<string> lstTemp = new List<string>();

      #region Parameter

      //try
      //{
      //  if (dic.ContainsKey("date1"))
      //  {
      //    pp = dic["date1"];
      //    if (pp.IsSet)
      //    {
      //      date1 = (DateTime)pp.Value;
      //    }
      //  }
      //  if (dic.ContainsKey("date2"))
      //  {
      //    pp = dic["date2"];
      //    if (pp.IsSet)
      //    {
      //      date2 = (DateTime)pp.Value;
      //    }
      //  }
      //  if (dic.ContainsKey("supplier"))
      //  {
      //    pp = dic["supplier"];
      //    if (pp.IsSet)
      //    {
      //      supplier = (pp.Value == null ? new string[0] : (string[])pp.Value);
      //    }
      //  }
      //  else
      //  {
      //    supplier = new string[0];
      //  }
      //  if (dic.ContainsKey("item"))
      //  {
      //    pp = dic["item"];
      //    if (pp.IsSet)
      //    {
      //      arrItems = (pp.Value == null ? new string[0] : (string[])pp.Value);
      //    }
      //  }
      //  else
      //  {
      //    arrItems = new string[0];
      //  }
      //  if (dic.ContainsKey("divprin"))
      //  {
      //    pp = dic["divprin"];
      //    if (pp.IsSet)
      //    {
      //      divprin = (pp.Value == null ? new string[0] : (string[])pp.Value);
      //    }
      //  }
      //  else
      //  {
      //    divprin = new string[0];
      //  }
      //}
      //catch (Exception ex)
      //{
      //  Logger.WriteLine(
      //    "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales Paramter - {0}", ex.StackTrace);
      //}

      //if (date1.Equals(DateTime.MinValue) || date2.Equals(DateTime.MinValue))
      //{
      //  return null;
      //}

      #endregion

      #region Query

      // stok Awal
      listSATemp = (from q in db.LG_Stocks
                    //join qarrItems in db.FA_MasItms on q.c_iteno equals qarrItems.c_iteno
                    //join qDivAms in db.FA_DivAMs on q.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //join qDivPri in db.FA_Divpris on q.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where (q.s_tahun == datePrev1.Year) && (q.t_bulan == datePrev1.Month)
                      && (q.c_gdg == gud)
                    //&& ((arrItems.Length > 0) ? arrItems.Contains(q.c_iteno) : true)
                    //&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group q by new { q.c_gdg, q.c_no, q.c_iteno, q.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_no,
                      d_date = Functionals.StandardSqlDateTime,
                      c_iteno = g.Key.c_iteno,
                      c_batch = (string.IsNullOrEmpty(g.Key.c_batch) ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listStokAwal.AddRange(listSATemp.ToArray());
      listSATemp.Clear();

      listSAProsesAwal = (from q in listStokAwal
                          //where ((string.IsNullOrEmpty(q.c_batch) ? string.Empty : q.c_batch.Trim()).Length != 0)
                          //where (string.IsNullOrEmpty(q.c_batch) ? "<null>" : q.c_batch.Trim()).Length > 0
                          group q by new { q.c_gdg, q.c_no, q.c_iteno, q.c_batch } into g
                          select new LG_KB_PROCESS()
                          {
                            c_gdg = g.Key.c_gdg,
                            c_iteno = g.Key.c_iteno,
                            c_no = g.Key.c_no,
                            c_batch = (string.IsNullOrEmpty(g.Key.c_batch) ? "<null>" : g.Key.c_batch.Trim()),
                            n_gqty = g.Sum(x => x.n_gqty),
                            n_bqty = g.Sum(x => x.n_bqty),
                            c_cusno = ""
                          }).Distinct().ToList();

      var t1 = listStokAwal.Sum(x => x.n_gqty);
      var tt = listSAProsesAwal.Sum(x => x.n_gawal);
      var tt2 = listStokAwal.Sum(x => x.n_gqty);
      var tt1 = listSAProsesAwal.Sum(x => x.n_bawal);

      #region Old Coded

      //// RN
      //lstTemp.Add("05");
      //listSATemp = (from q in db.LG_RNHs
      //              join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              join qDivAms in db.FA_DivAMs on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_rndate >= datePrev1) && (q.d_rndate <= datePrev2))
      //                && (!lstTemp.Contains(q.c_type)) && (q.c_gdg == gud)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_rnno, q.d_rndate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_rnno,
      //                d_date = (g.Key.d_rndate.HasValue ? g.Key.d_rndate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// RN Gudang
      //listSATemp = (from q in db.LG_SJHs
      //              join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_update >= datePrev1) && (q.d_update <= datePrev2))
      //                && (q.l_status == true) && (q.c_gdg == gud)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg2, q.c_sjno, q.d_update, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '0'),
      //                c_no = g.Key.c_sjno,
      //                d_date = (g.Key.d_update.HasValue ? g.Key.d_update.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// Combo In
      //listSATemp = (from q in db.LG_ComboHs
      //              join qarrItems in db.FA_MasItms on q.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_combodate >= datePrev1) && (q.d_combodate <= datePrev2))
      //                && (q.c_gdg == gud) && (q.l_confirm == true) && (q.c_type == "01")
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group q by new { q.c_gdg, q.c_combono, q.d_combodate, q.c_iteno, q.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_combono,
      //                d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// RC
      //listSATemp = (from q in db.LG_RCHes
      //              join q1 in db.LG_RCD1s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_rcdate >= datePrev1) && (q.d_rcdate <= datePrev2))
      //                && (q.c_gdg == gud)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_rcno, q.d_rcdate, q1.c_iteno, q1.c_batch, q.c_cusno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_rcno,
      //                d_date = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = g.Sum(t => (t.q1.c_type == "01" ? (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) : 0)),
      //                n_bqty = g.Sum(t => (t.q1.c_type != "01" ? (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) : 0)),
      //                c_cusno = g.Key.c_cusno
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// RS
      //listSATemp = (from q in db.LG_RSHes
      //              join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_rsdate >= datePrev1) && (q.d_rsdate <= datePrev2))
      //                && (q.c_gdg == gud)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_rsno, q.d_rsdate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_rsno,
      //                d_date = (g.Key.d_rsdate.HasValue ? g.Key.d_rsdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
      //                n_bqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();

      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// SJ
      //listSATemp = (from q in db.LG_SJHs
      //              join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_sjdate >= datePrev1) && (q.d_sjdate <= datePrev2))
      //                && (q.c_gdg == gud)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_sjno, q.d_sjdate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_sjno,
      //                d_date = (g.Key.d_sjdate.HasValue ? g.Key.d_sjdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
      //                n_bqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// PL
      //listSATemp = (from q in db.LG_PLHs
      //              join q1 in db.LG_PLD1s on new { q.c_plno } equals new { q1.c_plno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_pldate >= datePrev1) && (q.d_pldate <= datePrev2))
      //                && (q.c_gdg == gud) && (q.l_confirm == true)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_plno, q.d_pldate, q1.c_iteno, q1.c_batch, q.c_cusno } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = (g.Key.c_gdg.HasValue ? g.Key.c_gdg.Value : '0'),
      //                c_no = g.Key.c_plno,
      //                d_date = (g.Key.d_pldate.HasValue ? g.Key.d_pldate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
      //                n_bqty = 0,
      //                c_cusno = g.Key.c_cusno
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// STT
      //listSATemp = (from q in db.LG_STHs
      //              join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_stdate >= datePrev1) && (q.d_stdate <= datePrev2))
      //                && (q.c_gdg == gud)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_stno, q.d_stdate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_stno,
      //                d_date = (g.Key.d_stdate.HasValue ? g.Key.d_stdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
      //                n_bqty = 0,
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// Combo
      //listSATemp = (from q in db.LG_ComboHs
      //              join q1 in db.LG_ComboD1s on new { q.c_gdg, q.c_combono } equals new { q1.c_gdg, q1.c_combono }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              join qDivAms in db.FA_DivAMs on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_combodate >= datePrev1) && (q.d_combodate <= datePrev2))
      //                && (q.c_gdg == gud) && (q.l_confirm == true)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_combono, q.d_combodate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_combono,
      //                d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
      //                n_bqty = 0,
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      //// Adjust
      //listSATemp = (from q in db.LG_AdjustHs
      //              join q1 in db.LG_AdjustD1s on new { q.c_gdg, q.c_adjno } equals new { q1.c_gdg, q1.c_adjno }
      //              join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
      //              //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
      //              //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
      //              //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
      //              //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
      //              where ((q.d_adjdate >= datePrev1) && (q.d_adjdate <= datePrev2))
      //                && (q.c_gdg == gud)
      //                && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
      //                && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
      //              //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
      //              //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
      //              //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
      //              group new { q, q1 } by new { q.c_gdg, q.c_adjno, q.d_adjdate, q1.c_iteno, q1.c_batch } into g
      //              select new LG_KB_PROCESS()
      //              {
      //                c_gdg = g.Key.c_gdg,
      //                c_no = g.Key.c_adjno,
      //                d_date = (g.Key.d_adjdate.HasValue ? g.Key.d_adjdate.Value : Functionals.StandardSqlDateTime),
      //                c_iteno = g.Key.c_iteno,
      //                c_batch = g.Key.c_batch,
      //                n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
      //                n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
      //                c_cusno = string.Empty
      //              }).Distinct().ToList();
      //listSATemp1 = listStokAwal.Union(listSATemp).ToList();
      //listStokAwal.Clear();
      //listSATemp.Clear();
      //listStokAwal.AddRange(listSATemp1.ToArray());
      //listSATemp1.Clear();

      #endregion

      // Transaksi Berjalan

      #region Transaksi

      // RN
      lstTemp.Add("05");
      listSATemp = (from q in db.LG_RNHs
                    join q1 in db.LG_RND1s on new { q.c_gdg, q.c_rnno } equals new { q1.c_gdg, q1.c_rnno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_rndate >= date1) && (q.d_rndate <= date2))
                      && (q.c_gdg == gud) && (!lstTemp.Contains(q.c_type))
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_rnno, q.d_rndate, q1.c_iteno, q1.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_rnno,
                      d_date = (g.Key.d_rndate.HasValue ? g.Key.d_rndate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listSAProsesJalan.AddRange(listSATemp.ToArray());
      listSATemp.Clear();

      // RN Gudang
      listSATemp = (from q in db.LG_SJHs
                    join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_update >= date1) && (q.d_update <= date2))
                      && (q.c_gdg == gud) && (q.l_status == true)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg2, q.c_sjno, q.d_update, q1.c_iteno, q1.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = (g.Key.c_gdg2.HasValue ? g.Key.c_gdg2.Value : '0'),
                      c_no = g.Key.c_sjno,
                      d_date = (g.Key.d_update.HasValue ? g.Key.d_update.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // Combo In
      listSATemp = (from q in db.LG_ComboHs
                    join qarrItems in db.FA_MasItms on q.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_combodate >= date1) && (q.d_combodate <= date2))
                      && (q.l_confirm == true) && (q.c_type == "01") && (q.c_gdg == gud)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group q by new { q.c_gdg, q.c_combono, q.d_combodate, q.c_iteno, q.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_combono,
                      d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = g.Sum(t => (t.n_gqty.HasValue ? t.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.n_bqty.HasValue ? t.n_bqty.Value : 0)),
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // RC
      listSATemp = (from q in db.LG_RCHes
                    join q1 in db.LG_RCD1s on new { q.c_gdg, q.c_rcno } equals new { q1.c_gdg, q1.c_rcno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_rcdate >= date1) && (q.d_rcdate <= date2))
                      && (q.c_gdg == gud)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_rcno, q.d_rcdate, q1.c_iteno, q1.c_batch, q.c_cusno } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_rcno,
                      d_date = (g.Key.d_rcdate.HasValue ? g.Key.d_rcdate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = g.Sum(t => (t.q1.c_type == "01" ? (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) : 0)),
                      n_bqty = g.Sum(t => (t.q1.c_type != "01" ? (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0) : 0)),
                      c_cusno = g.Key.c_cusno
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // RS
      listSATemp = (from q in db.LG_RSHes
                    join q1 in db.LG_RSD1s on new { q.c_gdg, q.c_rsno } equals new { q1.c_gdg, q1.c_rsno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_rsdate >= date1) && (q.d_rsdate <= date2))
                      && (q.c_gdg == gud)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_rsno, q.d_rsdate, q1.c_iteno, q1.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_rsno,
                      d_date = (g.Key.d_rsdate.HasValue ? g.Key.d_rsdate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                      n_bqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                      c_cusno = string.Empty
                    }).Distinct().ToList();

      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // SJ
      listSATemp = (from q in db.LG_SJHs
                    join q1 in db.LG_SJD1s on new { q.c_gdg, q.c_sjno } equals new { q1.c_gdg, q1.c_sjno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_sjdate >= date1) && (q.d_sjdate <= date2))
                      && (q.c_gdg == gud)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_sjno, q.d_sjdate, q1.c_iteno, q1.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_sjno,
                      d_date = (g.Key.d_sjdate.HasValue ? g.Key.d_sjdate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                      n_bqty = -g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // PL
      listSATemp = (from q in db.LG_PLHs
                    join q1 in db.LG_PLD1s on new { q.c_plno } equals new { q1.c_plno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_pldate >= date1) && (q.d_pldate <= date2))
                      && (q.l_confirm == true) && (q.c_gdg == gud)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_plno, q.d_pldate, q1.c_iteno, q1.c_batch, q.c_cusno } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = (g.Key.c_gdg.HasValue ? g.Key.c_gdg.Value : '0'),
                      c_no = g.Key.c_plno,
                      d_date = (g.Key.d_pldate.HasValue ? g.Key.d_pldate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                      n_bqty = 0,
                      c_cusno = g.Key.c_cusno
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // STT
      listSATemp = (from q in db.LG_STHs
                    join q1 in db.LG_STD1s on new { q.c_gdg, q.c_stno } equals new { q1.c_gdg, q1.c_stno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_stdate >= date1) && (q.d_stdate <= date2))
                      && (q.c_gdg == gud)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_stno, q.d_stdate, q1.c_iteno, q1.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_stno,
                      d_date = (g.Key.d_stdate.HasValue ? g.Key.d_stdate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                      n_bqty = 0,
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // Combo
      listSATemp = (from q in db.LG_ComboHs
                    join q1 in db.LG_ComboD1s on new { q.c_gdg, q.c_combono } equals new { q1.c_gdg, q1.c_combono }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_combodate >= date1) && (q.d_combodate <= date2))
                      && (q.c_gdg == gud) && (q.l_confirm == true)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_combono, q.d_combodate, q1.c_iteno, q1.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_combono,
                      d_date = (g.Key.d_combodate.HasValue ? g.Key.d_combodate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = -g.Sum(t => (t.q1.n_qty.HasValue ? t.q1.n_qty.Value : 0)),
                      n_bqty = 0,
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      // Adjust
      listSATemp = (from q in db.LG_AdjustHs
                    join q1 in db.LG_AdjustD1s on new { q.c_gdg, q.c_adjno } equals new { q1.c_gdg, q1.c_adjno }
                    join qarrItems in db.FA_MasItms on q1.c_iteno equals qarrItems.c_iteno
                    //					join qDivAms in db.FA_DivAMS on q1.c_iteno equals qDivAms.c_iteno into q_DivAms
                    //					from qarrDivAms in q_DivAms.DefaultIfEmpty()
                    //					join qDivPri in db.FA_Divpris on q1.c_iteno equals qDivPri.c_iteno into q_DivPri
                    //					from qarrDivPri in q_DivPri.DefaultIfEmpty()
                    where ((q.d_adjdate >= date1) && (q.d_adjdate <= date2))
                      && (q.c_gdg == gud)
                      && ((q.l_delete.HasValue ? q.l_delete.Value : false) == false)
                      && ((arrItems.Length > 0) ? arrItems.Contains(q1.c_iteno) : true)
                    //						&& ((arrSupls.Length > 0) ? arrSupls.Contains(qarrItems.c_iteno) : true)
                    //						&& ((arrDivAms.Length > 0) ? arrDivAms.Contains(qarrDivAms.c_iteno) : true)
                    //						&& ((arrDivSupl.Length > 0) ? arrDivSupl.Contains(qarrDivPri.c_iteno) : true)
                    group new { q, q1 } by new { q.c_gdg, q.c_adjno, q.d_adjdate, q1.c_iteno, q1.c_batch } into g
                    select new LG_KB_PROCESS()
                    {
                      c_gdg = g.Key.c_gdg,
                      c_no = g.Key.c_adjno,
                      d_date = (g.Key.d_adjdate.HasValue ? g.Key.d_adjdate.Value : Functionals.StandardSqlDateTime),
                      c_iteno = g.Key.c_iteno,
                      c_batch = (g.Key.c_batch == null ? "<null>" : g.Key.c_batch.Trim()),
                      n_gqty = g.Sum(t => (t.q1.n_gqty.HasValue ? t.q1.n_gqty.Value : 0)),
                      n_bqty = g.Sum(t => (t.q1.n_bqty.HasValue ? t.q1.n_bqty.Value : 0)),
                      c_cusno = string.Empty
                    }).Distinct().ToList();
      listSAProsesJalan1 = listSAProsesJalan.Union(listSATemp).ToList();
      listSAProsesJalan.Clear();
      listSATemp.Clear();
      listSAProsesJalan.AddRange(listSAProsesJalan1.ToArray());
      listSAProsesJalan1.Clear();

      #endregion

      var qryAwal = listSAProsesAwal
        .GroupBy(t => new
        {
          t.c_gdg,
          t.c_iteno,
          t.c_batch
        })
        .Select(x => new CLASS_TEMP_AWAL()
        {
          c_gdg = x.Key.c_gdg,
          c_iteno = x.Key.c_iteno,
          c_batch = x.Key.c_batch,
          n_gawal = x.Sum(y => y.n_gqty),
          n_bawal = x.Sum(y => y.n_bqty)
        });

      //var tbl = qryAwal.OrderBy(x=> x.c_gdg).ThenBy(y => y.c_iteno).ThenBy(z => z.c_batch).ToList().CopyToDataTableObject();
      ////tbl.WriteXml("C:\\test.xml");

      //var aaa = qryAwal.ToList();
      //var bbb = listSAProsesJalan.ToList();

      //var left = (from q in qryAwal
      //            join q1 in listSAProsesJalan on new { q.c_gdg, q.c_iteno, q.c_batch } equals new { q1.c_gdg, q1.c_iteno, q1.c_batch }
      //            select new
      //            {
      //              q.c_gdg,
      //              q.c_iteno,
      //              q.c_batch
      //            }).ToList();

      //var te = qryAwal.GroupJoin(listSAProsesJalan,
      //  x => new { x.c_gdg, x.c_iteno, x.c_batch },
      //  y => new { y.c_gdg, y.c_iteno, y.c_batch },
      //  (kiri, kanan) => new
      //  {
      //    kiri.c_gdg,
      //    kiri.c_iteno,
      //    kiri.c_batch,
      //    t = kanan
      //  }).ToList();

      //te = te.Where(y => (y.t == null) || (y.t.Count() < 1)).ToList();

      //Func<TIn, bool> leftWhere = delegate(TIn t)
      //{
      //  return (t == null);
      //};

      var yyy = qryAwal.FullJoin(listSAProsesJalan,
        x => new EqualityKey() { c_gdg = x.c_gdg, c_iteno = x.c_iteno, c_batch = x.c_batch },
        y => new EqualityKey() { c_gdg = y.c_gdg, c_iteno = y.c_iteno, c_batch = y.c_batch },
        (kiri, kanan) => new
        {
          kiri.c_gdg,
          kiri.c_iteno,
          c_batch = (string.IsNullOrEmpty(kiri.c_batch) ? string.Empty : kiri.c_batch.Trim()),
          j = 0
        },
        (kiri, kanan) => new
        {
          kiri.c_gdg,
          kiri.c_iteno,
          c_batch = (string.IsNullOrEmpty(kiri.c_batch) ? string.Empty: kiri.c_batch.Trim()),
          j = (kanan == null ? 0 : kanan.Count())
        },
        t => (t.j < 1),
        (kanan, kiri) => new
        {
          kanan.c_gdg,
          kanan.c_iteno,
          c_batch = (string.IsNullOrEmpty(kanan.c_batch) ? string.Empty : kanan.c_batch.Trim()),
          j = (kiri == null ? 0 : kiri.Count())
        },
        t => (t.j < 1), new EqualComparer());
      
      var xxx = yyy.OrderBy(x => x.c_gdg).ThenBy(y => y.c_iteno).ThenBy(z => z.c_batch).ToList();
      var zzz = xxx.CopyToDataTableObject();

      Func<CLASS_TEMP_AWAL, LG_KB_PROCESS, bool> match =
        delegate(CLASS_TEMP_AWAL kiri, LG_KB_PROCESS kanan)
        {
          //return ((kiri == null) || (kanan == null) ? false : 
          //  (ReferenceEquals(kiri.c_fjno, (string.IsNullOrEmpty(kanan.c_fjno) ? string.Empty : kanan.c_fjno.Trim())) &&
          //    ReferenceEquals(kiri.c_iteno, (string.IsNullOrEmpty(kanan.c_iteno) ? string.Empty : kanan.c_iteno.Trim())) &&
          //    ReferenceEquals(kiri.c_cusno, (string.IsNullOrEmpty(kanan.c_cusno) ? string.Empty : kanan.c_cusno.Trim()))));

          return (kanan == null ? false :
            kiri.c_gdg.Equals(kanan.c_gdg) &&
            kiri.c_iteno.Trim().Equals((string.IsNullOrEmpty(kanan.c_iteno) ? string.Empty : kanan.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
            kiri.c_batch.Trim().Equals((string.IsNullOrEmpty(kanan.c_batch) ? string.Empty : kanan.c_batch.Trim()), StringComparison.OrdinalIgnoreCase));
        };

      //qryAwal.Join<
      var qqq = qryAwal.FullOuterJoin(listSAProsesJalan, match).ToList();

      var xxxx = qqq.Select(x => new
      {
        gdg = (x.Key == null ? (x.Value == null ? '0' : x.Value.c_gdg) : x.Key.c_gdg),
        item = (x.Key == null ? (x.Value == null ? string.Empty : x.Value.c_iteno) : x.Key.c_iteno),
        batch = (x.Key == null ? (x.Value == null ? string.Empty : x.Value.c_batch) : x.Key.c_batch)
      }).Distinct().ToList();

      #endregion

      #region Populate

      if (listSAFinal != null)
      {
        try
        {
          table = listStokAwal.CopyToDataTableObject();

          if (table != null)
          {
            dataSet = new System.Data.DataSet();

            dataSet.Tables.Add(table);
          }
        }
        catch (Exception ex)
        {
          Logger.WriteLine(
            "ScmsSoaLibrary.Core.Reports.ReportDatasetBind:ReportSales PopulateDataset - {0}", ex.StackTrace);

          Logger.WriteLine(ex.Message);
          Logger.WriteLine(ex.StackTrace);
        }
      }

      #endregion

      if (listSAFinal != null)
      {
        listSAFinal.Clear();
      }

      return dataSet;
    }

    #endregion

    static void TestingSPAdmin()
    {
      string dataSp = @"<Structure name=""SuratPesananAdmin"" method=""Add"">
  <Fields ID="""" Entry=""Rudi"" Customer=""0163"" Tanggal=""20120510000000000"" SPCabang=""SPO0219249"" Keterangan=""Test"" Tipe=""05"">
    <Field name=""0"" New=""true"" Item=""4039"" Qty=""5"" Acc=""5"">Test</Field>
  </Fields>
</Structure>";
      //4464
      //3880

      string result = null;

      ScmsSoaLibraryInterface.Components.PostDataParser parser = new ScmsSoaLibraryInterface.Components.PostDataParser();

      ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponse responseResult = default(ScmsSoaLibraryInterface.Components.PostDataParser.StructureResponse);

      //ScmsSoaLibrary.Bussiness.Pembelian beli = new ScmsSoaLibrary.Bussiness.Pembelian();

      //result = beli.SuratPesananAdmin(sps)

      ScmsSoaLibrary.Services.Service srvc = new ScmsSoaLibrary.Services.Service();
      
      result = srvc.PostData(dataSp);

      Console.ReadKey();

      responseResult = parser.ResponseParser(result);
    }

    static void TestingFormatBDP()
    {
      List<ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField> lst = new List<ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField>();

      lst.Add(new ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureField()
      {
        Name = "1",
        Item = "3666",
        Quantity = 100,
        Bonus = 0,
        Discount = 12,
        DivisiAMS = "062",
        DivisiSupplier = "089",
        NomorSP = "SP13077",
        Salpri = 19250,
      });

      ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure ocr = new ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure()
      {
        Name = "SyncRNCabang",
        Method = "Submit",
        Fields = new ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructureFields()
        {
          Cabang = "0065",
          Entry = "Rudi",
          Keterangan = "Test",
          NoReceive = "PB37404",
          TanggalRN = "20120131000000000",
          Supplier = "00001",
          Top = 5,
          NomorInvoice = "FJ12010052",
          TanggalInvoice = "20120131000000000",
          NomorDO = "DO12010052",
          TanggalDO = "20120131000000000",
          Field = lst.ToArray()
        }
      };

      lst.Clear();

      string sss = ScmsSoaLibrary.Parser.Class.OrderCustomerReceiveStructure.Deserialize(ocr);

      ScmsSoaLibrary.Services.Service srvc = new Service();

      string ret = srvc.PostData(sss);

      Console.WriteLine(ret);
    }

    static void TestingFormatRDP()
    {
      List<ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructureField> lst = new List<ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructureField>();

      lst.Add(new ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructureField()
      {
        Name = "1",
        Item = "3666",
        QuantityGood = 10,
        QuantityBad = 0,
        BonusQuantityGood = 0,
        BonusQuantityBad = 0
      });

      ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructure ocr = new ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructure()
      {
        Name = "SyncRSCabang",
        Method = "Submit",
        Fields = new ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructureFields()
        {
          Entry = "Rudi",
          Cabang = "0065",
          Supplier = "00001",
          NomorRetur = "RS14089",
          TanggalRetur = "20120131000000000",
          NomorRC = "PB37404",
          TanggalRC = "20120131000000000",
          ExFaktur = "0092022",
          ReturValue = 1905000,
          ExNoFaktur = "",
          TanggalExFaktur = "20120131000000000",
          ExFakturCo = "",
          ExFaktur2 = "0092022",
          ExNoFaktur2 = "",
          TanggalExFaktur2 = "20120131000000000",
          PpnBM = 0,
          Field = lst.ToArray()
        }
      };

      lst.Clear();

      string sss = ScmsSoaLibrary.Parser.Class.ReturCustomerReceiveStructure.Deserialize(ocr);
    }

    static void GetQueryPLGenerator()
    {
      string[][] arr = new string[][] {
          new string[] { "gdg", "1",  "System.Char"},
          new string[] { "cusno", "0065",  "System.String"},
          new string[] { "supl", "00127",  "System.String"},
          new string[] { "itemCat", "",  "System.String"},
      };


      Service srvc = new Service();

      string xxx = srvc.GlobalQueryJson(0, 10, false, string.Empty, string.Empty, "3202", arr);

      Logger.WriteLine(xxx);
    }

    static void TestAutoSchedule()
    {
      //ScmsSoaLibrary.Core.Schedule.AutoRunning ar = new ScmsSoaLibrary.Core.Schedule.AutoRunning(new Config());

      //ar.Start();

      //Console.ReadLine();

      //ar.Stop();
    }

    static void LinqBetweenString()
    {
      ORMDataContext db = new ORMDataContext();

      var qqq = (from q in db.LG_SPHs
                 where (q.d_spdate.Value.Year == 2012)
                 select q).AsQueryable();

      var low = new DateTime(2012, 3, 1);
      var high = new DateTime(2012, 3, 5);

      var yyy = qqq.Between("c_spno", "SP12030001", "SP12030100").AsQueryable();
      //var yyy = qqq.Between("d_spdate", low, high);

      var str = yyy.Provider.ToString();
    }

    static void TestingLaporanPBF()
    {
      Service srvc = new Service();

      srvc.Testing();

      Console.WriteLine("Testing");
    }

    static void Main(string[] args)
    {
        //Config cfg1 = new Config();

        //SqlConnection cn = new SqlConnection(cfg1.ConnectionString);
        //SqlCommand cmd = new SqlCommand();
        //cmd.Connection = cn;
        //DataTable tbl = new DataTable();
        
        //cmd = new SqlCommand();
        //cmd.Connection = cn;
        //cmd.CommandText = "select * from LG_DOPHErr2";
        //SqlDataAdapter adp = new SqlDataAdapter(cmd.CommandText, cn);
        //adp.Fill(tbl);

        //string path = cfg1.PathTempExtractMail + "DOPharErr\\tes.xlsx";
        //XLWorkbook wb = new XLWorkbook();
        ////DataTable dt = GetDataTableOrWhatever();
        //wb.Worksheets.Add(tbl, "WorksheetName");
        //wb.SaveAs(path);
      //TestingLaporanPBF();

      //LinqBetweenString();

      //TestAutoSchedule();

      //GetQueryPLGenerator();

      //TestingFormatBDP();

      //TestingFormatRDP();

      //TestingSPAdmin();

      //RunningCreateDBF();

      //TestReadData();

      //ReportKartuBarang(new ORMDataContext(), new Dictionary<string, Functionals.ParameterParser>());

      //TestingFullOuterJoin();

      //TestOCR();

      //PostDataDO();

      ////GetFakturSP();

      ////CallRpt();

      //TestingCreateDBF();

      //RunningCreateDBF();

      //ShowForm();

      //TestingGetMail();
      //TestingMailer();

      //TestReadData();

      //ReadDbf(@"D:\Rudi\SampelData\Data DO Pharos\20120106_PML\DODETAIL.DBF");

      //System.Data.DataTable table = ReadDbfDatabase(@"D:\Rudi\SampelData\Data DO Pharos\20120106_PML\DODETAIL.DBF");
      
      //TestingCodeGenerated();

      //TestingSendMail();

      //TestingKirim();

      //ConvertClassToJson();

      //MultiPostKirim();

      //Testing();

      //ConvertClassToJson();

      //TestingJSonRpc();

      //return;

      string strtX = @"<Structure name=""UserGroupAccess"" method=""Modify"">
  <Fields Nip=""105839H"">
    <Field name=""0"" Delete=""false"">Admin</Field>
    <Field name=""1"" New=""false"">DC</Field>
    <Field name=""2"" New=""false"" Delete=""false"">Testing</Field>
  </Fields>
</Structure>";

      string[] arrX = new string[] {"0", "1"};
      string tmp = arrX.GetType().FullName; ;

      //ReadAllSettings();

      //string[][] arr = new string[][] {
      //    new string[] {"qPLH.c_plno", "PL10%", "" },
      //    new string[] {"qPLH.d_pldate = @0", "", "System.DateTime"},
      //    new string[] {"qPLH.c_gdg = @0", "1", "System.Char" },
      //    new string[] {"qPLH.c_cusno = @0", "", "System.String" },
      //    new string[] {"qPLH.c_nosup = @0", "", "System.String" },
      //    new string[] {"qPLH.l_confirm = @0", "", "System.Boolean" },
      //    new string[] {"qPLH.l_print = @0", "", "System.Boolean" },
      //    new string[] {"l_do = @0", "", "System.Boolean" }
      //  };

      System.Data.SqlClient.SqlConnectionStringBuilder conBuild = new System.Data.SqlClient.SqlConnectionStringBuilder();
      conBuild.AsynchronousProcessing = true;
      conBuild.DataSource = "SCMS";
      conBuild.FailoverPartner = "CORE-AMS";
      conBuild.InitialCatalog = "AMS";
      conBuild.MultipleActiveResultSets = true;
      //conBuild.Password = "4M5M1S";
      conBuild.PersistSecurityInfo = true;
      //conBuild.Replication = true;
      //conBuild.UserID = "SA";
      conBuild.WorkstationID = "Aplikasi Aig";
      conBuild.IntegratedSecurity = true;

      System.Diagnostics.Debug.WriteLine(conBuild.ConnectionString);

      var t = System.Type.GetType("System.Collections.Generic.List`1[System.String]");

      
      string[][] arr = new string[][] {
          //new string[] {"c_cusno", "['0115';'0163']", "System.String" },
          //new string[] {"c_cusno", "0115 @ 0163", "System.String" },
          //new string[] {"c_dono", "DO10%4%102", "" },
          //new string[] {"c_dono", "['DO10094102';'DO10104102';'DO10114102';'DO10124102']", "System.String[]" },
          new string[] { "gdgStok", "3",  "System.Char"},
          new string[] { "gdgTrx", "3",  "System.Char"},
          //new string[] { "c_nosup", "['00130', '00140']",  "System.String[]"},
          new string[] { "@0.Contains(q.c_nosup)", "['00130', '00140']",  "System.String[]"},
      };

      ///ScmsModel.Core.ExpressionParser.IEnumerableSignatures
      /*
       * Item
       * TypeBeli
       * NoSp
       * NoBatch
       */

      //Type classType = typeof(System.Linq.Enumerable);

      //// Grabbing the specific static method
      //System.Reflection.MethodInfo methodInfo = classType.GetMethod("Contains", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
      
      //// Binding the method info to generic arguments
      //Type[] genericArguments = new Type[] { typeof(Program) };
      //System.Reflection.MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(genericArguments);

      //List<string> lst = new List<string>();

      //lst.Add("Kode 1");
      //lst.Add("Kode 2");
      //lst.Add("Kode 3");
      //lst.Add("Kode 4");
      //lst.Add("Kode 5");
      
      //System.Reflection.MemberInfo[] mis = typeof(System.Linq.Enumerable).GetMember("Contains*");
      //if (mis.Length == 0) { Console.WriteLine("No Teste Methods"); return; }

      //System.Reflection.ParameterModifier pm = new System.Reflection.ParameterModifier(1);

      //Type U = ((System.Reflection.MethodInfo)mis[0]).GetGenericArguments()[0]; // assume we know the class structure above, for simplicity.

      ////System.Reflection.MethodInfo mInfo = typeof(System.Linq.Enumerable).GetMethod("Contains",
      ////  new Type[] { System.Type.GetType("System.Collections.Generic.IEnumerable`1"), U });
      //System.Reflection.MethodInfo mInfo = mis[0] as System.Reflection.MethodInfo;

      ////System.Reflection.MemberInfo[] mis = typeof(System.Linq.Enumerable).GetMember("Contains*");
      ////if (mis.Length == 0) { Console.WriteLine("No Teste Methods"); return; }
      
      ////System.Reflection.ParameterModifier pm = new System.Reflection.ParameterModifier(1);      

      ////Type U = ((System.Reflection.MethodInfo)mis[0]).GetGenericArguments()[0]; // assume we know the class structure above, for simplicity.

      //////System.Reflection.MethodInfo mInfo = typeof(System.Linq.Enumerable).GetMethod("Contains",
      //////  new Type[] { System.Type.GetType("System.Collections.Generic.IEnumerable`1"), U });
      ////System.Reflection.MethodInfo mInfo = mis[0] as System.Reflection.MethodInfo;

      //if (mInfo.IsGenericMethod)
      //{
      //  mInfo = mInfo.MakeGenericMethod(typeof(string));
      //  //mInfo.Invoke(new Test(), new object[] { "Test - calling generic method", 1 });
      //  object ret = mInfo.Invoke(null, new object[] { lst, "Kode 12" });
      //}

      Service srvc = new Service();

      //string xxx = srvc.GlobalQueryJson(0, 10, false, string.Empty, string.Empty, "0007", arr);
      //string xxx = srvc.GlobalQueryServiceClient(0, 10, false, string.Empty, string.Empty, "0007", arr);
      //string xxx = srvc.GlobalQueryServiceClient(0, 10, false, string.Empty, string.Empty, "10006", arr);      

      //      string dataX = @"<Structure name=""User"" method=""Add"">
      //        <Field name=""Nip"">105839H</Field>
      //        <Field name=""Nama"">Rudi</Field>
      //        <Field name=""Password"">password</Field>
      //        <Field name=""GC"">1</Field>
      //        <Field name=""Aktif"">true</Field>
      //        <Field name=""User"">105839H</Field>
      //      </Structure>";

      //      string dataChangeX = @"<Structure name=""User"" method=""modify"">
      //        <Field name=""Nip"">105839H</Field>
      //        <Field name=""Nama"">Om Rudi</Field>
      //        <Field name=""Aktif"">true</Field>
      //        <Field name=""User"">105839H</Field>
      //      </Structure>";

      //      string dataDeleteX = @"<Structure name=""User"" method=""delete"">
      //        <Field name=""Nip"">105839H</Field>
      //        <Field name=""User"">105839H</Field>
      //      </Structure>";

      //      string dataX = @"<Structure name=""Group"" method=""Add"">
      //        <Field name=""Group"">Testing</Field>
      //        <Field name=""Description"">Group Testing</Field>
      //        <Field name=""Aktif"">true</Field>
      //        <Field name=""Akses""></Field>
      //        <Field name=""User"">105839H</Field>
      //      </Structure>";

      //      string dataChangeX = @"<Structure name=""Group"" method=""modify"">
      //        <Field name=""Group"">Testing</Field>
      //        <Field name=""Description"">Group Testing</Field>
      //        <Field name=""Aktif"">true</Field>
      //        <Field name=""Akses"">Akses</Field>
      //        <Field name=""User"">105839H</Field>
      //      </Structure>";

      //      string dataDeleteX = @"<Structure name=""Group"" method=""delete"">
      //        <Field name=""Group"">Admin</Field>
      //        <Field name=""User"">105839H</Field>
      //      </Structure>";

      //srvc.Testing();

      Config cfg = new Config();

      //string reslt = srvc.GlobalQueryJson(0, 10, false, "c_orno", "ASC", "0013", new string[][]{
      //  new string[] {"!|c_pono", "%", ""},
      //  new string[] {"c_orno = @0", "OR11060001", "System.String"} 
      //});

      //srvc.PostData(dataDeleteX);
      ServiceHost hostSoa = StartingSoaLibrary(cfg, true, cfg.BaseUri);
      ServiceHost hostRpt = StartingSoaReporting(cfg, cfg.BaseUriReporting);

      #region Production
      DOPharosReader dopr = null;
      DOPharmanetReader dophar = null;
      DOPharmanetReaderPerItem dopharItem = null;
      //SendPOAuto spoa = null;
      SPCabangReader spc = null;
      SPPharmanetReader sph = null;
      //ScmsSoaLibrary.Core.Schedule.AutoRunning sar = null;
      SPCabangReaderBackup sob = null;

      SendRCAuto srca = null;
        //suwandi 15 maret 2018
      //dopr = DOPharosMailer(cfg);

      dophar = DOPharmanetMailer(cfg);

      //dopharItem = DOPharmanetMailerPerItem(cfg);
      //spoa = POSendingMailer(cfg);
      //sar = ScheduleAutoRunning(cfg);
      sob = SPCabNonMail(cfg);
      //sar = ScheduleAutoRunning(cfg);

      //sph = SPPharMailer(cfg);

      if (cfg.IsActiveSPMail)
      {
          spc = SPCabMailer(cfg);
          //sph = SPPharMailer(cfg);
          dopr = DOPharosMailer(cfg);
          dophar = DOPharmanetMailer(cfg);
      }

      if (cfg.IsActiveSendRCAuto)
      {
          srca = RCSendingMailer(cfg);
      }
      
      System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
      foreach (String printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
      {
          if (printer == cfg.PathPrintStorageLat2)
          {
              SetDefaultPrinter(printer);
              break;
          }
      }
        
      #endregion

      #region Old Coded

      //bool bActivateJsonP = true;

      //for (int nLoop = 0; nLoop < args.Length; nLoop++)
      //{
      //  if (args[nLoop].Equals("/jsonp", StringComparison.OrdinalIgnoreCase))
      //  {
      //    bActivateJsonP = true;
      //  }
      //}

      //Service.IsJsonPaddingActive = bActivateJsonP;

      //Config cfg = new Config();

      //Console.WriteLine("Connection to sql : {0} @ {1}", cfg.SqlServer, cfg.Database);
      //Console.WriteLine();

      ////string baseaddr = "http://localhost:8080/Aig";
      //Uri baseAddress = new Uri(cfg.BaseUri);

      //// Create the ServiceHost.
      //using (ServiceHost host = new ServiceHost(typeof(Service), baseAddress))
      //{
      //  // Enable metadata publishing.
      //  ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
      //  smb.HttpGetEnabled = true;
      //  smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy12;
      //  smb.HttpGetUrl = baseAddress;
      //  //smb.MetadataExporter = new Soap11ConformantWsdlExporter();
      //  host.Description.Behaviors.Add(smb);

      //  ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>() as ServiceDebugBehavior;
      //  if (debug == null)
      //  {
      //    debug = new ServiceDebugBehavior();

      //    debug.IncludeExceptionDetailInFaults = true;

      //    host.Description.Behaviors.Add(debug);
      //  }
      //  else
      //  {
      //    debug.IncludeExceptionDetailInFaults = true;
      //  }

      //  ServiceThrottlingBehavior throttle = host.Description.Behaviors.Find<ServiceThrottlingBehavior>() as ServiceThrottlingBehavior;
      //  if (throttle == null)
      //  {
      //    throttle = new ServiceThrottlingBehavior()
      //    {
      //      MaxConcurrentCalls = 12,
      //      MaxConcurrentInstances = 56,
      //      MaxConcurrentSessions = 34
      //    };

      //    host.Description.Behaviors.Add(throttle);
      //  }
      //  else
      //  {
      //    throttle.MaxConcurrentCalls = 12;
      //    throttle.MaxConcurrentInstances = 56;
      //    throttle.MaxConcurrentSessions = 34;
      //  }

      //  ServiceEndpoint endPoint = null;
      //  CustomBinding binding = null;

      //  if (bActivateJsonP)
      //  {
      //    binding = new CustomBinding();

      //    JSONPBindingElement jsonpBE = new JSONPBindingElement();
      //    binding.Elements.Add(jsonpBE);

      //    HttpTransportBindingElement bindingelemt = new HttpTransportBindingElement();
      //    bindingelemt.ManualAddressing = true;
      //    binding.Elements.Add(bindingelemt);

      //    endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), binding, "WebJsonP");

      //    endPoint.Behaviors.Add(new WebQueryStringBehaviour()
      //      {
      //        DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json
      //      });

      //    Console.WriteLine("Activate JSON Padding...");
      //  }

      //  Binding bind = new BasicHttpBinding();
      //  endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), bind, "Soap");

      //  endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), new WebHttpBinding(), "Web");
      //  //whb = new WebHttpBehavior();
      //  endPoint.Behaviors.Add(new WebQueryStringBehaviour());

      //  endPoint = host.AddServiceEndpoint(typeof(ScmsSoaLibraryInterface.IScmsSoaLibrary), new WSHttpBinding(), "Ws");

      //  try
      //  {
      //    //for some reason a default endpoint does not get created here
      //    host.Open();

      //    Console.WriteLine("The service is ready at {0}", baseAddress);
      //    Console.WriteLine("Press <Enter> to stop the service.");

      //    Console.WriteLine();
      //  }
      //  catch (Exception ex)
      //  {
      //    Console.WriteLine(ex.Message);
      //  }

      //  Console.Write("Press ENTER to close the host");
      //  Console.ReadLine();

      //  host.Close();
      //}

      #endregion

      Console.WriteLine("Auto Response Turn off...");
      Console.WriteLine("Press ENTER to close the host");
      Console.WriteLine();
      Console.ReadLine();
      

      #region Production

      //if (sob != null)
      //{
      //  sob = null;
      //}
      //if (sar != null)
      //{
      //  sar.Stop();
      //}
      //if (spoa != null)
      //{
      //  spoa.Stop();
      //}
      if (dopr != null)
      {
          dopr.Stop();
      }
      if (dophar != null)
      {
          dophar.Stop();
      }
      if (srca != null)
      {
          srca.Stop();
      }
      if (spc != null)
      {
        spc.Stop();
      }
      if (sph != null)
      {
          sph.Stop();
      }
      if (sob != null)
      {
        sob.Stop();
      }

      #endregion

      if (hostSoa != null)
      {
        hostSoa.Close();
      }
      if (hostRpt != null)
      {
        hostRpt.Close();
      }
    }
  }

  class EqualityKey
  {
    public char c_gdg { get; set; }
    public string c_iteno { get; set; }
    public string c_batch { get; set; }
  }

  class EqualComparer : IEqualityComparer<EqualityKey>
  {
    #region IEqualityComparer<EqualityKey> Members

    public bool Equals(EqualityKey x, EqualityKey y)
    {
      var xx = "";

      return ((x == null) || (y == null) ? false :
        x.c_gdg.Equals(y.c_gdg) &&
        (string.IsNullOrEmpty(x.c_iteno) ? string.Empty : x.c_iteno.Trim()).Equals((string.IsNullOrEmpty(y.c_iteno) ? string.Empty : y.c_iteno.Trim()), StringComparison.OrdinalIgnoreCase) &&
        (string.IsNullOrEmpty(x.c_batch) ? string.Empty : x.c_batch.Trim()).Equals((string.IsNullOrEmpty(y.c_batch) ? string.Empty : y.c_batch.Trim()), StringComparison.OrdinalIgnoreCase));

      return false;
    }

    public int GetHashCode(EqualityKey obj)
    {
      return obj.GetHashCode();
    }

    #endregion
  }
}