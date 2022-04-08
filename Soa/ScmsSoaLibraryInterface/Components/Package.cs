using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibraryInterface.Components
{
  public class Package
  {
    private const int TOTAL_BUFFER = 4096;

    public MemoryStream CompresDeflate(MemoryStream mem)
    {
      if((mem ==null) || (!mem.CanRead) || (mem.Length < 1))
      {
        return null;
      }

      MemoryStream memData = null,
        memStream = null;
      DeflateStream Compress = null;
      byte[] buff = null;
      int numRead = 0;

      try
      {
        mem.Position = 0;

        memData = new MemoryStream();

        Compress = new DeflateStream(memData, CompressionMode.Compress);

        buff = new byte[TOTAL_BUFFER];

        while ((numRead = mem.Read(buff, 0, TOTAL_BUFFER)) != 0)
        {
          numRead = (numRead < TOTAL_BUFFER ? (numRead + 1) : numRead);

          Compress.Write(buff, 0, numRead);

          Array.Clear(buff, 0, numRead);
        }

        if (memData.Length == 0)
        {
          memData.Close();
          memData.Dispose();
          memData = null;
        }
        else
        {
          memStream = new MemoryStream();

          memData.WriteTo(memStream);
          
          memStream.Position = 0;
        }
      }
      catch (Exception ex)
      {

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        if (Compress != null)
        {
          Compress.Close();
          Compress.Dispose();
        }

        if (memData != null)
        {
          memData.Close();
          memData.Dispose();
        }
      }

      return memStream;
    }

    public MemoryStream DecompresDeflate(MemoryStream mem)
    {
      if ((mem == null) || (!mem.CanRead) || (mem.Length < 1))
      {
        return null;
      }

      MemoryStream memData = null;
      DeflateStream Decompress = null;
      byte[] buff = null;
      int numRead = 0;

      try
      {
        mem.Position = 0;

        memData = new MemoryStream();

        Decompress = new DeflateStream(mem, CompressionMode.Decompress);

        buff = new byte[TOTAL_BUFFER];

        while ((numRead = Decompress.Read(buff, 0, TOTAL_BUFFER)) != 0)
        {
          memData.Write(buff, 0, numRead);

          Array.Clear(buff, 0, numRead);
        }

        memData.Position = 0;
      }
      catch (Exception ex)
      {
        if (memData != null)
        {
          memData.Close();
          memData.Dispose();
          memData = null;
        }

        Logger.WriteLine(ex.Message);
        Logger.WriteLine(ex.StackTrace);
      }
      finally
      {
        if (Decompress != null)
        {
          Decompress.Close();
          Decompress.Dispose();
        }
      }

      return memData;
    }
  }
}
