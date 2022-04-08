using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using Ext.Net;

public partial class transaksi_penjualan_SuratPesananPrintCtrl : System.Web.UI.UserControl
{
  protected void Page_Load(object sender, EventArgs e)
  {
    //if (!Functional.IsAllowView(this.Page as Scms.Web.Core.PageHandler))
    //{
    //  return;
    //}
      if (!this.IsPostBack)
      {
          Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

          if (pag.IsCabang)
          {
              Functional.SetComboData(cbCustomer, "c_cusno", pag.ActiveGudangDescription, pag.ActiveGudang);
              cbCustomer.Disabled = true;
          }
      }
  }

  public void ShowPrintPage()
  {
    winPrintDetail.Title = "Cetak Surat Pesanan";

    winPrintDetail.Hidden = false;
    winPrintDetail.ShowModal();

    txPeriode1.Disabled = true;
    txPeriode2.Disabled = true;
    txPeriode1.Text = DateTime.Now.ToString("dd-MM-yyyy");
    txPeriode2.Text = DateTime.Now.ToString("dd-MM-yyyy");
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  protected void Report_OnGenerate(object sender, DirectEventArgs e)
  {
    Scms.Web.Core.PageHandler pag = this.Page as Scms.Web.Core.PageHandler;

    if (!pag.IsAllowPrinting)
    {
      Functional.ShowMsgError("Maaf, anda tidak mempunyai hak akses untuk mencetak data.");
      return;
    }

    string custId = (e.ExtraParams["CustomeID"] ?? string.Empty);
    string sp1 = (e.ExtraParams["SPID1"] ?? string.Empty);
    string sp2 = (e.ExtraParams["SPID2"] ?? string.Empty);
    string tmp = (e.ExtraParams["Async"] ?? string.Empty);

    if (string.IsNullOrEmpty(custId) &&
      string.IsNullOrEmpty(sp1) && string.IsNullOrEmpty(sp2) && rdgKPDF.Checked)
    {
      Functional.ShowMsgError("Maaf, kriteria tidak dapat dibaca.");
      return;
    }

    ReportParser rptParse = new ReportParser();

    List<ReportParameter> lstRptParam = new List<ReportParameter>();

    List<string> lstData = new List<string>();
    bool isAsync = false;

    bool.TryParse(tmp, out isAsync);

    rptParse.ReportingID = "10113";

    if (rdgKPDF.Checked)
    {
        #region Linq Filter Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "c_cusno = @0",
            ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId),
            IsLinqFilterParameter = true
        });

