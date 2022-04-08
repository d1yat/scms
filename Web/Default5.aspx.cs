using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Core;
using System.Reflection;

public partial class Default5 : System.Web.UI.Page
{
  protected void Page_Load(object sender, EventArgs e)
  {
    //Button1_Click(null, null);
  }

//  protected void Button1_Click_X(object sender, EventArgs e)
//  {
//    string fileX = @"<?xml version=""1.0""?>
//        <GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
//          <RightAccess>
//            <RightAccess>
//              <NodeName>PL</NodeName>
//              <NodeID>PL</NodeID>
//              <Pages>
//                <PageRightAccess>
//                  <Url>/web/transaksi/penjualan/PackingList.aspx</Url>
//                  <Name>Packing List</Name>
//                  <IsView>true</IsView>
//                  <IsPrint>false</IsPrint>
//                  <IsAdd>true</IsAdd>
//                  <IsEdit>false</IsEdit>
//                  <IsDelete>true</IsDelete>
//                </PageRightAccess>
//                <PageRightAccess>
//                  <Url>/web/transaksi/penjualan/PackingListConfirm.aspx</Url>
//                  <Name>Packing List Confirm</Name>
//                  <IsView>false</IsView>
//                  <IsPrint>false</IsPrint>
//                  <IsAdd>false</IsAdd>
//                  <IsEdit>false</IsEdit>
//                  <IsDelete>false</IsDelete>
//                </PageRightAccess>
//              </Pages>
//            </RightAccess>
//          </RightAccess>
//        </GroupAccess>";

//    string fileX1 = @"<?xml version=""1.0""?>
//<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
//  <RightAccess>
//    <RightAccess>
//      <NodeName>RPTIVT</NodeName>
//      <NodeID>RPTIVT_KG</NodeID>
//      <Pages>
//        <PageRightAccess>
//          <Url>/web/reporting/inventory/Default.aspx</Url>
//          <Name>Kartu Gudang</Name>
//          <IsView>true</IsView>
//          <IsPrint>false</IsPrint>
//          <IsAdd>false</IsAdd>
//          <IsEdit>false</IsEdit>
//          <IsDelete>false</IsDelete>
//        </PageRightAccess>
//      </Pages>
//    </RightAccess>
//    <RightAccess>
//      <NodeName>PL</NodeName>
//      <NodeID>PL</NodeID>
//      <Pages>
//        <PageRightAccess>
//          <Url>/web/transaksi/penjualan/PackingList.aspx</Url>
//          <Name>Packing List</Name>
//          <IsView>false</IsView>
//          <IsPrint>false</IsPrint>
//          <IsAdd>false</IsAdd>
//          <IsEdit>false</IsEdit>
//          <IsDelete>false</IsDelete>
//        </PageRightAccess>
//        <PageRightAccess>
//          <Url>/web/transaksi/penjualan/PackingListConfirm.aspx</Url>
//          <Name>Packing List Confirm</Name>
//          <IsView>true</IsView>
//          <IsPrint>false</IsPrint>
//          <IsAdd>false</IsAdd>
//          <IsEdit>false</IsEdit>
//          <IsDelete>false</IsDelete>
//        </PageRightAccess>
//      </Pages>
//    </RightAccess>
//  </RightAccess>
//</GroupAccess>";

//    List<RightBuilder.GroupAccess> lstGA = new List<RightBuilder.GroupAccess>();

//    RightBuilder.GroupAccess ga = RightBuilder.GroupAccess.Serialize(fileX);
//    lstGA.Add(ga);

//    ga = RightBuilder.GroupAccess.Serialize(fileX1);
//    lstGA.Add(ga);

//    MenuBuilder mb = new MenuBuilder();

//    MenuBuilder.MenuConfiguration mc = mb.RebuildMenuConfiguration(StaticObjects.MenuConfiguration); ;

//    MenuBuilder.MenuModule mod = null;
//    Ext.Net.DesktopModule deskMod = null;
//    Ext.Net.MenuItem launcher = null;
//    MenuBuilder.MenuLauncer launch = null;
//    MenuBuilder.MenuItem item = null;
//    Ext.Net.Menu menuItems = null;
//    Ext.Net.MenuItem menu = null;
//    Ext.Net.Menu nestedMenu = null;
//    Ext.Net.MenuItem nestedMenuItems = null;
//    Ext.Net.MenuSeparator separator = null;
//    Ext.Net.DesktopWindow dw = null;
//    int nLoopC = 0,
//      nLenC = 0;
    
//    List<Ext.Net.DesktopModule> lstDesktop = new List<Ext.Net.DesktopModule>();
    
//    IDictionary<string, Ext.Net.DesktopWindow> dicDW = new Dictionary<string, Ext.Net.DesktopWindow>(StringComparer.OrdinalIgnoreCase);

//    for (int nLoop = 0, nLen = mc.Modules.Length; nLoop < nLen; nLoop++)
//    {
//      mod = mc.Modules[nLoop];

//      if ((mod != null) && (mod.Launcher != null))
//      {
//        deskMod = lstDesktop.Find(delegate(Ext.Net.DesktopModule dlgt_desk)
//        {
//          if (dlgt_desk.ModuleID.Equals(mod.ID))
//          {
//            return true;
//          }

//          return false;
//        });

//        if (deskMod == null)
//        {
//          deskMod = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
//          {
//            //AutoRun = mod.AutoRun,
//            ModuleID = mod.ID,
//            //WindowID = mod.WindowID
//          });
//        }
//        launcher = deskMod.Launcher;

//        if (mod.Launcher.Direct)
//        {
//          item = mod.Launcher.SingleItem;

//          if (!deskMod.WindowID.Equals(item.ID))
//          {
//            launcher.Text = mod.Launcher.Text;
//            launcher.Icon = (Ext.Net.Icon)mod.Launcher.Icon;
//            launcher.IconCls = (string.IsNullOrEmpty(mod.Launcher.CustomIcon) ? string.Empty : mod.Launcher.CustomIcon);

//            deskMod.WindowID = item.ID;

//            if (!dicDW.ContainsKey(item.ID))
//            {
//              dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
//              {
//                Title = item.Title,
//                Icon = (Ext.Net.Icon)item.Icon,
//                InitCenter = item.Center,
//                Width = Unit.Pixel(item.Width),
//                Height = Unit.Pixel(item.Height)
//              })
//              {
//                ID = item.WinID,
//                IconCls = (string.IsNullOrEmpty(item.CustomIcon) ? string.Empty : item.CustomIcon)
//              };

//              dw.AutoLoad.Url = (item.Url.StartsWith("~") ? this.ResolveUrl(item.Url) : item.Url);
//              dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
//              dw.AutoLoad.ShowMask = true;

//              dicDW.Add(item.WinID, dw);
//            }
//          }
//        }
//        else
//        {
//          launcher.Text = mod.Launcher.Text;
//          launcher.Icon = (Ext.Net.Icon)mod.Launcher.Icon;
//          launcher.IconCls = (string.IsNullOrEmpty(item.CustomIcon) ? string.Empty : item.CustomIcon);
//          launcher.Listeners.Click.Handler = "return false;";

//          launch = mod.Launcher;

//          if (launch.Nested)
//          {
//            if (launcher.Menu.Count > 0)
//            {
//              nestedMenu = launcher.Menu[0] as Ext.Net.Menu;
//            }
//            else
//            {
//              nestedMenu = new Ext.Net.Menu();

//              launcher.Menu.Add(nestedMenu);
//            }

//            if (nestedMenu.Items.Count > 0)
//            {
//              nestedMenuItems = nestedMenu.Items.Find(delegate(Ext.Net.Component c)
//              {
//                Ext.Net.MenuItem mnuItem = c as Ext.Net.MenuItem;
//                if (mnuItem.Text.Equals(launch.NestedText, StringComparison.OrdinalIgnoreCase) &&
//                  ((int)mnuItem.Icon == launch.NestedIcon))
//                {
//                  return true;
//                }

//                return false;
//              }) as Ext.Net.MenuItem;

//              if (nestedMenuItems != null)
//              {
//                menuItems = nestedMenuItems.Menu[0] as Ext.Net.Menu;
//              }
//            }

//            if (nestedMenuItems == null)
//            {
//              nestedMenuItems = new Ext.Net.MenuItem(launch.NestedText);
//              nestedMenuItems.Icon = (Ext.Net.Icon)launch.NestedIcon;
//              nestedMenuItems.IconCls = (string.IsNullOrEmpty(launch.CustomIcon) ? string.Empty : launch.CustomIcon);
//              nestedMenuItems.Listeners.Click.Handler = "return false;";

//              nestedMenu.Items.Add(nestedMenuItems);

//              menuItems = new Ext.Net.Menu();

//              nestedMenuItems.Menu.Add(menuItems);
//            }
//          }
//          else
//          {
//            menuItems = new Ext.Net.Menu();
//            launcher.Menu.Add(menuItems);
//          }

//          if ((launch.Items != null) && (launch.Items.Length > 0))
//          {
//            for (nLoopC = 0, nLenC = launch.Items.Length; nLoopC < nLenC; nLoopC++)
//            {
//              item = launch.Items[nLoopC];

//              if (item.Separator)
//              {
//                separator = new Ext.Net.MenuSeparator();

//                menuItems.Items.Add(separator);
//              }
//              else
//              {
//                menu = menuItems.Items.Find(delegate(Ext.Net.Component c)
//                {
//                  Ext.Net.MenuItem mnuItem = c as Ext.Net.MenuItem;
//                  if (mnuItem != null)
//                  {
//                    if (mnuItem.ID.Equals(item.ID, StringComparison.OrdinalIgnoreCase))
//                    {
//                      return true;
//                    }
//                  }

//                  return false;
//                }) as Ext.Net.MenuItem;

//                if (menu == null)
//                {
//                  menu = new Ext.Net.MenuItem();

//                  menu.ID = item.ID;
//                  menu.Text = item.Title;
//                  menu.Icon = (Ext.Net.Icon)item.Icon;

//                  if (!dicDW.ContainsKey(item.ID))
//                  {
//                    dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
//                    {
//                      Title = item.Title,
//                      Icon = (Ext.Net.Icon)item.Icon,
//                      InitCenter = item.Center,
//                      Width = Unit.Pixel(item.Width),
//                      Height = Unit.Pixel(item.Height)
//                    })
//                    {
//                      ID = item.WinID
//                    };

//                    //ph.Controls.Add(dw);

//                    dw.AutoLoad.Url = (item.Url.StartsWith("~") ? this.ResolveUrl(item.Url) : item.Url);
//                    dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
//                    dw.AutoLoad.ShowMask = true;

//                    dicDW.Add(item.WinID, dw);
//                  }

//                  menu.Listeners.Click.Handler = string.Format("{0}.show();", (string.IsNullOrEmpty(dw.ClientID) ? item.ID : dw.ClientID));

//                  menuItems.Items.Add(menu);
//                }
//              }
//            }
//          }
//        }

//        lstDesktop.Add(deskMod);
//      }
//    }
    
//    MenuBuilder.MenuConfiguration mcFiltered = null;

//    for (int xLoop = 0; xLoop < lstGA.Count; xLoop++)
//    {
//      mcFiltered = mb.FilterMenuConfiguration(mc, ga);
//    }
//  }

