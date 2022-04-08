using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;

public partial class proses_snapshot_bdprdp : Scms.Web.Core.PageHandler
{
  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      DateTime date = DateTime.Now;

      Functional.PopulateBulan(cbBulan, date.Month);

      Functional.PopulateTahun(cbTahun, date.Year, 1, 0);
    }
  }
}
