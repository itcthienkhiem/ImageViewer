using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;
using System.Data;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.ShelfComponentTools.PrintTool.WinForms
{
    public sealed class GridViewSettings : ApplicationSettingsBase
    {
        // Fields
        private XPathNavigator _nav;
        private static GridViewSettings defaultInstance = ((GridViewSettings)SettingsBase.Synchronized(new GridViewSettings()));

        // Methods
        private Type ConvertToType(string valueType)
        {
            valueType = valueType.ToLower();
            switch (valueType)
            {
                case "string":
                    return typeof(string);

                case "datetime":
                    return typeof(DateTime);

                case "int":
                    return typeof(int);

                case "long":
                    return typeof(long);

                case "bool":
                    return typeof(bool);
            }
            return typeof(string);
        }

        public DataTable GetComboBoxValue(string comboBoxName)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Display");
            table.Columns.Add("Value");
            XPathNodeIterator iterator = this.Nav.Select("/CreateColumn/ComboBoxName[@name='" + comboBoxName + "']");
            while (iterator.MoveNext())
            {
                XPathNodeIterator iterator2 = iterator.Current.SelectDescendants("Row", "", false);
                while (iterator2.MoveNext())
                {
                    DataRow row = table.NewRow();
                    row["Display"] = iterator2.Current.GetAttribute("DisplayMember", "");
                    row["Value"] = iterator2.Current.GetAttribute("ValueMember", "");
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        public List<DataGridViewColumn> GetViewColumns(string tableName)
        {
            List<DataGridViewColumn> list = new List<DataGridViewColumn>();
            XPathNodeIterator iterator = this.Nav.Select("/CreateColumn/tableName[@name='" + tableName + "']");
            while (iterator.MoveNext())
            {
                XPathNodeIterator iterator2 = iterator.Current.SelectDescendants("column", "", false);
                while (iterator2.MoveNext())
                {
                    DataGridViewColumn column;
                    if (iterator2.Current.GetAttribute("Type", "") == "bool")
                    {
                        column = new DataGridViewCheckBoxColumn();
                    }
                    else
                    {
                        column = new DataGridViewTextBoxColumn();
                    }
                    column.Name = column.DataPropertyName = iterator2.Current.GetAttribute("DataName", "");
                    column.HeaderText = iterator2.Current.GetAttribute("DisplayName", "");
                    column.ReadOnly = Convert.ToBoolean(iterator2.Current.GetAttribute("ReadOnly", ""));
                    column.Visible = Convert.ToBoolean(iterator2.Current.GetAttribute("Visible", ""));
                    column.ValueType = this.ConvertToType(iterator2.Current.GetAttribute("Type", ""));
                    list.Add(column);
                }
            }
            return list;
        }

        // Properties
        public static GridViewSettings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        //[UserScopedSetting, DefaultSettingValue("True"), DebuggerNonUserCode]
        [UserScopedSetting, DefaultSettingValue("True")]
        public bool LockShelf
        {
            get
            {
                return (bool)this["LockShelf"];
            }
            set
            {
                this["LockShelf"] = value;
            }
        }

        private XPathNavigator Nav
        {
            get
            {
                if (this._nav == null)
                {
                    this._nav = new XPathDocument(string.Format("{0}{1}", Platform.InstallDirectory, this.SettingFileName)).CreateNavigator();
                }
                return this._nav;
            }
        }

        //[UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("DisplayConfig.xml")]
        [UserScopedSetting,  DefaultSettingValue("DisplayConfig.xml")]
        public string SettingFileName
        {
            get
            {
                return (string)this["SettingFileName"];
            }
            set
            {
                this["SettingFileName"] = value;
            }
        }
    }

}
