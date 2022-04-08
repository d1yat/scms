<%--
Created By Indra
20171231FM
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StockOpnameNewBatch.ascx.cs" Inherits="proses_stock_opname_StockOpnameNewBatch" %>

<script type="text/javascript">
  
</script>

<ext:Window ID="winDetail2" runat="server" Hight="260" Width="400" Hidden="true"
  Maximizable="false" MinHeight="260" MinWidth="400" Layout="Fit" Resizable="false">
   <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfNomorForm" runat="server" />
    <ext:Hidden ID="hfSupllier" runat="server" />
    <ext:Hidden ID="hfDivSupllier" runat="server" />
  </Content>
  
  <Items>
    <ext:FormPanel ID="FormPanel1" runat="server" Layout="Form" Title="New Batch" Padding="5" LabelWidth="150">
      <Items>     
        <ext:Label ID="lbPemasok" runat="server" FieldLabel = "Principle"></ext:Label>
        <ext:Label ID="lbDivPemasok" runat="server" FieldLabel = "Div. Principle"></ext:Label>
        <ext:ComboBox ID="cbItems" runat="server" ValueField="c_iteno" FieldLabel="Product"
          DisplayField="v_itnam" Width="170" ListWidth="500" 
          PageSize="10" ItemSelector="tr.search-item" AllowBlank="true" ForceSelection="false">
          <CustomConfig>
            <ext:ConfigItem Name="allowBlank" Value="false" />
          </CustomConfig>
          <Store>
            <ext:Store ID="Store5" runat="server">
              <Proxy>
                <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                  CallbackParam="soaScmsCallback" />
              </Proxy>
              <BaseParams>
                <ext:Parameter Name="start" Value="={0}" />
                <ext:Parameter Name="limit" Value="={10}" />
                <ext:Parameter Name="model" Value="2141" />
                <ext:Parameter Name="parameters" Value="[['@in.c_nosup', paramValueMultiGetter(#{hfSupllier}), 'System.String[]'],
                        ['@in.c_kddivpri', paramValueMultiGetter(#{hfDivSupllier}), 'System.String[]'],
                        ['@contains.c_iteno.Contains(@0) || @contains.v_itnam.Contains(@0)', paramTextGetter(#{cbItems}), '']]" Mode="Raw" />                        
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
          <Template ID="Template1" runat="server">
            <Html>
            <table cellpading="0" cellspacing="0" style="width: 500px">
            <tr><td class="body-panel">Kode</td><td class="body-panel">Kode Pemasok</td><td class="body-panel">Nama</td></tr>
            <tpl for=".">
              <tr class="search-item">
                <td>{c_iteno}</td><td>{c_itenopri}</td><td>{v_itnam}</td>
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
        <ext:TextField ID="txBatch" runat="server" MaxLength="25" Width="160" FieldLabel="Kd. Batch" AllowBlank="false" />
        <ext:CompositeField ID="CompositeField1" runat="server" FieldLabel="Expired">
          <Items>
            <ext:DateField ID="txExpired" runat="server" Vtype="daterange" Format="dd-MM-yyyy"
              AllowBlank="false">
            </ext:DateField>
          </Items>
        </ext:CompositeField>
      </Items>
    </ext:FormPanel>
  </Items>
  <Buttons>
    <ext:Button ID="btnSave" runat="server" Icon="ScriptAdd" Text="Simpan">
      <DirectEvents>
        <Click OnEvent="SaveBtn_Click" Before="return verifyHeaderAndDetail(#{FormPanel1});">
          <Confirmation BeforeConfirm="return verifyHeaderAndDetail(#{frmHeaders});"
            ConfirmRequest="true" Title="Simpan ?" Message="Anda yakin ingin menyimpan data ini." />
          <ExtraParams>
            <ext:Parameter Name="StoreID" Value="#{hfStoreID}.getValue()" Mode="Raw" />
          </ExtraParams>
        </Click>
      </DirectEvents>
    </ext:Button>
  </Buttons>
  </ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />