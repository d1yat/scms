<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Main.aspx.cs" Inherits="Main" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Desktop ID="myDesktop" runat="server" BackgroundColor="blue" ShortcutTextColor="White">
    <StartButton Text="Mulai" IconCls="start-button" />
    <Modules>
      <ext:DesktopModule WindowID="" AutoRun="true">
        <Launcher runat="server" Text="Penjualan" Icon="Folder" HideOnClick="false">
          <Menu>
            <ext:Menu runat="server">
              <Items>
                <ext:MenuItem Text="Packing List" Icon="Application">
                  <Listeners>
                    <Click Handler="alert('test');" />
                  </Listeners>
                </ext:MenuItem>
              </Items>
            </ext:Menu>
          </Menu>
        </Launcher>
      </ext:DesktopModule>
    </Modules>
    <Content>
      <asp:PlaceHolder ID="ph" runat="server" />
    </Content>
  </ext:Desktop>
</asp:Content>