  protected void Button1_Click(object sender, EventArgs e)
  {
    string fileX = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>PL</NodeName>
      <NodeID>PL</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingList.aspx</Url>
          <Name>Packing List</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingListConfirm.aspx</Url>
          <Name>Packing List Confirm</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
    <RightAccess>
      <NodeName>PL</NodeName>
      <NodeID>PL</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingList.aspx</Url>
          <Name>Packing List</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingListConfirm.aspx</Url>
          <Name>Packing List Confirm</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX1 = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>RPTIVT</NodeName>
      <NodeID>RPTIVT_KG</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/reporting/inventory/Default.aspx?mod=SG</Url>
          <Name>Kartu Gudang</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
    <RightAccess>
      <NodeName>PL</NodeName>
      <NodeID>PL</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingList.aspx</Url>
          <Name>Packing List</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingListConfirm.aspx</Url>
          <Name>Packing List Confirm</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
    <RightAccess>
      <NodeName>RPTIVT</NodeName>
      <NodeID>RPTIVT</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/reporting/inventory/Default.aspx?mod=SG</Url>
          <Name>Stok Gudang</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX2 = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>PL</NodeName>
      <NodeID>PL</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingList.aspx</Url>
          <Name>Packing List</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingListConfirm.aspx</Url>
          <Name>Packing List Confirm</Name>
          <IsView>false</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX3 = @"<?xml version=""1.0""?>
<GroupAccess>
  <RightAccess>
    <RightAccess>
      <NodeName>PL</NodeName>
      <NodeID>PL</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingList.aspx</Url>
          <Name>Packing List</Name>
          <IsView>false</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/PackingList.aspx?mode=confirm</Url>
          <Name>Packing List Confirm</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
    <RightAccess>
      <NodeName>RPTIVT</NodeName>
      <NodeID>RPTIVT</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/reporting/inventory/Default.aspx?mod=SG</Url>
          <Name>Stok Gudang</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
    <RightAccess>
      <NodeName>DO</NodeName>
      <NodeID>DO</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/DeliveryOrder.aspx?mode=pl</Url>
          <Name>Delivery Order PL</Name>
          <IsView>false</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/penjualan/DeliveryOrder.aspx?mode=stt</Url>
          <Name>Delivery Order STT</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX4 = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>BYSPC</NodeName>
      <NodeID>BYSPC</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/pembelian/SuratPesanan.aspx</Url>
          <Name>Surat Pesanan</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
    <RightAccess>
      <NodeName>BYORD</NodeName>
      <NodeID>BYORD</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/pembelian/OrderRequestPrincipal.aspx</Url>
          <Name>Order Principal</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/pembelian/OrderRequestPrincipal.aspx?mode=process</Url>
          <Name>Proses Order Principal</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/pembelian/OrderRequestGudang.aspx</Url>
          <Name>Order Gudang</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/pembelian/OrderRequestGudang.aspx?mode=process</Url>
          <Name>Proses Order Gudang</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
    <RightAccess>
      <NodeName>BYPOP</NodeName>
      <NodeID>BYPOP</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/pembelian/PurchaseOrder.aspx</Url>
          <Name>Purchase Order</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX5 = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>EXPD</NodeName>
      <NodeID>EXPD</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/pengiriman/Ekspedisi.aspx</Url>
          <Name>Ekspedisi</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX6 = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>TRRSSUPL</NodeName>
      <NodeID>TRRSSUPL</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/transaksi/retur/ReturSupplier.aspx?mode=pembelian</Url>
          <Name>Pembelian</Name>
          <IsView>false</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/transaksi/retur/ReturSupplier.aspx?mode=repack</Url>
          <Name>Repack</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX7 = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>TRRSSUPL</NodeName>
      <NodeID>TRRSSUPL</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/reporting/inventory/Default.aspx?mode=stockgudang</Url>
          <Name>Pembelian</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/reporting/inventory/Default.aspx?mode=mutasiinventori</Url>
          <Name>Repack</Name>
          <IsView>true</IsView>
          <IsPrint>true</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    string fileX8 = @"<?xml version=""1.0""?>
<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <RightAccess>
    <RightAccess>
      <NodeName>MSTR</NodeName>
      <NodeID>MSTR</NodeID>
      <Pages>
        <PageRightAccess>
          <Url>/web/master/item/MasterItem.aspx</Url>
          <Name>Barang</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>false</IsAdd>
          <IsEdit>false</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/master/batch/Batch.aspx</Url>
          <Name>Batch</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>false</IsDelete>
        </PageRightAccess>
        <PageRightAccess>
          <Url>/web/master/discount/Discount.aspx</Url>
          <Name>Discount</Name>
          <IsView>true</IsView>
          <IsPrint>false</IsPrint>
          <IsAdd>true</IsAdd>
          <IsEdit>true</IsEdit>
          <IsDelete>true</IsDelete>
        </PageRightAccess>
      </Pages>
    </RightAccess>
  </RightAccess>
</GroupAccess>";

    List<RightBuilder.GroupAccess> lstGA = new List<RightBuilder.GroupAccess>();
    RightBuilder.GroupAccess ga = null;

    ga = RightBuilder.GroupAccess.Serialize(fileX1);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX2);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX3);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX4);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX5);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX6);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX7);
    //lstGA.Add(ga);

    ga = RightBuilder.GroupAccess.Serialize(fileX8);
    lstGA.Add(ga);

    MenuBuilder mb = new MenuBuilder();

    MenuBuilder.MenuConfiguration mc = null;
    MenuBuilder.MenuConfiguration mcFiltered = null;

    List<Ext.Net.DesktopModule> lstDesktop = new List<Ext.Net.DesktopModule>();

    IDictionary<string, Ext.Net.DesktopWindow> dicDW = new Dictionary<string, Ext.Net.DesktopWindow>(StringComparer.OrdinalIgnoreCase);
    IDictionary<string, Ext.Net.DesktopShortcut> dicDS = new Dictionary<string, Ext.Net.DesktopShortcut>(StringComparer.OrdinalIgnoreCase);

    //Ext.Net.Desktop desktop = myDesktop;
    Ext.Net.Desktop desktop = new Ext.Net.Desktop();

    //MenuBuilder.MenuModule mbMenuModule = null;
    //MenuBuilder.MenuLauncer mbMenuLaunch = null;
    //MenuBuilder.MenuItem mbMenuItem = null;
    //Ext.Net.DesktopModule deskModule = null;
    //Ext.Net.MenuItem mnuLauncher = null;
    //Ext.Net.Menu mnuMenu = null;
    //Ext.Net.MenuItem menuItem = null;
    //Ext.Net.Menu nestedMenu = null;
    //Ext.Net.MenuItem nestedMenuItems = null;
    //Ext.Net.MenuSeparator separator = null;
    //Ext.Net.DesktopWindow dw = null;
    //int nLoopC = 0,
    //  nLenC = 0;

    MenuBuilder.MenuApplication mbMenuApp = null;
    MenuBuilder.MenuModule mbMenuModule = null;
    MenuBuilder.MenuLauncer mbMenuLaunch = null;
    MenuBuilder.MenuItem mbMenuItem = null;
    Ext.Net.DesktopModule deskModule = null;
    Ext.Net.DesktopModule deskModuleSc = null;
    Ext.Net.Menu mnuMainLauncher = null;
    Ext.Net.MenuItem mnuMainItemLauncher = null;
    Ext.Net.MenuItem mnuLauncher = null;
    Ext.Net.Menu mnuMenu = null;
    Ext.Net.MenuItem menuItem = null;
    Ext.Net.Menu nestedFirstMenu = null;
    Ext.Net.MenuItem nestedFirstMenuItem = null;
    //Ext.Net.Menu mnuMenuSingle = null;
    Ext.Net.DesktopWindow dw = null;
    Ext.Net.DesktopShortcut dscrt = null;
    int nLoopC = 0,
      nLenC = 0,
      nLoopX = 0,
      nLenX = 0,
      nLoop = 0,
      nLen = 0;
    MenuBuilder.MenuItemLauncher itemLaunch = null;

    mc = mb.RebuildMenuConfiguration(StaticObjects.MenuConfiguration);

    mcFiltered = mb.FilterMenuConfigurationArray(mc, lstGA.ToArray());

    //myDesktop.Modules.Clear();

    if ((mcFiltered.Applications != null) && (mcFiltered.Applications.Length > 0))
    {
      for (int yLoop = 0, yLen = mcFiltered.Applications.Length; yLoop < yLen; yLoop++)
      {
        mbMenuApp = mcFiltered.Applications[yLoop];

        deskModule = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
        {
          AutoRun = mbMenuApp.AutoRun,
          ModuleID = mbMenuApp.ID,
          WindowID = mbMenuApp.WindowID
        });

        mnuLauncher = deskModule.Launcher;

        mnuLauncher.Text = mbMenuApp.Text;
        mnuLauncher.Listeners.Click.Handler = "return false;";

        if (string.IsNullOrEmpty(mbMenuApp.CustomIcon))
        {
          mnuLauncher.Icon = (Ext.Net.Icon)mbMenuApp.Icon;
        }
        else
        {
          mnuLauncher.Icon = Ext.Net.Icon.None;
          mnuLauncher.IconCls = (string.IsNullOrEmpty(mbMenuApp.CustomIcon) ? mnuLauncher.IconCls : mbMenuApp.CustomIcon);
        }

        mnuMainLauncher = new Ext.Net.Menu();

        //mnuMainLauncher.Items.Add(mnuMainItemLauncher);

        #region Module

        for (nLoop = 0, nLen = mbMenuApp.Modules.Length; nLoop < nLen; nLoop++)
        {
          mbMenuModule = mbMenuApp.Modules[nLoop];

          //if ((mbMenuModule != null) && (mbMenuModule.Launcher != null) && (mbMenuModule.Launcher.MenuItemLauncher != null) && (mbMenuModule.Launcher.MenuItemLauncher.Length > 0))
          if ((mbMenuModule != null) && (mbMenuModule.Launcher != null) &&
            (((mbMenuModule.Launcher.MenuItemLauncher != null) && (mbMenuModule.Launcher.MenuItemLauncher.Length > 0)) || mbMenuModule.Launcher.Direct))
          {
            if (mbMenuModule.Launcher.Direct)
            {
              mbMenuItem = mbMenuModule.Launcher.SingleItem;

              if (!deskModule.WindowID.Equals(mbMenuItem.ID))
              {
                mnuMainItemLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                {
                  Text = mbMenuModule.Launcher.Text
                });

                if (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon))
                {
                  mnuMainItemLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
                }
                else
                {
                  mnuMainItemLauncher.Icon = Ext.Net.Icon.None;
                  mnuMainItemLauncher.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon) ? mnuMainItemLauncher.IconCls : mbMenuModule.Launcher.CustomIcon);
                }

                mnuMainItemLauncher.Listeners.Click.Handler = "return false;";

                //if (!dicDW.ContainsKey(mbMenuItem.ID))
                if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                {
                  dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                  {
                    Title = mbMenuItem.Title,
                    Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                    InitCenter = mbMenuItem.Center,
                    Width = Unit.Pixel(mbMenuItem.Width),
                    Height = Unit.Pixel(mbMenuItem.Height),
                    Resizable = mbMenuItem.Resizeable,
                    Maximizable = mbMenuItem.Resizeable,
                    CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                  })
                  {
                    ID = mbMenuItem.ID
                  };

                  dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                  dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                  dw.AutoLoad.ShowMask = true;

                  dicDW.Add(mbMenuItem.ID, dw);

                  #region Added Shortcut

                  if (mbMenuItem.IsShortcut)
                  {
                    deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                    {
                      AutoRun = false,
                      ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                      WindowID = mbMenuItem.ID
                    });

                    if (!lstDesktop.Contains(deskModuleSc))
                    {
                      lstDesktop.Add(deskModuleSc);
                    }

                    if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                    {
                      dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                      {
                        Text = mbMenuItem.Text,
                        ModuleID = deskModuleSc.ModuleID,
                        IconCls = mbMenuItem.ShortcutClass
                      });

                      dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                    }
                  }

                  #endregion
                }

                nestedFirstMenu = new Ext.Net.Menu();

                nestedFirstMenuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                {
                  Text = mbMenuModule.Launcher.SingleItem.Text
                });

                if (string.IsNullOrEmpty(mbMenuModule.Launcher.SingleItem.CustomIcon))
                {
                  nestedFirstMenuItem.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.SingleItem.Icon;
                }
                else
                {
                  nestedFirstMenuItem.Icon = Ext.Net.Icon.None;
                  nestedFirstMenuItem.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.SingleItem.CustomIcon) ? nestedFirstMenuItem.IconCls : mbMenuModule.Launcher.SingleItem.CustomIcon);
                }

                if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                {
                  nestedFirstMenuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                    string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                }
                else
                {
                  nestedFirstMenuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                }

                nestedFirstMenu.Items.Add(nestedFirstMenuItem);

                //mnuLauncher.Menu.Add(nestedFirstMenu);
                mnuMainItemLauncher.Menu.Add(nestedFirstMenu);

                mnuMainLauncher.Items.Add(mnuMainItemLauncher);
              }
            }
            else
            {
              //mnuLauncher.Text = mbMenuModule.Launcher.Text;
              //mnuLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
              //mnuLauncher.Listeners.Click.Handler = "return false;";

              mbMenuLaunch = mbMenuModule.Launcher;

              if (mbMenuLaunch.Nested)
              {
                #region Nested

                mnuMainItemLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                {
                  Text = mbMenuModule.Launcher.Text
                });

                if (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon))
                {
                  mnuMainItemLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
                }
                else
                {
                  mnuMainItemLauncher.Icon = Ext.Net.Icon.None;
                  mnuMainItemLauncher.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon) ? mnuMainItemLauncher.IconCls : mbMenuModule.Launcher.CustomIcon);
                }

                mnuMainItemLauncher.Listeners.Click.Handler = "return false;";

                nestedFirstMenu = new Ext.Net.Menu();

                for (nLoopC = 0, nLenC = mbMenuLaunch.MenuItemLauncher.Length; nLoopC < nLenC; nLoopC++)
                {
                  itemLaunch = mbMenuLaunch.MenuItemLauncher[nLoopC];

                  if (itemLaunch.Items != null)
                  {
                    nestedFirstMenuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                    {
                      Text = itemLaunch.Text
                    });

                    if (string.IsNullOrEmpty(itemLaunch.CustomIcon))
                    {
                      nestedFirstMenuItem.Icon = (Ext.Net.Icon)itemLaunch.Icon;
                    }
                    else
                    {
                      nestedFirstMenuItem.Icon = Ext.Net.Icon.None;
                      nestedFirstMenuItem.IconCls = (string.IsNullOrEmpty(itemLaunch.CustomIcon) ? nestedFirstMenuItem.IconCls : itemLaunch.CustomIcon);
                    }

                    nestedFirstMenu.Items.Add(nestedFirstMenuItem);

                    if (itemLaunch.Items.Length > 0)
                    {
                      nestedFirstMenuItem.Listeners.Click.Handler = "return false;";

                      mnuMenu = new Ext.Net.Menu();

                      for (nLoopX = 0, nLenX = itemLaunch.Items.Length; nLoopX < nLenX; nLoopX++)
                      {
                        mbMenuItem = itemLaunch.Items[nLoopX];

                        if (mbMenuItem.OutsideSubMenu)
                        {
                          #region Outside Submenu

                          if (mbMenuItem.Separator)
                          {
                            nestedFirstMenu.Items.Add(new Ext.Net.MenuSeparator());
                          }
                          else
                          {
                            //if (!dicDW.ContainsKey(mbMenuItem.ID))
                            if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                              {
                                Title = mbMenuItem.Title,
                                Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                InitCenter = mbMenuItem.Center,
                                Width = Unit.Pixel(mbMenuItem.Width),
                                Height = Unit.Pixel(mbMenuItem.Height),
                                Resizable = mbMenuItem.Resizeable,
                                Maximizable = mbMenuItem.Resizeable,
                                CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                              })
                              {
                                ID = mbMenuItem.ID
                              };

                              dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                              dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                              dw.AutoLoad.ShowMask = true;

                              dicDW.Add(mbMenuItem.ID, dw);

                              #region Added Shortcut

                              if (mbMenuItem.IsShortcut)
                              {
                                deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                {
                                  AutoRun = false,
                                  ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                  WindowID = mbMenuItem.ID
                                });

                                if (!lstDesktop.Contains(deskModuleSc))
                                {
                                  lstDesktop.Add(deskModuleSc);
                                }

                                if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                {
                                  dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                  {
                                    Text = mbMenuItem.Text,
                                    ModuleID = deskModuleSc.ModuleID,
                                    IconCls = mbMenuItem.ShortcutClass
                                  });

                                  dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                }
                              }

                              #endregion
                            }

                            if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              nestedFirstMenuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                            }
                            else
                            {
                              nestedFirstMenuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                            }
                          }

                          mnuMenu = null;

                          #endregion
                        }
                        else
                        {
                          #region Inside Submenu

                          if (mbMenuItem.Separator)
                          {
                            mnuMenu.Items.Add(new Ext.Net.MenuSeparator());
                          }
                          else
                          {
                            menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                            {
                              Text = mbMenuItem.Text
                            });

                            if (string.IsNullOrEmpty(mbMenuItem.CustomIcon))
                            {
                              menuItem.Icon = (Ext.Net.Icon)mbMenuItem.Icon;
                            }
                            else
                            {
                              menuItem.Icon = Ext.Net.Icon.None;
                              menuItem.IconCls = (string.IsNullOrEmpty(mbMenuItem.CustomIcon) ? menuItem.IconCls : mbMenuItem.CustomIcon);
                            }

                            //if (!dicDW.ContainsKey(mbMenuItem.ID))
                            if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                              {
                                Title = mbMenuItem.Title,
                                Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                InitCenter = mbMenuItem.Center,
                                Width = Unit.Pixel(mbMenuItem.Width),
                                Height = Unit.Pixel(mbMenuItem.Height),
                                Resizable = mbMenuItem.Resizeable,
                                Maximizable = mbMenuItem.Resizeable,
                                CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                              })
                              {
                                ID = mbMenuItem.ID
                              };

                              dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                              dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                              dw.AutoLoad.ShowMask = true;

                              dicDW.Add(mbMenuItem.ID, dw);

                              #region Added Shortcut

                              if (mbMenuItem.IsShortcut)
                              {
                                deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                {
                                  AutoRun = false,
                                  ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                  WindowID = mbMenuItem.ID
                                });

                                if (!lstDesktop.Contains(deskModuleSc))
                                {
                                  lstDesktop.Add(deskModuleSc);
                                }

                                if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                {
                                  dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                  {
                                    Text = mbMenuItem.Text,
                                    ModuleID = deskModuleSc.ModuleID,
                                    IconCls = mbMenuItem.ShortcutClass
                                  });

                                  dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                }
                              }

                              #endregion
                            }

                            if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              menuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                            }
                            else
                            {
                              menuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                            }

                            mnuMenu.Items.Add(menuItem);
                          }

                          #endregion
                        }
                      }

                      if (mnuMenu != null)
                      {
                        nestedFirstMenuItem.Menu.Add(mnuMenu);
                      }
                    }
                    else
                    {
                      throw new Exception("Check error.");
                    }
                  }
                }

                //if (nestedFirstMenu.Items.Count > 0)
                //{
                //  mnuLauncher.Menu.Add(nestedFirstMenu);
                //}

                //mnuLauncher.Menu.Add(nestedFirstMenu);
                mnuMainItemLauncher.Menu.Add(nestedFirstMenu);

                mnuMainLauncher.Items.Add(mnuMainItemLauncher);

                #endregion
              }
              else if (mbMenuLaunch.NestedDirected)
              {
                #region NestedDirected

                mnuMainItemLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                {
                  Text = mbMenuModule.Launcher.Text
                });
                
                if (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon))
                {
                  mnuMainItemLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
                }
                else
                {
                  mnuMainItemLauncher.Icon = Ext.Net.Icon.None;
                  mnuMainItemLauncher.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon) ? mnuMainItemLauncher.IconCls : mbMenuModule.Launcher.CustomIcon);
                }

                mnuMainItemLauncher.Listeners.Click.Handler = "return false;";

                nestedFirstMenu = new Ext.Net.Menu();

                for (nLoopC = 0, nLenC = mbMenuLaunch.MenuItemLauncher.Length; nLoopC < nLenC; nLoopC++)
                {
                  itemLaunch = mbMenuLaunch.MenuItemLauncher[nLoopC];

                  if (itemLaunch.Items != null)
                  {
                    nestedFirstMenuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                    {
                      Text = itemLaunch.Text
                    });

                    if (string.IsNullOrEmpty(itemLaunch.CustomIcon))
                    {
                      nestedFirstMenuItem.Icon = (Ext.Net.Icon)itemLaunch.Icon;
                    }
                    else
                    {
                      nestedFirstMenuItem.Icon = Ext.Net.Icon.None;
                      nestedFirstMenuItem.IconCls = (string.IsNullOrEmpty(itemLaunch.CustomIcon) ? nestedFirstMenuItem.IconCls : itemLaunch.CustomIcon);
                    }

                    nestedFirstMenu.Items.Add(nestedFirstMenuItem);

                    if (itemLaunch.Items.Length > 0)
                    {
                      nestedFirstMenuItem.Listeners.Click.Handler = "return false;";

                      mnuMenu = new Ext.Net.Menu();

                      for (nLoopX = 0, nLenX = itemLaunch.Items.Length; nLoopX < nLenX; nLoopX++)
                      {
                        mbMenuItem = itemLaunch.Items[nLoopX];

                        if (mbMenuItem.OutsideSubMenu)
                        {
                          #region Outside Submenu

                          if (mbMenuItem.Separator)
                          {
                            nestedFirstMenu.Items.Add(new Ext.Net.MenuSeparator());
                          }
                          else
                          {
                            if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                              {
                                Title = mbMenuItem.Title,
                                Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                InitCenter = mbMenuItem.Center,
                                Width = Unit.Pixel(mbMenuItem.Width),
                                Height = Unit.Pixel(mbMenuItem.Height),
                                Resizable = mbMenuItem.Resizeable,
                                Maximizable = mbMenuItem.Resizeable,
                                CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                              })
                              {
                                ID = mbMenuItem.ID
                              };

                              dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                              dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                              dw.AutoLoad.ShowMask = true;

                              dicDW.Add(mbMenuItem.ID, dw);

                              #region Added Shortcut

                              if (mbMenuItem.IsShortcut)
                              {
                                deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                {
                                  AutoRun = false,
                                  ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                  WindowID = mbMenuItem.ID
                                });

                                if (!lstDesktop.Contains(deskModuleSc))
                                {
                                  lstDesktop.Add(deskModuleSc);
                                }

                                if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                {
                                  dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                  {
                                    Text = mbMenuItem.Text,
                                    ModuleID = deskModuleSc.ModuleID,
                                    IconCls = mbMenuItem.ShortcutClass
                                  });

                                  dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                }
                              }

                              #endregion
                            }

                            if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              nestedFirstMenuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                            }
                            else
                            {
                              nestedFirstMenuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                            }
                          }

                          mnuMenu = null;

                          #endregion
                        }
                        else
                        {
                          #region Inside Submenu

                          if (mbMenuItem.Separator)
                          {
                            mnuMenu.Items.Add(new Ext.Net.MenuSeparator());
                          }
                          else
                          {
                            menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                            {
                              Text = mbMenuItem.Text
                            });

                            if (string.IsNullOrEmpty(mbMenuItem.CustomIcon))
                            {
                              menuItem.Icon = (Ext.Net.Icon)mbMenuItem.Icon;
                            }
                            else
                            {
                              menuItem.Icon = Ext.Net.Icon.None;
                              menuItem.IconCls = (string.IsNullOrEmpty(mbMenuItem.CustomIcon) ? menuItem.IconCls : mbMenuItem.CustomIcon);
                            }

                            if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                              {
                                Title = mbMenuItem.Title,
                                Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                InitCenter = mbMenuItem.Center,
                                Width = Unit.Pixel(mbMenuItem.Width),
                                Height = Unit.Pixel(mbMenuItem.Height),
                                Resizable = mbMenuItem.Resizeable,
                                Maximizable = mbMenuItem.Resizeable,
                                CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                              })
                              {
                                ID = mbMenuItem.ID
                              };

                              dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                              dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                              dw.AutoLoad.ShowMask = true;

                              dicDW.Add(mbMenuItem.ID, dw);

                              #region Added Shortcut

                              if (mbMenuItem.IsShortcut)
                              {
                                deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                {
                                  AutoRun = false,
                                  ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                  WindowID = mbMenuItem.ID
                                });

                                if (!lstDesktop.Contains(deskModuleSc))
                                {
                                  lstDesktop.Add(deskModuleSc);
                                }

                                if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                {
                                  dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                  {
                                    Text = mbMenuItem.Text,
                                    ModuleID = deskModuleSc.ModuleID,
                                    IconCls = mbMenuItem.ShortcutClass
                                  });

                                  dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                }
                              }

                              #endregion
                            }

                            if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                            {
                              menuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                            }
                            else
                            {
                              menuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                            }

                            mnuMenu.Items.Add(menuItem);
                          }

                          #endregion
                        }
                      }

                      if (mnuMenu != null)
                      {
                        nestedFirstMenuItem.Menu.Add(mnuMenu);
                      }
                    }
                    else
                    {
                      throw new Exception("Check error.");
                    }
                  }
                }

                //if (nestedFirstMenu.Items.Count > 0)
                //{
                //  mnuLauncher.Menu.Add(nestedFirstMenu);
                //}

                //mnuLauncher.Menu.Add(nestedFirstMenu);
                mnuMainItemLauncher.Menu.Add(nestedFirstMenu);

                mnuMainLauncher.Items.Add(mnuMainItemLauncher);

                #endregion
              }

              #region Old Coded

              //if (launch.Nested)
              //{
              //  nestedFirstMenu = new Ext.Net.Menu();

              //  launcher.Menu.Add(nestedFirstMenu);

              //  nestedMenuItems = new Ext.Net.MenuItem(launch.NestedText);
              //  nestedMenuItems.Icon = (Ext.Net.Icon)launch.NestedIcon;
              //  nestedMenuItems.Listeners.Click.Handler = "return false;";

              //  nestedFirstMenu.Items.Add(nestedMenuItems);

              //  menuItems = new Ext.Net.Menu();

              //  nestedMenuItems.Menu.Add(menuItems);
              //}
              //else
              //{
              //  menuItems = new Ext.Net.Menu();

              //  launcher.Menu.Add(menuItems);
              //}

              //if ((launch.Items != null) && (launch.Items.Length > 0))
              //{
              //  for (nLoopC = 0, nLenC = launch.Items.Length; nLoopC < nLenC; nLoopC++)
              //  {
              //    item = launch.Items[nLoopC];

              //    if (item.Separator)
              //    {
              //      separator = new Ext.Net.MenuSeparator();

              //      menuItems.Items.Add(separator);
              //    }
              //    else
              //    {
              //      menu = menuItems.Items.Find(delegate(Ext.Net.Component c)
              //      {
              //        Ext.Net.MenuItem mnuItem = c as Ext.Net.MenuItem;
              //        if (mnuItem != null)
              //        {
              //          if (mnuItem.Text.Equals(item.Title, StringComparison.OrdinalIgnoreCase) &&
              //            ((int)mnuItem.Icon == item.Icon))
              //          {
              //            return true;
              //          }
              //        }

              //        return false;
              //      }) as Ext.Net.MenuItem;

              //      if (menu == null)
              //      {
              //        menu = new Ext.Net.MenuItem();

              //        menu.Text = item.Title;
              //        menu.Icon = (Ext.Net.Icon)item.Icon;

              //        if (dicDW.ContainsKey(item.ID))
              //        {
              //          dw = dicDW[item.ID];
              //        }
              //        else
              //        {
              //          dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
              //          {
              //            Title = item.Title,
              //            Icon = (Ext.Net.Icon)item.Icon,
              //            InitCenter = item.Center,
              //            Width = Unit.Pixel(item.Width),
              //            Height = Unit.Pixel(item.Height)
              //          })
              //          {
              //            ID = item.ID
              //          };

              //          dw.AutoLoad.Url = (item.Url.StartsWith("~") ? this.ResolveUrl(item.Url) : item.Url);
              //          dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
              //          dw.AutoLoad.ShowMask = true;

              //          dicDW.Add(item.ID, dw);
              //        }

              //        //menu.Listeners.Click.Handler = string.Format("{0}.show();",
              //        //  string.Concat(ph.NamingContainer.ClientID, "_", item.ID));

              //        menuItems.Items.Add(menu);
              //      }
              //    }
              //  }
              //}

              #endregion
            }

            lstDesktop.Add(deskModule);
          }
        }

        #endregion

        mnuLauncher.Menu.Add(mnuMainLauncher);
      }

      foreach (KeyValuePair<string, Ext.Net.DesktopShortcut> kvp in dicDS)
      {
        //ph.Controls.Add(kvp.Value);
        //desktop.Shortcuts.Add(kvp.Value);
      }

      foreach (KeyValuePair<string, Ext.Net.DesktopWindow> kvp in dicDW)
      {
        ph.Controls.Add(kvp.Value);
      }

      dicDW.Clear();

      //myDesktop.Modules.AddRange(lstDesktop.ToArray());

      lstDesktop.Clear();
    }
  }

