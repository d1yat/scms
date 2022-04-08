#undef SHA_384

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ScmsSoaLibraryInterface.Core.Crypto
{
  public class GlobalCrypto
  {
    public static byte[] Crypt1WayMD5(string val)
    {
      MD5 md5 = new MD5CryptoServiceProvider();
      Encoding enc = Encoding.ASCII;

      byte[] byt = enc.GetBytes(val);
      byte[] hash = md5.ComputeHash(byt);

      Array.Clear(byt, 0, byt.Length);
      byt = null;

      GC.Collect();

      return hash;
    }

    public static string Crypt1WayMD5String(string val)
    {
      byte[] byts = Crypt1WayMD5(val);
      string hash = string.Empty;

      foreach (byte byt in byts)
      {
        hash += byt.ToString("X");
      }
      Array.Clear(byts, 0, byts.Length);
      byts = null;

      GC.Collect();

      return hash;
    }

    public static byte[] GetStrongKeys(string key)
    {
      RNGCryptoServiceProvider rng;
      if (string.IsNullOrEmpty(key))
        rng = new RNGCryptoServiceProvider();
      else
        rng = new RNGCryptoServiceProvider(key);

      byte[] byt = new byte[64];
      rng.GetBytes(byt);

      return byt;
    }

    public static string GetHashPassword(byte[] strongKey, string pass)
    {
      byte[] byt = Encoding.UTF8.GetBytes(pass);
      string strRet = string.Empty;

      try
      {
        HMACRIPEMD160 mac3Des = new HMACRIPEMD160(strongKey);
        byte[] bytRet = mac3Des.ComputeHash(byt);
        strRet = Convert.ToBase64String(bytRet);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CGlobalCrypto:GetHashPassword - {0}", ex.Message));

        strRet = string.Empty;
      }

      return strRet;
    }

    public static string GetHashLicenses(byte[] strongKey, string pc)
    {
      byte[] byt = Encoding.UTF8.GetBytes(pc);
      string strRet = string.Empty;

      try
      {
        HMACSHA512 macSha512 = new HMACSHA512(strongKey);
        byte[] bytRet = macSha512.ComputeHash(byt);
        strRet = Convert.ToBase64String(bytRet);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CGlobalCrypto:GetHashLicenses - {0}", ex.Message));

        strRet = string.Empty;
      }

      return strRet;
    }

    public static byte[] FromBase64(string value)
    {
      byte[] byt = null;

      try
      {
        value = value.Replace('-', '+');
        value = value.Replace('_', '/');
        value = value.Replace('^', '=');

        byt = Convert.FromBase64String(value);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CGlobalCrypto:FromBase64 - {0}", ex.Message));

        byt = null;
      }

      return byt;
    }

    public static string ToBase64(byte[] value)
    {
      string str = string.Empty;

      try
      {
        str = Convert.ToBase64String(value);
        str = str.Replace('+', '-');
        str = str.Replace('/', '_');
        str = str.Replace('=', '^');
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CGlobalCrypto:ToBase64 - {0}", ex.Message));

        str = string.Empty;
      }
      return str;
    }
  }

  public abstract class CryptoBase
  {
    private string errMsg;

    protected ICryptoTransform transfCrypt, transfDeCrypt;

    private byte[] Transform(byte[] input, ICryptoTransform transf)
    {
      MemoryStream ms = new MemoryStream();

      byte[] ret = null;

      try
      {
        using (CryptoStream cryptStream = new CryptoStream(ms,
                                                           transf, CryptoStreamMode.Write))
        {
          bool gotError = false;
          try
          {
            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();

            ms.Position = 0;
            ret = ms.ToArray();
          }
          catch (Exception ex)
          {
            Debug.WriteLine(
              string.Format("ScmsSoaLibraryInterface.Core.Crypto.CryptoBase:Transform 1 - {0}", ex.Message));

            gotError = false;
            //isErr = true;
            errMsg = ex.Message;
            ret = null;
          }

          if ((cryptStream != null) && (!gotError))
          {
            try
            {
              cryptStream.Close();
              cryptStream.Dispose();
            }
            catch (Exception ex)
            {
              Debug.WriteLine(
                string.Format("ScmsSoaLibraryInterface.Core.Crypto.CryptoBase:Transform 2 - {0}", ex.Message));
            }
          }
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CryptoBase:Transform 3 - {0}", ex.Message));
      }

      if (ms != null)
      {
        ms.Close();
        ms.Dispose();
      }

      return ret;
    }

    private byte[] FromBase64(string value)
    {
      byte[] byt = null;

      try
      {
        value = value.Replace('-', '+');
        value = value.Replace('_', '/');
        value = value.Replace('.', '=');

        byt = Convert.FromBase64String(value);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CryptoBase:FromBase64 - {0}", ex.Message));

        byt = null;
      }

      return byt;
    }

    private string ToBase64(byte[] value)
    {
      string str = string.Empty;

      try
      {
        str = Convert.ToBase64String(value);
        str = str.Replace('+', '-');
        str = str.Replace('/', '_');
        str = str.Replace('=', '.');
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CryptoBase:ToBase64 - {0}", ex.Message));

        str = string.Empty;
      }
      return str;
    }

    public string Crypt(string value)
    {
      UTF8Encoding utf8 = (UTF8Encoding) Encoding.UTF8;

      byte[] inp = utf8.GetBytes(value);
      byte[] outp = CryptArray(inp);

      if (outp == null)
        return string.Empty;

      return ToBase64(outp);
    }

    public string DeCrypt(string value)
    {
      //byte[] inp = Convert.FromBase64String(value);
      byte[] inp = FromBase64(value);
      byte[] outp = DeCryptArray(inp);

      if (outp == null)
        return string.Empty;

      UTF8Encoding utf8 = (UTF8Encoding) Encoding.UTF8;
      return utf8.GetString(outp);
    }

    public string Crypt(byte[] value)
    {
      //byte[] inp = utf8.GetBytes(value);
      byte[] outp = CryptArray(value);

      return ToBase64(outp);
    }

    public string DeCrypt(byte[] value)
    {
      byte[] outp = DeCryptArray(value);

      UTF8Encoding utf8 = (UTF8Encoding) Encoding.UTF8;
      return utf8.GetString(outp);
    }

    public byte[] CryptArray(byte[] value)
    {
      if (value == null)
        return null;

      byte[] outp = Transform(value, transfCrypt);
      return outp;
    }

    public byte[] DeCryptArray(byte[] value)
    {
      if (value == null)
        return null;

      byte[] outp = Transform(value, transfDeCrypt);
      return outp;
    }

    public byte[] CryptArray(string value)
    {
      if (string.IsNullOrEmpty(value))
        return null;

      UTF8Encoding utf8 = (UTF8Encoding) Encoding.UTF8;
      byte[] inp = utf8.GetBytes(value);
      byte[] outp = CryptArray(inp);

      return outp;
    }

    public byte[] DeCryptArray(string value)
    {
      if (string.IsNullOrEmpty(value))
        return null;

      //byte[] inp = Convert.FromBase64String(value);
      byte[] inp = FromBase64(value);
      byte[] outp = DeCryptArray(inp);

      return outp;
    }
  }

  public class CryptorRijndael : CryptoBase
  {
    private readonly string errMsg;
    private readonly bool isErr;
    private readonly byte[] keyHash;
    private readonly byte[] keyIVHash;
    private readonly Rijndael oCrypt;

    #region "Private Prosedur"

    private byte[] GetTruncateHashShaMd5(string key, int length)
    {
      byte[] keyByte, hash;

#if SHA_384
      SHA384 sha = new SHA384CryptoServiceProvider();
#else
      SHA1 sha = new SHA1CryptoServiceProvider();
#endif

      MD5 md5 = new MD5CryptoServiceProvider();

      keyByte = Encoding.UTF8.GetBytes(key);
      hash = md5.ComputeHash(keyByte);
      Array.Clear(keyByte, 0, keyByte.Length);
      keyByte = null;

      keyByte = hash;

      hash = sha.ComputeHash(keyByte);
      Array.Resize(ref hash, length);

      GC.Collect();

      return hash;
    }

    #endregion

    public CryptorRijndael(string key, string keyIv)
    {
      errMsg = string.Empty;
      isErr = false;
      oCrypt = new RijndaelManaged();

      try
      {
        keyHash = GetTruncateHashShaMd5(key, oCrypt.KeySize/8);
        keyIVHash = GetTruncateHashShaMd5(keyIv, oCrypt.BlockSize/8);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CCryptorRijndael:CCryptorRijndael - {0}", ex.Message));

        errMsg = ex.Message;
        isErr = true;
      }

      base.transfCrypt = oCrypt.CreateEncryptor(keyHash, keyIVHash);
      base.transfDeCrypt = oCrypt.CreateDecryptor(keyHash, keyIVHash);
    }

    public bool IsError
    {
      get { return isErr; }
    }

    public string GetError
    {
      get { return errMsg; }
    }
  }

  public class Cryptor3DES : CryptoBase
  {
    private readonly string errMsg;
    private readonly bool isErr;
    private readonly byte[] keyHash;
    private readonly byte[] keyIVHash;
    private readonly TripleDES oCrypt;

    public Cryptor3DES(string key, string keyIv)
    {
      errMsg = string.Empty;
      isErr = false;
      oCrypt = new TripleDESCryptoServiceProvider();

      try
      {
        keyHash = GetTruncateHashShaMd5(key, oCrypt.KeySize/8);
        keyIVHash = GetTruncateHashShaMd5(keyIv, oCrypt.BlockSize/8);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CCryptor3DES:CCryptor3DES - {0}", ex.Message));

        errMsg = ex.Message;
        isErr = true;
      }

      base.transfCrypt = oCrypt.CreateEncryptor(keyHash, keyIVHash);
      base.transfDeCrypt = oCrypt.CreateDecryptor(keyHash, keyIVHash);
    }

    public bool IsError
    {
      get { return isErr; }
    }

    public string GetError
    {
      get { return errMsg; }
    }

    private byte[] GetTruncateHashShaMd5(string key, int length)
    {
      byte[] keyByte, hash;

#if SHA_384
      SHA384 sha = new SHA384CryptoServiceProvider();
#else
      SHA1 sha = new SHA1CryptoServiceProvider();
#endif
      MD5 md5 = new MD5CryptoServiceProvider();

      keyByte = Encoding.UTF8.GetBytes(key);
      hash = md5.ComputeHash(keyByte);
      Array.Clear(keyByte, 0, keyByte.Length);
      keyByte = null;

      keyByte = hash;

      hash = sha.ComputeHash(keyByte);
      Array.Resize(ref hash, length);

      GC.Collect();

      return hash;
    }
  }

  public class CryptorDES : CryptoBase
  {
    private readonly string errMsg;
    private readonly bool isErr;
    private readonly byte[] keyHash;
    private readonly byte[] keyIVHash;
    private readonly DES oCrypt;

    public CryptorDES(string key, string keyIv)
    {
      errMsg = string.Empty;
      isErr = false;
      oCrypt = new DESCryptoServiceProvider(); //TripleDESCryptoServiceProvider();

      try
      {
        keyHash = GetTruncateHashShaMd5(key, oCrypt.KeySize/8);
        keyIVHash = GetTruncateHashShaMd5(keyIv, oCrypt.BlockSize/8);
      }
      catch (Exception ex)
      {
        Debug.WriteLine(
          string.Format("ScmsSoaLibraryInterface.Core.Crypto.CCryptorDES:CCryptorDES - {0}", ex.Message));

        errMsg = ex.Message;
        isErr = true;
      }

      oCrypt.Mode = CipherMode.CFB;
      base.transfCrypt = oCrypt.CreateEncryptor(keyHash, keyIVHash);
      base.transfDeCrypt = oCrypt.CreateDecryptor(keyHash, keyIVHash);
    }

    public bool IsError
    {
      get { return isErr; }
    }

    public string GetError
    {
      get { return errMsg; }
    }

    private byte[] GetTruncateHashShaMd5(string key, int length)
    {
      byte[] keyByte, hash;
#if SHA_384
      SHA384 sha = new SHA384CryptoServiceProvider();
#else
      SHA1 sha = new SHA1CryptoServiceProvider();
#endif
      //MD5 md5 = new MD5CryptoServiceProvider();

      keyByte = Encoding.UTF8.GetBytes(key);
      //hash = md5.ComputeHash(keyByte);
      //Array.Clear(keyByte, 0, keyByte.Length);
      //keyByte = hash;

      hash = sha.ComputeHash(keyByte);
      Array.Resize(ref hash, length);

      return hash;
    }
  }
}