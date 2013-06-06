using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace HanoiTowerGUI
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      this.textBox1.KeyPress += new KeyPressEventHandler(this.textBox1_KeyPress);
      this.button1.Click += new EventHandler(this.button1_Click);
      this.richTextBox1.ReadOnly = true;
    }

    private int height;
    private List<string> steps = new List<string>();
    private List<Rectangle>[] stack = new List<Rectangle>[3];
    private bool isDrawing = false;

    private void solve(int n, char a, char b, char c)
    {
      if (n == 0) return;
      solve(n - 1, a, c, b);
      steps.Add("Move " + n + " from " + a + " to " + c + "\n");
      solve(n - 1, b, a, c);
    }

    private /* async */ void Draw() // async & await version is only for 2012
    {
      isDrawing = true;

      Pen myPen = new Pen(Color.Red);
      Graphics formGraphics = this.CreateGraphics();
      foreach (string str in steps)
      {
        this.textBox2.Text = str; // Show state

        formGraphics.Clear(Color.White); // Clear
        myPen.Color = Color.Red;

        formGraphics.DrawLine(myPen, 250, 500, 350, 500); // Bottom of A
        formGraphics.DrawLine(myPen, 400, 500, 500, 500); // Bottom of B
        formGraphics.DrawLine(myPen, 550, 500, 650, 500); // Bottom of C

        int[] top = new int[]{500, 500, 500};
        for (int i = 0; i < 3; i++ )
          foreach (Rectangle rect in stack[i])
            if (rect.Y < top[i]) top[i] = rect.Y; // Get the top position of every stack

        int locNow = str[12] - 'A', locTarget = str[17] - 'A';

        Rectangle topRect = new Rectangle();

        // Move a peg from now to target
        topRect = stack[locNow][stack[locNow].Count - 1];
        topRect.X -= (250 + locNow * 150);
        stack[locNow].RemoveAt(stack[locNow].Count - 1);
        topRect.X += (250 + locTarget * 150);
        topRect.Y = top[locTarget] - 23;
        stack[locTarget].Add(topRect);

        for (int i = 0; i < 3; i++)
          foreach (Rectangle rect in stack[i])
            formGraphics.DrawRectangle(myPen, rect);

        // await Task.Delay(300);
        Thread.Sleep(300);
      }
      myPen.Dispose();
      formGraphics.Dispose();
      isDrawing = false;
    }

    private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
      // Only allow users input digits
      if (!char.IsDigit(e.KeyChar)) e.Handled = true;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (isDrawing) // Still moving
      {
        MessageBox.Show("Not finished yet");
        return;
      }
      height = Convert.ToInt32(this.textBox1.Text);
      if (height > 9)
      {
        height = 9;
        this.textBox1.Text = "9";
        MessageBox.Show("Height <= 9");
      }
      steps.Clear();
      this.richTextBox1.Clear();
      solve(height, 'A', 'B', 'C');
      foreach (string str in steps)
        this.richTextBox1.AppendText(str);
      for (int i = 0; i < 3; i++)
        stack[i] = new List<Rectangle>();
      for (int i = 1; i <= height; i++)
        stack[0].Add(new Rectangle(250 + 5 * i, 500 - 23 * i, 100 - 10 * i, 20));

      Form.CheckForIllegalCrossThreadCalls = false;
      Thread drawThread = new Thread(Draw); // New thread for animation
      drawThread.Start();
    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }
  }
}
