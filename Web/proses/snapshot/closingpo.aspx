<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true"
  CodeFile="closingpo.aspx.cs" Inherits="proses_snapshot_closingpo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="Server">

  <script type="text/javascript" language="javascript">
    var onProsesClick = function(stor) {
      showMaskLoad(Ext, 'Mohon tunggu..', false);

      stor.load();
    }
    var onLoadStore = function() {
      showMaskLoad(Ext, 'Mohon tunggu..', true);

      ShowInformasi('Proses telah dijalankan.');
    }
    var onExceptionStore = function() {
      showMaskLoad(Ext, 'Mohon tunggu..', true);
    }
  </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="Server">
  <ext:Hidden ID="hfUser" runat="server" />
  <ext:Store ID="storCallSP" runat="server">
    <Proxy>
      <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
        CallbackParam="soaScmsCallback" />
    </Proxy>
    <Reader>
      <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
        IDProperty="ID">
        <Fields>
          <ext:RecordField Name="ID" />
          <ext:RecordField Name="Result" Type="Boolean" />
        </Fields>
      </ext:JsonReader>
    </Reader>
    <BaseParams>
      <ext:Parameter Name="start" Value="0" />
      <ext:Parameter Name="limit" Value="-1" />
      <ext:Parameter Name="allQuery" Value="true" />
      <ext:Parameter Name="model" Value="30002" />
      <ext:Parameter Name="parameters" Value="[['Tahun', paramValueGetter(#{cbTahun}), 'System.Int32'],
                      ['Bulan', paramValueGetter(#{cbBulan}), 'System.Int32'],
                      ['User', paramValueGetter(#{hfUser}), 'System.String'],
                      ['Item', paramValueGetter(#{cbItems}), 'System.String']]" Mode="Raw" />
      <ext:Parameter Name="sort" Value="" />
      <ext:Parameter Name="dir" Value="ASC" />
    </BaseParams>
    <Listeners>
      <Load Fn="onLoadStore" />
      <Exception Fn="onExceptionStore" />
    </Listeners>
  </ext:Store>
  <ext:Viewport ID="Viewport1" runat="server" Layout="Fit">
    <Items>
      <ext:Panel ID="pnlMainControl" runat="server" AutoScroll="true" Layout="FormLayout"
        Padding="5" ButtonAlign="Center">
        <Items>
          <ext:SelectBox ID="cbTahun" runat="server" FieldLabel="Tahun" Width="75" AllowBlank="false" />
          <ext:SelectBox ID="cbBulan" runat="server" FieldLabel="Bulan" Width="100" AllowBlank="false" />
          <ext:ComboBox ID="cbItems" runat="server" FieldLabel="Barang" AllowBlank="true" ForceSelection="false"
            Width="250" ValueField="c_iteno" DisplayField="v_itnam" ListWidth="350" ItemSelector="tr.search-item"
            PageSize="10">
            <CustomConfig>
              <ext:ConfigItem Name="allowBlank" Value="true" />
            </CustomConfig>
            <Triggers>
              <ext:FieldTrigger Icon="Clear" Qtip="Clear" HideTrigger="true" />
              <ext:FieldTrigger Icon="Search" Qtip="Reload" />
            </Triggers>
            <Listeners>
              <Select Handler="this.triggers[0].show();" />
              <BeforeQuery Handler="this.triggers[0][ this.getRawValue().toString().length == 0 ? 'hide' : 'show']();" />
              <TriggerClick Handler="if(index == 0) { this.clearValue(); this.triggers[0].hide(); } else if(index == 1) { reloadData(this); }" />
            </Listeners>
            <Template ID="Template1" runat="server">
              <Html>
              <table cellpading="0" cellspacing="0" style="width: 500px">
                <tr><td class="body-panel">Kode</td><td class="body-panel">Nama</td></tr>
                <tpl for=".">
                  <tr class="search-item">
                    <td>{c_iteno}</td><td>{v_itnam}</td>
                  </tr>
                </tpl>
                </table>
              </Html>
            </Template>
            <Store>
              <ext:Store ID="Store1" runat="server">
                <Proxy>
                  <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                    CallbackParam="soaScmsCallback" />
                </Proxy>
                <BaseParams>
                  <ext:Parameter Name="start" Value="={0}" />
                  <ext:Parameter Name="limit" Value="={10}" />
                  <ext:Parameter Name="model" Value="2061" />
                  <ext:Parameter Name="parameters" Value="[['l_hide != @0', true, 'System.Boolean'],
                      ['l_aktif == @0', true, 'System.Boolean']]" Mode="Raw" />
                  <ext:Parameter Name="sort" Value="v_itnam" />
                  <ext:Parameter Name="dir" Value="ASC" />
                </BaseParams>
                <Reader>
                  <ext:JsonReader IDProperty="c_iteno" Root="d.records" SuccessProperty="d.success"
                    TotalProperty="d.totalRows">
                    <Fields>
                      <ext:RecordField Name="c_iteno" />
                      <ext:RecordField Name="c_itenopri" />
                      <ext:RecordField Name="v_itnam" />
                    </Fields>
                  </ext:JsonReader>
                </Reader>
              </ext:Store>
            </Store>
          </ext:ComboBox>
        </Items>
        <Buttons>
          <ext:Button ID="btnProses" runat="server" Text="Proses" Icon="CogStart">
            <Listeners>
              <Click Handler="onProsesClick(#{storCallSP});" />
            </Listeners>
          </ext:Button>
        </Buttons>
      </ext:Panel>
    </Items>
  </ext:Viewport>
</asp:Content>
