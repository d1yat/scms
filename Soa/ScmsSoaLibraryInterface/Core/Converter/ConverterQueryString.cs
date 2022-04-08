using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Diagnostics;

namespace ScmsSoaLibraryInterface.Core.Converter
{
  public class WebQueryStringBehaviour : WebHttpBehavior
  {
    protected override System.ServiceModel.Dispatcher.QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
    {
      //if(operationDescription.Name.Equals("PostItemQuantityInfo", StringComparison.OrdinalIgnoreCase))
      //{
      //  Debug.Assert(false);
      //}

      //return base.GetQueryStringConverter(operationDescription);
      return new MyConverter(base.GetQueryStringConverter(operationDescription), operationDescription);
    }

    class MyConverter : QueryStringConverter
    {
      QueryStringConverter inner;
      OperationDescription operDesc;

      public MyConverter(QueryStringConverter inner, OperationDescription operationDescription)
      {
        this.inner = inner;
        this.operDesc = operationDescription;
      }

      public override bool CanConvert(Type type)
      {
        bool canConvert = false;

        if(type.Equals(typeof(string[])))
        {
          canConvert = true;
        }
        else if (type.Equals(typeof(string[][])))
        {
          canConvert = true;
        }
        else
        {
          canConvert = inner.CanConvert(type);
        }

        return canConvert;
      }

      public override object ConvertStringToValue(string parameter, Type parameterType)
      {
        //Debug.WriteLine(System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.ToString());

        if (parameterType == typeof(string[]))
        {
          string[] result = null;

          if (parameter != null)
          {
            result = parameter.Split(',');
          }
          else
          {
            result = new string[0];
          }

          return result;
        }
        else if (parameterType == typeof(string[][]))
        {
          if (parameter != null)
          {
            List<string[]> list = new List<string[]>();

            #region GlobalQueryJson

            if (operDesc.Name.Equals("GlobalQueryJson", StringComparison.OrdinalIgnoreCase) ||
              operDesc.Name.Equals("GlobalQueryServiceClient", StringComparison.OrdinalIgnoreCase))
            {
              string[] formater = parameter.Split(new[] { ',' });

              //result = parameter.Split(',');

              if (formater.Length >= 3)
              {
                int xLoop = 0;
                int nLoopC = 0;
                for (int nLoop = 0; nLoop < formater.Length; nLoop++)
                {
                  list.Add(new string[3]);
                  for (nLoopC = 0; nLoopC < 3; nLoopC++, nLoop++)
                  {
                    list[xLoop][nLoopC] = formater[nLoop];
                  }
                  xLoop++;
                  nLoop--;
                }
              }
            }

            #endregion

            return list.ToArray();
          }
          return null;
        }


        return inner.ConvertStringToValue(parameter, parameterType);
      }

      // The override below is not needed if used only at the server
      public override string ConvertValueToString(object parameter, Type parameterType)
      {
        //Debug.WriteLine(System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.ToString());

        if (parameterType == typeof(string[]))
        {
          StringBuilder sb = null;

          if (parameter != null)
          {
            string[] strArray = (string[])parameter;
            sb = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
            {
              if (i > 0) sb.Append(',');
              sb.Append(strArray[i]);
            }
          }

          return sb.ToString();
        }

        return inner.ConvertValueToString(parameter, parameterType);
      }
    }
  }
}