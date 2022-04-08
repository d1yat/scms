using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Scms.Web.Core;
using Scms.Web.Common;
using Ext.Net;

public partial class Main : Scms.Web.Core.PageHandler
{
  private bool IsImageUserExists(string imgName)
  {
    if (string.IsNullOrEmpty(imgName))
    {
      return false;
    }

    return System.IO.File.Exists(System.IO.Path.Combine(this.Server.MapPath("~/App_Data/Users/"), imgName));
  }

  protected void Page_Load(object sender, EventArgs e)
  {
    if (!X.IsAjaxRequest)
    {
      UserInformation userInfo = this.Session[Constant.SESSION_LOGIN_INFORMATION] as UserInformation;
      if (userInfo == null)
      {
        this.Response.Redirect("~/", true);
      }
      else
      {
        RebuildMenu(userInfo.NIP, userInfo.GroupAccess);
      }
    }
  }

  private void RebuildMenu(string NipUser, RightBuilder.GroupAccess[] groupAccess)
  {
    //if ((groupAccess == null) || (groupAccess.Length < 1))
    //{
    //  return;
    //}

    MenuBuilder mb = new MenuBuilder();

    MenuBuilder.MenuConfiguration mc = null;
    MenuBuilder.MenuConfiguration mcFiltered = null;

    List<Ext.Net.DesktopModule> lstDesktop = new List<Ext.Net.DesktopModule>();

    IDictionary<string, Ext.Net.DesktopWindow> dicDW = new Dictionary<string, Ext.Net.DesktopWindow>(StringComparer.OrdinalIgnoreCase);
    IDictionary<string, Ext.Net.DesktopShortcut> dicDS = new Dictionary<string, Ext.Net.DesktopShortcut>(StringComparer.OrdinalIgnoreCase);

    //Ext.Net.Desktop desktop = myDesktop;
    Ext.Net.Desktop desktop = new Ext.Net.Desktop();

    MenuBuilder.MenuApplication mbMenuApp = null;
    MenuBuilder.MenuModule mbMenuModule = null;
    MenuBuilder.MenuLauncer mbMenuLaunch = null;
    MenuBuilder.MenuItem mbMenuItem = null;
    Ext.Net.DesktopModule deskModule = null;
    Ext.Net.DesktopModule deskModuleSc = null;
    Ext.Net.Menu mnuMainLauncher = null;
    Ext.Net.MenuItem mnuMainItemLauncher = null;
    Ext.Net.MenuItem mnuLauncher = null;
    Ext.Net.Menu mnuMenu = null;
    Ext.Net.MenuItem menuItem = null;
    Ext.Net.Menu nestedFirstMenu = null;
    Ext.Net.MenuItem nestedFirstMenuItem = null;
    //Ext.Net.Menu mnuMenuSingle = null;
    Ext.Net.DesktopWindow dw = null;
    Ext.Net.DesktopShortcut dscrt = null;
    int nLoopC = 0,
      nLenC = 0,
      nLoopX = 0,
      nLenX = 0,
      nLoop = 0,
      nLen = 0;
    MenuBuilder.MenuItemLauncher itemLaunch = null;
    Ext.Net.DesktopShortcut deskShort = null;

    mc = mb.RebuildMenuConfiguration(StaticObjects.MenuConfiguration);

    mcFiltered = mb.FilterMenuConfigurationArray(mc, groupAccess);

    myDesktop.Modules.Clear();

    if (mcFiltered != null)
    {
      #region Rebuild Menu Desktop

      if ((mcFiltered.Applications != null) && (mcFiltered.Applications.Length > 0))
      {
        for (int yLoop = 0, yLen = mcFiltered.Applications.Length; yLoop < yLen; yLoop++)
        {
          mbMenuApp = mcFiltered.Applications[yLoop];

          deskModule = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
          {
            AutoRun = mbMenuApp.AutoRun,
            ModuleID = mbMenuApp.ID,
            WindowID = mbMenuApp.WindowID
          });

          mnuLauncher = deskModule.Launcher;

          mnuLauncher.Text = mbMenuApp.Text;
          mnuLauncher.Listeners.Click.Handler = "return false;";

          if (string.IsNullOrEmpty(mbMenuApp.CustomIcon))
          {
            mnuLauncher.Icon = (Ext.Net.Icon)mbMenuApp.Icon;
          }
          else
          {
            mnuLauncher.Icon = Ext.Net.Icon.None;
            mnuLauncher.IconCls = (string.IsNullOrEmpty(mbMenuApp.CustomIcon) ? mnuLauncher.IconCls : mbMenuApp.CustomIcon);
          }

          mnuMainLauncher = new Ext.Net.Menu();

          //mnuMainLauncher.Items.Add(mnuMainItemLauncher);

          #region Module

          for (nLoop = 0, nLen = mbMenuApp.Modules.Length; nLoop < nLen; nLoop++)
          {
            mbMenuModule = mbMenuApp.Modules[nLoop];

            //if ((mbMenuModule != null) && (mbMenuModule.Launcher != null) && (mbMenuModule.Launcher.MenuItemLauncher != null) && (mbMenuModule.Launcher.MenuItemLauncher.Length > 0))
            if ((mbMenuModule != null) && (mbMenuModule.Launcher != null) &&
              (((mbMenuModule.Launcher.MenuItemLauncher != null) && (mbMenuModule.Launcher.MenuItemLauncher.Length > 0)) || mbMenuModule.Launcher.Direct))
            {
              if (mbMenuModule.Launcher.Direct)
              {
                mbMenuItem = mbMenuModule.Launcher.SingleItem;

                if (!deskModule.WindowID.Equals(mbMenuItem.ID))
                {
                  mnuMainItemLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                  {
                    Text = mbMenuModule.Launcher.Text
                  });

                  if (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon))
                  {
                    mnuMainItemLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
                  }
                  else
                  {
                    mnuMainItemLauncher.Icon = Ext.Net.Icon.None;
                    mnuMainItemLauncher.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon) ? mnuMainItemLauncher.IconCls : mbMenuModule.Launcher.CustomIcon);
                  }

                  mnuMainItemLauncher.Listeners.Click.Handler = "return false;";

                  //if (!dicDW.ContainsKey(mbMenuItem.ID))
                  if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                  {
                    dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                    {
                      Title = mbMenuItem.Title,
                      Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                      InitCenter = mbMenuItem.Center,
                      Width = Unit.Pixel(mbMenuItem.Width),
                      Height = Unit.Pixel(mbMenuItem.Height),
                      Resizable = mbMenuItem.Resizeable,
                      Maximizable = mbMenuItem.Resizeable,
                      CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                    })
                    {
                      ID = mbMenuItem.ID
                    };

                    dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                    dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                    dw.AutoLoad.ShowMask = true;

                    dicDW.Add(mbMenuItem.ID, dw);

                    #region Added Shortcut

                    if (mbMenuItem.IsShortcut)
                    {
                      deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                      {
                        AutoRun = false,
                        ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                        WindowID = mbMenuItem.ID
                      });

                      if (!lstDesktop.Contains(deskModuleSc))
                      {
                        lstDesktop.Add(deskModuleSc);
                      }

                      if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                      {
                        dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                        {
                          Text = mbMenuItem.Text,
                          ModuleID = deskModuleSc.ModuleID,
                          IconCls = mbMenuItem.ShortcutClass
                        });

                        dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                      }
                    }

                    #endregion
                  }

