using System;
using System.Timers;
using System.Drawing;
using Microsoft.AspNet.SignalR;
using System.Web;

namespace SignalRChat
{
    public class GameOfLife : Hub
    {
        private static readonly int NUM_CELLS = 30;
        private static readonly string DEAD = "#FFFFFF";
        private static string[,] cells;
        private static Timer timer;
        private static ElapsedEventHandler curHandler;

        static GameOfLife()
        {
            timer = new Timer(1000);
            cells = new string[NUM_CELLS, NUM_CELLS];
            curHandler = null;
            erase();
        }

        public void play()
        {
            if (curHandler == null)
            {
                curHandler = new ElapsedEventHandler(step);
                timer.Elapsed += curHandler;
                timer.Enabled = true;
            }
        }

        public void pause()
        {
            stop();
        }

        public static void stop()
        {
            if (curHandler != null)
            {
                timer.Elapsed -= curHandler;
                curHandler = null;
                timer.Enabled = false;
            }
        }

        public void reset()
        {
            stop();
            erase();
            Clients.All.setCells(cells);
        }

        public static void erase()
        {
            for (int x = 0; x < NUM_CELLS; x++)
                for (int y = 0; y < NUM_CELLS; y++)
                    cells[x, y] = DEAD;
        }

        public void click(int x, int y, string color)
        {
            if (cells[x, y] != DEAD)
                color = DEAD;
            cells[x, y] = color;
            Clients.All.setCell(x, y, color);

        }

        private void step(Object source, System.Timers.ElapsedEventArgs e)
        {
            string[,] newGeneration = new string[NUM_CELLS, NUM_CELLS];

            // Checks if cell should be alive or dead.
            for (int x = 0; x < NUM_CELLS; x++)
                for (int y = 0; y < NUM_CELLS; y++)
                    newGeneration[x, y] = updateCell(x, y);

            cells = newGeneration;

            Clients.All.setCells(cells);
        }

        private static string updateCell(int x, int y)
        {
            int numNeighbors = 0;
            int r = 0;
            int g = 0;
            int b = 0;

            for (int nx = x - 1; nx <= x + 1; nx++)
            {
                if (nx < 0 || nx >= NUM_CELLS)
                    continue;
                for (int ny = y - 1; ny <= y + 1; ny++)
                {
                    if (ny < 0 || ny >= NUM_CELLS || (nx == 0 && ny == 0) || (nx == x && ny == y))
                        continue;
                    if (cells[nx, ny] != DEAD)
                    {
                        numNeighbors++;
                        Color c = ColorTranslator.FromHtml(cells[nx, ny]); //find a usable alternative for .NET Core
                        r += c.R;
                        g += c.G;
                        b += c.B;
                    }
                }
            }

            // Over population and under population cells will die.
            if ((numNeighbors < 2) || (numNeighbors > 3))
                return DEAD;
            // Living cells and dead cells with 2 neighbors stay the same
            else if (cells[x, y] != DEAD || numNeighbors == 2)
                return cells[x, y];
            // Dead cells with 3 neighbors come alive
            else
                return string.Format("#{0:X2}{1:X2}{2:X2}",
                    r / numNeighbors,
                    g / numNeighbors,
                    b / numNeighbors
                );
        }
    }
}