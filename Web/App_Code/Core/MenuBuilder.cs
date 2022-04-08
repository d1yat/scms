using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Xml;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Soap;
using Scms.Web.Common;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Scms.Web.Core
{
  #region Menu Builder

  /// <summary>
  /// Summary description for MenuBuilder
  /// </summary>
  public class MenuBuilder
  {
    #region Internal Class

    public class MenuConfiguration
    {
      public string Scripting { get; set; }
      public MenuModule[] Modules { get; set; }
      public MenuApplication[] Applications { get; set; }
    }

    public class MenuApplication
    {
      public string ID { get; set; }
      public int Icon { get; set; }
      public string CustomIcon { get; set; }
      public bool AutoRun { get; set; }
      public string WindowID { get; set; }
      public string Text { get; set; }
      public MenuModule[] Modules { get; set; }
      public int OrderID { get; set; }
    }

    public class MenuModule
    {
      public string ID { get; set; }
      public MenuLauncer Launcher { get; set; }
      public int OrderID { get; set; }
    }

    public class MenuLauncer
    {
      public string Text { get; set; }
      public int Icon { get; set; }
      public string CustomIcon { get; set; }
      public bool Direct { get; set; }
      public bool Nested { get; set; }
      public MenuItem SingleItem { get; set; }
      public MenuItem[] Items { get; set; }
      public MenuItemLauncher[] MenuItemLauncher { get; set; }
      public bool Separator { get; set; }
      public bool NestedDirected { get; set; }
    }

    public class MenuItemLauncher
    {
      public string ID { get; set; }
      public int Icon { get; set; }
      public string CustomIcon { get; set; }
      public string Text { get; set; }
      public MenuItem[] Items { get; set; }
      public MenuItem SingleItem { get; set; }
      public bool IsSingle { get; set; }
    }

    public class MenuItem
    {
      public string ID { get; set; }
      public string WinID { get; set; }
      public string Title { get; set; }
      public string Text { get; set; }
      public int Icon { get; set; }
      public string CustomIcon { get; set; }
      public bool Center { get; set; }
      public bool FullClosed { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
      public string Url { get; set; }
      public string ShortUrl { get; set; }
      public string Query { get; set; }
      public bool Separator { get; set; }
      public int OrderID { get; set; }
      public bool Resizeable { get; set; }
      public bool OutsideSubMenu { get; set; }
      public string DirectJSClick { get; set; }
      public bool IsShortcut { get; set; }
      public string ShortcutClass { get; set; }
      public string DesktopShortcutModID { get; set; }
    }

    #endregion

    #region Private

    private Random rnd;

    private int ConvertTypeIcon(string name)
    {
      if (string.IsNullOrEmpty(name))
      {
        return 0;
      }

      int result = 0;

      System.Type type = typeof(Ext.Net.Icon);
      System.Reflection.FieldInfo fi = null;
      Ext.Net.Icon icon = Ext.Net.Icon.None;

      try
      {
        fi = type.GetField(name);
        icon = (Ext.Net.Icon)fi.GetValue(null);

        result = (int)icon;
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.MenuBuilder:ConvertType - {0}", ex.Message);
      }

      return result;
    }

    private string RandomEmptyID(string currentId)
    {
      string result = null;

      if (string.IsNullOrEmpty(currentId))
      {
        result = rnd.Next(1, int.MaxValue).ToString();
      }
      else
      {
        result = currentId;
      }

      return result;
    }

    #endregion

    public MenuBuilder()
    {
      //
      // TODO: Add constructor logic here
      //

      rnd = new Random((int)DateTime.Now.Ticks);
    }

    public MenuConfiguration RebuildMenuConfiguration(string xml)
    {
      if (string.IsNullOrEmpty(xml))
      {
        return null;
      }

      XmlDocument xDoc = new XmlDocument();
      XmlNode node = null;
      MenuConfiguration menuConfig = new MenuConfiguration();

      try
      {
        xDoc.LoadXml(xml);

        node = xDoc.SelectSingleNode("Config");

        if ((node != null) && node.HasChildNodes)
        {
          node = node.FirstChild;
          XmlNode nodeC = null;
          XmlNode nodeApp = null;
          XmlNode nodeCMod = null;
          XmlNode nodePCLaunch = null;
          XmlNode nodeCLaunch = null;

          List<MenuApplication> listApps = new List<MenuApplication>();
          List<MenuModule> listModules = new List<MenuModule>();
          List<MenuItem> listItems = new List<MenuItem>();
          //MenuModule mod = null;
          MenuLauncer launc = null;
          MenuItemLauncher mnuLaunc = null;
          List<MenuItemLauncher> listMnuItmLauch = new List<MenuItemLauncher>();
          MenuItem mnuItem = null;

          bool isDirect = false;

          while (node != null)
          {
            if (node.NodeType == XmlNodeType.CDATA)
            {
              menuConfig.Scripting = node.InnerText;
            }
            else if ((node.NodeType == XmlNodeType.Element) && node.Name.Equals("Items", StringComparison.OrdinalIgnoreCase))
            {
              // Items
              nodeApp = node.FirstChild;

              while (nodeApp != null)
              {
                if ((nodeApp.NodeType == XmlNodeType.Element) && nodeApp.Name.Equals("Application"))
                {
                  #region Application

                  nodeC = nodeApp.FirstChild;

                  while (nodeC != null)
                  {

                    if ((nodeC.NodeType == XmlNodeType.Element) && nodeC.Name.Equals("Module"))
                    {
                      #region Modules

                      // Modules
                      nodeCMod = nodeC.FirstChild;

                      while (nodeCMod != null)
                      {
                        // Modules

                        if ((nodeCMod.NodeType == XmlNodeType.Element) && nodeCMod.Name.Equals("Launcher"))
                        {
                          #region Launchers

                          launc = new MenuLauncer();

                          isDirect = ReaderXml.ReadAttributeBool(nodeCMod, "direct");

                          launc.Text = ReaderXml.ReadAttribute(nodeCMod, "text");

                          launc.Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeCMod, "icon"));

                          launc.CustomIcon = ReaderXml.ReadAttribute(nodeCMod, "CustomIcon");

                          if (isDirect)
                          {
                            launc.Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeCMod, "launcherIcon"));

                            launc.Direct = true;

                            mnuItem = new MenuItem()
                            {
                              ID = RandomEmptyID(ReaderXml.ReadAttribute(nodeCMod, "ID")),
                              WinID = ReaderXml.ReadAttribute(nodeCMod, "WinID"),
                              Center = ReaderXml.ReadAttributeBool(nodeCMod, "Center", true),
                              FullClosed = ReaderXml.ReadAttributeBool(nodeCMod, "FullClosed"),
                              Height = ReaderXml.ReadAttributeInt(nodeCMod, "Height"),
                              Width = ReaderXml.ReadAttributeInt(nodeCMod, "Width"),
                              //Icon = ReaderXml.ReadAttribute(nodeCMod, "icon"),
                              Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeCMod, "icon")),
                              CustomIcon = ReaderXml.ReadAttribute(nodeCMod, "CustomIcon"),
                              Title = ReaderXml.ReadAttribute(nodeCMod, "Title"),
                              Text = ReaderXml.ReadAttribute(nodeCMod, "Text"),
                              Url = ReaderXml.ReadAttribute(nodeCMod, "Url"),
                              ShortUrl = ReaderXml.ReadAttribute(nodeCMod, "short"),
                              Query = ReaderXml.ReadAttribute(nodeCMod, "query"),
                              OrderID = ReaderXml.ReadAttributeInt(nodeCMod, "order"),
                              Resizeable = ReaderXml.ReadAttributeBool(nodeCMod, "Resizeable", true),
                              DirectJSClick = ReaderXml.ReadAttribute(nodeCMod, "directJSClick"),
                              IsShortcut = ReaderXml.ReadAttributeBool(nodeCMod, "IsShortcut", false),
                              ShortcutClass = ReaderXml.ReadAttribute(nodeCMod, "ShortcutClass")
                            };

                            mnuItem.Text = (string.IsNullOrEmpty(mnuItem.Text) ? mnuItem.Title : mnuItem.Text);
                            mnuItem.DesktopShortcutModID = (mnuItem.IsShortcut ? string.Concat("dm_", mnuItem.ID) : string.Empty);

                            launc.SingleItem = mnuItem;
                          }
                          else
                          {
                            #region Menu Item Launch

                            nodePCLaunch = nodeCMod.FirstChild;

                            while (nodePCLaunch != null)
                            {
                              #region Items

                              if ((nodePCLaunch.NodeType == XmlNodeType.Element) && nodePCLaunch.Name.Equals("Items"))
                              {
                                launc.Nested = true;

                                mnuLaunc = new MenuItemLauncher();

                                mnuLaunc.ID = RandomEmptyID(ReaderXml.ReadAttribute(nodePCLaunch, "id"));

                                mnuLaunc.Text = ReaderXml.ReadAttribute(nodePCLaunch, "text");

                                mnuLaunc.Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodePCLaunch, "icon"));

                                mnuLaunc.CustomIcon = ReaderXml.ReadAttribute(nodePCLaunch, "CustomIcon");

                                nodeCLaunch = nodePCLaunch.FirstChild;

                                while (nodeCLaunch != null)
                                {
                                  if ((nodeCLaunch.NodeType == XmlNodeType.Element) && nodeCLaunch.Name.Equals("Item"))
                                  {
                                    mnuItem = new MenuItem()
                                    {
                                      ID = RandomEmptyID(ReaderXml.ReadAttribute(nodeCLaunch, "ID")),
                                      WinID = ReaderXml.ReadAttribute(nodeCLaunch, "WinID"),
                                      Center = ReaderXml.ReadAttributeBool(nodeCLaunch, "Center", true),
                                      FullClosed = ReaderXml.ReadAttributeBool(nodeCLaunch, "FullClosed"),
                                      Height = ReaderXml.ReadAttributeInt(nodeCLaunch, "Height"),
                                      Width = ReaderXml.ReadAttributeInt(nodeCLaunch, "Width"),
                                      Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeCLaunch, "icon")),
                                      CustomIcon = ReaderXml.ReadAttribute(nodeCLaunch, "CustomIcon"),
                                      Title = ReaderXml.ReadAttribute(nodeCLaunch, "Title"),
                                      Text = ReaderXml.ReadAttribute(nodeCLaunch, "Text"),
                                      Url = ReaderXml.ReadAttribute(nodeCLaunch, "Url"),
                                      ShortUrl = ReaderXml.ReadAttribute(nodeCLaunch, "short"),
                                      Query = ReaderXml.ReadAttribute(nodeCLaunch, "query"),
                                      OrderID = ReaderXml.ReadAttributeInt(nodeCLaunch, "order"),
                                      Resizeable = ReaderXml.ReadAttributeBool(nodeCLaunch, "Resizeable", true),
                                      DirectJSClick = ReaderXml.ReadAttribute(nodeCLaunch, "directJSClick"),
                                      IsShortcut = ReaderXml.ReadAttributeBool(nodeCLaunch, "IsShortcut", false),
                                      ShortcutClass = ReaderXml.ReadAttribute(nodeCLaunch, "ShortcutClass")
                                    };

                                    mnuItem.Text = (string.IsNullOrEmpty(mnuItem.Text) ? mnuItem.Title : mnuItem.Text);
                                    mnuItem.DesktopShortcutModID = (mnuItem.IsShortcut ? string.Concat("dm_", mnuItem.ID) : string.Empty);

                                    listItems.Add(mnuItem);
                                  }
                                  else if ((nodeCLaunch.NodeType == XmlNodeType.Element) && nodeCLaunch.Name.Equals("Separator"))
                                  {
                                    listItems.Add(new MenuItem()
                                    {
                                      Separator = true,
                                      OrderID = ReaderXml.ReadAttributeInt(nodeCLaunch, "order")
                                    });
                                  }
                                  nodeCLaunch = nodeCLaunch.NextSibling;
                                }

                                mnuLaunc.Items = listItems.ToArray();

                                listMnuItmLauch.Add(mnuLaunc);

                                listItems.Clear();
                              }
                              else if ((nodePCLaunch.NodeType == XmlNodeType.Element) && nodePCLaunch.Name.Equals("Item"))
                              {
                                mnuItem = new MenuItem()
                                  {
                                    ID = RandomEmptyID(ReaderXml.ReadAttribute(nodePCLaunch, "ID")),
                                    WinID = ReaderXml.ReadAttribute(nodePCLaunch, "WinID"),
                                    Center = ReaderXml.ReadAttributeBool(nodePCLaunch, "Center", true),
                                    FullClosed = ReaderXml.ReadAttributeBool(nodePCLaunch, "FullClosed"),
                                    Height = ReaderXml.ReadAttributeInt(nodePCLaunch, "Height"),
                                    Width = ReaderXml.ReadAttributeInt(nodePCLaunch, "Width"),
                                    Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodePCLaunch, "icon")),
                                    CustomIcon = ReaderXml.ReadAttribute(nodePCLaunch, "CustomIcon"),
                                    Title = ReaderXml.ReadAttribute(nodePCLaunch, "Title"),
                                    Text = ReaderXml.ReadAttribute(nodePCLaunch, "Text"),
                                    Url = ReaderXml.ReadAttribute(nodePCLaunch, "Url"),
                                    ShortUrl = ReaderXml.ReadAttribute(nodePCLaunch, "short"),
                                    Query = ReaderXml.ReadAttribute(nodePCLaunch, "query"),
                                    OrderID = ReaderXml.ReadAttributeInt(nodePCLaunch, "order"),
                                    Resizeable = ReaderXml.ReadAttributeBool(nodePCLaunch, "Resizeable", true),
                                    OutsideSubMenu = true,
                                    DirectJSClick = ReaderXml.ReadAttribute(nodePCLaunch, "directJSClick"),
                                    IsShortcut = ReaderXml.ReadAttributeBool(nodePCLaunch, "IsShortcut", false),
                                    ShortcutClass = ReaderXml.ReadAttribute(nodePCLaunch, "ShortcutClass")
                                  };

                                mnuItem.Text = (string.IsNullOrEmpty(mnuItem.Text) ? mnuItem.Title : mnuItem.Text);
                                mnuItem.DesktopShortcutModID = (mnuItem.IsShortcut ? string.Concat("dm_", mnuItem.ID) : string.Empty);

                                listMnuItmLauch.Add(new MenuItemLauncher()
                                {
                                  Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodePCLaunch, "icon")),
                                  CustomIcon = ReaderXml.ReadAttribute(nodePCLaunch, "CustomIcon"),
                                  ID = RandomEmptyID(ReaderXml.ReadAttribute(nodePCLaunch, "ID")),
                                  IsSingle = true,
                                  //Text = ReaderXml.ReadAttribute(nodePCLaunch, "Title"),
                                  Text = mnuItem.Text,
                                  SingleItem = mnuItem
                                });
                              }

                              #endregion

                              nodePCLaunch = nodePCLaunch.NextSibling;
                            }

                            #endregion
                          }

                          launc.MenuItemLauncher = listMnuItmLauch.ToArray();

                          listMnuItmLauch.Clear();

                          #region Old Coded

                          //if (nodePCLaunch == null)
                          //{
                          //  isDirect = ReaderXml.ReadAttributeBool(nodeCMod, "direct");

                          //  if (isDirect)
                          //  {
                          //    launc.Direct = true;
                          //    launc.SingleItem = new MenuItem()
                          //    {
                          //      ID = ReaderXml.ReadAttribute(nodeCMod, "ID"),
                          //      WinID = ReaderXml.ReadAttribute(nodeCMod, "WinID"),
                          //      Center = ReaderXml.ReadAttributeBool(nodeCMod, "Center"),
                          //      Height = ReaderXml.ReadAttributeInt(nodeCMod, "Height"),
                          //      Width = ReaderXml.ReadAttributeInt(nodeCMod, "Width"),
                          //      //Icon = ReaderXml.ReadAttribute(nodeCMod, "icon"),
                          //      Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeCMod, "icon")),
                          //      Title = ReaderXml.ReadAttribute(nodeCMod, "Title"),
                          //      Url = ReaderXml.ReadAttribute(nodeCMod, "Url"),
                          //      ShortUrl = ReaderXml.ReadAttribute(nodeCMod, "short"),
                          //      Query = ReaderXml.ReadAttribute(nodeCMod, "query"),
                          //      OrderID = ReaderXml.ReadAttributeInt(nodeCMod, "order"),
                          //      Resizeable = ReaderXml.ReadAttributeBool(nodeCMod, "Resizeable")
                          //    };
                          //  }
                          //}
                          //else
                          //{
                          //  if ((nodePCLaunch.NodeType == XmlNodeType.Element) && nodePCLaunch.Name.Equals("Items"))
                          //  {
                          //    launc.Nested = true;

                          //    launc.NestedText = ReaderXml.ReadAttribute(nodePCLaunch, "text");

                          //    launc.NestedIcon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodePCLaunch, "icon"));

                          //    //nodePCLaunch = nodePCLaunch.FirstChild;
                          //  }

                          //  while (nodePCLaunch != null)
                          //  {
                          //    nodeCLaunch = nodePCLaunch.FirstChild;

                          //    while (nodeCLaunch != null)
                          //    {
                          //      if ((nodeCLaunch.NodeType == XmlNodeType.Element) && nodeCLaunch.Name.Equals("Item"))
                          //      {
                          //        listItems.Add(new MenuItem()
                          //        {
                          //          ID = ReaderXml.ReadAttribute(nodeCLaunch, "ID"),
                          //          WinID = ReaderXml.ReadAttribute(nodeCLaunch, "WinID"),
                          //          Center = ReaderXml.ReadAttributeBool(nodeCLaunch, "Center"),
                          //          Height = ReaderXml.ReadAttributeInt(nodeCLaunch, "Height"),
                          //          Width = ReaderXml.ReadAttributeInt(nodeCLaunch, "Width"),
                          //          Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeCLaunch, "icon")),
                          //          Title = ReaderXml.ReadAttribute(nodeCLaunch, "Title"),
                          //          Url = ReaderXml.ReadAttribute(nodeCLaunch, "Url"),
                          //          ShortUrl = ReaderXml.ReadAttribute(nodeCLaunch, "short"),
                          //          Query = ReaderXml.ReadAttribute(nodeCLaunch, "query"),
                          //          OrderID = ReaderXml.ReadAttributeInt(nodeCLaunch, "order"),
                          //          Resizeable = ReaderXml.ReadAttributeBool(nodeCLaunch, "Resizeable")
                          //        });
                          //      }
                          //      else if ((nodeCLaunch.NodeType == XmlNodeType.Element) && nodeCLaunch.Name.Equals("Separator"))
                          //      {
                          //        listItems.Add(new MenuItem()
                          //        {
                          //          Separator = true,
                          //          OrderID = ReaderXml.ReadAttributeInt(nodeCLaunch, "order")
                          //        });
                          //      }
                          //      nodeCLaunch = nodeCLaunch.NextSibling;
                          //    }

                          //    if (nodePCLaunch == null)
                          //      break;

                          //    nodePCLaunch = nodePCLaunch.NextSibling;
                          //  }
                          //}

                          //launc.Text = ReaderXml.ReadAttribute(nodeCMod, "text");
                          //launc.Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeCMod, "icon"));

                          //launc.Items = listItems.ToArray();

                          //listItems.Clear();

                          #endregion

                          #endregion

                          break;
                        }

                        nodeCMod = nodeCMod.NextSibling;
                      }

                      #endregion

                      //mod = new MenuModule()
                      //{
                      //  AutoRun = ReaderXml.ReadAttributeBool(nodeC, "autoRun"),
                      //  ID = RandomEmptyID(ReaderXml.ReadAttribute(nodeC, "id")),
                      //  Launcher = launc,
                      //  OrderID = ReaderXml.ReadAttributeInt(nodeC, "order")
                      //};

                      listModules.Add(new MenuModule()
                      {
                        ID = RandomEmptyID(ReaderXml.ReadAttribute(nodeC, "id")),
                        Launcher = launc,
                        OrderID = ReaderXml.ReadAttributeInt(nodeC, "order")
                      });
                    }

                    nodeC = nodeC.NextSibling;
                  }

                  listApps.Add(new MenuApplication()
                  {
                    AutoRun = ReaderXml.ReadAttributeBool(nodeApp, "autoRun"),
                    ID = RandomEmptyID(ReaderXml.ReadAttribute(nodeApp, "id")),
                    Icon = ConvertTypeIcon(ReaderXml.ReadAttribute(nodeApp, "icon")),
                    CustomIcon = ReaderXml.ReadAttribute(nodeApp, "CustomIcon"),
                    Modules = listModules.ToArray(),
                    OrderID = ReaderXml.ReadAttributeInt(nodeApp, "order"),
                    Text = ReaderXml.ReadAttribute(nodeApp, "text"),
                    WindowID = ReaderXml.ReadAttribute(nodeApp, "windowID")
                  });

                  listModules.Clear();

                  #endregion
                }

                nodeApp = nodeApp.NextSibling;
              }
            }

            node = node.NextSibling;
          }

          //menuConfig.Modules = listModules.ToArray();
          menuConfig.Applications = listApps.ToArray();

          //listModules.Clear();
          listApps.Clear();
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.MenuBuilder:RebuildMenuConfiguration - {0}", ex.Message);
      }

      GC.Collect();

      return menuConfig;
    }

    public MenuConfiguration FilterMenuConfiguration(MenuConfiguration menuConfiguration, RightBuilder.GroupAccess groupAccess)
    {
      if ((menuConfiguration == null) || (groupAccess == null))
      {
        return null;
      }
      MenuConfiguration menuConfig = new MenuConfiguration();

      try
      {
        int nLoop = 0,
          nLen = 0,
          nLoopC = 0,
          nLenC = 0,
          nNext = 0;

        RightBuilder.PageRightAccess pra = null;
        string uri = null;
        string query = null;
        MenuModule mod = null;
        bool beforeIsSeparator = false;

        menuConfig.Scripting = menuConfiguration.Scripting;

        if ((menuConfiguration.Modules != null) && (menuConfiguration.Modules.Length > 0))
        {
          List<MenuModule> listModule = new List<MenuModule>();
          List<MenuItem> listItem = new List<MenuItem>();

          for (nLoop = 0, nLen = menuConfiguration.Modules.Length; nLoop < nLen; nLoop++, listItem.Clear())
          {
            mod = menuConfiguration.Modules[nLoop];
            if ((mod != null) && (mod.Launcher != null))
            {
              if (mod.Launcher.Direct)
              {
                uri = mod.Launcher.SingleItem.ShortUrl;
                query = mod.Launcher.SingleItem.Query;

                pra = groupAccess.FindPage(uri, query);
                
                if (pra == null)
                {
                  mod.Launcher = null;
                }
                else
                {
                  listModule.Add(mod);
                }
              }
              else
              {
                listModule.Add(mod);

                for (nLoopC = 0, nLenC = mod.Launcher.Items.Length; nLoopC < nLenC; nLoopC++)
                {
                  if (!mod.Launcher.Items[nLoopC].Separator)
                  {
                    uri = mod.Launcher.Items[nLoopC].ShortUrl;
                    query = mod.Launcher.Items[nLoopC].Query;

                    pra = groupAccess.FindPage(uri, query);

                    if (pra == null)
                    {
                      mod.Launcher.Items[nLoopC] = null;
                    }
                    else if (!pra.IsView)
                    {
                      mod.Launcher.Items[nLoopC] = null;
                    }
                  }
                }

                beforeIsSeparator = false;

                for (nLoopC = 0, nLenC = mod.Launcher.Items.Length; nLoopC < nLenC; nLoopC++)
                {
                  if (mod.Launcher.Items[nLoopC] != null)
                  {
                    if (mod.Launcher.Items[nLoopC].Separator)
                    {
                      if (!beforeIsSeparator)
                      {
                        nNext = (nLoopC + 1);

                        if (nNext < nLenC)
                        {
                          if ((mod.Launcher.Items[nNext] != null) && (!mod.Launcher.Items[nNext].Separator))
                          {
                            nNext = (nLoopC - 1);

                            if (nNext >= 0)
                            {
                              if ((mod.Launcher.Items[nNext] != null) && (!mod.Launcher.Items[nNext].Separator))
                              {
                                listItem.Add(mod.Launcher.Items[nLoopC]);

                                beforeIsSeparator = true;
                              }
                            }
                          }
                        }
                      }
                    }
                    else
                    {
                      beforeIsSeparator = false;

                      listItem.Add(mod.Launcher.Items[nLoopC]);
                    }
                  }
                }

                if (listItem.Count < 1)
                {
                  listModule.Remove(mod);
                }
                else
                {
                  mod.Launcher.Items = listItem.ToArray();
                }
              }
            }
          }

          menuConfig.Modules = listModule.ToArray();

          listModule.Clear();
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.MenuBuilder:FilterMenuConfiguration - {0}", ex.Message);
      }

      GC.Collect();

      return menuConfig;
    }

    public MenuConfiguration FilterMenuConfigurationArray(MenuConfiguration menuConfiguration, params RightBuilder.GroupAccess[] groupAccess)
    {
      if ((menuConfiguration == null) || (groupAccess == null) || (groupAccess.Length < 1))
      {
        return null;
      }
      MenuConfiguration menuConfig = new MenuConfiguration();

      try
      {
        int nLoop = 0,
          nLen = 0,
          nLoopC = 0,
          nLenC = 0,
          nLoopX = 0,
          nLenX = 0,
          yLoop = 0,
          yLen = 0,
          tLen = 0;

        RightBuilder.PageRightAccess pra = null;
        string uri = null;
        string query = null;
        MenuApplication mnuApp = null;
        MenuApplication appli = null;
        MenuModule mod = null;
        MenuModule newMod = null;
        MenuItem itm = null;
        bool isLastSeparator = false;

        RightBuilder.GroupAccess grpAcc = null;

        menuConfig.Scripting = menuConfiguration.Scripting;

        List<MenuApplication> listApps = new List<MenuApplication>();
        List<MenuApplication> listAppFilter = new List<MenuApplication>();
        List<MenuModule> listModule = new List<MenuModule>();
        List<MenuModule> listModuleFilter = new List<MenuModule>();
        List<MenuItem> listItem = new List<MenuItem>();
        List<MenuItem> listItemStored = new List<MenuItem>();
        List<MenuItem> listFinder = null;
        List<int> listIndexs = new List<int>();
        MenuItemLauncher mnuItmLaunch = null;
        MenuItemLauncher mnuItmLaunchX = null;
        List<MenuItemLauncher> listMnuItmLaunch = new List<MenuItemLauncher>();
        //MenuModule mnuModul = null;
        List<MenuModule> listMnuModul = new List<MenuModule>();
        MenuItem mnuItem = null;

        if ((menuConfiguration.Applications != null) && (menuConfiguration.Applications.Length > 0))
        {
          for (int xLoop = 0, xLen = groupAccess.Length; xLoop < xLen; xLoop++)
          {
            grpAcc = groupAccess[xLoop];

            for (yLoop = 0, yLen = menuConfiguration.Applications.Length; yLoop < yLen; yLoop++, listItem.Clear())
            {
              mnuApp = menuConfiguration.Applications[yLoop];

              appli = listApps.Find(delegate(MenuApplication app)
              {
                return mnuApp.ID.Equals(app.ID, StringComparison.OrdinalIgnoreCase);
              });

              listModule.Clear();

              if (appli == null)
              {
                appli = new MenuApplication()
                {
                  AutoRun = mnuApp.AutoRun,
                  ID = mnuApp.ID,
                  Icon = mnuApp.Icon,
                  CustomIcon = mnuApp.CustomIcon,
                  OrderID = mnuApp.OrderID,
                  Text  = mnuApp.Text,
                  WindowID = mnuApp.WindowID
                };

                listApps.Add(appli);
              }
              else
              {
                listModule.AddRange(appli.Modules);
              }

              #region Populate

              for (nLoop = 0, nLen = mnuApp.Modules.Length; nLoop < nLen; nLoop++, listItem.Clear())
              {
                mod = mnuApp.Modules[nLoop];

                if ((mod != null) & (mod.Launcher != null))
                {
                  if (mod.Launcher.Direct)
                  {
                    #region Direct

                    newMod = listModule.Find(delegate(MenuModule menuModule)
                    {
                      return menuModule.ID.Equals(mod.ID, StringComparison.OrdinalIgnoreCase);
                    });

                    if (newMod == null)
                    {
                      uri = mod.Launcher.SingleItem.ShortUrl;
                      query = mod.Launcher.SingleItem.Query;

                      pra = grpAcc.FindPage(uri, query);

                      if ((pra != null) && pra.IsView)
                      {
                        listModule.Add(mod);
                      }
                    }

                    #endregion
                  }
                  else
                  {
                    newMod = listModule.Find(delegate(MenuModule menuModule)
                    {
                      return menuModule.ID.Equals(mod.ID, StringComparison.OrdinalIgnoreCase);
                    });

                    for (nLoopX = 0, nLenX = mod.Launcher.MenuItemLauncher.Length; nLoopX < nLenX; nLoopX++)
                    {
                      mnuItmLaunch = mod.Launcher.MenuItemLauncher[nLoopX];

                      #region Verify

                      if (mnuItmLaunch.IsSingle)
                      {
                        #region Single Item

                        itm = mnuItmLaunch.SingleItem;

                        if (itm.Separator)
                        {
                          listItem.Add(itm);
                        }
                        else
                        {
                          uri = itm.ShortUrl;
                          query = itm.Query;

                          pra = grpAcc.FindPage(uri, query);

                          if ((pra != null) && pra.IsView)
                          {
                            if (!listItem.Exists(delegate(MenuItem menuItem)
                            {
                              if (!menuItem.Separator)
                              {
                                if (menuItem.ID.Equals(itm.ID, StringComparison.OrdinalIgnoreCase))
                                {
                                  return true;
                                }
                              }

                              return false;
                            }))
                            {
                              listItem.Add(itm);
                            }
                          }
                        }

                        #endregion
                      }
                      else
                      {
                        #region Multi Items

                        for (nLoopC = 0, nLenC = mnuItmLaunch.Items.Length; nLoopC < nLenC; nLoopC++)
                        {
                          itm = mnuItmLaunch.Items[nLoopC];

                          if (itm.Separator)
                          {
                            listItem.Add(itm);
                          }
                          else
                          {
                            uri = itm.ShortUrl;
                            query = itm.Query;

                            pra = grpAcc.FindPage(uri, query);

                            if ((pra != null) && pra.IsView)
                            {
                              if (!listItem.Exists(delegate(MenuItem menuItem)
                              {
                                if (!menuItem.Separator)
                                {
                                  if (menuItem.ID.Equals(itm.ID, StringComparison.OrdinalIgnoreCase))
                                  {
                                    return true;
                                  }
                                }

                                return false;
                              }))
                              {
                                listItem.Add(itm);
                              }
                            }
                          }
                        }

                        #endregion
                      }

                      #endregion

                      if (newMod == null)
                      {
                        #region Clone

                        //listMnuItmLaunch.Add();

                        if (mod.Launcher.SingleItem != null)
                        {
                          mnuItem = new MenuItem()
                            {
                              Center = mod.Launcher.SingleItem.Center,
                              FullClosed = mod.Launcher.SingleItem.FullClosed,
                              Height = mod.Launcher.SingleItem.Height,
                              Icon = mod.Launcher.SingleItem.Icon,
                              CustomIcon = mod.Launcher.SingleItem.CustomIcon,
                              ID = mod.Launcher.SingleItem.ID,
                              OrderID = mod.Launcher.SingleItem.OrderID,
                              Query = mod.Launcher.SingleItem.Query,
                              Separator = mod.Launcher.SingleItem.Separator,
                              ShortUrl = mod.Launcher.SingleItem.ShortUrl,
                              Title = mod.Launcher.SingleItem.Title,
                              Text = mod.Launcher.SingleItem.Text,
                              Url = mod.Launcher.SingleItem.Url,
                              Width = mod.Launcher.SingleItem.Width,
                              WinID = mod.Launcher.SingleItem.WinID,
                              IsShortcut = mod.Launcher.SingleItem.IsShortcut,
                              ShortcutClass = mod.Launcher.SingleItem.ShortcutClass
                            };

                          mnuItem.Text = (string.IsNullOrEmpty(mnuItem.Text) ? mnuItem.Title : mnuItem.Text);
                          mnuItem.DesktopShortcutModID = (mnuItem.IsShortcut ? string.Concat("dm_", mnuItem.ID) : string.Empty);
                        }

                        newMod = new MenuModule()
                        {
                          ID = mod.ID,
                          OrderID = mod.OrderID,
                          Launcher = (mod.Launcher != null ? new MenuLauncer()
                          {
                            Direct = mod.Launcher.Direct,
                            Icon = mod.Launcher.Icon,
                            CustomIcon = mod.Launcher.CustomIcon,
                            //Items = listItem.ToArray(),
                            //MenuItemLauncher = listMnuItmLaunch.ToArray(),

                            MenuItemLauncher = new MenuItemLauncher[] {new MenuItemLauncher()
                                              {
                                                ID = mnuItmLaunch.ID,
                                                Icon = mnuItmLaunch.Icon,
                                                Text = mnuItmLaunch.Text,
                                                Items = listItem.ToArray()
                                              }},
                            Nested = mod.Launcher.Nested,
                            //NestedIcon = mod.Launcher.NestedIcon,
                            //NestedText = mod.Launcher.NestedText,
                            SingleItem = (mod.Launcher.SingleItem != null ? mnuItem : null),
                            Text = mod.Launcher.Text
                          } : null)
                        };

                        #endregion

                        listModule.Clear();

                        if ((appli.Modules != null) && (appli.Modules.Length > 0))
                        {
                          listModule.AddRange(appli.Modules);
                        }

                        listModule.Add(newMod);

                        appli.Modules = listModule.ToArray();
                      }
                      else
                      {
                        #region Revisi Data

                        if (listItem.Count > 0)
                        {
                          if ((listItem.Count == 1) && listItem[0].Separator)
                          {
                            ; // Empty Codes
                          }
                          else
                          {
                            #region Merge

                            mnuItmLaunchX = Array.Find<MenuItemLauncher>(newMod.Launcher.MenuItemLauncher, delegate(MenuItemLauncher mil)
                            {
                              return mil.ID.Equals(mnuItmLaunch.ID, StringComparison.OrdinalIgnoreCase);
                            });

                            if (mnuItmLaunchX == null)
                            {
                              listMnuItmLaunch.AddRange(newMod.Launcher.MenuItemLauncher);

                              listMnuItmLaunch.Add(new MenuItemLauncher()
                              {
                                ID = mnuItmLaunch.ID,
                                Icon = mnuItmLaunch.Icon,
                                CustomIcon = mnuItmLaunch.CustomIcon,
                                Text = mnuItmLaunch.Text,
                                Items = new MenuItem[0]
                              });

                              newMod.Launcher.MenuItemLauncher = listMnuItmLaunch.ToArray();

                              listMnuItmLaunch.Clear();

                              mnuItmLaunchX = newMod.Launcher.MenuItemLauncher[newMod.Launcher.MenuItemLauncher.Length - 1];
                            }

                            if (mnuItmLaunchX != null)
                            {
                              listItemStored.AddRange(mnuItmLaunchX.Items);

                              for (nLoopC = 0, nLenC = listItem.Count; nLoopC < nLenC; nLoopC++)
                              {
                                itm = listItem[nLoopC];

                                if (!listItemStored.Exists(delegate(MenuItem menuItem)
                                {
                                  if (menuItem.Separator)
                                  {
                                    if (menuItem.OrderID == itm.OrderID)
                                    {
                                      return true;
                                    }
                                  }
                                  else
                                  {
                                    if (menuItem.ID.Equals(itm.ID, StringComparison.OrdinalIgnoreCase)
                                      && (menuItem.OrderID == itm.OrderID))
                                    {
                                      return true;
                                    }
                                  }

                                  return false;
                                }))
                                {
                                  listItemStored.Add(itm);
                                }

                                #region Old Coded

                                //listFinder = listItemStored.FindAll(delegate(MenuItem menuItem)
                                //{
                                //  if (menuItem.Separator)
                                //  {
                                //    if (menuItem.OrderID == itm.OrderID)
                                //    {
                                //      return true;
                                //    }
                                //  }
                                //  else
                                //  {
                                //    if (menuItem.ID.Equals(itm.ID, StringComparison.OrdinalIgnoreCase)
                                //      && (menuItem.OrderID == itm.OrderID))
                                //    {
                                //      return true;
                                //    }
                                //  }

                                //  return false;
                                //});

                                //if (itm.Separator)
                                //{
                                //  if ((listFinder == null) || (listFinder.Count < 1))
                                //  {
                                //    listItemStored.Add(itm);
                                //  }
                                //}
                                //else
                                //{
                                //  if ((listFinder == null) || (listFinder.Count < 1))
                                //  {
                                //    listItemStored.Add(itm);
                                //  }
                                //}

                                //listFinder.Clear();

                                #endregion
                              }

                              #region Logic Sort

                              listItemStored.Sort(delegate(MenuItem x, MenuItem y)
                              {
                                if (x.OrderID < y.OrderID)
                                {
                                  return -1;
                                }
                                else if (x.OrderID > y.OrderID)
                                {
                                  return 1;
                                }

                                return 0;
                              });

                              #endregion

                              mnuItmLaunchX.Items = listItemStored.ToArray();

                              if ((mod.Launcher.MenuItemLauncher.Length > 0) && (!mod.Launcher.Nested))
                              {
                                newMod.Launcher.NestedDirected =
                                  mod.Launcher.NestedDirected = true;
                              }
                            }

                            #endregion

                            listItemStored.Clear();
                          }
                        }

                        #endregion
                      }

                      #region Old Coded

                      //if (newMod == null)
                      //{
                      //  #region Clone

                      //  newMod = new MenuModule()
                      //  {
                      //    AutoRun = mod.AutoRun,
                      //    ID = mod.ID,
                      //    Launcher = (mod.Launcher != null ? new MenuLauncer()
                      //    {
                      //      Direct = mod.Launcher.Direct,
                      //      Icon = mod.Launcher.Icon,
                      //      Items = listItem.ToArray(),
                      //      Nested = mod.Launcher.Nested,
                      //      NestedIcon = mod.Launcher.NestedIcon,
                      //      NestedText = mod.Launcher.NestedText,
                      //      SingleItem = (mod.Launcher.SingleItem != null ?
                      //        new MenuItem()
                      //        {
                      //          Center = mod.Launcher.SingleItem.Center,
                      //          Height = mod.Launcher.SingleItem.Height,
                      //          Icon = mod.Launcher.SingleItem.Icon,
                      //          ID = mod.Launcher.SingleItem.ID,
                      //          OrderID = mod.Launcher.SingleItem.OrderID,
                      //          Query = mod.Launcher.SingleItem.Query,
                      //          Separator = mod.Launcher.SingleItem.Separator,
                      //          ShortUrl = mod.Launcher.SingleItem.ShortUrl,
                      //          Title = mod.Launcher.SingleItem.Title,
                      //          Url = mod.Launcher.SingleItem.Url,
                      //          Width = mod.Launcher.SingleItem.Width,
                      //          WinID = mod.Launcher.SingleItem.WinID
                      //        } : null),
                      //      Text = mod.Launcher.Text
                      //    } : null)
                      //  };

                      //  #endregion

                      //  newMod.Launcher.Items = listItem.ToArray();

                      //  listModule.Add(newMod);
                      //}
                      //else
                      //{
                      //  if (listItem.Count > 0)
                      //  {
                      //    #region Merge

                      //    listItemStored.AddRange(newMod.Launcher.Items);

                      //    for (nLoopC = 0, nLenC = listItem.Count; nLoopC < nLenC; nLoopC++)
                      //    {
                      //      itm = listItem[nLoopC];

                      //      listFinder = listItemStored.FindAll(delegate(MenuItem menuItem)
                      //      {
                      //        if (menuItem.Separator)
                      //        {
                      //          if (menuItem.OrderID == itm.OrderID)
                      //          {
                      //            return true;
                      //          }
                      //        }
                      //        else
                      //        {
                      //          if (menuItem.ID.Equals(itm.ID, StringComparison.OrdinalIgnoreCase)
                      //            && (menuItem.OrderID == itm.OrderID))
                      //          {
                      //            return true;
                      //          }
                      //        }

                      //        return false;
                      //      });

                      //      if (itm.Separator)
                      //      {
                      //        if ((listFinder == null) || (listFinder.Count < 1))
                      //        {
                      //          listItemStored.Add(itm);
                      //        }
                      //      }
                      //      else
                      //      {
                      //        if ((listFinder == null) || (listFinder.Count < 1))
                      //        {
                      //          listItemStored.Add(itm);
                      //        }
                      //      }

                      //      listFinder.Clear();
                      //    }

                      //    #endregion

                      //    listItemStored.Sort(delegate(MenuItem x, MenuItem y)
                      //    {
                      //      if (x.OrderID < y.OrderID)
                      //      {
                      //        return -1;
                      //      }
                      //      else if (x.OrderID > y.OrderID)
                      //      {
                      //        return 1;
                      //      }

                      //      return 0;
                      //    });

                      //    newMod.Launcher.Items = listItemStored.ToArray();

                      //    listItemStored.Clear();
                      //  }
                      //}

                      #endregion

                      listItem.Clear();
                    }
                  }
                }
              }

              #endregion

              appli.Modules = listModule.ToArray();

              #region Old Coded

              //appli = listApps.Find(delegate(MenuApplication app)
              //{
              //  return app.ID.Equals(mnuApp.ID, StringComparison.OrdinalIgnoreCase);
              //});

              //if (appli == null)
              //{
              //  listApps.Add(new MenuApplication()
              //  {
              //    ID = mnuApp.ID,
              //    Icon = mnuApp.Icon,
              //    Modules = listModule.ToArray()
              //  });
              //}
              //else
              //{
              //  #region Merger Module

              //  for (nLoop = 0, nLen = listModule.Count; nLoop < nLen; nLoop++)
              //  {
              //    mod = listModule[nLoop];

              //    newMod = Array.Find<MenuModule>(appli.Modules, delegate(MenuModule m)
              //    {
              //      return mod.ID.Equals(m.ID, StringComparison.OrdinalIgnoreCase);
              //    });

              //    if (newMod != null)
              //    {
                    
              //    }
              //  }

              //  #endregion
              //}

              //listModule.Clear();

              #endregion
            }
          }
          
          if (listFinder != null)
          {
            listFinder.Clear();
          }
          else
          {
            listFinder = new List<MenuItem>();
          }

          for (yLoop = 0, yLen = listApps.Count; yLoop < yLen; yLoop++)
          {
            mnuApp = listApps[yLoop];

            if ((mnuApp.Modules != null) && (mnuApp.Modules.Length > 0))
            {
              listModuleFilter.Clear();

              for (nLoopC = 0, nLenC = mnuApp.Modules.Length; nLoopC < nLenC; nLoopC++)
              {
                tLen = 0;
                mod = mnuApp.Modules[nLoopC];

                if (mod.Launcher != null)
                {
                  if (((mod.Launcher.MenuItemLauncher != null) && (mod.Launcher.MenuItemLauncher.Length > 0)) && (mod.Launcher.Nested))
                  {
                    listMnuItmLaunch.Clear();

                    listMnuItmLaunch.AddRange(mod.Launcher.MenuItemLauncher);

                    for (nLoopX = 0, nLen = 0, nLenX = mod.Launcher.MenuItemLauncher.Length; nLoopX < nLenX; nLoopX++)
                    {
                      mnuItmLaunch = mod.Launcher.MenuItemLauncher[nLoopX];

                      if ((mnuItmLaunch.Items != null) && (mnuItmLaunch.Items.Length > 0))
                      {
                        //listModuleFilter.Add(mod);

                        listItem.AddRange(mnuItmLaunch.Items);

                        for (nLoop = 0, nLen = listItem.Count; nLoop < nLen; nLoop++)
                        {
                          itm = listItem[nLoop];

                          if (itm.Separator && ((nLoop == 0) || ((nLoop + 1) == nLen) || isLastSeparator))
                          {
                            listFinder.Add(itm);

                            isLastSeparator = true;
                          }
                          else
                          {
                            isLastSeparator = false;
                          }
                        }

                        for (nLoop = 0, nLen = listFinder.Count; nLoop < nLen; nLoop++)
                        {
                          itm = listFinder[nLoop];

                          if (listItem.Contains(itm))
                          {
                            listItem.Remove(itm);
                          }
                        }

                        if (listItem.Count > 0)
                        {
                          mnuItmLaunch.Items = listItem.ToArray();

                          tLen++;
                        }
                        else
                        {
                          listMnuItmLaunch.Remove(mnuItmLaunch);
                        }

                        listItem.Clear();
                      }
                      else
                      {
                        listMnuItmLaunch.Remove(mnuItmLaunch);
                      }
                    }

                    //mod.Launcher.MenuItemLauncher = listMnuItmLaunch.ToArray();
                  }
                  else if (((mod.Launcher.MenuItemLauncher != null) && (mod.Launcher.MenuItemLauncher.Length > 0)) && (mod.Launcher.NestedDirected))
                  {
                    listMnuItmLaunch.Clear();

                    listMnuItmLaunch.AddRange(mod.Launcher.MenuItemLauncher);

                    for (nLoopX = 0, nLen = 0, nLenX = mod.Launcher.MenuItemLauncher.Length; nLoopX < nLenX; nLoopX++)
                    {
                      mnuItmLaunch = mod.Launcher.MenuItemLauncher[nLoopX];

                      if ((mnuItmLaunch.Items != null) && (mnuItmLaunch.Items.Length > 0))
                      {
                        //listModuleFilter.Add(mod);

                        listItem.AddRange(mnuItmLaunch.Items);

                        for (nLoop = 0, nLen = listItem.Count; nLoop < nLen; nLoop++)
                        {
                          itm = listItem[nLoop];

                          if (itm.Separator && ((nLoop == 0) || ((nLoop + 1) == nLen) || isLastSeparator))
                          {
                            listFinder.Add(itm);

                            isLastSeparator = true;
                          }
                          else
                          {
                            isLastSeparator = false;
                          }
                        }

                        for (nLoop = 0, nLen = listFinder.Count; nLoop < nLen; nLoop++)
                        {
                          itm = listFinder[nLoop];

                          if (listItem.Contains(itm))
                          {
                            listItem.Remove(itm);
                          }
                        }

                        if (listItem.Count > 0)
                        {
                          mnuItmLaunch.Items = listItem.ToArray();

                          tLen++;
                        }
                        else
                        {
                          listMnuItmLaunch.Remove(mnuItmLaunch);
                        }

                        listItem.Clear();
                      }
                      else
                      {
                        listMnuItmLaunch.Remove(mnuItmLaunch);
                      }
                    }

                    //mod.Launcher.MenuItemLauncher = listMnuItmLaunch.ToArray();
                  }
                  else if (mod.Launcher.Direct && (mod.Launcher.SingleItem != null))
                  {
                    tLen++;
                  }

                  mod.Launcher.MenuItemLauncher = listMnuItmLaunch.ToArray();

                  if (tLen > 0)
                  {
                    listModuleFilter.Add(mod);
                  }
                }
              }

              if (listModuleFilter.Count > 0)
              {
                #region Sort Module

                listModuleFilter.Sort(delegate(MenuModule x, MenuModule y)
                {
                  if (x.OrderID < y.OrderID)
                    return -1;
                  else if (x.OrderID > y.OrderID)
                    return 1;

                  return 0;
                });

                #endregion

                mnuApp.Modules = listModuleFilter.ToArray();

                listModuleFilter.Clear();

                listAppFilter.Add(mnuApp);
              }
            }
          }

          #region Sorting Application

          listAppFilter.Sort(delegate(MenuApplication x, MenuApplication y)
          {
            if (x.OrderID < y.OrderID)
              return -1;
            else if (x.OrderID > y.OrderID)
              return 1;

            return 0;
          });

          #endregion

          //menuConfig.Modules = listModuleFilter.ToArray();
          menuConfig.Applications = listAppFilter.ToArray();
          
          //listModule.Clear();
          listAppFilter.Clear();

          #region Old Coded

          //#region Remove invalid position Separator

          //if (listFinder != null)
          //{
          //  listFinder.Clear();
          //}
          //else
          //{
          //  listFinder = new List<MenuItem>();
          //}

          //if (menuConfig.Modules != null)
          //{
          //  listMnuModul.AddRange(menuConfig.Modules);

          //  for (nLoop = 0, nLen = listMnuModul.Count; nLoop < nLen; nLoop++)
          //  {
          //    mnuModul = menuConfig.Modules[nLoop];

          //    if ((mnuModul.Launcher != null) && (mnuModul.Launcher.MenuItemLauncher != null) && (menuConfig.Modules[nLoop].Launcher.MenuItemLauncher.Length > 0))
          //    {
          //      listMnuItmLaunch.AddRange(mnuModul.Launcher.MenuItemLauncher);

          //      for (nLoopX = 0, nLenX = listMnuItmLaunch.Count; nLoopX < nLenX; nLoopX++)
          //      {
          //        mnuItmLaunch = mnuModul.Launcher.MenuItemLauncher[nLoopX];

          //        listItem.AddRange(mnuItmLaunch.Items);

          //        for (nLoopC = 0, nLenC = listItem.Count; nLoopC < nLenC; nLoopC++)
          //        {
          //          itm = listItem[nLoopC];
          //          if (itm.Separator && ((nLoopC == 0) || ((nLoopC + 1) == nLenC) || isLastSeparator))
          //          {
          //            listFinder.Add(itm);

          //            isLastSeparator = true;
          //          }
          //          else
          //          {
          //            isLastSeparator = false;
          //          }
          //        }

          //        for (nLoopC = 0, nLenC = listFinder.Count; nLoopC < nLenC; nLoopC++)
          //        {
          //          itm = listFinder[nLoopC];

          //          if (listItem.Contains(itm))
          //          {
          //            listItem.Remove(itm);
          //          }
          //        }

          //        if (listItem.Count > 0)
          //        {
          //          mnuItmLaunch.Items = listItem.ToArray();
          //        }
          //        else
          //        {
          //          listMnuItmLaunch.Remove(mnuItmLaunch);
          //        }

          //        listItem.Clear();
          //      }

          //      if ((mnuModul.Launcher == null) || (listMnuItmLaunch.Count < 1))
          //      {
          //        listMnuModul.Remove(mnuModul);
          //      }
          //      else
          //      {
          //        mnuModul.Launcher.MenuItemLauncher = listMnuItmLaunch.ToArray();
          //      }

          //      listMnuItmLaunch.Clear();
          //    }
          //  }

          //  menuConfig.Modules = listMnuModul.ToArray();

          //  listMnuModul.Clear();
          //}

          //#region Old Coded

          ////nLoopC = 0;
          ////nLenC = listItemStored.Count;
          ////listFinder.Clear();

          ////while (nLoopC < nLenC)
          ////{
          ////  itm = listItemStored[nLoopC];

          ////  if (itm.Separator && (nLoopC == 0))
          ////  {
          ////    listFinder.Add(itm);
          ////  }
          ////  else if (((nLoopC + 1) >= nLenC) && itm.Separator)
          ////  {
          ////    listFinder.Add(itm);
          ////  }
          ////  else if (isLastSeparator && itm.Separator)
          ////  {
          ////    listFinder.Add(itm);
          ////  }
          ////  else
          ////  {
          ////    isLastSeparator = false;
          ////  }


          ////  nLoopC++;
          ////}

          ////while (listFinder.Count > 0)
          ////{
          ////  itm = listFinder[0];

          ////  listItemStored.Remove(itm);

          ////  listFinder.RemoveAt(0);
          ////}

          //#endregion

          //#endregion

          #endregion
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.MenuBuilder:FilterMenuConfiguration - {0}", ex.Message);
      }

      GC.Collect();

      return menuConfig;
    }
  }

  #endregion

  #region Right Builder

  public class RightBuilder
  {
    #region Inner & Internal

    public class RightApplication
    {
      public RightApplication(string name, string id)
      {
        this.Name = name;
        this.ID = id;
      }

      public string Name
      { get; private set; }

      public string ID
      { get; private set; }

      public IDictionary<string, Module> Modules
      { get; internal set; }
    }

    public class Module
    {
      public Module(string name, string id)
      {
        this.Name = name;
        this.ID = id;
      }

      public string Name
      { get; internal set; }

      public string ID
      { get; internal set; }

      public IDictionary<string, Item> Items
      { get; internal set; }
    }

    public class Item
    {
      public Item(string name, string id, string node)
      {
        this.Name = name;
        this.ID = id;
        this.Node = node;
      }

      public string Name
      { get; internal set; }

      public string ID
      { get; internal set; }

      public string Node
      { get; private set; }
    }

    public class Page
    {
      private int _access;

      public Page(string name, int access, string uri)
      {
        this.Name = name;
        this._access = access;

        this.Url = uri;

        this.IsView = ((access & 1) == 1);
        this.IsPrint = ((access & 2) == 2);
        this.IsAdd = ((access & 4) == 4);
        this.IsEdit = ((access & 8) == 8);
        this.IsDelete = ((access & 16) == 16);

        this.HasAccess = (_access > 0);
      }

      public string Url
      { get; private set; }

      public string Name
      { get; private set; }

      public bool IsView
      { get; private set; }

      public bool IsPrint
      { get; private set; }

      public bool IsAdd
      { get; private set; }

      public bool IsEdit
      { get; private set; }

      public bool IsDelete
      { get; private set; }

      public bool HasAccess
      { get; private set; }
    }

    #region Serialize

    [Serializable]
    [XmlRoot]
    public class GroupAccess
    {
      public RightAccess[] RightAccess
      { get; set; }

      public static GroupAccess Serialize(string rawData)
      {
        if (string.IsNullOrEmpty(rawData))
        {
          return null;
        }

        GroupAccess ga = null;

        MemoryStream memory = new MemoryStream();
        //SoapFormatter formatter = new SoapFormatter();
        XmlSerializer formatter = new XmlSerializer(typeof(GroupAccess));

        try
        {
          memory.Write(Encoding.UTF8.GetBytes(rawData), 0, rawData.Length);

          memory.Position = 0;

          ga = formatter.Deserialize(memory) as GroupAccess;
        }
        catch (Exception ex)
        {
          Logger.WriteLine("Scms.Web.Core.RightBuilder.RightAccess:Serialize - {0}", ex.Message);
        }
        finally
        {
          if (memory != null)
          {
            memory.Close();
            memory.Dispose();
          }
        }

        return ga;
      }

      public static string Deserialize(GroupAccess rightAccess)
      {
        if (rightAccess == null)
        {
          return null;
        }

        MemoryStream memory = new MemoryStream();
        //SoapFormatter formatter = new SoapFormatter();
        XmlSerializer formatter = new XmlSerializer(typeof(GroupAccess));
        string result = null;

        try
        {
          formatter.Serialize(memory, rightAccess);

          result = Encoding.UTF8.GetString(memory.ToArray());
        }
        catch (Exception ex)
        {
          Logger.WriteLine("Scms.Web.Core.RightBuilder.RightAccess:Serialize - {0}", ex.Message);
        }
        finally
        {
          if (memory != null)
          {
            memory.Close();
            memory.Dispose();
          }
        }

        return result;
      }

      public PageRightAccess FindPage(string url, string query)
      {
        PageRightAccess pra = null;
        RightAccess ra = null;
        string urlQuery = (string.IsNullOrEmpty(query) ? url : string.Concat(url, "?", query));
        int nLoopC = 0,
          nLenC = 0;

        for (int nLoop = 0, nLen = this.RightAccess.Length; nLoop < nLen; nLoop++)
        {
          ra = this.RightAccess[nLoop];
          if ((ra != null) && (ra.Pages != null) && (ra.Pages.Length > 0))
          {
            for (nLoopC = 0, nLenC = ra.Pages.Length; nLoopC < nLenC; nLoopC++, pra = null)
            {
              pra = ra.Pages[nLoopC];
              if (pra.Url.Equals(urlQuery, StringComparison.OrdinalIgnoreCase))
              {
                break;
              }
            }
          }

          if (pra != null)
          {
            break;
          }
        }

        return pra;
      }
    }

    [Serializable]
    public class RightAccess
    {
      public string NodeName
      { get; set; }

      public string NodeID
      { get; set; }

      public PageRightAccess[] Pages
      { get; set; }
    }

    [Serializable]
    public class PageRightAccess
    {
      public string Url
      { get; set; }

      public string Name
      { get; set; }

      public bool IsView
      { get; set; }

      public bool IsPrint
      { get; set; }

      public bool IsAdd
      { get; set; }

      public bool IsEdit
      { get; set; }

      public bool IsDelete
      { get; set; }
    }

    #endregion

    #endregion

    private readonly string _rightConfig;

    private List<RightApplication> listRA;

    public RightBuilder()
    {
      _rightConfig = StaticObjects.RightConfiguration;
    }

    public void Populate()
    {
      XmlDocument doc = new XmlDocument();
      XmlNode node = null;

      if (listRA != null)
      {
        listRA.Clear();
      }

      listRA = new List<RightApplication>();

      try
      {
        doc.LoadXml(_rightConfig);

        node = doc.SelectSingleNode("Scms/Application");

        PopulateApplication(node);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.RightBuilder:Populate - {0}", ex.Message);
      }

      GC.Collect();
    }

    public RightApplication[] Rights
    {
      get
      {
        if ((listRA == null) || (listRA.Count < 1))
        {
          return new RightApplication[0];
        }

        return listRA.ToArray();
      }
    }

    public Page[] PageAccess(string nodeName)
    {
      if (string.IsNullOrEmpty(nodeName))
      {
        return new Page[0];
      }

      XmlDocument doc = new XmlDocument();
      XmlNode node = null;

      List<string> lstNames = new List<string>();
      List<Page> lstPages = new List<Page>();

      string tmp = null;
      string url = null;
      int num = 0;

      try
      {
        doc.LoadXml(_rightConfig);

        node = doc.SelectSingleNode(string.Concat("Scms/Items/", nodeName));

        if ((node != null) && node.HasChildNodes)
        {
          node = node.FirstChild;

          while (node != null)
          {
            if ((node.NodeType == XmlNodeType.Element) && node.Name.Equals("Page", StringComparison.OrdinalIgnoreCase))
            {
              tmp = ReaderXml.ReadAttribute(node, "name");

              if (!lstNames.Contains(tmp))
              {
                lstNames.Add(tmp);

                num = ReaderXml.ReadAttributeInt(node, "access");
                url = ReaderXml.ReadAttribute(node, "url");

                lstPages.Add(new Page(tmp, num, url));
              }
            }

            node = node.NextSibling;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.RightBuilder:Populate - {0}", ex.Message);
      }

      lstNames.Clear();

      return lstPages.ToArray();
    }

    public GroupAccess RebuildGroupAccess(string vAkses, string nodeId, string node, List<Dictionary<string, object>> list)
    {
      if (string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(node) || (list == null) || (list.Count < 1))
      {
        return null;
      }

      RightApplication[] rightApp = listRA.ToArray();
      GroupAccess ga = null;

      RightAccess ra = null;

      List<RightAccess> listRightAccess = new List<RightAccess>();

      if (string.IsNullOrEmpty(vAkses))
      {
        ra = new RightAccess();

        if (MergeRightAccess(rightApp, ra, nodeId, node, list))
        {
          listRightAccess.Add(ra);

          ga = new GroupAccess();

          ga.RightAccess = listRightAccess.ToArray();
        }
      }
      else
      {
        ga = GroupAccess.Serialize(vAkses);
        if (ga != null)
        {
          string[] arr = nodeId.Split(new char[] { '_' });
          string tmpNodeID = null;

          if ((arr != null) && (arr.Length > 0))
          {
            tmpNodeID = arr[arr.Length - 1];
          }
          else
          {
            tmpNodeID = nodeId;
          }

          listRightAccess.AddRange(ga.RightAccess);

          ra = RebuildRightAccess(ga, tmpNodeID, node);
          if (ra == null)
          {
            ra = new RightAccess();
          }
          else
          {
            listRightAccess.Remove(ra);
          }

          if (MergeRightAccess(rightApp, ra, nodeId, node, list))
          {
            listRightAccess.Add(ra);
            
            ga.RightAccess = listRightAccess.ToArray();
          }
        }
      }

      listRightAccess.Clear();

      return ga;
    }

    public RightAccess RebuildRightAccess(string vAkses, string nodeId, string subNode)
    {
      if (string.IsNullOrEmpty(vAkses) || string.IsNullOrEmpty(subNode))
      {
        return null;
      }

      GroupAccess ga = GroupAccess.Serialize(vAkses);

      return RebuildRightAccess(ga, nodeId, subNode);
    }

    public RightAccess RebuildRightAccess(GroupAccess ga, string nodeId, string subNode)
    {
      if (ga == null || string.IsNullOrEmpty(subNode))
      {
        return null;
      }

      RightAccess ra = null;

      if (ga != null)
      {
        for (int nLoop = 0; nLoop < ga.RightAccess.Length; nLoop++, ra = null)
        {
          ra = ga.RightAccess[nLoop];
          if (ra != null)
          {
            if (ra.NodeID.Equals(subNode, StringComparison.OrdinalIgnoreCase) && ra.NodeName.Equals(nodeId, StringComparison.OrdinalIgnoreCase))
            {
              break;
            }
          }
        }
      }

      return ra;
    }

    #region Private

    private bool MergeRightAccess(RightApplication[] rightApp, RightAccess ra, string nodeId, string subNode, List<Dictionary<string, object>> list)
    {
      if ((rightApp == null) || (ra == null) || string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(subNode))
      {
        return false;
      }

      if (string.IsNullOrEmpty(ra.NodeName))
      {
        string[] arr = nodeId.Split(new char[] { '_' });

        if ((arr != null) && (arr.Length > 0))
        {
          ra.NodeName = arr[arr.Length - 1];
        }
        else
        {
          ra.NodeName = nodeId;
        }
      }
      if (string.IsNullOrEmpty(ra.NodeID))
      {
        ra.NodeID = subNode;
      }

      RightBuilder.Item item = RetriveItem(rightApp, nodeId, subNode);
      RightBuilder.Page page = null;
      List<PageRightAccess> listPRA = new List<PageRightAccess>();

      if (item != null)
      {
        string pageName = null;
        Dictionary<string, object> dic = null;

        for (int nLoop = 0, nLen = list.Count; nLoop < nLen; nLoop++)
        {
          dic = list[nLoop];
          if ((dic != null) && (dic.Count > 0))
          {
            pageName = (dic.ContainsKey("Nama") ? (string)dic["Nama"] : null);
            if (!string.IsNullOrEmpty(pageName))
            {
              page = RetrivePage(item, subNode, pageName);

              if ((page != null) && page.HasAccess)
              {
                listPRA.Add(new PageRightAccess()
                {
                  IsAdd = (page.IsAdd ? dic.GetValueParser<bool>("isAdd") : false),
                  IsDelete = (page.IsDelete ? dic.GetValueParser<bool>("isDelete") : false),
                  IsEdit = (page.IsEdit ? dic.GetValueParser<bool>("isEdit") : false),
                  IsPrint = (page.IsPrint ? dic.GetValueParser<bool>("isPrint") : false),
                  IsView = (page.IsView ? dic.GetValueParser<bool>("isView") : false),
                  Name = page.Name,
                  Url = page.Url
                });
              }
            }
          }
        }
      }

      ra.Pages = listPRA.ToArray();

      list.Clear();

      return true;
    }

    private RightBuilder.Item RetriveItem(RightApplication[] rightApp, string nodeId, string subNode)
    {
      if ((rightApp == null) || string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(subNode))
      {
        return null;
      }

      string[] arr = nodeId.Split(new char[] { '_' });

      if (arr.Length < 1)
      {
        return null;
      }

      RightApplication ra = null;
      RightBuilder.Item item = null;

      for (int nLoop = 0, nLen = rightApp.Length; nLoop < nLen; nLoop++)
      {
        ra = rightApp[nLoop];
        if (ra.ID.Equals(arr[0]))
        {
          if ((arr.Length > 1) && (ra.Modules != null) && (ra.Modules.Count > 0))
          {
            foreach (KeyValuePair<string, Module> kvp in ra.Modules)
            {
              if (kvp.Value.ID.Equals(arr[1]))
              {
                if ((arr.Length > 2) && (kvp.Value.Items != null) && (kvp.Value.Items.Count > 0))
                {
                  foreach (KeyValuePair<string, Item> kvpC in kvp.Value.Items)
                  {
                    if (kvpC.Value.ID.Equals(arr[2]) && kvpC.Value.Node.Equals(subNode))
                    {
                      item = kvpC.Value;
                    }

                    if (item != null)
                    {
                      break;
                    }
                  }
                }
              }

              if (item != null)
              {
                break;
              }
            }
          }
        }

        if (item != null)
        {
          break;
        }
      }

      return item;
    }

    private RightBuilder.Page RetrivePage(RightBuilder.Item item, string subNode, string pageName)
    {
      if ((item == null) || string.IsNullOrEmpty(pageName))
      {
        return null;
      }

      RightBuilder.Page[] pages = null;
      RightBuilder.Page page = null;

      pages = this.PageAccess(subNode);

      if (pages != null)
      {
        for (int nLoop = 0, nLen = pages.Length; nLoop < nLen; nLoop++)
        {
          page = pages[nLoop];

          if (page.Name.Equals(pageName))
          {
            break;
          }
        }
      }

      return page;
    }

    private void PopulateApplication(XmlNode node)
    {
      RightApplication ra = null;
      string tmp = null;

      while (node != null)
      {
        if ((node.NodeType == XmlNodeType.Element) && node.Name.Equals("Application", StringComparison.OrdinalIgnoreCase))
        {
          tmp = ReaderXml.ReadAttribute(node, "name");

          ra = new RightApplication(tmp,
            ReaderXml.ReadAttribute(node, "id").Replace('_', '-'));

          listRA.Add(ra);

          if (node.HasChildNodes)
          {
            ra.Modules = PopulateModule(node.FirstChild);
          }
        }

        node = node.NextSibling;
      }
    }

    private IDictionary<string, Module> PopulateModule(XmlNode node)
    {
      IDictionary<string, Module> list = new Dictionary<string, Module>(StringComparer.OrdinalIgnoreCase);
      Module mod = null;
      string tmp = null;

      while (node != null)
      {
        if ((node.NodeType == XmlNodeType.Element) && node.Name.Equals("Module", StringComparison.OrdinalIgnoreCase))
        {
          tmp = ReaderXml.ReadAttribute(node, "name");

          if (!list.ContainsKey(tmp))
          {
            mod = new Module(tmp,
              ReaderXml.ReadAttribute(node, "id").Replace('_', '-'));

            list.Add(tmp, mod);

            if (node.HasChildNodes)
            {
              mod.Items = PopulateItem(node.FirstChild);
            }
          }
        }

        node = node.NextSibling;
      }

      return list;
    }

    private IDictionary<string, Item> PopulateItem(XmlNode node)
    {
      IDictionary<string, Item> list = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);
      Item itm = null;
      string tmp = null;

      while (node != null)
      {
        if ((node.NodeType == XmlNodeType.Element) && node.Name.Equals("Item", StringComparison.OrdinalIgnoreCase))
        {
          tmp = ReaderXml.ReadAttribute(node, "name");

          if (!list.ContainsKey(tmp))
          {
            itm = new Item(tmp,
              ReaderXml.ReadAttribute(node, "id").Replace('_', '-'),
              ReaderXml.ReadAttribute(node, "node"));

            list.Add(tmp, itm);
          }
        }

        node = node.NextSibling;
      }

      return list;
    }

    //private IDictionary<string, Page> PopulatePage(XmlNode node)
    //{
    //  IDictionary<string, Page> list = new Dictionary<string, Page>(StringComparer.OrdinalIgnoreCase);
    //  Page page = null;
    //  string tmp = null;
    //  int num = 0;

    //  while (node != null)
    //  {
    //    if ((node.NodeType == XmlNodeType.Element) && node.Name.Equals("Page", StringComparison.OrdinalIgnoreCase))
    //    {
    //      tmp = ReaderXml.ReadAttribute(node, "name");

    //      if (!list.ContainsKey(tmp))
    //      {
    //        num = ReaderXml.ReadAttributeInt(node, "access");
    //        page = new Page(tmp, num);

    //        list.Add(tmp, page);
    //      }
    //    }

    //    node = node.NextSibling;
    //  }

    //  return list;
    //}

    #endregion
  }

  #endregion

  #region Internal

  internal class ReaderXml
  {
    public static string ReadAttribute(XmlNode node, string name)
    {
      string result = string.Empty;

      try
      {
        result = (node.Attributes[name] == null ? string.Empty : node.Attributes[name].Value);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.ReaderXml:ReadAttribute - {0}", ex.Message);

        result = string.Empty;
      }

      return result;
    }

    public static bool ReadAttributeBool(XmlNode node, string name)
    {
      return ReadAttributeBool(node, name, false);
    }

    public static bool ReadAttributeBool(XmlNode node, string name, bool defaultValue)
    {
      bool result;

      try
      {
        bool.TryParse((node.Attributes[name] == null ? (defaultValue ? "true" : "false") : node.Attributes[name].Value), out result);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.ReaderXml:ReadAttributeBool - {0}", ex.Message);

        result = false;
      }

      return result;
    }

    public static System.Type ReadAttributeType(XmlNode node, string name)
    {
      System.Type result;

      try
      {
        result = System.Type.GetType((node.Attributes[name].Value ?? string.Empty));
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.ReaderXml:ReadAttributeType - {0}", ex.Message);

        return null;
      }

      return result;
    }

    public static int ReadAttributeInt(XmlNode node, string name)
    {
      int result;

      try
      {
        int.TryParse((node.Attributes[name] == null ? string.Empty : node.Attributes[name].Value), out result);
      }
      catch (Exception ex)
      {
        Logger.WriteLine("Scms.Web.Core.ReaderXml:ReadAttributeInt - {0}", ex.Message);

        result = 0;
      }

      return result;
    }
  }

  #endregion
}