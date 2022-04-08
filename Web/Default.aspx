<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript">
    var verifyBeforeSubmit = function(f, p, h) {
      if (!f.getForm().isValid()) {
        ShowWarning('Silahkan di cek setiap inputan, apakah tidak ada yang kosong dan sudah benar.');
        return false;
      }

      h.setValue(p.getValue());

      p.setValue('password');
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Window ID="Window1" runat="server" Closable="false" Resizable="false" Height="150"
    Icon="Lock" Title="Login" Draggable="false" Width="260" Modal="true" Layout="Form">
    <Items>
      <ext:FormPanel ID="frm" runat="server" Layout="Form" Padding="5" MonitorValid="true">
        <Items>
          <ext:TextField ID="txUsername" runat="server" FieldLabel="N I P" AllowBlank="false"
            BlankText="NIP harus di isi." MaxLength="15" />
          <ext:TextField ID="txPassword" runat="server" InputType="Password" FieldLabel="Kata kunci"
            AllowBlank="false" BlankText="Kata kunci harus diisi." MaxLength="25" />
          <ext:Checkbox ID="chkRemember" runat="server" FieldLabel="Remember" />
          <ext:Hidden ID="hidPwd" runat="server" />
        </Items>
        <KeyMap>
          <ext:KeyBinding>
            <Keys>
              <ext:Key Code="ENTER" />
            </Keys>
            <Listeners>
              <Event Handler="if(key == Ext.EventObject.ENTER) { #{btnLogin}.fireEvent('click', #{btnLogin}); }" />
            </Listeners>
          </ext:KeyBinding>
        </KeyMap>
        <Listeners>
          <ClientValidation Handler="#{btnLogin}.setDisabled(!valid);" />
        </Listeners>
      </ext:FormPanel>
    </Items>
    <Buttons>
      <ext:Button ID="btnLogin" runat="server" Text="Login" Icon="Accept">
        <DirectEvents>
          <Click OnEvent="btnLogin_Click" Before="return verifyBeforeSubmit(#{frm}, #{txPassword}, #{hidPwd});">
            <EventMask ShowMask="true" Msg="Verifying..." />
          </Click>
        </DirectEvents>
      </ext:Button>
      <ext:Button runat="server" Text="Ulangi" Icon="Reload">
        <Listeners>
          <Click Handler="#{frm}.getForm().reset();" />
        </Listeners>
      </ext:Button>
    </Buttons>
  </ext:Window>
</asp:Content>
