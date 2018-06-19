﻿using System;
using System.Drawing;
using System.Timers;

public class Conways_Game_Of_Life
{
    private static readonly int NUM_CELLS = 16;
    private static readonly string DEAD = "#FFFFFF";
    private static string[,] cells;
    private static Timer timer;

    static Conways_Game_Of_Life()
    {
        timer = new Timer(1000);
        timer.Elapsed += new ElapsedEventHandler(step);
		cells = new string[NUM_CELLS, NUM_CELLS];
        reset();
    }

    public static void play()
    {
        timer.Enabled = true;
    }

    public static void pause()
    {
        timer.Enabled = false;
    }

    public static void reset()
    {
        pause();
        for (int x = 1; x < NUM_CELLS; x++)
        for (int y = 1; y < NUM_CELLS; y++)
            cells[x, y] = DEAD;
    }

	private static void step(Object source, System.Timers.ElapsedEventArgs e)
	{
		string[,] newGeneration = new string[NUM_CELLS, NUM_CELLS];

		// Checks if cell should be alive or dead.
		for (int x = 1; x < NUM_CELLS; x++)
		for (int y = 1; y < NUM_CELLS; y++)
			newGeneration[x, y] = updateCell(x, y);

        cells = newGeneration;
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
                if (ny < 0 || ny >= NUM_CELLS || (nx == 0 && ny == 0))
                    continue;
                if (cells[nx, ny] != DEAD)
                {
                    numNeighbors++;
					r += Convert.ToInt32(cells[nx, ny].Substring(1, 2), 16);
					//Color c = System.Drawing.ColorTranslator.FromHtml(cells[nx, ny]); //find a usable alternative for .NET Core
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
                r/numNeighbors,
                g/numNeighbors,
                b/numNeighbors
            );
    }
}