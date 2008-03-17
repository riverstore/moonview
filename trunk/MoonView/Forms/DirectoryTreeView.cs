//System NS
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

//MoonView NS
using MoonView.Path;

namespace MoonView.Forms
{
    public class DirectoryTreeView : TreeView
    {
        //Constants
        const int DISK_ICON = 0;
        const int FOLDER_ICON = 1;
        const int ARCH_ICON = 1;

        //Object
        MoonViewForm _parent;

        public new MoonViewForm Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public DirectoryTreeView()
            : base()
        {
            //this.AfterSelect += new TreeViewEventHandler(DirectoryTreeView_AfterSelect);
            //this.KeyDown += new KeyEventHandler(DirectoryTreeView_KeyDown);
            this.DoubleClick += new EventHandler(DirectoryTreeView_DoubleClick);
        }

        public void Initialise(MoonViewForm parent)
        {
            _parent = parent;

            this.ImageList = new ImageList();
            this.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            this.ImageList.Images.Add(Utility.DiskIconSmall);
            this.ImageList.Images.Add(Utility.FolderIconSmall);

            PopulateBaseNode();
        }

        void DirectoryTreeView_DoubleClick(object sender, EventArgs e)
        {
            if (this.SelectedNode == null)
                 return;
             string path = "";
             TreeNode currNode = this.SelectedNode;
             while (currNode != null)
             {
                 path = System.IO.Path.Combine(currNode.Name, path);
                 currNode = currNode.Parent;
             }
             _parent.Open(this, path);
        }

        void PopulateBaseNode()
        {
            foreach (string drive in Directory.GetLogicalDrives())
                this.Nodes.Add(drive, drive, DISK_ICON, DISK_ICON);
        }

        public void PopulateNodes(IDirectoryInfo dirInfo)
        {
            List<string> pathList = new List<string>();
            IDirectoryInfo currDirInfo = dirInfo;
            while (true)
            {
                pathList.Insert(0, currDirInfo.Name);
                Console.WriteLine(currDirInfo.Name);
                currDirInfo = currDirInfo.Parent;
                if (currDirInfo == null)
                    break;
            }

            int i = 0;
            TreeNode lastNode = ExpandNode(ref i, this.Nodes, pathList.ToArray());
            this.Update();
        }

        public TreeNode ExpandNode(ref int level, TreeNodeCollection nodes, string[] pathList)
        {
            TreeNode currNode;
            string name = pathList[level];
            if (nodes.ContainsKey(name))
                currNode = nodes[name];
            else
                currNode = nodes.Add(name, name, FOLDER_ICON, FOLDER_ICON);
            level++;
            if (!currNode.IsExpanded)
                currNode.Toggle();
            if (level >= pathList.Length)
                return currNode;
            return ExpandNode(ref level, currNode.Nodes, pathList);
        }
    }
}
