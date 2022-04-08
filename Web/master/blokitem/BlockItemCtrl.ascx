<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BlockItemCtrl.ascx.cs"
  Inherits="master_item_MasterItemCtrl" %>
<ext:Window ID="winDetail" runat="server" Height="400" Width="800" Hidden="true"
  Maximizable="true" MinHeight="400" MinWidth="725" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfItemNo" runat="server" />
  </Content>
  <Items>
    <ext:GridPanel ID="gridDetail" runat="server">
      <SelectionModel>
        <ext:RowSelectionModel SingleSelect="true" />
      </SelectionModel>
      <Store>
        <ext:Store ID="Store1" runat="server" RemotePaging="true" RemoteSort="true">
          <Proxy>
            <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
              CallbackParam="soaScmsCallback" />
          </Proxy>
          <BaseParams>
            <ext:Parameter Name="start" Value="0" />
            <ext:Parameter Name="limit" Value="parseInt(#{cbGmPagingBB}.getValue())" Mode="Raw" />
            <ext:Parameter Name="model" Value="0164" />
            <ext:Parameter Name="sort" Value="d_start" />
            <ext:Parameter Name="dir" Value="DESC" />
            <ext:Parameter Name="parameters" Value="[['itemCode', #{hfItemNo}.getValue(), 'System.String']]"
              Mode="Raw" />
          </BaseParams>
          <Reader>
            <ext:JsonReader TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success"
              IDProperty="idx">
              <Fields>
                <ext:RecordField Name="idx" />
                <ext:RecordField Name="d_start" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="d_end" Type="Date" DateFormat="M$" />
                <ext:RecordField Name="l_status" Type="Boolean" />
                <ext:RecordField Name="UEntry" />
                <ext:RecordField Name="UUpdate" />
              </Fields>
            </ext:JsonReader>
          </Reader>
          <SortInfo Field="d_start" Direction="DESC" />
        </ext:Store>
      </Store>
      <ColumnModel>
        <Columns>
          <ext:DateColumn DataIndex="d_start" Header="Awal" Format="dd-MM-yyyy" />
          <ext:DateColumn DataIndex="d_end" Header="Akhir" Format="dd-MM-yyyy" />
          <ext:Column DataIndex="UEntry" Header="Kunci" Width="150" />
          <ext:Column DataIndex="UUpdate" Header="Buka" Width="150" />
        </Columns>
      </ColumnModel>
      <BottomBar>
        <ext:PagingToolbar runat="server" ID="gmPagingBB" PageSize="20">
          <Items>
            <ext:Label ID="Label1" runat="server" Text="Page size:" />
            <ext:ToolbarSpacer ID="ToolbarSpacer1" runat="server" Width="10" />
            <ext:ComboBox ID="cbGmPagingBB" runat="server" Width="80">
              <Items>
                <ext:ListItem Text="5" />
                <ext:ListItem Text="10" />
                <ext:ListItem Text="20" />
                <ext:ListItem Text="50" />
                <ext:ListItem Text="100" />
              </Items>
              <SelectedItem Value="20" />
              <Listeners>
                <Select Handler="#{gmPagingBB}.pageSize = parseInt(this.getValue()); #{gmPagingBB}.doLoad();" />
              </Listeners>
            </ext:ComboBox>
          </Items>
        </ext:PagingToolbar>
      </BottomBar>
    </ext:GridPanel>
  </Items>
  <Buttons>
    <ext:Button ID="Button3" runat="server" Icon="Cancel" Text="Keluar">
      <Listeners>
        <Click Handler="#{winDetail}.hide();" />
      </Listeners>
    </ext:Button>
  </Buttons>
</ext:Window>
