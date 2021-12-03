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
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

        }

        // Calculate the next generation of cells
        private void NextGeneration() // Majority of work here      <<<------
        {
            //bool isAlive = false;
            int neighborCount = 0;
            bool[,] scratchPad = new bool[universe.GetLength(0), universe.GetLength(1)];
            // Itterate through current universe generation.
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    //if (terrainTypeToolStripMenuItem == finiteToolStripMenuItem)
                    //{
                    //    // Get neighbor count per cell
                    //    neighborCount = CountNeighborsFinite(x, y);
                    //}
                    //else if (terrainTypeToolStripMenuItem == teroidalToolStripMenuItem)
                    //{
                    //    neighborCount = CountNeighborsToroidal(x, y);
                    //}

                    //neighborCount = CountNeighborsFinite(x, y);
                    neighborCount = CountNeighborsToroidal(x, y);

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
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = scratchPad[x, y];
                }
            }



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

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
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
                // DON'T FORGET TO CALL ME!!!           <<<--------
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();       // Allows the user to exit the program through File -> Exit Tool Strip
        }

        private void toolStripPlayButton_Click(object sender, EventArgs e)
        {
            timer.Start();      // Starts the Game of life on button click
        }

        private void toolStripPauseButton_Click(object sender, EventArgs e)
        {
            timer.Stop();       // Stops the Game of life on button click
        }

        private void toolStripOneGenFwd_Click(object sender, EventArgs e)
        {
            NextGeneration();       // Displays the next generation
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
    }
}
