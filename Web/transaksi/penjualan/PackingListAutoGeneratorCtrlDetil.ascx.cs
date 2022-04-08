using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Xml;


public class PanelType1 : Ext.Net.Panel
{
  public PanelType1(int type)
  {
    this.ID = "id_paneltype_1";
    this.Title = "Video ";

    if (type == 1)
      this.Title += "MP3" + "... Updated on " + DateTime.Now;
    else
      this.Title += "WMV" + "... Updated on " + DateTime.Now;

    this.Height = 300;

    BodyStyle = "background:#FF0000";

    this.Items.Add(new TabPanel
    {
      Items = {
                        new Ext.Net.Panel { Title="tab1",BodyStyle = "background:#00FF00" },
                        new Ext.Net.Panel { Title="tab2",BodyStyle = "background:#0000FF" } }
    });

    Ext.Net.Button b = new Ext.Net.Button { ID = "panel1_removeBtn", Text = "Remove this panel" };

    this.TopBar.Add(new Toolbar { Items = { b } });
  }
}

public class PanelType2 : Ext.Net.Panel
{
  Ext.Net.Button bTn1;
  public PanelType2()
  {
    this.ID = "id_paneltype_2";
    this.Title = "Image " + "... Updated on " + DateTime.Now;
    this.Height = 600;
    this.EnableViewState = false;
    BodyStyle = "background:#0F0FF0";
    this.Add(new TabPanel
    {
      EnableViewState = true,
      Items = {
                       new Ext.Net.Panel { ID="id_infopan2",Title="Online",BodyStyle = "background:#00FF00" },
                       new Ext.Net.Panel { ID="id_infopan3",Title="Offline",BodyStyle = "background:#0000FF" } }
    });

    //////////////////////////////////////////////////////////////////////////////////////////////
    // Removing of this line makes application to work
    this.Add(new Ext.Net.Panel { EnableViewState = true, ID = "id_infopan1", Title = "Info", BodyStyle = "background:#F0F0F0" });
    //////////////////////////////////////////////////////////////////////////////////////////////

    bTn1 = new Ext.Net.Button { ID = "panel2_btnHelp", Text = "Help", EnableToggle = true, Pressed = false };
    bTn1.DirectEvents.Click.Event += btn2_click;
    this.TopBar.Add(new Toolbar { ID = "id_infopan4", EnableViewState = true, Items = { bTn1 } });

  }

  public void btn2_click(object sender, DirectEventArgs e)
  {
    return;
  }

}

public partial class transaksi_penjualan_PackingListAutoGeneratorCtrlDetil : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {

  }

  private void populateData(string sId)
  {

    
  }

  public void CommandPopulate(bool isAdd, string pID, string plVia, string plSup)
  {
    if (isAdd)
    {
      Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

      hfGudangDet.Text = pag.ActiveGudang;
      hfNip.Text = pag.Nip;

      populateData(pID);

      string tmp = null;

      Ext.Net.Store store = gridDetail.GetStore();

      tmp = string.Format(@"var xOpts = {{
                                  params: {{
                                      start: '0',
                                      limit: '-1',
                                      allQuery: 'true',
                                      model: '0203',
                                      sort: '',
                                      dir: '',
                                      parameters: [['nip', paramValueGetter({0}), 'System.String'],
                                                  ['nosup', '{1}', 'System.String'],
                                                  ['gdg', paramValueGetter({2}), 'System.Char']]
                                    }}
                                  }};", hfNip.ClientID, plSup, hfGudangDet.ClientID);

      X.AddScript(tmp);
      X.AddScript(string.Format("{0}.removeAll();{0}.reload(xOpts);", store.ClientID));

      winDetail.Hidden = false;
      winDetail.Visible = true;
      winDetail.ShowModal();
      winDetail.Show();

      GC.Collect();

      //PackingListCtrl1.Initialize(pag.ActiveGudang, pag.ActiveGudangDescription, Store1.ClientID);

    }
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void showDetil(object sender, DirectEventArgs e)
  {
    string sId = (e.ExtraParams["sId"] ?? string.Empty);

    string sPlNo = sId.Substring(9, 10);

    if (sPlNo.StartsWith("PL"))
    {
      PackingListCtrl1.CommandPopulate(false, sPlNo);
    }
  }

  private void Report_OnGenerate(Dictionary<string, string>[] gridDataAll,bool isAuto)
  {
    Dictionary<string, string> dicData = null,
     dicDataTmp = null;
    PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

    PostDataParser parser = new PostDataParser();

    Dictionary<string, string> dicAttr = null;

    string noPl = null,
      tmp = null,
      noPlAll = null,
      plChck = null;
    string varData = null;

    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    List<ReportParameter> lstRptParam = new List<ReportParameter>();
    ReportParser rptParse = new ReportParser();

    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    rptParse.ReportingID = "10201";
    //rptParse.ReportingID = "10101";

    for (int nLoop = 0, nLen = gridDataAll.Length; nLoop < nLen; nLoop++)
    {
      tmp = nLoop.ToString();

      dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      dicDataTmp = gridDataAll[nLoop];

      plChck = dicDataTmp.GetValueParser<string>("c_plno");

      if (plChck == noPl)
      {
        continue;
      }

      noPl = dicDataTmp.GetValueParser<string>("c_plno");

      noPlAll += string.Concat(noPl, ",");
      //noPlAll += string.Concat("'",noPl, "',");
    }

    noPlAll = noPlAll.Remove((noPlAll.Length - 1), 1);

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(char).FullName,
      ParameterName = "LG_PLH.c_gdg",
      ParameterValue = (string.IsNullOrEmpty(pag.ActiveGudang) ? string.Empty : pag.ActiveGudang)
    });

    lstRptParam.Add(new ReportParameter()
    {
      DataType = typeof(string).FullName,
      ParameterName = "LG_PLH.c_plno",
      ParameterValue = (string.IsNullOrEmpty(noPlAll) ? string.Empty : noPlAll),
      IsBetweenValue = true,
    });

    rptParse.PaperID = "8.5x5.5";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.IsAutoPrint = isAuto;
    rptParse.User = pag.Nip;
    rptParse.UserDefinedName = "PRINTPLALL";
    rptParse.OutputReport = PopulateMode.pmToWord;

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    string result = null;

    if (isAuto)
    {
      result = soa.GeneratorReport(true,xmlFiles);
    }
    else
    {
      result = soa.GeneratorReport(xmlFiles);
    }
    

    ReportingResult rptResult = ReportingResult.Serialize(result);

    if (rptResult == null)
    {
      Functional.ShowMsgError("Pembuatan report gagal.");
    }
    else
    {
      if (rptResult.IsSuccess)
      {

        string tmpUri = Functional.UriDownloadGenerator(pag,
          rptResult.OutputFile, "Packing List Auto Generator", rptResult.Extension);

        //wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void PrintBtn_Click(object sender, DirectEventArgs e)
  {
    string iId = (e.ExtraParams["gridValuesAll"] ?? string.Empty);

    Dictionary<string, string>[] gridDataAll = JSON.Deserialize<Dictionary<string, string>[]>(iId);

    Report_OnGenerate(gridDataAll, false);
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void PrintBtnAuto_Click(object sender, DirectEventArgs e)
  {
    string iId = (e.ExtraParams["gridValuesAll"] ?? string.Empty);

    Dictionary<string, string>[] gridDataAll = JSON.Deserialize<Dictionary<string, string>[]>(iId);

    Report_OnGenerate(gridDataAll, true);
  }
}
