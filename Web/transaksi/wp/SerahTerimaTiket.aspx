<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SerahTerimaTiket.aspx.cs" 
Inherits="serahterima_tiket" MasterPageFile="~/Master.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Content>
    <ext:Hidden ID="hfNumberId" runat="server" />
    <ext:Window ID="wndDown" runat="server" Hidden="true" />
   </Content>
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" AutoScroll="true" Layout="FormLayout"
        Padding="5" ButtonAlign="Center">
        <Items>
          <ext:ComboBox ID="cbSuplier" runat="server" FieldLabel="Pemasok" ValueField="c_nosup"
          DisplayField="v_nama" Width="250" ListWidth="300" PageSize="10"
          ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="true" />
          </CustomConfig>
          <Store>
            <ext:Store ID="Store3" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2021" />
                <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                  ['l_aktif == @0', true, 'System.Boolean'],
                  ['@contains.c_nosup.Contains(@0) || @contains.v_nama.Contains(@0)', paramTextGetter(#{cbSuplier}), '']]" Mode="Raw" />
                <ext:Parameter Name="sort" Value="v_nama" />
                <ext:Parameter Name="dir" Value="ASC" />
              </BaseParams>
              <Reader>
                <ext:JsonReader IDProperty="c_nosup" Root="d.records" SuccessProperty="d.success"
                  TotalProperty="d.totalRows">
                  <Fields>
                    <ext:RecordField Name="c_nosup" />
                    <ext:RecordField Name="v_nama" />
                  </Fields>
                </ext:JsonReader>
              </Reader>
            </ext:Store>
          </Store>
          <Template ID="Template2" runat="server">
            <Html>
            <table cellpading="0" cellspacing="0" style="width: 300px">
            <tr><td class="body-panel" style="width: 50px">Kode</td><td class="body-panel" style="width: 250px">Pemasok</td></tr>
            <tpl for=".">
              <tr class="search-item">
                <td>{c_nosup}</td><td>{v_nama}</td>
              </tr>
            </tpl>
            </table>
            </Html>
          </Template>
          <Triggers>
            <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
            <ext:FieldTrigger Icon="Search" Qtip="Reload" />
          </Triggers>
          <Listeners>
            <Select Handler="this.triggers[0].show();" />
            <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
            <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
          </Listeners>
        </ext:ComboBox>
        <ext:TextField ID="txNopol" runat="server" AllowBlank="false" FieldLabel="Nomor Kendaraan"
          MaxLength="9" Width="100" />
        <ext:Checkbox ID="chkAsync" runat="server" FieldLabel="Asyncronous" Checked="false" hidden="true" />
        </Items>
        <Buttons>
        <ext:Button ID="Button1" runat="server" Icon="Disk" Text="Generate">
          <DirectEvents>
            <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{frmHeaders});">
              <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
                ConfirmRequest="true" Title="Generate ?" Message="Generate Tiket?" />
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="NumberID" Value="#{hfNumberId}.getValue()" Mode="Raw" />
              </ExtraParams>
            </Click>
          </DirectEvents>
        </ext:Button>
        <%--<ext:Button ID="btnPrint" runat="server" Icon="Printer" Text="Cetak">
          <DirectEvents>
            <Click OnEvent="Report_OnGenerate">
                <Confirmation ConfirmRequest="true" Title="Cetak" Message="Anda yakin ingin mencetak data ini?" />
              <EventMask ShowMask="true" />
              <ExtraParams>
                <ext:Parameter Name="NumberID" Value="#{hfNumberId}.getValue()" Mode="Raw" />
              </ExtraParams>
            </Click>
          </DirectEvents>
        </ext:Button>--%>
        </Buttons>
      </ext:Panel>
    </Items>
   </ext:Viewport>
</asp:Content>