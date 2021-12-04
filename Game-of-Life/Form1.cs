using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[20, 20];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Cyan;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        // Neighbors stores the number of living cells for printing to a single cell graphicsPanel1_Paint
        int neighbors = 0;

        // Neighbor Count stores the number of living cells for the GOL Logic in NextGeneration
        int neighborCount = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // ScratchPad array used tostore the next generation and replace the current 2D 'universe'
            bool[,] scratchPad = new bool[universe.GetLength(0), universe.GetLength(1)];

            // Itterate through current universe generation.
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Get neighbor count per cell
                    if (finiteToolStripMenuItem.Enabled == true)
                    {
                        neighborCount = CountNeighborsFinite(x, y);
                    }
                    else if (toroidalToolStripMenuItem.Enabled == true)
                    {
                        neighborCount = CountNeighborsToroidal(x, y);
                    }

                    // Apply rules of GOL
                    // Turn cells on/off in second array
                    if (universe[x, y] == true && neighborCount < 2)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (universe[x, y] == true && neighborCount > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (universe[x, y] == true && neighborCount == 2 || neighborCount == 3)
                    {
                        scratchPad[x, y] = true;
                    }
                    else if (universe[x, y] == false && neighborCount == 3)
                    {
                        scratchPad[x, y] = true;
                    }
                }
            }

            // Swap second array with the original 'universe' array
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    
                    if (xOffset == 0 && yOffset == 0)    // if xOffset and yOffset are both equal to 0 then continue
                    {
                        continue;
                    }
                    else if (xCheck < 0)    // if xCheck is less than 0 then continue
                    {
                        continue;
                    }
                    else if (yCheck < 0)    // if yCheck is less than 0 then continue
                    {
                        continue;
                    }
                    else if (xCheck >= xLen)    // if xCheck is greater than or equal too xLen then continue
                    {
                        continue;
                    }
                    else if (yCheck >= yLen)    // if yCheck is greater than or equal too yLen then continue
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    if (xOffset == 0 && yOffset == 0)    // if xOffset and yOffset are both equal to 0 then continue
                    {
                        continue;
                    }
                    if (xCheck < 0)    // if xCheck is less than 0 then set to xLen - 1
                    {
                        xCheck = xLen - 1;
                    }
                    if (yCheck < 0)    // if yCheck is less than 0 then set to yLen - 1
                    {
                        yCheck = yLen - 1;
                    }
                    if (xCheck >= xLen)    // if xCheck is greater than or equal too xLen then set to 0
                    {
                        xCheck = 0;
                    }
                    if (yCheck >= yLen)    // if yCheck is greater than or equal too yLen then set to 0
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Font for displaying info in cells (font type, font size)
            Font font = new Font("Arial", 12f);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = Rectangle.Empty;
                    //Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    if (finiteToolStripMenuItem.Enabled == true)
                    {
                        neighbors = CountNeighborsFinite(x, y);
                    }
                    else if (toroidalToolStripMenuItem.Enabled == true)
                    {
                        
                        neighbors = CountNeighborsToroidal(x, y);
                    }

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, cellRect, stringFormat);
                    }

                    if (universe[x,y] == false && neighbors > 0)
                    {
                        e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Black, cellRect, stringFormat);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                // DON'T FORGET TO CALL INVALIDATE!!!           <<<--------
                graphicsPanel1.Invalidate();
            }
        }

        // Allows the user to exit the program through File -> Exit Tool Strip
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Allows the user to exit the program through File -> Exit Tool Strip
            this.Close();
        }

        // Starts the Game of life on button click
        private void toolStripPlayButton_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        // Stops the Game of life on button click
        private void toolStripPauseButton_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        // Stops, then Displays the next generation
        private void toolStripOneGenFwd_Click(object sender, EventArgs e)
        {
            timer.Stop();
            NextGeneration();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }


        // The following methods turn on/off which terrain mode the program runs
        // The if statements need more work to properly turn off the opposite method automatically
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toroidalToolStripMenuItem.Enabled = true;
            if (finiteToolStripMenuItem.Enabled == true)
            {
                finiteToolStripMenuItem.Enabled = false;
                finiteToolStripMenuItem.Checked = false;
            }
        }

        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            finiteToolStripMenuItem.Enabled = true;
            if (toroidalToolStripMenuItem.Enabled == true)
            {
                toroidalToolStripMenuItem.Enabled = false;
                toroidalToolStripMenuItem.Checked = false;
            }
        }
    }
}
