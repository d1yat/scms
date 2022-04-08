using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using Scms.Web.Core;
using System.Linq;
using ScmsSoaLibraryInterface.Components;
using System.Collections;

public partial class transaksi_pengiriman_Return_DO : Scms.Web.Core.PageHandler
{
    #region Private

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            hfGdg.Text = this.ActiveGudang;
            hfNip.Text = this.Nip;
        }
    }


    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void SaveBtn_Click(object sender, DirectEventArgs e)
    {
        string jsonGridValues = (e.ExtraParams["gridValues"] ?? string.Empty);
        string gdg = (e.ExtraParams["Gudang"] ?? string.Empty);

        Dictionary<string, string>[] gridData = JSON.Deserialize<Dictionary<string, string>[]>(jsonGridValues);

        bool isAdd = false;

        PostDataParser.StructureResponse respon = SaveParser(isAdd, gridData, gdg);

        if (respon.IsSet)
      {
          if (respon.Response == PostDataParser.ResponseStatus.Success)
          {
              Ext.Net.Store store = gridMain.GetStore();
              X.AddScript(string.Format("{0}.removeAll();{0}.reload();", store.ClientID));
              Functional.ShowMsgInformation("Data berhasil tersimpan.");

              txTglBalik.Clear();
              txTglTerima.Clear();
              cbReturnDO.Clear();
              Ext.Net.Store cbReturnDOstr = cbReturnDO.GetStore();
              if (cbReturnDOstr != null)
              {
                  cbReturnDOstr.RemoveAll();
              }
          }
          else
          {
              e.ErrorMessage = respon.Message;

              e.Success = false;
          }
      }
        else
        {
            e.ErrorMessage = "Unknown response";

            e.Success = false;
        }
    }

    private PostDataParser.StructureResponse SaveParser(bool isAdd, Dictionary<string, string>[] dics, string gudang)
    {
        PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

        PostDataParser parser = new PostDataParser();
        IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

        Dictionary<string, string> dicData = null;

        PostDataParser.StructurePair pair = new PostDataParser.StructurePair();

        Dictionary<string, string> dicAttr = null;

        pair.IsSet = true;
        pair.IsList = true;
        pair.Value = "returnDO";
        pair.DicValues = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicValuesExtra = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);
        pair.DicAttributeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string tmp = null,
            varData = null,
            ketVoid = null,
            dono = null,
            tglterima = null,
            tglbalik = null;
        
        bool isNew = false,
          isModify = false,
          isVoid = false;


        DateTime date = DateTime.Today;

        dic.Add("ID", pair);
        pair.DicAttributeValues.Add("Entry", ((Scms.Web.Core.PageHandler)this.Page).Nip);
        pair.DicAttributeValues.Add("Gudang", (string.IsNullOrEmpty(gudang) ? "1" : gudang.Trim()));

        int nLoop = 0,
          nLen = 0;

        #region Grid Details

        for (nLoop = 0, nLen = dics.Length; nLoop < nLen; nLoop++)
        {
            tmp = nLoop.ToString();

            dicData = dics[nLoop];

            dicAttr = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            isNew = dicData.GetValueParser<bool>("l_new");
            isModify = dicData.GetValueParser<bool>("l_modified");
            isVoid = dicData.GetValueParser<bool>("l_void");

            if (isNew && (!isVoid) && (!isModify))
            {
                dicAttr.Add("New", isNew.ToString().ToLower());
                dicAttr.Add("Delete", isVoid.ToString().ToLower());
                dicAttr.Add("Modified", isModify.ToString().ToLower());

                dono = dicData.GetValueParser<string>("c_dono");
                tglterima = dicData.GetValueParser<string>("d_terima");
                tglbalik = dicData.GetValueParser<string>("d_balik");

                dicAttr.Add("dono", dono);
                               
                date = DateTime.Parse(tglterima);
                dicAttr.Add("tglTerima", date.ToString("yyyyMMdd"));

                date = DateTime.Parse(tglbalik);
                dicAttr.Add("tglBalik", date.ToString("yyyyMMdd"));

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    DicAttributeValues = dicAttr
                });
            }
            else if ((!isModify) && isVoid && (!isNew))
            {
                dicAttr.Add("New", isNew.ToString().ToLower());
                dicAttr.Add("Delete", isVoid.ToString().ToLower());
                dicAttr.Add("Modified", isModify.ToString().ToLower());

                dono = dicData.GetValueParser<string>("c_dono");
                tglterima = dicData.GetValueParser<string>("d_terima");
                tglbalik = dicData.GetValueParser<string>("d_balik");

                dicAttr.Add("dono", dono);
                Functional.DateParser(tglterima, "yyyy-MM-ddTHH:mm:ss", out date);
                dicAttr.Add("tglTerima", date.ToString("yyyyMMdd"));
                Functional.DateParser(tglbalik, "yyyy-MM-ddTHH:mm:ss", out date);
                dicAttr.Add("tglBalik", date.ToString("yyyyMMdd"));

                pair.DicValues.Add(tmp, new PostDataParser.StructurePair()
                {
                    IsSet = true,
                    Value = (string.IsNullOrEmpty(ketVoid) ? "Human error" : ketVoid),
                    DicAttributeValues = dicAttr
                });
            
            }

            dicData.Clear();
        }

        #endregion

        try
        {
            varData = parser.ParserData("ClassReturnDO", (isAdd ? "Add" : "Modify"), dic);
        }
        catch (Exception ex)
        {
            Scms.Web.Common.Logger.WriteLine("transaksi_InvoiceEkspedisiCtrl SaveParser : {0} ", ex.Message);
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

    [Ext.Net.DirectMethod(Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
    protected void Submit_scan(object sender, DirectEventArgs e)
    {
        string s = e.ExtraParams["DO"].ToString(),
        gdg = e.ExtraParams["Gudang"].ToString(),
        gdgdesc = "Gudang " + gdg;

        if (!string.IsNullOrEmpty(s) && (s.Length == 10))
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string jsonGridValues = (e.ExtraParams["Grid"] ?? string.Empty);
            bool isExist = jsonGridValues.Contains(s);

            if (!isExist)
            {
                sb.AppendFormat(@"{0}.insert(0, new Ext.data.Record({{
                                    'v_gdgdesc': '{1}',
                                    'c_dono': '{2}',
                                    'd_terima': '{3}',
                                    'd_balik': '{4}',
                                    'l_new': true,
                                  }})); ", gridMain.GetStore().ClientID, gdgdesc, s, txTglTerima.Text, txTglBalik.Text);

                X.AddScript(sb.ToString());
            }
            else
            {
                Functional.ShowMsgInformation("Data telah ada.");
            }
        }

        cbReturnDO.Clear();
    }
    
}