                  nestedFirstMenu = new Ext.Net.Menu();

                  nestedFirstMenuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                  {
                    Text = mbMenuModule.Launcher.SingleItem.Text
                  });

                  if (string.IsNullOrEmpty(mbMenuModule.Launcher.SingleItem.CustomIcon))
                  {
                    nestedFirstMenuItem.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.SingleItem.Icon;
                  }
                  else
                  {
                    nestedFirstMenuItem.Icon = Ext.Net.Icon.None;
                    nestedFirstMenuItem.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.SingleItem.CustomIcon) ? nestedFirstMenuItem.IconCls : mbMenuModule.Launcher.SingleItem.CustomIcon);
                  }

                  if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                  {
                    nestedFirstMenuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                      string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                  }
                  else
                  {
                    nestedFirstMenuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                  }

                  nestedFirstMenu.Items.Add(nestedFirstMenuItem);

                  //mnuLauncher.Menu.Add(nestedFirstMenu);
                  mnuMainItemLauncher.Menu.Add(nestedFirstMenu);

                  mnuMainLauncher.Items.Add(mnuMainItemLauncher);
                }
              }
              else
              {
                //mnuLauncher.Text = mbMenuModule.Launcher.Text;
                //mnuLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
                //mnuLauncher.Listeners.Click.Handler = "return false;";

                mbMenuLaunch = mbMenuModule.Launcher;

                if (mbMenuLaunch.Nested)
                {
                  #region Nested

                  mnuMainItemLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                  {
                    Text = mbMenuModule.Launcher.Text
                  });

                  if (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon))
                  {
                    mnuMainItemLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
                  }
                  else
                  {
                    mnuMainItemLauncher.Icon = Ext.Net.Icon.None;
                    mnuMainItemLauncher.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon) ? mnuMainItemLauncher.IconCls : mbMenuModule.Launcher.CustomIcon);
                  }

                  mnuMainItemLauncher.Listeners.Click.Handler = "return false;";

                  nestedFirstMenu = new Ext.Net.Menu();

                  for (nLoopC = 0, nLenC = mbMenuLaunch.MenuItemLauncher.Length; nLoopC < nLenC; nLoopC++)
                  {
                    itemLaunch = mbMenuLaunch.MenuItemLauncher[nLoopC];

                    if (itemLaunch.Items != null)
                    {
                      nestedFirstMenuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                      {
                        Text = itemLaunch.Text
                      });

                      if (string.IsNullOrEmpty(itemLaunch.CustomIcon))
                      {
                        nestedFirstMenuItem.Icon = (Ext.Net.Icon)itemLaunch.Icon;
                      }
                      else
                      {
                        nestedFirstMenuItem.Icon = Ext.Net.Icon.None;
                        nestedFirstMenuItem.IconCls = (string.IsNullOrEmpty(itemLaunch.CustomIcon) ? nestedFirstMenuItem.IconCls : itemLaunch.CustomIcon);
                      }

                      nestedFirstMenu.Items.Add(nestedFirstMenuItem);

                      if (itemLaunch.Items.Length > 0)
                      {
                        nestedFirstMenuItem.Listeners.Click.Handler = "return false;";

                        mnuMenu = new Ext.Net.Menu();

                        for (nLoopX = 0, nLenX = itemLaunch.Items.Length; nLoopX < nLenX; nLoopX++)
                        {
                          mbMenuItem = itemLaunch.Items[nLoopX];

                          if (mbMenuItem.OutsideSubMenu)
                          {
                            #region Outside Submenu

                            if (mbMenuItem.Separator)
                            {
                              nestedFirstMenu.Items.Add(new Ext.Net.MenuSeparator());
                            }
                            else
                            {
                              //if (!dicDW.ContainsKey(mbMenuItem.ID))
                              if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                                {
                                  Title = mbMenuItem.Title,
                                  Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                  InitCenter = mbMenuItem.Center,
                                  Width = Unit.Pixel(mbMenuItem.Width),
                                  Height = Unit.Pixel(mbMenuItem.Height),
                                  Resizable = mbMenuItem.Resizeable,
                                  Maximizable = mbMenuItem.Resizeable,
                                  CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                                })
                                {
                                  ID = mbMenuItem.ID
                                };

                                dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                                dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                                dw.AutoLoad.ShowMask = true;

                                dicDW.Add(mbMenuItem.ID, dw);

                                #region Added Shortcut

                                if (mbMenuItem.IsShortcut)
                                {
                                  deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                  {
                                    AutoRun = false,
                                    ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                    WindowID = mbMenuItem.ID
                                  });

                                  if (!lstDesktop.Contains(deskModuleSc))
                                  {
                                    lstDesktop.Add(deskModuleSc);
                                  }

                                  if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                  {
                                    dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                    {
                                      Text = mbMenuItem.Text,
                                      ModuleID = deskModuleSc.ModuleID,
                                      IconCls = mbMenuItem.ShortcutClass
                                    });

                                    dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                  }
                                }

                                #endregion
                              }

                              if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                nestedFirstMenuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                  string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                              }
                              else
                              {
                                nestedFirstMenuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                              }
                            }

                            mnuMenu = null;

                            #endregion
                          }
                          else
                          {
                            #region Inside Submenu

                            if (mbMenuItem.Separator)
                            {
                              mnuMenu.Items.Add(new Ext.Net.MenuSeparator());
                            }
                            else
                            {
                              menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                              {
                                Text = mbMenuItem.Text
                              });

                              if (string.IsNullOrEmpty(mbMenuItem.CustomIcon))
                              {
                                menuItem.Icon = (Ext.Net.Icon)mbMenuItem.Icon;
                              }
                              else
                              {
                                menuItem.Icon = Ext.Net.Icon.None;
                                menuItem.IconCls = (string.IsNullOrEmpty(mbMenuItem.CustomIcon) ? menuItem.IconCls : mbMenuItem.CustomIcon);
                              }

                              //if (!dicDW.ContainsKey(mbMenuItem.ID))
                              if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                                {
                                  Title = mbMenuItem.Title,
                                  Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                  InitCenter = mbMenuItem.Center,
                                  Width = Unit.Pixel(mbMenuItem.Width),
                                  Height = Unit.Pixel(mbMenuItem.Height),
                                  Resizable = mbMenuItem.Resizeable,
                                  Maximizable = mbMenuItem.Resizeable,
                                  CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                                })
                                {
                                  ID = mbMenuItem.ID
                                };

                                dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                                dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                                dw.AutoLoad.ShowMask = true;

                                dicDW.Add(mbMenuItem.ID, dw);

                                #region Added Shortcut

                                if (mbMenuItem.IsShortcut)
                                {
                                  deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                  {
                                    AutoRun = false,
                                    ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                    WindowID = mbMenuItem.ID
                                  });

                                  if (!lstDesktop.Contains(deskModuleSc))
                                  {
                                    lstDesktop.Add(deskModuleSc);
                                  }

                                  if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                  {
                                    dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                    {
                                      Text = mbMenuItem.Text,
                                      ModuleID = deskModuleSc.ModuleID,
                                      IconCls = mbMenuItem.ShortcutClass
                                    });

                                    dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                  }
                                }

                                #endregion
                              }

                              if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                menuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                  string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                              }
                              else
                              {
                                menuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                              }

                              mnuMenu.Items.Add(menuItem);
                            }

                            #endregion
                          }
                        }

                        if (mnuMenu != null)
                        {
                          nestedFirstMenuItem.Menu.Add(mnuMenu);
                        }
                      }
                      else
                      {
                        throw new Exception("Check error.");
                      }
                    }
                  }

                  //if (nestedFirstMenu.Items.Count > 0)
                  //{
                  //  mnuLauncher.Menu.Add(nestedFirstMenu);
                  //}

                  //mnuLauncher.Menu.Add(nestedFirstMenu);
                  mnuMainItemLauncher.Menu.Add(nestedFirstMenu);

                  mnuMainLauncher.Items.Add(mnuMainItemLauncher);

                  #endregion
                }
                else if (mbMenuLaunch.NestedDirected)
                {
                  #region NestedDirected

                  mnuMainItemLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                  {
                    Text = mbMenuModule.Launcher.Text
                  });

                  if (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon))
                  {
                    mnuMainItemLauncher.Icon = (Ext.Net.Icon)mbMenuModule.Launcher.Icon;
                  }
                  else
                  {
                    mnuMainItemLauncher.Icon = Ext.Net.Icon.None;
                    mnuMainItemLauncher.IconCls = (string.IsNullOrEmpty(mbMenuModule.Launcher.CustomIcon) ? mnuMainItemLauncher.IconCls : mbMenuModule.Launcher.CustomIcon);
                  }

                  mnuMainItemLauncher.Listeners.Click.Handler = "return false;";

                  nestedFirstMenu = new Ext.Net.Menu();

                  for (nLoopC = 0, nLenC = mbMenuLaunch.MenuItemLauncher.Length; nLoopC < nLenC; nLoopC++)
                  {
                    itemLaunch = mbMenuLaunch.MenuItemLauncher[nLoopC];

                    if (itemLaunch.Items != null)
                    {
                      nestedFirstMenuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                      {
                        Text = itemLaunch.Text
                      });

                      if (string.IsNullOrEmpty(itemLaunch.CustomIcon))
                      {
                        nestedFirstMenuItem.Icon = (Ext.Net.Icon)itemLaunch.Icon;
                      }
                      else
                      {
                        nestedFirstMenuItem.Icon = Ext.Net.Icon.None;
                        nestedFirstMenuItem.IconCls = (string.IsNullOrEmpty(itemLaunch.CustomIcon) ? nestedFirstMenuItem.IconCls : itemLaunch.CustomIcon);
                      }

                      nestedFirstMenu.Items.Add(nestedFirstMenuItem);

                      if (itemLaunch.Items.Length > 0)
                      {
                        nestedFirstMenuItem.Listeners.Click.Handler = "return false;";

                        mnuMenu = new Ext.Net.Menu();

                        for (nLoopX = 0, nLenX = itemLaunch.Items.Length; nLoopX < nLenX; nLoopX++)
                        {
                          mbMenuItem = itemLaunch.Items[nLoopX];

                          if (mbMenuItem.OutsideSubMenu)
                          {
                            #region Outside Submenu

                            if (mbMenuItem.Separator)
                            {
                              nestedFirstMenu.Items.Add(new Ext.Net.MenuSeparator());
                            }
                            else
                            {
                              if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                                {
                                  Title = mbMenuItem.Title,
                                  Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                  InitCenter = mbMenuItem.Center,
                                  Width = Unit.Pixel(mbMenuItem.Width),
                                  Height = Unit.Pixel(mbMenuItem.Height),
                                  Resizable = mbMenuItem.Resizeable,
                                  Maximizable = mbMenuItem.Resizeable,
                                  CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                                })
                                {
                                  ID = mbMenuItem.ID
                                };

                                dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                                dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                                dw.AutoLoad.ShowMask = true;

                                dicDW.Add(mbMenuItem.ID, dw);

                                #region Added Shortcut

                                if (mbMenuItem.IsShortcut)
                                {
                                  deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                  {
                                    AutoRun = false,
                                    ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                    WindowID = mbMenuItem.ID
                                  });

                                  if (!lstDesktop.Contains(deskModuleSc))
                                  {
                                    lstDesktop.Add(deskModuleSc);
                                  }

                                  if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                  {
                                    dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                    {
                                      Text = mbMenuItem.Text,
                                      ModuleID = deskModuleSc.ModuleID,
                                      IconCls = mbMenuItem.ShortcutClass
                                    });

                                    dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                  }
                                }

                                #endregion
                              }

                              if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                nestedFirstMenuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                  string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                              }
                              else
                              {
                                nestedFirstMenuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                              }
                            }

                            mnuMenu = null;

                            #endregion
                          }
                          else
                          {
                            #region Inside Submenu

                            if (mbMenuItem.Separator)
                            {
                              mnuMenu.Items.Add(new Ext.Net.MenuSeparator());
                            }
                            else
                            {
                              menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
                              {
                                Text = mbMenuItem.Text
                              });

                              if (string.IsNullOrEmpty(mbMenuItem.CustomIcon))
                              {
                                menuItem.Icon = (Ext.Net.Icon)mbMenuItem.Icon;
                              }
                              else
                              {
                                menuItem.Icon = Ext.Net.Icon.None;
                                menuItem.IconCls = (string.IsNullOrEmpty(mbMenuItem.CustomIcon) ? menuItem.IconCls : mbMenuItem.CustomIcon);
                              }

                              if ((!dicDW.ContainsKey(mbMenuItem.ID)) && string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                                {
                                  Title = mbMenuItem.Title,
                                  Icon = (Ext.Net.Icon)mbMenuItem.Icon,
                                  InitCenter = mbMenuItem.Center,
                                  Width = Unit.Pixel(mbMenuItem.Width),
                                  Height = Unit.Pixel(mbMenuItem.Height),
                                  Resizable = mbMenuItem.Resizeable,
                                  Maximizable = mbMenuItem.Resizeable,
                                  CloseAction = (mbMenuItem.FullClosed ? Ext.Net.CloseAction.Close : Ext.Net.CloseAction.Hide),
                                })
                                {
                                  ID = mbMenuItem.ID
                                };

                                dw.AutoLoad.Url = (mbMenuItem.Url.StartsWith("~") ? this.ResolveUrl(mbMenuItem.Url) : mbMenuItem.Url);
                                dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
                                dw.AutoLoad.ShowMask = true;

                                dicDW.Add(mbMenuItem.ID, dw);

                                #region Added Shortcut

                                if (mbMenuItem.IsShortcut)
                                {
                                  deskModuleSc = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
                                  {
                                    AutoRun = false,
                                    ModuleID = string.Concat(mbMenuItem.WinID, mbMenuItem.ID),
                                    WindowID = mbMenuItem.ID
                                  });

                                  if (!lstDesktop.Contains(deskModuleSc))
                                  {
                                    lstDesktop.Add(deskModuleSc);
                                  }

                                  if (!dicDS.ContainsKey(mbMenuItem.DesktopShortcutModID))
                                  {
                                    dscrt = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
                                    {
                                      Text = mbMenuItem.Text,
                                      ModuleID = deskModuleSc.ModuleID,
                                      IconCls = mbMenuItem.ShortcutClass
                                    });

                                    dicDS.Add(mbMenuItem.DesktopShortcutModID, dscrt);
                                  }
                                }

                                #endregion
                              }

                              if (string.IsNullOrEmpty(mbMenuItem.DirectJSClick))
                              {
                                menuItem.Listeners.Click.Handler = string.Format("{0}.show();",
                                  string.Concat(ph.NamingContainer.ClientID, "_", mbMenuItem.ID));
                              }
                              else
                              {
                                menuItem.Listeners.Click.Handler = mbMenuItem.DirectJSClick;
                              }

                              mnuMenu.Items.Add(menuItem);
                            }

                            #endregion
                          }
                        }

                        if (mnuMenu != null)
                        {
                          nestedFirstMenuItem.Menu.Add(mnuMenu);
                        }
                      }
                      else
                      {
                        throw new Exception("Check error.");
                      }
                    }
                  }

                  //if (nestedFirstMenu.Items.Count > 0)
                  //{
                  //  mnuLauncher.Menu.Add(nestedFirstMenu);
                  //}

                  //mnuLauncher.Menu.Add(nestedFirstMenu);
                  mnuMainItemLauncher.Menu.Add(nestedFirstMenu);

                  mnuMainLauncher.Items.Add(mnuMainItemLauncher);

                  #endregion
                }
              }

              lstDesktop.Add(deskModule);
            }
          }

          #endregion

          mnuLauncher.Menu.Add(mnuMainLauncher);
        }

        foreach (KeyValuePair<string, Ext.Net.DesktopShortcut> kvp in dicDS)
        {
          myDesktop.Shortcuts.Add(kvp.Value);
        }

        foreach (KeyValuePair<string, Ext.Net.DesktopWindow> kvp in dicDW)
        {
          ph.Controls.Add(kvp.Value);
        }

        dicDW.Clear();

        myDesktop.Modules.AddRange(lstDesktop.ToArray());

        lstDesktop.Clear();
      }

      #endregion
    }

    #region Admin Area

    if (this.IsGroupAdmin)
    {
      menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
      {
        Icon = Ext.Net.Icon.HtmlValid,
        Text = "Admin",
      });

      mnuMenu = new Ext.Net.Menu(new Ext.Net.Menu.Config());

      #region User

      dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
      {
        Title = "Pengaturan Pemakai",
        Icon = Ext.Net.Icon.UserEarth,
        InitCenter = true,
        Width = 640,
        Height = 480
      })
      {
        ID = "UserManagement",
        Resizable = true,
        Maximizable = true
      };

      dw.AutoLoad.Url = this.ResolveUrl("~/management/Users.aspx");
      dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
      dw.AutoLoad.ShowMask = true;

      ph.Controls.Add(dw);

      mnuLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
      {
        Text = "Pengaturan Pemakai",
        Icon = Ext.Net.Icon.UserEarth,
      });

      mnuLauncher.Listeners.Click.Handler = string.Format("{0}.show();", dw.ClientID);

      mnuMenu.Items.Add(mnuLauncher);

      #endregion

      mnuMenu.Items.Add(new MenuSeparator());

      #region Group

      dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
      {
        Title = "Pengaturan Grup",
        Icon = Ext.Net.Icon.GroupLink,
        InitCenter = true,
        Width = 640,
        Height = 480
      })
      {
        ID = "GroupManagement",
        Resizable = true,
        Maximizable = true
      };

      dw.AutoLoad.Url = this.ResolveUrl("~/management/Groups.aspx");
      dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
      dw.AutoLoad.ShowMask = true;

      ph.Controls.Add(dw);

      mnuLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
      {
        Text = "Pengaturan Grup",
        Icon = Ext.Net.Icon.GroupLink,
      });

      mnuLauncher.Listeners.Click.Handler = string.Format("{0}.show();", dw.ClientID);

      mnuMenu.Items.Add(mnuLauncher);

      #endregion

      mnuMenu.Items.Add(new MenuSeparator());

      #region Group Management

      dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
      {
        Title = "Pengaturan Grup Hak Akses",
        Icon = Ext.Net.Icon.ChartOrganisation,
        InitCenter = true,
        Width = 640,
        Height = 480
      })
      {
        ID = "UserGroupManagement",
        Resizable = true,
        Maximizable = true
      };

      dw.AutoLoad.Url = this.ResolveUrl("~/management/UserGroup.aspx");
      dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
      dw.AutoLoad.ShowMask = true;

      ph.Controls.Add(dw);

      mnuLauncher = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
      {
        Text = "Pengaturan Grup Hak Akses",
        Icon = Ext.Net.Icon.ChartOrganisation,
      });

      mnuLauncher.Listeners.Click.Handler = string.Format("{0}.show();", dw.ClientID);

      mnuMenu.Items.Add(mnuLauncher);

      #endregion

      menuItem.Listeners.Click.Handler = "return false;";

      menuItem.Menu.Add(mnuMenu);

      myDesktop.StartMenu.ToolItems.Add(menuItem);

      myDesktop.StartMenu.ToolItems.Add(new Ext.Net.MenuSeparator());
    }

    #endregion

    #region Reporting

    //menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
    //{
    //  Icon = Ext.Net.Icon.Report,
    //  Text = "Laporan"
    //});

    dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
                        {
                          Title = "Laporan",
                          //Icon = Ext.Net.Icon.Report,
                          IconCls = "open-folder16",
                          InitCenter = true,
                          Width = 640,
                          Height = 480
                        })
                        {
                          ID = "ReportingViewer",
                          Resizable = true,
                          Maximizable = true
                        };

    dw.AutoLoad.Url = this.ResolveUrl("~/reporting/");
    dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
    dw.AutoLoad.ShowMask = true;

    ph.Controls.Add(dw);

    #region Module

    deskModule = new Ext.Net.DesktopModule(new Ext.Net.DesktopModule.Config()
    {
      ModuleID = "modRptViewer",
      WindowID = "ReportingViewer",
      AutoRun = false
    });

    myDesktop.Modules.Add(deskModule);

    //menuItem = deskModule.Launcher;

    menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
    {
      Text = "Laporan",
      //Icon = Ext.Net.Icon.Report
      IconCls = "open-folder16"
    });
    menuItem.ID = "lnchModRptViewer";
    menuItem.Listeners.Click.Handler = string.Format("{0}.show();", dw.ClientID);

    #endregion

    #region Shortcut

    deskShort = new Ext.Net.DesktopShortcut(new Ext.Net.DesktopShortcut.Config()
    {
      Text = "Laporan",
      //ShortcutID = "scLaporan",
      IconCls = "shortcut-icon icon-folder48",
      ModuleID = "modRptViewer"
    });

    myDesktop.Shortcuts.Add(deskShort);

    #endregion

    myDesktop.StartMenu.ToolItems.Add(menuItem);

    #endregion

    myDesktop.StartMenu.ToolItems.Add(new Ext.Net.MenuSeparator());

    #region Setting

    //Width = 515,
    //Height = 333
    dw = new Ext.Net.DesktopWindow(new Ext.Net.DesktopWindow.Config()
    {
      Title = "Panel Pengguna",
      Icon = Ext.Net.Icon.Wrench,
      InitCenter = true,
      Resizable = false,
      Maximizable = false,
      Width = 550,
      Height = 375,
      //CloseAction = CloseAction.Close
    })
    {
      ID = "PengaturanUserPanel",
      Resizable = false,
      Maximizable = false
    };

    dw.AutoLoad.Url = this.ResolveUrl("~/management/UserPanel.aspx");
    dw.AutoLoad.Mode = Ext.Net.LoadMode.IFrame;
    dw.AutoLoad.ShowMask = true;

    ph.Controls.Add(dw);

    #region Module

    menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
    {
      Icon = Ext.Net.Icon.Wrench,
      Text = "Pengaturan"
    });
    menuItem.ID = "lnchModUsrPanel";
    menuItem.Listeners.Click.Handler = string.Format("{0}.show();", dw.ClientID);

    #endregion

    myDesktop.StartMenu.ToolItems.Add(menuItem);

    #endregion

    myDesktop.StartMenu.ToolItems.Add(new Ext.Net.MenuSeparator());
    
    #region Logout

    menuItem = new Ext.Net.MenuItem(new Ext.Net.MenuItem.Config()
    {
      Icon = Ext.Net.Icon.Disconnect,
      Text = "Keluar"
    });

    menuItem.Listeners.Click.Handler = string.Format(@"ShowConfirm('Keluar ?', 'Anda yakin ingin keluar dari aplikasi ini ?', function(btn) {{
      if(btn == 'yes') {{
        Ext.net.DirectMethods.LogoutSystem('{0}');
      }}
    }});", NipUser);

    //menuItem.DirectEvents.Click.Event += Click_Logout_Event;
    //menuItem.DirectEvents.Click.EventMask.ShowMask = true;
    //menuItem.DirectEvents.Click.ExtraParams.Add(new Ext.Net.Parameter("NIP", NipUser));
    //menuItem.DirectEvents.Click.Confirmation.ConfirmRequest = true;
    //menuItem.DirectEvents.Click.Confirmation.Title = "Keluar ?";
    //menuItem.DirectEvents.Click.Confirmation.Message = "Anda yakin ingin keluar dari aplikasi ini ?";

    myDesktop.StartMenu.ToolItems.Add(menuItem);

    #endregion

    #region Image Picture Foto

    #region Old Coded

    /*
    .icon-linechart48
    {
      background-image: url('../images/Line_chart48.png') !important;
      filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='../images/Line_chart48.png', sizingMethod='scale');
    }
     */

    //    if (!string.IsNullOrEmpty(this.ImagePicName))
    //    {
    //      //Ext.Net.Image imgPic = new Ext.Net.Image(new Ext.Net.Image.Config()
    //      //{
    //      //  AlternateText = "Foto",
    //      //  Width = Unit.Parse("16"),
    //      //  Height = Unit.Parse("16"),
    //      //  ImageUrl = this.ResolveClientUrl(string.Concat("~/Images.aspx?m=user&f=", this.ImagePicName)),
    //      //});

    //      //imgPic.ToolTips.Add(new ToolTip()
    //      //{
    //      //  Title = this.Nip,
    //      //});

    //      //myDesktop.StartMenu.ToolItems.Add(imgPic);
    //      string tmp = this.ResolveClientUrl(string.Concat("~/Images.aspx?m=user&f=", this.ImagePicName));

    //      System.Web.UI.HtmlControls.HtmlGenericControl hgc = new System.Web.UI.HtmlControls.HtmlGenericControl("style");
    //      hgc.Attributes.Add("type", "text/css");
    //      hgc.Attributes.Add("media", "all");
    //      hgc.InnerHtml = string.Format(@".icon-pua {{
    //        background-image: url('{0}') !important;
    //        filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='{0}', sizingMethod='scale');
    //      }}", tmp);

    //      //filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(src='{0}', sizingMethod='scale');

    //      this.Page.Header.Controls.Add(hgc);

    //      myDesktop.StartMenu.IconCls = "icon-pua";
    //    }
    //    else
    //    {
    //      myDesktop.StartMenu.Icon = Ext.Net.Icon.User;
    //    }

    #endregion

    if ((!string.IsNullOrEmpty(this.ImagePicName)) && IsImageUserExists(this.ImagePicName))
    {
      #region Old Coded

      //System.Web.UI.HtmlControls.HtmlGenericControl hgc = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
      //hgc.ID = "userImagePicture";
      //hgc.Style[HtmlTextWriterStyle.Margin] = "5px";
      //hgc.Style.Add("float", "right");
      
      //hgc.InnerHtml = string.Format("<img id='imgPicUser' src='{0}' class='tengah' />", this.ResolveClientUrl(string.Concat("~/Images.aspx?m=user&f=", this.ImagePicName)));

      //ToolTip ttp = new ToolTip(new ToolTip.Config()
      //{
      //   Target = "imgPicUser",
      //   Title = this.Nip,
      //   Html = this.Username,
      //});

      ////imgPic.ToolTips.Add(new ToolTip()
      ////{
      ////  Title = this.Nip,
      ////  Html = this.Username
      ////});

      #endregion

      System.Web.UI.HtmlControls.HtmlImage hgc = new System.Web.UI.HtmlControls.HtmlImage();

      hgc.ID = "userImagePicture";
      hgc.Src = string.Concat("~/Images.aspx?m=user&f=", this.ImagePicName);

      //hgc.Style[HtmlTextWriterStyle.Position] = "fixed";
      //hgc.Style[HtmlTextWriterStyle.Top] = "50%";
      //hgc.Style[HtmlTextWriterStyle.Left] = "50%";
      //hgc.Style[HtmlTextWriterStyle.MarginTop] = "-50px";
      //hgc.Style[HtmlTextWriterStyle.MarginLeft] = "-100px";

      hgc.Style[HtmlTextWriterStyle.Display] = "block";
      hgc.Style[HtmlTextWriterStyle.MarginTop] = "10px";
      hgc.Style[HtmlTextWriterStyle.MarginLeft] = "auto";
      hgc.Style[HtmlTextWriterStyle.MarginRight] = "auto";

      myDesktop.Controls.Add(hgc);

      ToolTip ttp = new ToolTip(new ToolTip.Config()
      {
        Target = hgc.ClientID,
        Title = this.Nip,
        Html = this.Username,
      });

      myDesktop.Controls.Add(ttp);
    }

    myDesktop.StartMenu.Icon = Ext.Net.Icon.User;
    myDesktop.StartMenu.Title = this.Username;

    #endregion

    #region Image Wallpaper

    if ((!string.IsNullOrEmpty(this.ImageWallpaperName)) && IsImageUserExists(this.ImageWallpaperName))
    {
      string tmp = string.Format("{0}.getDesktop().setWallpaper('{1}');", myDesktop.ClientID, 
        this.ResolveClientUrl(string.Concat("~/Images.aspx?m=user&f=", this.ImageWallpaperName)));

      //myDesktop.Wallpaper = this.ResolveClientUrl(string.Concat("~/Images.aspx?m=user&f=", this.ImageWallpaperName));
      myDesktop.AddScript(tmp);
    }
    else
    {
      myDesktop.Wallpaper = "images/desktop.jpg";
    }
    string myScript = "\n<script type=\"text/javascript\" language=\"Javascript\" id=\"EventScriptBlock\">\n";
    myScript += "setTimeout(function() {ctl00_cphContent_SPPLDO.show();},5000);";
    myScript += "\n\n </script>";
    Page.ClientScript.RegisterStartupScript(this.GetType(), "myKey", myScript, false);

    #endregion

    //WinImageShow = new Ext.Net.Window("Testing show", Icon.Application);
    //WinImageShow.ID = "WinImageShow";
    //WinImageShow.Show();
  }

  [Ext.Net.DirectMethod(ShowMask = true, Timeout = Constant.DEFAULT_DIRECTEVENT_TIMEOUT)]
  public void LogoutSystem(string sNip)
  {
    Functional.DeleteCookies(this);

    this.Session.Remove(Constant.COOKIES_NAME);

    this.Session.Remove(Constant.SESSION_LOGIN_INFORMATION);

    this.Session.Clear();

    string tmp = this.ResolveClientUrl("~/");

    X.Redirect(tmp, "Mohon tunggu, sedang proses keluar dari sistem...");

    this.Session.Abandon();
  }
}
