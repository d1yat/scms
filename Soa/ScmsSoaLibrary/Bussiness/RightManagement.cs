using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsModel;
using ScmsSoaLibrary.Parser;
using ScmsSoaLibrary.Commons;
using ScmsSoaLibraryInterface.Core.Crypto;
using ScmsSoaLibraryInterface.Commons;

namespace ScmsSoaLibrary.Bussiness
{
  class RightManagement
  {
    public string UserManagement(Parser.Parser.StructureXmlHeaderParser xmlParser, Parser.Parser.StructureDataNamingHeader dataParser)
    {
      string result = null;

      MyAssembly myAsm = new MyAssembly();

      SCMS_USER tableUser = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      Parser.Parser.StructureDataInputDetail[] sis = null;

      SCMS_USER tableUserEdit = null;

      DateTime date = DateTime.Now;

      string sNip = null,
        passWd = null,
        strongKey = null,
        sNipEdit = null,
        tmp = null;

      string oldPassword = null,
        newPassword = null;

      bool isReset = false;
      
      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

      try
      {
        tableUser = myAsm.Populate<SCMS_USER>(xmlParser, dataParser);

        switch (dataParser.Method)
        {
          #region Add

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsAdd:
            {
              strongKey = GlobalCrypto.Crypt1WayMD5String(string.Concat((string.IsNullOrEmpty(sNip) ? "Mochamad Rudi" : sNip), DateTime.Now.ToString("yyyyMMddHHmmssfff")));

              sNip = (tableUser.c_nip ?? string.Empty);

              sNipEdit = (tableUser.c_update ?? string.Empty);

              if (string.IsNullOrEmpty(sNip))
              {
                result = "Nip dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }
              else if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              passWd = (tableUser.v_password ?? "SCMS");

              result = myAsm.VerifyParser(xmlParser, dataParser);

              if (!string.IsNullOrEmpty(result))
              {
                result = "Verifikasi gagal.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              // Check Duplikat Nip
              int nCount = (from q in db.SCMS_USERs
                            where q.c_nip == sNip
                            select q.c_nip).Count();

              if (nCount > 0)
              {
                result = "Nip telah ada.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              tableUser.d_update = tableUser.d_entry = date;

              tableUser.x_hash = strongKey;

              tableUser.v_password = Functionals.CryptHashRjdnl(strongKey, passWd);

              db.SCMS_USERs.InsertOnSubmit(tableUser);

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Update

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsUpdate:
            {
              //tableUser = myAsm.Populate<SCMS_USER>(xmlParser, dataParser);

              sNip = (tableUser.c_nip ?? string.Empty);

              sNipEdit = (tableUser.c_entry ?? string.Empty);

              if (string.IsNullOrEmpty(sNip))
              {
                result = "Nip dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }
              else if (sNip.Equals("Administrator", StringComparison.OrdinalIgnoreCase) && (!sNip.Equals(sNipEdit, StringComparison.OrdinalIgnoreCase)))
              {
                result = "User 'Administrator' tidak dapat diubah oleh sembarang user.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }
              else if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              sis = myAsm.SeekNameData("OldPassword", dataParser);
              if ((sis != null) && (sis.Length > 0))
              {
                oldPassword = (sis[0].Value ?? string.Empty);
                if (!string.IsNullOrEmpty(oldPassword))
                {
                  isReset = true;
                }
              }

              if (isReset && string.IsNullOrEmpty(oldPassword))
              {
                result = "Password yang lama dibutuhkan untuk merubah password.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              tableUserEdit = db.SCMS_USERs.Where(x => x.c_nip == sNip).Take(1).SingleOrDefault();
              if (tableUserEdit == null)
              {
                result = "Data tidak di temukan.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              strongKey = tableUserEdit.x_hash;

              if (isReset)
              {
                passWd = Functionals.DecryptHashRjdnl(strongKey, tableUserEdit.v_password);

                if (!passWd.Equals(oldPassword))
                {
                  result = "Password tidak sama.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }
                else if (passWd.Equals(tableUser.v_password))
                {
                  result = "Password yang ingin dirubah tidak boleh sama.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                tableUserEdit.v_password = Functionals.CryptHashRjdnl(strongKey, tableUser.v_password);
              }

              tableUserEdit.l_aktif = tableUser.l_aktif;
              tableUserEdit.v_username = tableUser.v_username;
              tableUserEdit.c_gdg = tableUser.c_gdg; //add by suwandi 25 april 2017
              tableUserEdit.c_nosup = tableUser.c_nosup;
              tableUserEdit.c_kddivpri = tableUser.c_kddivpri;

              tableUserEdit.c_update = sNipEdit;
              tableUserEdit.d_update = date;

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Delete

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsDelete:
            {
              //tableUser = myAsm.Populate<SCMS_USER>(xmlParser, dataParser);

              sNip = (tableUser.c_nip ?? string.Empty);

              sNipEdit = (tableUser.c_update ?? string.Empty);

              if (string.IsNullOrEmpty(sNip))
              {
                result = "Nip dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }
              else if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }
              else if (sNip.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
              {
                result = "User 'Administrator' tidak dapat dihapus.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              tableUserEdit = db.SCMS_USERs.Where(x => x.c_nip == sNip).Take(1).SingleOrDefault();
              if (tableUserEdit == null)
              {
                result = "Data tidak di temukan.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              db.SCMS_USERs.DeleteOnSubmit(tableUserEdit);

              var ieUserGroups = db.SCMS_GROUPEDs.Where(x => x.c_nip == sNip);

              db.SCMS_GROUPEDs.DeleteAllOnSubmit<SCMS_GROUPED>(ieUserGroups);

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Unknown

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.Unknown:
            {
              if(dataParser.CustomMethod.Equals("UserPanel", StringComparison.OrdinalIgnoreCase))
              {
                #region UserPanel

                bool isClearPic = false,
                  isClearWall = false;

                sNip = (tableUser.c_nip ?? string.Empty);

                sNipEdit = (tableUser.c_update ?? string.Empty);

                if (string.IsNullOrEmpty(sNip))
                {
                  result = "Nip dibutuhkan.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }
                else if (string.IsNullOrEmpty(sNipEdit))
                {
                  result = "Nip penanggung jawab dibutuhkan.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }
                else if (sNip.Equals("Administrator", StringComparison.OrdinalIgnoreCase) && (!sNip.Equals(sNipEdit, StringComparison.OrdinalIgnoreCase)))
                {
                  result = "User 'Administrator' tidak dapat diubah oleh sembarang user.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                sis = myAsm.SeekNameData("Password", dataParser);
                if ((sis != null) && (sis.Length > 0))
                {
                  newPassword = (sis[0].Value ?? string.Empty);
                  if (!string.IsNullOrEmpty(newPassword))
                  {
                    isReset = true;
                  }
                }

                sis = myAsm.SeekNameData("ClearPic", dataParser);
                if ((sis != null) && (sis.Length > 0))
                {
                  bool.TryParse((sis[0].Value ?? string.Empty), out isClearPic);
                }

                sis = myAsm.SeekNameData("ClearWall", dataParser);
                if ((sis != null) && (sis.Length > 0))
                {
                  bool.TryParse((sis[0].Value ?? string.Empty), out isClearWall);
                }

                sis = myAsm.SeekNameData("OldPassword", dataParser);
                if ((sis != null) && (sis.Length > 0))
                {
                  oldPassword = (sis[0].Value ?? string.Empty);
                }

                if (isReset && string.IsNullOrEmpty(oldPassword))
                {
                  result = "Password yang lama dibutuhkan untuk merubah password.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                tableUserEdit = db.SCMS_USERs.Where(x => x.c_nip == sNip).Take(1).SingleOrDefault();
                if (tableUserEdit == null)
                {
                  result = "Data tidak di temukan.";

                  rpe = ResponseParser.ResponseParserEnum.IsError;

                  break;
                }

                strongKey = tableUserEdit.x_hash;

                passWd = Functionals.DecryptHashRjdnl(strongKey, tableUserEdit.v_password);

                if (!passWd.Equals(oldPassword))
                {
                  result = "Password tidak sama.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                if (isReset)
                {
                  if (passWd.Equals(tableUser.v_password))
                  {
                    result = "Password yang ingin dirubah tidak boleh sama.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    break;
                  }

                  tableUserEdit.v_password = Functionals.CryptHashRjdnl(strongKey, tableUser.v_password);
                }

                tableUserEdit.v_username = tableUser.v_username;

                tableUserEdit.c_update = sNipEdit;
                tableUserEdit.d_update = date;

                if (isClearPic)
                {
                  tableUserEdit.v_imgfile = null;
                }
                else
                {
                  sis = myAsm.SeekNameData("ImagePic", dataParser);
                  if ((sis != null) && (sis.Length > 0))
                  {
                    tmp = (sis[0].Value ?? string.Empty);
                    if (!string.IsNullOrEmpty(tmp))
                    {
                      tableUserEdit.v_imgfile = tmp;
                    }
                  }
                }

                if (isClearWall)
                {
                  tableUserEdit.v_wallpaper = null;
                }
                else
                {
                  sis = myAsm.SeekNameData("ImageWallpaper", dataParser);
                  if ((sis != null) && (sis.Length > 0))
                  {
                    tmp = (sis[0].Value ?? string.Empty);
                    if (!string.IsNullOrEmpty(tmp))
                    {
                      tableUserEdit.v_wallpaper = tmp;
                    }
                  }
                }

                db.SubmitChanges();

                rpe = ResponseParser.ResponseParserEnum.IsSuccess;

                #endregion
              }
              else if (dataParser.CustomMethod.Equals("ResetPassword", StringComparison.OrdinalIgnoreCase))
              {
                #region ResetPassword
                
                ScmsSoaLibraryInterface.Core.Crypto.Cryptor3DES des3 = null;

                sNip = (tableUser.c_nip ?? string.Empty);

                sNipEdit = (tableUser.c_update ?? string.Empty);

                if (string.IsNullOrEmpty(sNip))
                {
                  result = "Nip dibutuhkan.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }
                else if (string.IsNullOrEmpty(sNipEdit))
                {
                  result = "Nip penanggung jawab dibutuhkan.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }
                else if (sNip.Equals("Administrator", StringComparison.OrdinalIgnoreCase) && (!sNip.Equals(sNipEdit, StringComparison.OrdinalIgnoreCase)))
                {
                  result = "User 'Administrator' tidak dapat diubah oleh sembarang user.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                sis = myAsm.SeekNameData("ExchangeID", dataParser);
                if ((sis != null) && (sis.Length > 0))
                {
                  strongKey = (sis[0].Value ?? string.Empty);
                  if (string.IsNullOrEmpty(strongKey))
                  {
                    result = "Invalid hash code.";

                    rpe = ResponseParser.ResponseParserEnum.IsFailed;

                    break;
                  }
                }

                oldPassword = tableUser.v_password;
                if (string.IsNullOrEmpty(oldPassword))
                {
                  result = "Kata kunci baru tidak boleh kosong.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                des3 = new ScmsSoaLibraryInterface.Core.Crypto.Cryptor3DES(strongKey, ScmsSoaLibraryInterface.Core.Crypto.GlobalCrypto.Crypt1WayMD5String(sNip));

                newPassword = des3.DeCrypt(oldPassword);

                if (string.IsNullOrEmpty(newPassword))
                {
                  result = "HashCode password baru tidak dapat di verifikasi.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                tableUserEdit = db.SCMS_USERs.Where(x => x.c_nip == sNip).Take(1).SingleOrDefault();
                if (tableUserEdit == null)
                {
                  result = "Data tidak di temukan.";

                  rpe = ResponseParser.ResponseParserEnum.IsError;

                  break;
                }

                strongKey = tableUserEdit.x_hash;

                passWd = Functionals.DecryptHashRjdnl(strongKey, tableUserEdit.v_password);
                                
                if (passWd.Equals(newPassword))
                {
                  result = "Password yang ingin dirubah tidak boleh sama.";

                  rpe = ResponseParser.ResponseParserEnum.IsFailed;

                  break;
                }

                tableUserEdit.v_password = Functionals.CryptHashRjdnl(strongKey, newPassword);

                tableUserEdit.c_update = sNipEdit;
                tableUserEdit.d_update = date;

                db.SubmitChanges();

                rpe = ResponseParser.ResponseParserEnum.IsSuccess;

                #endregion
              }
            }
            break;

          #endregion
        }
      }
      catch (Exception ex)
      {
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.RightManagement:UserManagement - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);
      }

      result = Parser.ResponseParser.ResponseGenerator(rpe, null, result);

      db.Dispose();

      return result;
    }

    public string GroupManagement(Parser.Parser.StructureXmlHeaderParser xmlParser, Parser.Parser.StructureDataNamingHeader dataParser)
    {
      string result = null;

      DateTime date = DateTime.Now;

      MyAssembly myAsm = new MyAssembly();

      SCMS_GROUP tableGroup = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      //Parser.Parser.StructureDataInputDetail[] sis = null;

      string sGroup = null;
      string sNipEdit = null;
      string tmp = null;

      SCMS_GROUP tableGroupEdit = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

      string strongKey = GlobalCrypto.Crypt1WayMD5String("Mochamad Rudi @ 2011");

      try
      {
        tableGroup = myAsm.Populate<SCMS_GROUP>(xmlParser, dataParser);

        switch (dataParser.Method)
        {
          #region Add

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsAdd:
            {
              sGroup = (tableGroup.c_group ?? string.Empty);

              sNipEdit = (tableGroup.c_update ?? string.Empty);

              if (string.IsNullOrEmpty(sGroup))
              {
                result = "Group dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              result = myAsm.VerifyParser(xmlParser, dataParser);

              if (!string.IsNullOrEmpty(result))
              {
                result = "Verifikasi gagal.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              // Check Duplikat Group
              int nCount = (from q in db.SCMS_GROUPs
                            where q.c_group == sGroup
                            select q.c_group).Count();

              if (nCount > 0)
              {
                result = "Group telah ada.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              tmp = tableGroup.v_akses;

              tableGroup.v_akses = (string.IsNullOrEmpty(tmp) ? "" : Functionals.CryptHashRjdnl(strongKey, tmp));

              tableGroup.d_update = tableGroup.d_entry = date;

              db.SCMS_GROUPs.InsertOnSubmit(tableGroup);

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Update

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsUpdate:
            {
              sGroup = (tableGroup.c_group ?? string.Empty);

              sNipEdit = (tableGroup.c_update ?? string.Empty);

              if (string.IsNullOrEmpty(sGroup))
              {
                result = "Group dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              tableGroupEdit = db.SCMS_GROUPs.Where(x => x.c_group == sGroup).Take(1).SingleOrDefault();
              if (tableGroupEdit == null)
              {
                result = "Data tidak di temukan.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              tableGroupEdit.v_group_desc = tableGroup.v_group_desc;

              tmp = tableGroup.v_akses;

              if (!string.IsNullOrEmpty(tmp))
              {
                  tableGroupEdit.v_akses = (string.IsNullOrEmpty(tmp) ? "" : Functionals.CryptHashRjdnl(strongKey, tmp));
              }
              tableGroupEdit.l_aktif = tableGroup.l_aktif;

              tableGroupEdit.c_update = sNipEdit;
              tableGroupEdit.d_update = date;

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion

          #region Delete

          case ScmsSoaLibrary.Parser.Parser.InterpreterMethod.IsDelete:
            {
              sGroup = (tableGroup.c_group ?? string.Empty);

              sNipEdit = (tableGroup.c_update ?? string.Empty);

              if (string.IsNullOrEmpty(sGroup))
              {
                result = "Group dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }
              else if (string.IsNullOrEmpty(sNipEdit))
              {
                result = "Nip penanggung jawab dibutuhkan.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }
              else if (sGroup.Equals("Admin", StringComparison.OrdinalIgnoreCase))
              {
                result = "Group 'Admin' tidak dapat dihapus.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              if (sGroup.Equals("Admin", StringComparison.OrdinalIgnoreCase))
              {
                result = "Maaf, grup ini tidak dapat di hapus.";

                rpe = ResponseParser.ResponseParserEnum.IsFailed;

                break;
              }

              tableGroupEdit = db.SCMS_GROUPs.Where(x => x.c_group == sGroup).Take(1).SingleOrDefault();
              if (tableGroupEdit == null)
              {
                result = "Data tidak di temukan.";

                rpe = ResponseParser.ResponseParserEnum.IsError;

                break;
              }

              db.SCMS_GROUPs.DeleteOnSubmit(tableGroupEdit);

              var ieUserGroups = db.SCMS_GROUPEDs.Where(x => x.c_grouped == sGroup).ToList();

              db.SCMS_GROUPEDs.DeleteAllOnSubmit<SCMS_GROUPED>(ieUserGroups.ToArray());

              ieUserGroups.Clear();

              db.SubmitChanges();

              rpe = ResponseParser.ResponseParserEnum.IsSuccess;
            }
            break;

          #endregion
        }
      }
      catch (Exception ex)
      {
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.RightManagement:GroupManagement - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);
      }

      result = Parser.ResponseParser.ResponseGenerator(rpe, null, result);

      db.Dispose();

      return result;
    }

    public string UserGroupManagement(ScmsSoaLibrary.Parser.Class.UserGroupAccessStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      if (structure.IsGroupMode)
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Invalid structure method");
      }

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      SCMS_GROUPED grouped = null;
      ScmsSoaLibrary.Parser.Class.UserGroupAccessStructureField field = null;
      string sNip = null;
      string nipEntry = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

      try
      {
        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          sNip = (structure.Fields.Nip ?? string.Empty);
          nipEntry = (structure.Fields.Entry ?? string.Empty);

          if ((!string.IsNullOrEmpty(sNip)) && (!string.IsNullOrEmpty(nipEntry)) && (structure.Fields.Field != null))
          {
            for (int nLoop = 0, nLen = structure.Fields.Field.Length; nLoop < nLen; nLoop++)
            {
              field = structure.Fields.Field[nLoop];
              if ((field != null) && (!string.IsNullOrEmpty(field.Value)))
              {
                if (field.IsNew && (!field.IsDelete))
                {
                  grouped = new SCMS_GROUPED()
                  {
                    c_entry = nipEntry,
                    c_grouped = field.Value,
                    c_nip = sNip,
                    d_entry = DateTime.Now
                  };

                  db.SCMS_GROUPEDs.InsertOnSubmit(grouped);
                }
                else if ((!field.IsNew) && field.IsDelete)
                {
                  grouped = db.SCMS_GROUPEDs.Where(x => x.c_nip == sNip && x.c_grouped == field.Value).Take(1).SingleOrDefault();
                  if (grouped != null)
                  {
                    db.SCMS_GROUPEDs.DeleteOnSubmit(grouped);
                  }
                }
              }
            }

            db.SubmitChanges();

            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
          }
          else
          {
            result = "Anda Tidak Melakukan Perubahan.";

            rpe = ResponseParser.ResponseParserEnum.IsFailed;
          }
        }
      }
      catch (Exception ex)
      {
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.RightManagement:UserGroupManagement - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);
      }

      result = Parser.ResponseParser.ResponseGenerator(rpe, null, result);
      
      db.Dispose();

      return result;
    }

    public string GroupUserManagement(ScmsSoaLibrary.Parser.Class.UserGroupAccessStructure structure)
    {
      if ((structure == null) || (structure.Fields == null))
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Structure object null/invalid");
      }

      if (!structure.IsGroupMode)
      {
        return Parser.ResponseParser.ResponseGenerator(ResponseParser.ResponseParserEnum.IsError, null, "Invalid structure method");
      }

      string result = null;

      ORMDataContext db = new ORMDataContext(Functionals.ActiveConnectionString);

      SCMS_GROUPED grouped = null;
      ScmsSoaLibrary.Parser.Class.UserGroupAccessStructureField field = null;
      string sGroup = null;
      string nipEntry = null;

      ResponseParser.ResponseParserEnum rpe = ResponseParser.ResponseParserEnum.Unknown;

      try
      {
        if (structure.Method.Equals("Modify", StringComparison.OrdinalIgnoreCase))
        {
          sGroup = (structure.Fields.Group ?? string.Empty);
          nipEntry = (structure.Fields.Entry ?? string.Empty);

          if ((!string.IsNullOrEmpty(sGroup)) && (!string.IsNullOrEmpty(nipEntry)) && (structure.Fields.Field != null))
          {
            for (int nLoop = 0, nLen = structure.Fields.Field.Length; nLoop < nLen; nLoop++)
            {
              field = structure.Fields.Field[nLoop];
              if ((field != null) && (!string.IsNullOrEmpty(field.Value)))
              {
                if (field.IsNew && (!field.IsDelete))
                {
                  grouped = new SCMS_GROUPED()
                  {
                    c_entry = nipEntry,
                    c_grouped = sGroup,
                    c_nip = field.Value,
                    d_entry = DateTime.Now
                  };

                  db.SCMS_GROUPEDs.InsertOnSubmit(grouped);
                }
                else if ((!field.IsNew) && field.IsDelete)
                {
                  grouped = db.SCMS_GROUPEDs.Where(x => x.c_grouped == sGroup && x.c_nip == field.Value).Take(1).SingleOrDefault();
                  if (grouped != null)
                  {
                    db.SCMS_GROUPEDs.DeleteOnSubmit(grouped);
                  }
                }
              }
            }

            db.SubmitChanges();

            rpe = ResponseParser.ResponseParserEnum.IsSuccess;
          }
        }
        else
        {
          result = "Anda Tidak Melakukan Perubahan.";

          rpe = ResponseParser.ResponseParserEnum.IsFailed;
        }
      }
      catch (Exception ex)
      {
        rpe = ResponseParser.ResponseParserEnum.IsError;

        result = string.Format("ScmsSoaLibrary.Bussiness.RightManagement:GroupUserManagement - {0}", ex.Message);

        Logger.WriteLine(result);
        Logger.WriteLine(ex.StackTrace);
      }

      result = Parser.ResponseParser.ResponseGenerator(rpe, null, result);


      db.Dispose();

      return result;
    }
  }
}
