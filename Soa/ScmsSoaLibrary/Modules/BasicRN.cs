using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScmsSoaLibrary.Commons;
using ScmsModel.Core;
using ScmsModel;
using Ext.Net;
using System.Globalization;
using System.Data.Linq.SqlClient;

namespace ScmsSoaLibrary.Modules
{
  partial class BasicRN
  {
    ScmsModel.ORMDataContext ormdb;

    public BasicRN(ScmsModel.ORMDataContext db)
    {
      ormdb = db;
    }

    public decimal ModifyQuantity(string gudang, string rnNumber, string item, string batchId, decimal qty, bool isGood)
    {
      decimal result = -1;
      char gdg = (string.IsNullOrEmpty(gudang) ? '0' : gudang[0]);
      
      var qry = (from q in ormdb.LG_RND1s
                 where q.c_gdg == gdg && q.c_rnno == rnNumber && q.c_iteno == item && q.c_batch == batchId
                 select q).Take(1).SingleOrDefault();

      if (qry != null)
      {
        if (isGood)
        {
          result = (qry.n_gsisa.HasValue ? qry.n_gsisa.Value : 0);

          qry.n_gsisa += (qty > 0.00m ? qty : (-qty));
        }
        else
        {
          result = (qry.n_bsisa.HasValue ? qry.n_bsisa.Value : 0);

          qry.n_bsisa += (qty > 0.00m ? qty : (-qty));
        }

        ormdb.SubmitChanges();
      }

      return result;
    }
  }
}
