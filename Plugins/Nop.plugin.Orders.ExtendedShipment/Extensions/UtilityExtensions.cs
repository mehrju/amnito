using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Nop.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Extensions
{
	public static class UtilityExtensions
	{
		public static string Ext_ToFilename(this DateTime d1)
		{
			return d1.Year + "_" + d1.Month + "_" + d1.Day + "-" + d1.Hour + "_" + d1.Minute + "_" + d1.Second;
		}
        /// <summary>
        /// Export objects to XLSX
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="properties">Class access to the object through its properties</param>
        /// <param name="itemsToExport">The objects to export</param>
        /// <returns></returns>
        public static byte[] ExportToXlsx<T>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport)
        {
            using (var stream = new MemoryStream())
            {
                
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handles to the worksheets
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    var fWorksheet = xlPackage.Workbook.Worksheets.Add("DataForFilters");
                    fWorksheet.Hidden = eWorkSheetHidden.VeryHidden;
                    fWorksheet.View.RightToLeft = true;
                    fWorksheet.Cells["A1:K20"].AutoFitColumns();
                    //create Headers and format them 
                    var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore));
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        manager.CurrentObject = items;
                        manager.WriteToXlsx(worksheet, row++,false , fWorksheet: fWorksheet);
                    }

                    xlPackage.Save();
                }

                return stream.ToArray();
            }
        }
        static void SetCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }

        public static System.Data.DataTable Ext_ArrayToDataTable<T>(this List<T> list, bool headerQ = true)
		{
			//if (array == null || array.GetLength(1) == 0 || array.GetLength(0) == 0) return null;
			System.Data.DataTable table = new System.Data.DataTable();
			foreach(var p in typeof(T).GetProperties())
			{
				table.Columns.Add(p.Name);
			}

			for (int i = 0; i < list.Count; i++)
			{
				List<string> cells = new List<string>();
				foreach (var pr in list[i].GetType().GetProperties()) {
					cells.Add(pr.GetValue(list[i])?.ToString());
				}
				table.Rows.Add(cells.ToArray());
			}
			return table;
		}
		
        public static string Ext_ToJson(this System.Data.DataTable dt)
		{
			List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();
			Dictionary<string, object> item;
			foreach (DataRow row in dt.Rows)
			{
				item = new Dictionary<string, object>();
				foreach (DataColumn col in dt.Columns)
				{
					item.Add(col.ColumnName, (Convert.IsDBNull(row[col]) ? null : row[col]));
				}
				lst.Add(item);
			}
			return Newtonsoft.Json.JsonConvert.SerializeObject(lst);
		}
	}
}