//  protected void Button1_Click_Z(object sender, EventArgs e)
//  {
//    string fileX = @"<?xml version=""1.0""?>
//        <GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
//          <RightAccess>
//            <RightAccess>
//              <NodeName>PL</NodeName>
//              <NodeID>PL</NodeID>
//              <Pages>
//                <PageRightAccess>
//                  <Url>PackingList.aspx</Url>
//                  <Name>Packing List</Name>
//                  <IsView>true</IsView>
//                  <IsPrint>false</IsPrint>
//                  <IsAdd>true</IsAdd>
//                  <IsEdit>false</IsEdit>
//                  <IsDelete>true</IsDelete>
//                </PageRightAccess>
//                <PageRightAccess>
//                  <Url>PackingListConfirm.aspx</Url>
//                  <Name>Packing List Confirm</Name>
//                  <IsView>false</IsView>
//                  <IsPrint>false</IsPrint>
//                  <IsAdd>false</IsAdd>
//                  <IsEdit>false</IsEdit>
//                  <IsDelete>false</IsDelete>
//                </PageRightAccess>
//              </Pages>
//            </RightAccess>
//          </RightAccess>
//        </GroupAccess>";

//    string fileX1 = @"<?xml version=""1.0""?>
//<GroupAccess xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
//  <RightAccess>
//    <RightAccess>
//      <NodeName>RPTIVT</NodeName>
//      <NodeID>RPTIVT_KG</NodeID>
//      <Pages>
//        <PageRightAccess>
//          <Url>KartuGudang.aspx</Url>
//          <Name>Kartu Gudang</Name>
//          <IsView>true</IsView>
//          <IsPrint>false</IsPrint>
//          <IsAdd>false</IsAdd>
//          <IsEdit>false</IsEdit>
//          <IsDelete>false</IsDelete>
//        </PageRightAccess>
//      </Pages>
//    </RightAccess>
//    <RightAccess>
//      <NodeName>PL</NodeName>
//      <NodeID>PL</NodeID>
//      <Pages>
//        <PageRightAccess>
//          <Url>PackingList.aspx</Url>
//          <Name>Packing List</Name>
//          <IsView>false</IsView>
//          <IsPrint>false</IsPrint>
//          <IsAdd>false</IsAdd>
//          <IsEdit>false</IsEdit>
//          <IsDelete>false</IsDelete>
//        </PageRightAccess>
//        <PageRightAccess>
//          <Url>PackingListConfirm.aspx</Url>
//          <Name>Packing List Confirm</Name>
//          <IsView>true</IsView>
//          <IsPrint>false</IsPrint>
//          <IsAdd>false</IsAdd>
//          <IsEdit>false</IsEdit>
//          <IsDelete>false</IsDelete>
//        </PageRightAccess>
//      </Pages>
//    </RightAccess>
//  </RightAccess>
//</GroupAccess>";

//    List<RightBuilder.GroupAccess> lstGA = new List<RightBuilder.GroupAccess>();
//    RightBuilder.GroupAccess ga = null;
    
//    ga = RightBuilder.GroupAccess.Serialize(fileX1);
//    lstGA.Add(ga);

//    ga = RightBuilder.GroupAccess.Serialize(fileX);
//    lstGA.Add(ga);

//    MenuBuilder mb = new MenuBuilder();

//    MenuBuilder.MenuConfiguration mc = null;
//    MenuBuilder.MenuConfiguration mcFiltered = null;

//    List<Ext.Net.DesktopModule> lstDesktop = new List<Ext.Net.DesktopModule>();

//    IDictionary<string, Ext.Net.DesktopWindow> dicDW = new Dictionary<string, Ext.Net.DesktopWindow>(StringComparer.OrdinalIgnoreCase);

//    //Ext.Net.Desktop desktop = myDesktop;
//    Ext.Net.Desktop desktop = new Ext.Net.Desktop();

//    mc = mb.RebuildMenuConfiguration(StaticObjects.MenuConfiguration);

//    mcFiltered = mb.FilterMenuConfigurationArray(mc, lstGA.ToArray());
//  }
}
