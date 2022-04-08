<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="UserPanel.aspx.cs" Inherits="management_UserPanel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript" language="javascript">
    var fuSelectedFile = function(fu, valu, hid) {
      var fie = (valu || '');
      var exts = ((fie.length > 3) ? fie.substring((fie.length - 4)) : '');
      var extAvaible = new Array('.bmp', '.jpg', '.png', '.gif');
      var idx = extAvaible.indexOf(exts);

      if (idx == -1) {
        fu.reset();

        ShowWarning('Maaf, sistem tidak mendukung file dengan format ini.');

        return false;
      }
      else if (!Ext.isEmpty(hid)) {
        hid.reset();
      }
    }

    var resetItemImage = function(fu, hid) {
      if (!Ext.isEmpty(fu)) {
        fu.reset();
      }
      if (!Ext.isEmpty(hid)) {
        hid.setValue('true');
      }
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:FormPanel ID="frmHeaders" runat="server" Padding="5" LabelWidth="150" Frame="true"
    MonitorValid="true" Width="525" Height="300" AutoScroll="true">
    <Content>
      <ext:Hidden ID="hfClearPic" runat="server" />
      <ext:Hidden ID="hfClearWall" runat="server" />
    </Content>
    <Items>
      <ext:FormPanel runat="server" Frame="true" Border="true" AutoHeight="true" LabelWidth="150"
        MonitorValid="true">
        <Defaults>
          <ext:Parameter Name="anchor" Value="100%" Mode="Value" />
          <ext:Parameter Name="msgTarget" Value="side" Mode="Value" />
        </Defaults>
        <Items>
          <ext:TextField ID="txPwdLama" runat="server" FieldLabel="Kata kunci Lama" InputType="Password"
            AllowBlank="false" />
          <ext:TextField ID="txNama" runat="server" FieldLabel="Nama" MaxLength="100" />
          <ext:TextField ID="txPwdBaru1" runat="server" FieldLabel="Kata kunci Baru" InputType="Password" />
          <ext:TextField ID="txPwdBaru2" runat="server" FieldLabel="Konfirmasi Kata-kunci"
            InputType="Password" Vtype="password" MaxLength="20">
            <CustomConfig>
              <ext:ConfigItem Name="initialPassField" Value="#{txPwdBaru1}" Mode="Value" />
            </CustomConfig>
          </ext:TextField>
        </Items>
        <%--<Listeners>
          <ClientValidation Handler="#{btnSave}.setDisabled(!valid);" />
        </Listeners>--%>
      </ext:FormPanel>
      <ext:Label runat="server" />
      <ext:FormPanel ID="FormPanel1" runat="server" Frame="true" AutoHeight="true" LabelWidth="150">
        <Defaults>
          <ext:Parameter Name="anchor" Value="100%" Mode="Value" />
          <ext:Parameter Name="allowBlank" Value="true" Mode="Raw" />
          <ext:Parameter Name="msgTarget" Value="side" Mode="Value" />
        </Defaults>
        <Items>
          <ext:FileUploadField ID="fuYourPic" runat="server" FieldLabel="Foto Anda" EmptyText="Pilih foto anda..."
            ButtonText="Pilih..." Icon="ImageAdd" Note="(.bmp, .jpg, .gif, .png)">
            <Listeners>
              <FileSelected Handler="fuSelectedFile(this, value, #{hfClearPic});" />
            </Listeners>
          </ext:FileUploadField>
          <ext:CompositeField ID="CompositeField1" runat="server">
            <Items>
              <ext:Button ID="Button1" runat="server" Icon="Image" ToolTip="Lihat Gambar Yang Tersimpan">
                <DirectEvents>
                  <Click OnEvent="BtnShowPic_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:Button ID="Button2" runat="server" Icon="Cross" ToolTip="Hapus Gambar">
                <Listeners>
                  <Click Handler="resetItemImage(#{fuYourPic}, #{hfClearPic});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:CompositeField>
          <ext:FileUploadField ID="fuYourWall" runat="server" FieldLabel="Gambar Background"
            EmptyText="Pilih background anda..." ButtonText="Pilih..." Icon="ImageAdd" Note="(.bmp, .jpg, .gif, .png)">
            <Listeners>
              <FileSelected Handler="fuSelectedFile(this, value, #{hfClearWall});" />
            </Listeners>
          </ext:FileUploadField>
          <ext:CompositeField ID="CompositeField2" runat="server">
            <Items>
              <ext:Button ID="Button3" runat="server" Icon="Image" ToolTip="Lihat Gambar Yang Tersimpan">
                <DirectEvents>
                  <Click OnEvent="BtnShowWall_OnClick">
                    <EventMask ShowMask="true" />
                  </Click>
                </DirectEvents>
              </ext:Button>
              <ext:Button ID="Button4" runat="server" Icon="Cross" ToolTip="Hapus Gambar">
                <Listeners>
                  <Click Handler="resetItemImage(#{fuYourWall}, #{hfClearWall});" />
                </Listeners>
              </ext:Button>
            </Items>
          </ext:CompositeField>
        </Items>
      </ext:FormPanel>
    </Items>
    <Listeners>
      <ClientValidation Handler="#{btnSave}.setDisabled(!valid);" />
    </Listeners>
    <Buttons>
      <ext:Button ID="btnSave" runat="server" Text="Simpan" Icon="Disk">
        <DirectEvents>
          <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndCustomStore(#{frmHeaders});">
            <Confirmation BeforeConfirm="return verifyHeaderAndCustomStore(#{frmHeaders});" ConfirmRequest="true"
              Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
            <EventMask ShowMask="true" />
            <ExtraParams>
              <ext:Parameter Name="Pwd" Value="#{txPwdLama}.getValue()" Mode="Raw" />
              <ext:Parameter Name="Nama" Value="#{txNama}.getValue()" Mode="Raw" />
              <ext:Parameter Name="PwdNew" Value="#{txPwdBaru2}.getValue()" Mode="Raw" />
              <ext:Parameter Name="ClearPic" Value="#{hfClearPic}.getValue()" Mode="Raw" />
              <ext:Parameter Name="ClearWall" Value="#{hfClearWall}.getValue()" Mode="Raw" />
            </ExtraParams>
          </Click>
        </DirectEvents>
      </ext:Button>
    </Buttons>
  </ext:FormPanel>
  <ext:Window ID="wnd" runat="server" InitCenter="true" Hidden="true" Maximizable="true"
    AutoScroll="true" />
</asp:Content>
