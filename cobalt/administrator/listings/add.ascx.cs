using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Security;
using System.Web.UI;
using AjaxControlToolkit;
using Cobalt.Core;
using Cobalt.Common;

namespace Cobalt.administrator.listings
{
    public partial class add : CobaltControl
    {
        private int insertId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable table = new DataTable();
            string query = "SELECT CategoryId, Name FROM ListingCategory";

            Database.adapterSelect(query, ref table);

            lstcategory.DataSource = table;
            lstcategory.DataTextField = "Name";
            lstcategory.DataValueField = "CategoryId";
            lstcategory.DataBind();
        }

        protected void btnsubmit_Click(object sender, EventArgs e)
        {
            string query = "INSERT INTO Listings(IsApproved, CategoryId, UserId, IsFetured, IsSponsor, Title, description, CurrencyId, Price, CountryIso, PostalCode, City, TimeStamp, IsSell, IsClosed) ";
            query += "VALUES(@IsApproved, @CategoryId, @UserId, @IsFetured, @IsSponsor, @Title, @description, @CurrencyId, @Price, @CountryIso, @PostalCode, @City, @TimeStamp, @IsSell, @IsClosed)";

            MembershipUser user = Membership.GetUser();
            string guid = user.ProviderUserKey.ToString();

            List<System.Data.SqlClient.SqlParameter> paras = new List<System.Data.SqlClient.SqlParameter>();
            Database.BuildSqlParameter("@IsApproved", SqlDbType.Bit, cbxapproved.Checked, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@CategoryId", SqlDbType.Int, lstcategory.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@UserId", SqlDbType.UniqueIdentifier, guid, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@IsFetured", SqlDbType.Bit, cbxfetured.Checked, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@IsSponsor", SqlDbType.Bit, cbxsponsor.Checked, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@Title", SqlDbType.NVarChar, txttitle.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@CurrencyId", SqlDbType.Char, txtcurrncyid.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@Price", SqlDbType.Decimal, txtprice.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@CountryIso", SqlDbType.Char, txtcountryiso.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@PostalCode", SqlDbType.NVarChar, txtpostacode.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@City", SqlDbType.NVarChar, txtcity.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@TimeStamp", SqlDbType.DateTime, DateTime.Now.ToString(), ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@IsSell", SqlDbType.Bit, cbxapproved.Text, ParameterDirection.Input, ref paras);
            Database.BuildSqlParameter("@IsClosed", SqlDbType.Bit, cbxclosed.Text, ParameterDirection.Input, ref paras);

            int insertId = Database.sqlInsert(query, paras);

            if (!(insertId <= 0))
            {
                status.Text = CobaltException.errorMessage("Failed to add Item");
            }
        }

        protected void AsyncFileUpload1_UploadComplete(object sender, AsyncFileUploadEventArgs e)
        {
            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Uploaded size: " + AsyncFileUpload1.FileBytes.Length.ToString() + "';", true);

            string folder = Convert.ToString(DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day);
            string fullpath = folder + "/" + Path.GetFileName(e.FileName);
            string path = "~/content/attachment/" + fullpath;

            // Uncomment to save to AsyncFileUpload\Uploads folder.
            // ASP.NET must have the necessary permissions to write to the file system.
            if (!Directory.Exists(MapPath("~/content/attachment/" + folder)))
            {
                Directory.CreateDirectory(MapPath("~/content/attachment/" + folder));
            }

            string savePath = MapPath(path);
            AsyncFileUpload1.SaveAs(savePath);

            string query = "INSERT INTO ListingImages(ListingId, ImagePath, Ordering) VALUES(" + insertId + ", " + folder + ", NULL)";
            int aff = Database.sqlInsert(query);
            if (aff <= 0)
            {
                CobaltException.errorMessage("Data Insertion Failed");
            }
        }

        private void AsyncFileUpload1_UploadedFileError(object sender, AsyncFileUploadEventArgs e)
        {
            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "error", "top.$get(\"" + uploadResult.ClientID + "\").innerHTML = 'Error: " + e.StatusMessage + "';", true);
        }       

        protected void addItem_Click(object sender, EventArgs e)
        {
            addListing.ActiveViewIndex = 0;
        }

        protected void uploadImage_Click(object sender, EventArgs e)
        {
            if ((insertId <= 0))
                status.Text = CobaltException.errorMessage("Please Insert Item First");
              else
            addListing.ActiveViewIndex = 1;
        }
    }
}