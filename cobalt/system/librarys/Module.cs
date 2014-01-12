using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Cobalt.Core;

namespace Cobalt.system.librarys
{
    public class Module
    {
        public static List<XElement> getModuleData()
        {
            List<XElement> moduleData = new List<XElement>();
            string doc = HttpContext.Current.Server.MapPath("~/system/modules/test.xml");

            XDocument xdoc = XDocument.Load(doc);

            var data = from item in xdoc.Descendants("module")
                       select new
                           {
                               moduleid = item.Element("data").Element("moduleid"),
                               modulename = item.Element("data").Element("modulename"),
                               ismenu = item.Element("data").Element("ismenu"),
                               menuname = item.Element("data").Element("menuname"),
                               issetting = item.Element("data").Element("issetting"),
                               settingname = item.Element("data").Element("settingname"),
                               description = item.Element("data").Element("description"),

                               install = item.Element("install"),
                               uninstall = item.Element("uninstall")
                           };

            foreach (var dat in data)
            {
                moduleData.Add(dat.moduleid);
                moduleData.Add(dat.modulename);
                moduleData.Add(dat.ismenu);
                moduleData.Add(dat.menuname);
                moduleData.Add(dat.issetting);
                moduleData.Add(dat.settingname);
                moduleData.Add(dat.description);
                moduleData.Add(dat.install);
                moduleData.Add(dat.uninstall);
            }
            return moduleData;
        }

        public static int installModule()
        {
            string moduleid = null, modulename = null, menuname = null, settingname = null, description = null, install = null, uninstall = null;
            bool ismenu = false, issetting = false;

            string doc = HttpContext.Current.Server.MapPath("~/system/modules/test.xml");

            XDocument xdoc = XDocument.Load(doc);

            var data = from item in xdoc.Descendants("module")
                       select new
                           {
                               moduleid = item.Element("data").Element("moduleid"),
                               modulename = item.Element("data").Element("modulename"),
                               ismenu = item.Element("data").Element("ismenu"),
                               menuname = item.Element("data").Element("menuname"),
                               issetting = item.Element("data").Element("issetting"),
                               settingname = item.Element("data").Element("settingname"),
                               description = item.Element("data").Element("description"),

                               install = item.Element("install"),
                               uninstall = item.Element("uninstall")
                           };

            foreach (var dat in data)
            {
                moduleid = dat.moduleid.Value.Trim();
                modulename = dat.modulename.Value.Trim();
                menuname = dat.menuname.Value.Trim();
                issetting = Convert.ToBoolean(dat.issetting.Value.Trim());
                settingname = dat.settingname.Value.Trim();
                description = dat.description.Value.Trim();
                install = dat.install.Value.Trim();
                uninstall = dat.uninstall.Value.Trim();
                ismenu = Convert.ToBoolean(dat.ismenu.Value.Trim());
            }

            string query = "INSERT INTO Modules([ModuleId],[ModuleName],[IsMenu],[MenuName],[IsSetting],[SettingName],[Description],[InstallCode],[UninstallCode]) ";
            query += "VALUES(@ModuleId,@ModuleName,@IsActive,@IsMenu,@MenuName,@IsSetting,@SettingName,@Description,@InstallCode,@UninstallCode)";

            List<SqlParameter> paralist = new List<SqlParameter>();
            Database.BuildSqlParameter("@ModuleId", SqlDbType.NVarChar, moduleid, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@ModuleName", SqlDbType.NVarChar, modulename, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@IsActive", SqlDbType.Bit, false, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@IsMenu", SqlDbType.Bit, ismenu, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@MenuName", SqlDbType.NVarChar, menuname, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@IsSetting", SqlDbType.Bit, issetting, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@Description", SqlDbType.Text, description, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@InstallCode", SqlDbType.Text, install, ParameterDirection.Input, ref paralist);
            Database.BuildSqlParameter("@UninstallCode", SqlDbType.Text, uninstall, ParameterDirection.Input, ref paralist);

            int insertplug = Database.sqlInsert(query, paralist);
            if (insertplug <= 0)
            {
                return 100;
            }

            if (!string.IsNullOrEmpty(install))
            {
                int coustomq = Database.sqlCustomQuery(install);
                if (coustomq <= 0)
                {
                    return 200;
                }
            }

            return 1;
        }
    }
}