        if (!string.IsNullOrEmpty(sp1))
        {
            if (string.IsNullOrEmpty(sp2))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "c_spno = @0",
                    ParameterValue = (string.IsNullOrEmpty(sp1) ? string.Empty : sp1),
                    IsLinqFilterParameter = true
                });
            }
            else
            {
                if (sp1.CompareTo(sp2) >= 0)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = "c_spno = @0",
                        ParameterValue = (string.IsNullOrEmpty(sp1) ? string.Empty : sp1),
                        IsLinqFilterParameter = true
                    });
                }
                else
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = "c_spno",
                        ParameterValue = (string.IsNullOrEmpty(sp1) ? string.Empty : sp1),
                        IsLinqFilterParameter = true,
                        BetweenValue = (string.IsNullOrEmpty(sp2) ? string.Empty : sp2)
                    });
                }
            }
        }

        #endregion

        #region Report Parameter

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_SPH.c_cusno",
            ParameterValue = (string.IsNullOrEmpty(custId) ? string.Empty : custId)
        });

        if (!string.IsNullOrEmpty(sp1))
        {
            if (string.IsNullOrEmpty(sp2))
            {
                lstRptParam.Add(new ReportParameter()
                {
                    DataType = typeof(string).FullName,
                    ParameterName = "LG_SPH.c_spno",
                    ParameterValue = (string.IsNullOrEmpty(sp1) ? string.Empty : sp1)
                });
            }
            else
            {
                if (sp1.CompareTo(sp2) >= 0)
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = "LG_SPH.c_spno",
                        ParameterValue = (string.IsNullOrEmpty(sp1) ? string.Empty : sp1)
                    });
                }
                else
                {
                    lstRptParam.Add(new ReportParameter()
                    {
                        DataType = typeof(string).FullName,
                        ParameterName = string.Format("({{LG_SPH.c_spno}} IN ('{0}' TO '{1}'))", sp1, sp2),
                        IsReportDirectValue = true
                    });
                }
            }
        }

        #endregion

        #region TipeReport

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "tipereport",
            IsSqlParameter = true,
            ParameterValue = "01"
        });

        #endregion
    }
    else
    {
        #region Periode SP

        DateTime date1 = DateTime.Today,
                 date2 = DateTime.Today;

        if (Scms.Web.Common.Functional.DateParser(txPeriode1.RawText.Trim(), "d-M-yyyy", out date1))
        {
            if (Scms.Web.Common.Functional.DateParser(txPeriode2.RawText.Trim(), "d-M-yyyy", out date2))
            {
                if (date2.CompareTo(date1) <= 0)
                {
                    date2 = date1;
                }
            }
            else
            {
                date2 = date1;
            }

            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "date1",
                IsSqlParameter = true,
                ParameterValue = date1.ToString("d-M-yyyy")
            });
            lstRptParam.Add(new ReportParameter()
            {
                DataType = typeof(DateTime).FullName,
                ParameterName = "date2",
                IsSqlParameter = true,
                ParameterValue = date2.ToString("d-M-yyyy")
            });
        }

        #endregion

        #region CabangReport

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "cabang",
            IsSqlParameter = true,
            ParameterValue = (string.IsNullOrEmpty(custId) ? "0000" : custId)
        });

        #endregion

        #region User

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "user",
            IsSqlParameter = true,
            ParameterValue = pag.Nip
        });

        #endregion

        #region TipeReport

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(DateTime).FullName,
            ParameterName = "TipeReport",
            IsSqlParameter = true,
            ParameterValue = "02"
        });

        #endregion

	#region Print by Nip

        lstRptParam.Add(new ReportParameter()
        {
            DataType = typeof(string).FullName,
            ParameterName = "LG_SURATPESANAN_GRID.NIP",
            ParameterValue = pag.Nip
        });

        #endregion
    }

    rptParse.PaperID = "Letter";
    rptParse.ReportParameter = lstRptParam.ToArray();
    rptParse.User = pag.Nip;
    //Indra 20190327FM
    rptParse.OutputReport = ReportParser.ParsingOutputReport((rdgKPDF.Checked ? "01" : "02"));

    string xmlFiles = ReportParser.Deserialize(rptParse);

    SoaReportCaller soa = new SoaReportCaller();

    string result = soa.GeneratorReport(isAsync, xmlFiles);

    ReportingResult rptResult = ReportingResult.Serialize(result);

    if (rptResult == null)
    {
      Functional.ShowMsgError("Pembuatan report gagal.");
    }
    else
    {
      if (rptResult.IsSuccess)
      {
        if (isAsync)
        {
          Functional.ShowMsgInformation("Report sedang dibuat, silahkan cek di halaman report beberapa saat lagi.");
        }
        else
        {
          //string rptName = string.Concat("Packing_List_", pag.Nip, ".", rptResult.Extension);

          //string tmpUri = this.ResolveClientUrl("~/Viewer.aspx");
          //tmpUri = string.Format("{0}?o={1}&f={2}&p={3}&c={4}&dwnl=1",
          //  tmpUri, rptName, rptResult.OutputFile, "Reports", rptResult.Extension);

          string tmpUri = Functional.UriDownloadGenerator(pag,
            rptResult.OutputFile, "Surat Pesanan", rptResult.Extension);

          wndDown.LoadContent(new LoadConfig(tmpUri, LoadMode.IFrame, true));
        }
      }
      else
      {
        Functional.ShowMsgWarning(rptResult.MessageResponse);
      }
    }

    GC.Collect();
  }
}
