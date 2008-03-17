using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace MoonView.Thumbnail
{
    public class ThumbnailItemComparer : IComparer
    {
        // ListViewItemComparer Class members
        private int column;
        private string cDataType;
        private SortOrder sortorder = SortOrder.Ascending;

        //property to get/set the column index for comparison
        public int Column
        {
            get { return column; }
            set { column = value; }
        }


        //property to get/set the datatype of the column concerned
        public string ColumnDataType
        {
            get { return cDataType; }
            set { cDataType = value; }
        }

        //property to get/set the Sortorder for comparison
        public SortOrder SortOrder
        {
            get { return sortorder; }
            set { sortorder = value; }
        }

        //ListViewItemComparer Constructor
        public ThumbnailItemComparer()
        {
        }

        //Comparison Logic
        public int Compare(object x, object y)
        {
            //Convert the arguments to ListViewItem Objects
            ListViewItem itemX = (ListViewItem)x;
            ListViewItem itemY = (ListViewItem)y;

            int result;

            //handle logic for possible null values
            if (itemX == null && itemY == null) return 0;
            else if (itemX == null) result = -1;
            else if (itemY == null) result = 1;


            switch (cDataType)
            {
                case "Numeric":
                    //Convert column text to numbers before comparing.
                    decimal itemXVal, itemYVal;
                    itemXVal = Decimal.Parse(itemX.SubItems[column].Text);
                    itemYVal = Decimal.Parse(itemY.SubItems[column].Text);
                    result = Decimal.Compare(itemXVal, itemYVal);

                    break;

                case "DateTime":
                    //Convert column text to DateTime before comparing.
                    DateTime itemXDateTime, itemYDateTime;
                    if (itemX.SubItems[column].Text.Length == 0)
                        result = -1;
                    else
                    {
                        try
                        {
                            itemXDateTime = DateTime.Parse(itemX.SubItems[column].Text);
                            itemYDateTime = DateTime.Parse(itemY.SubItems[column].Text);
                            result = DateTime.Compare(itemXDateTime, itemYDateTime);
                        }
                        catch (Exception)
                        {
                            result = String.Compare(itemX.SubItems[column].Text, itemY.SubItems[column].Text);
                        }
                    }
                    break;
                default:
                    //String comparison
                    string itemXText = itemX.SubItems[column].Text;
                    string itemYText = itemY.SubItems[column].Text;

                    if (itemXText.Contains("Folder"))
                        result = -1;
                    else
                        result = string.Compare(itemXText, itemYText);
                    break;
            }

            if (sortorder == SortOrder.Ascending)
                return result;
            else
                return -result;
        }
    }
}
