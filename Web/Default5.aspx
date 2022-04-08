<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default5.aspx.cs" Inherits="Default5" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">
  <style type="text/css">
    .start-button
    {
      background-image: url('images/vista_start_button.gif') !important;
    }
    .shortcut-icon
    {
      width: 48px;
      height: 48px;
      filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='images/window.png', sizingMethod='scale');
    }
    .icon-grid48
    {
      background-image: url('images/grid48x48.png') !important;
      filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='images/grid48x48.png', sizingMethod='scale');
    }
    .icon-user48
    {
      background-image: url('images/user48x48.png') !important;
      filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='images/user48x48.png', sizingMethod='scale');
    }
    .icon-window48
    {
      background-image: url('images/window48x48.png') !important;
      filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='images/window48x48.png', sizingMethod='scale');
    }
    .desktopEl
    {
      position: absolute !important;
    }
  </style>

  <script type="text/javascript">
    var appScms = '';
    var readyDesktop = function(app) {
      appScms = app;
    };
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <%--<ext:Desktop ID="myDesktop" runat="server" BackgroundColor="blue" ShortcutTextColor="White"
    Wallpaper="images/desktop.jpg">
    <Listeners>
      <Ready Handler="readyDesktop(#{myDesktop})" />
    </Listeners>
    <StartButton Text="Start" IconCls="start-button" />
    <Content>
      <asp:PlaceHolder ID="ph" runat="server" />
    </Content>
  </ext:Desktop>--%>
  <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
  <asp:PlaceHolder ID="ph" runat="server" />
</asp:Content>
