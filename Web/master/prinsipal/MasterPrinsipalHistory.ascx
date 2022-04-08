<%--
 Created By Indra
 20171231FM
--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterPrinsipalHistory.ascx.cs" 
Inherits="master_prinsipal_MasterPrinsipalHistory" %>

<ext:Window ID="winDetail" runat="server" Height="500" Width="1350" Hidden="true" Resizable="false"
  Maximizable="true" MinHeight="500" MinWidth="1350" Layout="Fit">
  <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
  </Content>
  <Items>
    <ext:BorderLayout ID="bllayout" runat="server">
      <Center MinHeight="150">
        <ext:Panel ID="pnlGridDetail" runat="server" Title="History Perubahan Leadtime" Height="150" Layout="Fit">
          <Items>
            <ext:GridPanel ID="gridDetailHistory" runat="server">
              <SelectionModel>
                <ext:RowSelectionModel SingleSelect="true" />
              </SelectionModel>
              <Store>
                <ext:Store ID="Store1" runat="server" RemotePaging="false" RemoteSort="false">
                  <Proxy>
                    <ext:ScriptTagProxy Url="http://localhost:1234/scms/WebJsonP/GlobalQueryJson" Timeout="10000000"
                      CallbackParam="soaScmsCallback" />
                  </Proxy>
                  <BaseParams>
                    <ext:Parameter Name="start" Value="0" />
                    <ext:Parameter Name="limit" Value="-1" />
                    <ext:Parameter Name="allQuery" Value="true" />
                    <ext:Parameter Name="model" Value="0152b" />
                    <ext:Parameter Name="sort" Value="" />
                    <ext:Parameter Name="dir" Value="" />
                    <ext:Parameter Name="parameters" Value="[]" Mode="Raw" />
                  </BaseParams>
                  <Reader>
                    <ext:JsonReader IDProperty="c_iteno" TotalProperty="d.totalRows" Root="d.records" SuccessProperty="d.success">
                      <Fields>
                        <ext:RecordField Name="KODE_SUPPLIER" />
                        <ext:RecordField Name="NAMA_SUPPLIER" />
                        <ext:RecordField Name="NAMA_REQUESTOR" />
                        <ext:RecordField Name="ALASAN_REQUEST" />
                        <ext:RecordField Name="TGL_REQUEST" Type="Date" DateFormat="M$" />   
                        <ext:RecordField Name="NAMA_APPROVAL" />
                        <ext:RecordField Name="ALASAN_APPROVAL" />
                        <ext:RecordField Name="TGL_APPROVAL" Type="Date" DateFormat="M$" />   
                        <ext:RecordField Name="LEADTIME_AWAL" Type="Float" />
                        <ext:RecordField Name="LEADTIME_PERUBAHAN" Type="Float" />
                        <ext:RecordField Name="EFFECTTIVE_LEADTIME" Type="Date" DateFormat="M$" />                        
                        <ext:RecordField Name="STATUS" />
                      </Fields>
                    </ext:JsonReader>
                  </Reader>
                  <SortInfo Field="TGL_REQUEST" Direction="DESC" />
                </ext:Store>
              </Store>
              <ColumnModel>
                <Columns>
                
                  <ext:Column DataIndex="KODE_SUPPLIER" Header="Kode Supplier" Width="90" />
                  <ext:Column DataIndex="NAMA_SUPPLIER" Header="Nama Supplier" width="150" />
                  <ext:Column DataIndex="NAMA_REQUESTOR" Header="Nama Requestor" width="150" />
                  <ext:Column DataIndex="ALASAN_REQUEST" Header="Alasan Request" width="150" />
                  <ext:DateColumn DataIndex="TGL_REQUEST" Header="Tgl. Request" Format="dd-MM-yyyy" Width="87" />
                  <ext:Column DataIndex="NAMA_APPROVAL" Header="Nama Approval" width="150" />
                  <ext:Column DataIndex="ALASAN_APPROVAL" Header="Alasan Approved" width="150" />
                  <ext:DateColumn DataIndex="TGL_APPROVAL" Header="Tgl. Approved" Format="dd-MM-yyyy" Width="80" />
                  <ext:Column DataIndex="LEADTIME_AWAL" Header="LT. Awal" Width="70" />
                  <ext:Column DataIndex="LEADTIME_PERUBAHAN" Header="LT. Perubahan" Width="93"/>
                  <ext:DateColumn DataIndex="EFFECTTIVE_LEADTIME" Header="Effective Date" Format="dd-MM-yyyy" Width="80" />
                  <ext:Column DataIndex="STATUS" Header="St. Pengajuan" Width="80"/>
                </Columns>
              </ColumnModel>
            </ext:GridPanel>
          </Items>
        </ext:Panel>
      </Center>
    </ext:BorderLayout>
  </Items>
  
</ext:Window>
<ext:Window ID="wndDown" runat="server" Hidden="true" />