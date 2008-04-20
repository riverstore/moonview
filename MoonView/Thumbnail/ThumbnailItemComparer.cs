using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace MoonView.Thumbnail
{
    public class ThumbnailItemComparer : IComparer
    {
        // ListViewItemComparer Class members
        private int _column;
        private string _cDataType;
        private SortOrder _sortorder = SortOrder.Ascending;

        //property to get/set the column index for comparison
        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }


        //property to get/set the datatype of the column concerned
        public string ColumnDataType
        {
            get { return _cDataType; }
            set { _cDataType = value; }
        }

        //property to get/set the Sortorder for comparison
        public SortOrder SortOrder
        {
            get { return _sortorder; }
            set { _sortorder = value; }
        }

        //ListViewItemComparer Constructor
        public ThumbnailItemComparer()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Integer: Less than zero if x is less than y,
        ///                   Zero if x is equal to y, 
        ///                   and Greater than zero if x is greater than y</returns>
        public int Compare(object x, object y)
        {
            //Convert the arguments to ListViewItem Objects
            ListViewItem itemX = (ListViewItem)x;
            ListViewItem itemY = (ListViewItem)y;

            int result;

            //Parent directory
            if (itemX.SubItems[0].Text.Equals(".."))
                return -1;
            if (itemY.SubItems[0].Text.Equals(".."))
                return 1;

            //Folder compare to File
            bool isXFolder = (itemX.SubItems[2].Text.IndexOf("Folder") > -1);
            bool isYFolder = (itemY.SubItems[2].Text.IndexOf("Folder") > -1);
            if (isXFolder && !isYFolder)
                return -1;
            if (!isXFolder && isYFolder)
                return 1;

            //handle logic for possible null values
            if (itemX == null && itemY == null)
                return 0;
            else if (itemX == null)
                return ApplySortOrder(-1);
            else if (itemY == null)
                return ApplySortOrder(1);

            //handle empty string
            int xLen = itemX.SubItems[_column].Text.Length;
            int yLen = itemY.SubItems[_column].Text.Length;
            if (xLen == 0 && yLen == 0)
                return 0;
            else if (xLen > 0 && yLen == 0)
                return ApplySortOrder(1);
            else if (xLen == 0 && yLen > 0)
                return ApplySortOrder(-1);

            switch (_cDataType)
            {
                case "Numeric":
                    result = CompareNumeric(itemX, itemY);
                    break;
                case "DateTime":
                    result = CompareDateTime(itemX, itemY);
                    break;
                default:
                    //String
                    if (_column == 2 && itemX.SubItems[2].Text.Equals(itemY.SubItems[2].Text))
                        result = CompareStringXp(itemX, itemY, 0);
                    else
                        result = CompareStringXp(itemX, itemY, _column);
                    break;
            }
            return ApplySortOrder(result);
        }

        int ApplySortOrder(int result)
        {
            if (_sortorder == SortOrder.Ascending)
                return result;
            else
                return -result;
        }

        /// <summary>
        /// Compare string with respect to number in filename 
        /// E.g. file11.jpg will be greater than file2.jpg
        /// </summary>
        /// <param name="itemX"></param>
        /// <param name="itemY"></param>
        /// <returns></returns>
        int CompareStringXp(ListViewItem itemX, ListViewItem itemY, int column)
        {
            string strX = itemX.SubItems[column].Text;
            string strY = itemY.SubItems[column].Text;
            return Utility.CompareStringXp(strX, strY);
        }

        int CompareNumeric(ListViewItem itemX, ListViewItem itemY)
        {
            //Convert column text to numbers before comparing.
            decimal itemXVal, itemYVal;
            itemXVal = Decimal.Parse(itemX.SubItems[_column].Text);
            itemYVal = Decimal.Parse(itemY.SubItems[_column].Text);
            return Decimal.Compare(itemXVal, itemYVal);
        }

        int CompareDateTime(ListViewItem itemX, ListViewItem itemY)
        {
            //Convert column text to DateTime before comparing.
            DateTime dtX, dtY;
            try
            {
                dtX = DateTime.Parse(itemX.SubItems[_column].Text);
                dtY = DateTime.Parse(itemY.SubItems[_column].Text);
                return DateTime.Compare(dtX, dtY);
            }
            catch (Exception)
            {
                return string.Compare(itemX.SubItems[_column].Text, itemY.SubItems[_column].Text);
            }
        }
    }
}
