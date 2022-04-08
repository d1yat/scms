using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using System.IO;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Common;

public partial class reporting_Default : Scms.Web.Core.PageHandler
{
  public class ResultDeleteReport
  {
    public int Idx { get; set; }
    public bool IsSuccess { get; set; }
  }

  #region Private

  private PostDataParser.StructureResponse SaveParser(int Index)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    dic.Add("Idx", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = Index.ToString()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = "105839H"
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("Reporting", "Modify", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("reporting_Default SaveParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  private static PostDataParser.StructureResponse DeleteParser(int Index, string nipUser)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    dic.Add("Idx", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = Index.ToString()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = nipUser
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("Reporting", "Delete", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("reporting_Default DeleteParser : {0} ", ex.Message);
    }

    string result = null;

    if (!string.IsNullOrEmpty(varData))
    {
      Scms.Web.Core.SoaCaller soa = new Scms.Web.Core.SoaCaller();

      result = soa.PostData(varData);

      responseResult = parser.ResponseParser(result);
    }

    return responseResult;
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {
    hfActiveUser.Text = this.Nip;
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void gridMain_Command(object sender, DirectEventArgs e)
  {
    string cmd = (e.ExtraParams["Command"] ?? string.Empty);
    string tmp = (e.ExtraParams["Index"] ?? string.Empty);
    string rpt = (e.ExtraParams["Report"] ?? string.Empty);
    string ftype = (e.ExtraParams["Type"] ?? string.Empty);
    string rptName = (e.ExtraParams["Name"] ?? string.Empty);
    string userEntry = (e.ExtraParams["Entry"] ?? string.Empty);
    
    int Idx = 0;

    if (!int.TryParse(tmp, out Idx))
    {
      e.Success = false;
      e.ErrorMessage = "Tidak dapat parsing index report.";

      return;
    }
    
    tmp = (e.ExtraParams["RowIndex"] ?? string.Empty);
    
    int rowIndex = 0;

    if (!int.TryParse(tmp, out rowIndex))
    {
      rowIndex = -1;
    }

    tmp = (e.ExtraParams["JmlDownload"] ?? string.Empty);
    
    int jmlDownload = 0;
    int.TryParse(tmp, out jmlDownload);
    
    PostDataParser.StructureResponse response = default(PostDataParser.StructureResponse);

    string sPath = this.Server.MapPath("~/App_Data/Reports");
    Ext.Net.Store store = gridMain.GetStore();
    string reportFilePath = Path.Combine(sPath, rpt);

    if (cmd.Equals("Download", StringComparison.OrdinalIgnoreCase))
    {
      if (File.Exists(reportFilePath))
      {
        rptName = string.Format("{0}_{1}.{2}", userEntry, rptName.Replace(' ', '_'), ftype);

        string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
        tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
          tmpUri, rptName, rpt, "Reports", ftype);

        response = SaveParser(Idx);
        //response = new PostDataParser.StructureResponse()
        //{
        //  IsSet = true,
        //  Response = PostDataParser.ResponseStatus.Success
        //};

        if (response.IsSet)
        {
          if (response.Response == PostDataParser.ResponseStatus.Success)
          {
            if ((store != null) && (rowIndex != -1))
            {
              jmlDownload++;

              store.UpdateRecordField(rowIndex, "l_download", jmlDownload);
              store.CommitChanges();
            }
          }
          //else
          //{
          //  //Functional.ShowMsgWarning("Gagal dalam mengupdate jumlah download");
          //}
        }

        wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        e.Success = false;
        e.ErrorMessage = "Maaf, file tidak di temukan.";
      }
    }
    else if (cmd.Equals("Delete", StringComparison.OrdinalIgnoreCase))
    {
      response = DeleteParser(Idx, this.Nip);
      //response = new PostDataParser.StructureResponse()
      //  {
      //    IsSet = true,
      //    Response = PostDataParser.ResponseStatus.Success
      //  };

      if (response.IsSet)
      {
        if (response.Response == PostDataParser.ResponseStatus.Success)
        {
          FileInfo fi = new FileInfo(reportFilePath);

          try
          {
            if (fi.Exists)
            {
              fi.Delete();
            }
          }
          catch (Exception ex)
          {
            Logger.WriteLine("Delete file '{0}' failed - {1}", fi.Name, ex.Message);
          }

          if ((store != null) && (rowIndex != -1))
          {
            //store.RemoveRecord(rowIndex);

            X.AddScript(
              string.Format("var iRow = {0}.findExact('Idx', {1});if(iRow != -1) {{ {0}.removeAt(iRow); }}",
                store.ClientID, Idx));

            store.CommitChanges();
          }

          Functional.ShowMsgInformation("Data berhasil terhapus.");
        }
        else
        {
          e.ErrorMessage = response.Message;

          e.Success = false;
        }
      }
    }
    else
    {
      e.Success = false;
      e.ErrorMessage = "Unknown command id";
    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public static ResultDeleteReport DeleteReportMethod(string rptId, string rptName, string activeUser)
  {
    int rptIndex = 0;

    ResultDeleteReport rdr = new ResultDeleteReport();

    if (!int.TryParse(rptId, out rptIndex))
    {
      return rdr;
    }

    rdr.Idx = rptIndex;

    if (string.IsNullOrEmpty(activeUser))
    {
      return rdr;
    }
    else if (string.IsNullOrEmpty(rptName))
    {
      return rdr;
    }

    HttpServerUtility hs = HttpContext.Current.Server as HttpServerUtility;

    if (hs == null)
    {
      return rdr;
    }

    PostDataParser.StructureResponse response = default(PostDataParser.StructureResponse);

    string sPath = hs.MapPath("~/App_Data/Reports");
    string reportFilePath = Path.Combine(sPath, rptName);

    response = DeleteParser(rptIndex, activeUser);

    if (response.IsSet)
    {
      if (response.Response == PostDataParser.ResponseStatus.Success)
      {
        FileInfo fi = new FileInfo(reportFilePath);

        try
        {
          if (fi.Exists)
          {
            fi.Delete();
          }
        }
        catch (Exception ex)
        {
          Logger.WriteLine("Delete file '{0}' failed - {1}", fi.Name, ex.Message);
        }

        rdr.IsSuccess = true;
      }
    }

    return rdr;
  }
}
