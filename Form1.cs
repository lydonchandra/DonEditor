using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Diagnostics;

namespace DonEditor
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
#if DEBUG
            pictureBox1.Image = new Bitmap("../../don.tif");
#endif
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Graphics g = e.Graphics;
            //g.DrawImage(image,  ClientRectangle);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
  
            OpenFileDialog dialog = new OpenFileDialog(); 
            dialog.Filter = "tif files (*.tif)|*.tif|All files (*.*)|*.*"; 
            dialog.InitialDirectory = "."; 
            dialog.Title = "Select a text file"; 
            if ( dialog.ShowDialog() == DialogResult.OK ) {
                System.GC.Collect();
                pictureBox1.Image = new Bitmap(dialog.FileName);
                this.Invalidate();
            }

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void Form1_MouseHover(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.Cross;
        }

        public Point startSelection = Point.Empty;
        public Point endSelection = Point.Empty;
        public Boolean isDrag = false;

        

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Cross;

            if (startSelection != Point.Empty &&
                endSelection != Point.Empty)
            {                
                // undraw whatever previously drawn
                DrawSelection(pictureBox1);
            }

            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                endSelection = Point.Empty;
            }

            startSelection = new Point(e.X, e.Y);                
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                if (startSelection != Point.Empty &&
                    endSelection != Point.Empty)
                {                    
                    // undraw
                    DrawSelection(pictureBox1);
                }
                endSelection = new Point(e.X, e.Y);

                DrawSelection(pictureBox1);


            }            
        }

        public Rectangle DrawSelection(PictureBox pictureBox)
        {
            Size rectangleSize = new Size(endSelection.X - startSelection.X,
                                          endSelection.Y - startSelection.Y);
            
            Rectangle rect = new Rectangle(startSelection, rectangleSize);
            
            ControlPaint.DrawReversibleFrame( pictureBox.RectangleToScreen(rect), Color.Red, FrameStyle.Thick);
            
            return rect;
        }



        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                //DrawSelection(pictureBox1);                
                isDrag = false;
            }            
        }

        private void mosaicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select directory that has your images";

#if DEBUG
            mosaicImages(@"d:\pics");
#endif

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Image bigImage = mosaicImages(folderBrowserDialog.SelectedPath);

#if DEBUG                
                bigImage.Save(@"c:\temp\bigImage.jpg");
                return;
#endif
                bigImage.Save(folderBrowserDialog.SelectedPath+"\\..\\bigImage.jpg");
            
            }

        }

        private Image mosaicImages(String folderPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            
            int WIDTH = 20000;
            int HEIGHT = WIDTH;

            Image bigImage = new Bitmap( WIDTH, HEIGHT);
            Graphics graphics = Graphics.FromImage(bigImage);

            float x = 0.0F;
            float y = 0.0F;

            int maxHeight = 0;
            
            int columnWidth = 0;
            foreach ( FileInfo fileInfo in fileInfos ) {
                Debug.WriteLine(fileInfo.FullName);
                try
                {
                    Image image = Image.FromFile(fileInfo.FullName);
                    graphics.DrawImage(image, x, y);
                    
                    columnWidth += image.Width;
                    int tempWidth = WIDTH - columnWidth;

                    if (tempWidth < 0)
                    {
                        y = y + image.Height;
                        x = 0;
                        tempWidth = WIDTH;
                        columnWidth = 0;
                    }
                    else
                    {
                        x = x + image.Width;
                    }                    
                    

                } catch(Exception) {

                }
            }

            return bigImage;
        }
        
    }
}
