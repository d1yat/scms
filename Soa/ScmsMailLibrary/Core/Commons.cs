using System;
using System.Collections.Generic;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsMailLibrary.Core
{
  public class Commons
  {
    public static System.Data.DataTable ReadDbfDatabase(string dbfFile, string tahleName)
    {
      return ReadDbfDatabase(dbfFile, tahleName, true);
    }

    public static System.Data.DataTable ReadDbfDatabase(string dbfFile, string tableName, bool deleteWhenDone)
    {
      if (!System.IO.File.Exists(dbfFile))
      {
        return null;
      }

      System.Data.DataTable table = null;

      string fileName = System.IO.Path.GetFileName(dbfFile);
      string pathName = System.IO.Path.GetDirectoryName(dbfFile);
      string pathNameFull = (pathName.EndsWith("\\") ? pathName : string.Concat(pathName, "\\"));

      System.Data.OleDb.OleDbConnection con = null;
      System.Data.OleDb.OleDbCommand cmd = null;
      System.Data.OleDb.OleDbDataReader odbReader = null;

      try
      {
        con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties=dBASE IV;", pathName));
        //con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=vfpoledb;Data Source='{0}';Collating Sequence=general;", pathName));
        con.Open();

        if (con.State == System.Data.ConnectionState.Open)
        {
          cmd = con.CreateCommand();
          cmd.CommandText = string.Format("Select * From [{0}]", fileName);

          odbReader = cmd.ExecuteReader();

          if ((odbReader != null) && (odbReader.HasRows) && (!odbReader.IsClosed))
          {
            table = new System.Data.DataTable(tableName);
            table.Load(odbReader);
            
          }
        }
      }
        catch (Exception ex)
      {
        table = null;

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        if (odbReader != null)
        {
          odbReader.Close();
          odbReader.Dispose();
        }

        if (con != null)
        {
          if (con.State == System.Data.ConnectionState.Open)
          {
            con.Close();
          }
          con.Dispose();
        }
      }

      try
      {
        System.IO.File.Delete(dbfFile);
      }
      catch (Exception ex)
      {
        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }

      return table;
    }

    public static string DbfColumnParser(System.Data.DataColumn column)
    {
      string rets = null;

      if (column.DataType.Equals(typeof(DateTime)))
      {
          //modified by Handry 6 nov 2014, penambahan type date
          if (column.ExtendedProperties.ContainsKey("datetype"))
          {
              rets = string.Format("[{0}] date {1}",
                column.ColumnName,
                (column.AllowDBNull ? "NULL" : "NOT NULL"));
          }
          else
          {
              rets = string.Format("[{0}] datetime {1}",
                column.ColumnName,
                (column.AllowDBNull ? "NULL" : "NOT NULL"));
          }
      }
      else if (column.DataType.Equals(typeof(float)) ||
        column.DataType.Equals(typeof(double)) ||
        column.DataType.Equals(typeof(decimal)))
      {
          //modified by Handry 6 nov 2014, penambahan type numeric
          if (column.ExtendedProperties.ContainsKey("precision"))
          {
              rets = string.Format("[{0}] numeric ({1}) {2}",
                column.ColumnName,
                column.ExtendedProperties["precision"].ToString() != "" ? column.ExtendedProperties["precision"].ToString() : "18,2",
                (column.AllowDBNull ? "NULL" : "NOT NULL"));
          }
          else
          {
              rets = string.Format("[{0}] numeric (18,2) {1}",
                column.ColumnName,
                (column.AllowDBNull ? "NULL" : "NOT NULL"));
          }
      }
      else if (column.DataType.Equals(typeof(ushort)) ||
        column.DataType.Equals(typeof(short)) ||
        column.DataType.Equals(typeof(uint)) ||
        column.DataType.Equals(typeof(int)) ||
        column.DataType.Equals(typeof(ulong)) ||
        column.DataType.Equals(typeof(long)))
      {
        rets = string.Format("[{0}] int {1}",
          column.ColumnName,
          (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }
      else if (column.DataType.Equals(typeof(bool)))
      {
        rets = string.Format("[{0}] bit {1}",
          column.ColumnName,
          (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }
      else
      {
          //modified by Handry 6 nov 2014, penambahan pilihan length char
          rets = string.Format("[{0}] char({1}) {2}",
            column.ColumnName,
            column.MaxLength == -1 ? 254 : column.MaxLength,
            (column.AllowDBNull ? "NULL" : "NOT NULL"));
      }

      return rets;
    }
    
    public static string CreateDBF(System.Data.DataTable table, string pathName)
    {
      if (!System.IO.Directory.Exists(pathName) || (table == null))
      {
        return null;
      }

      string reslt = null;

      Random rnd = new Random((int)DateTime.Now.Ticks);

      string tableFileName = string.Empty,
        tmp = null;

      do
      {
        tableFileName = string.Concat("T", rnd.Next(0, (int)short.MaxValue).ToString("X04"));
        tmp = System.IO.Path.Combine(pathName, tableFileName);
      } while (System.IO.File.Exists(tmp));

      StringBuilder sb = new StringBuilder();
      string pathNameFull = System.IO.Path.Combine(pathName, tableFileName);
      string fileDbfName = (tableFileName.EndsWith(".dbf", StringComparison.OrdinalIgnoreCase) ? tableFileName : string.Concat(tableFileName, ".dbf"));
      string pathNameFullDbf = (tableFileName.EndsWith(".dbf", StringComparison.OrdinalIgnoreCase) ? pathNameFull : string.Concat(pathNameFull, ".dbf"));

      System.Data.OleDb.OleDbConnection con = null;
      System.Data.OleDb.OleDbCommand cmd = null;
      System.Data.OleDb.OleDbDataReader odbReader = null;
      System.Data.OleDb.OleDbDataAdapter adapt = null;
      System.Data.OleDb.OleDbCommandBuilder odbCmdBuild = null;
      System.Data.DataColumn col = null;
      System.Data.DataRow row = null;

      int nLoop = 0,
        nLen = 0,
        nLoopC = 0,
        nLenC = 0;

      bool bData = false;

      try
      {
        if (System.IO.File.Exists(pathNameFull))
        {
          System.IO.File.Delete(pathNameFull);
        }
        else if (System.IO.File.Exists(pathNameFullDbf))
        {
          System.IO.File.Delete(pathNameFullDbf);
        }

        //con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties=dBASE IV;", pathName));
        con = new System.Data.OleDb.OleDbConnection(string.Format("Provider=vfpoledb;Data Source='{0}';Collating Sequence=general;", pathName));
        con.Open();

        if (con.State == System.Data.ConnectionState.Open)
        {
          #region Create Table

          cmd = con.CreateCommand();

          sb.AppendFormat("CREATE TABLE {0} (", tableFileName);

          for (nLoop = 0, nLen = table.Columns.Count; nLoop < nLen; nLoop++)
          {
            if ((nLoop + 1) >= nLen)
            {
              sb.AppendFormat(" {0}", DbfColumnParser(table.Columns[nLoop]));
            }
            else
            {
              sb.AppendFormat(" {0},", DbfColumnParser(table.Columns[nLoop]));
            }
          }

          sb.Append(" )");

          cmd.CommandText = sb.ToString();
          cmd.ExecuteNonQuery();

          cmd.Dispose();

          sb.Remove(0, sb.Length);

          #endregion

          #region Populate Data

          cmd = con.CreateCommand();

          nLenC = table.Columns.Count;

          for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
          {
            col = table.Columns[nLoopC];

            reslt = string.Concat(reslt, ",", col.ColumnName);
          }

          reslt = (reslt.StartsWith(",", StringComparison.OrdinalIgnoreCase) ?
            reslt.Remove(0, 1) : reslt);

          for (nLoop = 0, nLen = table.Rows.Count; nLoop < nLen; nLoop++)
          {
            row = table.Rows[nLoop];

            sb.AppendFormat("Insert Into {0} ({1}) Values (", tableFileName, reslt);

            for (nLoopC = 0; nLoopC < nLenC; nLoopC++)
            {
              col = table.Columns[nLoopC];

              if (col.DataType.Equals(typeof(DateTime)))
              {
                //sb.AppendFormat("'{0}' ,", row.GetValue<DateTime>(col, ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime).ToString("yyyy-MM-dd"));
                //sb.AppendFormat("NULL ,", row.GetValue<DateTime>(col, ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime).ToString("yyyy-MM-dd"));
                sb.AppendFormat("DATE({0}) ,", row.GetValue<DateTime>(col, ScmsSoaLibrary.Commons.Functionals.StandardSqlDateTime).ToString("yyyy,MM,dd"));
              }
              else if (col.DataType.Equals(typeof(float)) ||
                col.DataType.Equals(typeof(double)) ||
                col.DataType.Equals(typeof(decimal)))
              {
                sb.AppendFormat("{0} ,", row.GetValue<decimal>(col, 0));
              }
              else if (col.DataType.Equals(typeof(ushort)) ||
                col.DataType.Equals(typeof(short)) ||
                col.DataType.Equals(typeof(uint)) ||
                col.DataType.Equals(typeof(int)) ||
                col.DataType.Equals(typeof(ulong)) ||
                col.DataType.Equals(typeof(long)))
              {
                sb.AppendFormat("{0} ,", row.GetValue<int>(col, 0));
              }
              else if (col.DataType.Equals(typeof(bool)))
              {
                bData = row.GetValue<bool>(col, false);
                sb.AppendFormat("{0} ,", (bData ? 1 : 0));
                //sb.AppendFormat("NULL ,", (bData ? 1 : 0));
              }
              else
              {
                sb.AppendFormat("'{0}' ,", row.GetValue<string>(col, string.Empty));
              }
            }

            sb.Remove(sb.Length - 1, 1);

            sb.AppendLine(" ) ");

            cmd.CommandText = sb.ToString();

            cmd.ExecuteNonQuery();

            sb.Remove(0, sb.Length);
          }

          #endregion

          if (System.IO.File.Exists(pathNameFull))
          {
            reslt = pathNameFull;
          }
          else if (System.IO.File.Exists(pathNameFullDbf))
          {
            reslt = pathNameFullDbf;
          }
          else
          {
            reslt = string.Empty;
          }
        }
      }
      catch (Exception ex)
      {
        reslt = null;

        table = null;

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        if (odbCmdBuild != null)
        {
          odbCmdBuild.Dispose();
        }

        if (adapt != null)
        {
          adapt.Dispose();
        }

        if (odbReader != null)
        {
          odbReader.Close();
          odbReader.Dispose();
        }

        if (cmd != null)
        {
          con.Dispose();
        }

        if (con != null)
        {
          if (con.State == System.Data.ConnectionState.Open)
          {
            con.Close();
          }
          con.Dispose();
        }
      }

      return reslt;
    }
    
    public static System.IO.MemoryStream CreateDBFStream(System.Data.DataTable table, string pathName)
    {
      string file = CreateDBF(table, pathName);

      System.IO.MemoryStream mem = null;
      System.IO.FileStream fs = null;
      bool isReaded = false;

      if (!string.IsNullOrEmpty(file))
      {
        if (System.IO.File.Exists(file))
        {
          try
          {
            fs = System.IO.File.OpenRead(file);

            mem = new System.IO.MemoryStream();

            mem.SetLength(fs.Length);

            fs.Read(mem.GetBuffer(), 0, (int)fs.Length);

            isReaded = true;

            if (fs != null)
            {
              fs.Close();
              fs.Dispose();
              fs = null;
            }

            System.IO.File.Delete(file);
          }
          catch (Exception ex)
          {
            Logger.WriteLine(ex.Message);
            Logger.WriteLine(ex.StackTrace);
          }
          finally
          {
            if (fs != null)
            {
              fs.Close();
              fs.Dispose();
            }

            if ((!isReaded) && (mem != null))
            {
              mem.Close();
              mem.Dispose();
              mem = null;
            }
          }
        }
      }

      return mem;
    }
  }
}
