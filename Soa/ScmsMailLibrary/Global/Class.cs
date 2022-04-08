using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScmsMailLibrary.Global
{
  public class ClassNotifyEventArgs : EventArgs
  {
    public enum TypeEnum
    {
      IsError = -1,
      IsInformation = 0,
      IsNotify = 1,
    }

    public ClassNotifyEventArgs(string msg, TypeEnum type)
    {
      this.Message = msg;
      this.TypeMessage = type;
    }

    public string Message
    { get; private set; }

    public TypeEnum TypeMessage
    { get; private set; }
  }
}
