<%--Created By Indra Monitoring Process 20180523FM--%>

<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MonitoringPLSummary.ascx.cs" Inherits="transaksi_wp_MonitoringPLSummary" %>


<ext:Window ID="winDetail2" runat="server" Hight="300" Width="400"
  Maximizable="false" MinHeight="300" MinWidth="400" Layout="Fit" Resizable="false">
   <Content>
    <ext:Hidden ID="hfStoreID" runat="server" />
    <ext:Hidden ID="hfNomorForm" runat="server" />
    <ext:Hidden ID="hfSupllier" runat="server" />
    <ext:Hidden ID="hfDivSupllier" runat="server" />
  </Content>
  
  <Items>
    <ext:FormPanel ID="frmHeaders" runat="server" Title="" Layout="Form" ButtonAlign="Center" MonitorValid="true" MinHeight="500">
        
        <Items>    
        <ext:Container ID="Container2" runat="server" Layout="FormLayout" Cls="keterangan" Height="300">     
            <Items>
                <ext:Label ID="lblCABANG" runat ="server" StyleSpec="font-size:16pt;"></ext:Label>
                <ext:Label ID="lblSPRECEIVED" runat ="server" StyleSpec="font-size:16pt;"></ext:Label>
                <ext:Label ID="lblCREATEPL" runat ="server" StyleSpec="font-size:16pt;" ></ext:Label>
                <ext:Label ID="lblPICKING" runat ="server" StyleSpec="font-size:16pt;"></ext:Label>            
                <ext:Label ID="lblCHECKING" runat ="server" StyleSpec="font-size:16pt;" ></ext:Label>
                <ext:Label ID="lblCREATEDO" runat ="server" StyleSpec="font-size:16pt;" ></ext:Label>
                <ext:Label ID="lblPACKING" runat ="server" StyleSpec="font-size:16pt;"></ext:Label>
                <ext:Label ID="lblREADY" runat ="server" StyleSpec="font-size:16pt;" ></ext:Label>
            </Items>
        </ext:Container>                        
            
        </Items>
    </ext:FormPanel>
  
  </Items>
  
  
  
</ext:Window>