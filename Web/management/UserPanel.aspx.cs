using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ext.Net;
using Scms.Web.Common;
using ScmsSoaLibraryInterface.Components;
using Scms.Web.Core;
using System.Text;
using System.Globalization;
using System.IO;

public partial class management_UserPanel : Scms.Web.Core.PageHandler
{
  #region Private

  private void ClearEntrys()
  {
    //winDetail.Title = "User Detail";
    
    hfClearPic.Clear();
    hfClearWall.Clear();

    txNama.Clear();
    
    txPwdLama.Clear();

    txPwdBaru1.Clear();
    txPwdBaru2.Clear();

    fuYourPic.Clear();
    hfClearPic.Clear();

    fuYourWall.Clear();
    hfClearWall.Clear();
  }

  private PostDataParser.StructureResponse SaveParser(string nip, string nama, string pass, string passNew, string imgPic, string imgWallp, bool clearPic, bool clearWall)
  {
    PostDataParser.StructureResponse responseResult = default(PostDataParser.StructureResponse);

    PostDataParser parser = new PostDataParser();
    IDictionary<string, PostDataParser.StructurePair> dic = new Dictionary<string, PostDataParser.StructurePair>(StringComparer.OrdinalIgnoreCase);

    dic.Add("Nip", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = nip
    });
    dic.Add("Nama", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = nama.Trim()
    });
    dic.Add("OldPassword", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = pass.Trim()
    });
    dic.Add("User", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = nip.Trim()
    });
    dic.Add("Password", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = passNew
    });
    dic.Add("ImagePic", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = imgPic
    });
    dic.Add("ImageWallpaper", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = imgWallp
    });
    dic.Add("ClearPic", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = clearPic.ToString().ToLower()
    });
    dic.Add("ClearWall", new PostDataParser.StructurePair()
    {
      IsSet = true,
      Value = clearWall.ToString().ToLower()
    });

    string varData = null;

    try
    {
      varData = parser.ParserData("User", "UserPanel", dic);
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("management_UserPanel SaveParser : {0} ", ex.Message);
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

  private bool SaveImageTemp(string tempName, HttpPostedFile postFile, out string fileOut)
  {
    fileOut = null;

    string fullTemp = null,
      filePath = null,
      extFile = null,
      fName = postFile.FileName.ToLower();

    bool bOk = false;

    string[] arrExt = new string[] {
      ".bmp", ".jpg", ".png", ".gif"
    };

    try
    {
      fullTemp = this.Server.MapPath(Scms.Web.Common.Constant.PATH_LOCATION_USER_IMAGE);
      extFile = Path.GetExtension(postFile.FileName);
      tempName = Path.ChangeExtension(tempName, extFile);
      filePath = Path.Combine(fullTemp, tempName);

      if (Array.IndexOf<string>(arrExt, extFile) != -1)
      {
        if (Directory.Exists(fullTemp))
        {
          if (File.Exists(filePath))
          {
            File.Delete(filePath);
          }

          postFile.SaveAs(filePath);

          fileOut = tempName;

          bOk = true;
        }
      }
    }
    catch (Exception ex)
    {
      Scms.Web.Common.Logger.WriteLine("management_UserPanel SaveImageTemp : {0} ", ex.Message);
    }

    return bOk;
  }

  private bool CekImage(bool isWallpaper)
  {
    bool bOk = false;

    return bOk;
  }

  #endregion

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!this.IsPostBack)
    {
      txNama.Text = this.Username;

      //this.ClearEntrys();
    }
  }

  protected void SaveBtn_Click(object sender, DirectEventArgs e)
  {
    string sNip = this.Nip,
      tmp = null;

    bool clearPic = false,
      clearWall = false;

    string pwd = (e.ExtraParams["Pwd"] ?? string.Empty);
    string pwdNew = (e.ExtraParams["PwdNew"] ?? string.Empty);
    string strName = (e.ExtraParams["Nama"] ?? string.Empty);

    tmp = (e.ExtraParams["ClearPic"] ?? string.Empty);
    bool.TryParse(tmp, out clearPic);

    tmp = (e.ExtraParams["ClearWall"] ?? string.Empty);
    bool.TryParse(tmp, out clearWall);

    string imgPic = null,
      imgWall = null;

    imgPic = string.Concat(sNip.ToLower(), "_pic.img").Replace(" ", "_");
    if (fuYourPic.HasFile)
    {
      if (!SaveImageTemp(imgPic, fuYourPic.PostedFile, out imgPic))
      {
        imgPic = null;
      }
    }
    else
    {
      imgPic = null;
    }

    imgWall = string.Concat(sNip.ToLower(), "_wall.img").Replace(" ", "_");
    if (fuYourWall.HasFile)
    {
      if (!SaveImageTemp(imgWall, fuYourWall.PostedFile, out imgWall))
      {
        imgWall = null;
      }
    }
    else
    {
      imgWall = null;
    }

    PostDataParser.StructureResponse respon = SaveParser(sNip, strName, pwd, pwdNew, imgPic, imgWall, clearPic, clearWall);

    if (respon.IsSet)
    {
      if (respon.Response == PostDataParser.ResponseStatus.Success)
      {
        //string scrpt = null;

        //string cust = (cbCustomerHdr.SelectedIndex != -1 ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        //string supl = (cbPrincipalHdr.SelectedIndex != -1 ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

        //string cust = (cbCustomerHdr.SelectedItem != null ? cbCustomerHdr.SelectedItem.Text : string.Empty);
        //string supl = (cbPrincipalHdr.SelectedItem != null ? cbPrincipalHdr.SelectedItem.Text : string.Empty);

        //string dateJs = null;
        //DateTime date = DateTime.Today;

//        if (isAdd)
//        {
//          if (respon.Values != null)
//          {
//            if (Functional.DateParser(respon.Values.GetValueParser<string>("Tanggal", string.Empty), "yyyyMMdd", out date))
//            {
//              dateJs = Functional.DateToJson(date);
//            }

//            if (!string.IsNullOrEmpty(storeId))
//            {
//              scrpt = string.Format(@"{0}.insert(0, new Ext.data.Record({{
//                'c_plno': '{1}',
//                'd_pldate': {2},
//                'v_gdgdesc': '{3}',
//                'v_cunam': '{4}',
//                'v_nama': '{5}',
//                'l_print': false,
//                'l_confirm': false,
//                'L_DO': false
//              }}));{0}.commitChanges();", storeId, respon.Values.GetValueParser<string>("PL", string.Empty),
//                       dateJs, hfGudangDesc.Text, cust, supl);

//              X.AddScript(scrpt);
//            }
//          }
//        }

        //this.ClearEntrys();

        Functional.ShowMsgInformation("Data berhasil tersimpan, keluar dulu jika ingin melihat perubahan.");
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

  protected void BtnShowPic_OnClick(object sender, DirectEventArgs e)
  {
    wnd.Title = "Foto";
    wnd.Hidden = false;
    wnd.LoadContent(new LoadConfig(this.ResolveClientUrl(string.Concat("~/Images.aspx?m=user&f=", this.ImagePicName)), LoadMode.IFrame, true));
    wnd.ShowModal();
  }

  protected void BtnShowWall_OnClick(object sender, DirectEventArgs e)
  {
    wnd.Title = "Wallpaper";
    wnd.Hidden = false;
    wnd.LoadContent(new LoadConfig(this.ResolveClientUrl(string.Concat("~/Images.aspx?m=user&f=", this.ImageWallpaperName)), LoadMode.IFrame, true));
    wnd.ShowModal();
  }
}
