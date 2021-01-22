using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dragRace
{
    public partial class Form1 : Form
    {
        string gameState = "start";
        Random random = new Random();

        int acceleration;
        double speed = 0;
        int gear = 1;
        double engineRPM;
        int[] gearRatio = new int[6];

        bool clutchIn;
        bool clutchUsed;
        bool breakIn;
        bool throttleIn;
        bool shiftUp = false;
        bool shiftDown;
        
        bool gameStart = true;
        bool greenLight;
        bool falseStart;
        int i;
        int lightTiming;
        bool goodLaunch;

        double time;
        int distance = 400;
        double position;
        double distancePerFrame;

        int roadWidth = 100;
        int roadLength = 700;

        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush grayBrush = new SolidBrush(Color.DarkGray);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush darkRedBrush = new SolidBrush(Color.DarkRed);
        SolidBrush greenBrush = new SolidBrush(Color.LimeGreen);
        SolidBrush darkGreenBrush = new SolidBrush(Color.DarkOliveGreen);
        SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
        SolidBrush darkYellowBrush = new SolidBrush(Color.DarkKhaki);
        Font smallFont = new Font("MS PGothic", 12);
        Font bigFont = new Font("MS PGothic", 22);

        public Form1()
        {
            InitializeComponent();
            gearRatio[1] = 120;
            gearRatio[2] = 80;
            gearRatio[3] = 60;
            gearRatio[4] = 40;
            gearRatio[5] = 30;
            lightTiming = random.Next(30, 100);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Z:
                    clutchIn = true;
                    break;
                case Keys.X:
                    breakIn = true;
                    break;
                case Keys.C:
                    throttleIn = true;
                    break;
                case Keys.Space:
                    if (gameState == "start" || gameState == "end")
                    {
                        gameState = "running";
                        gameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "start" || gameState == "end")
                    {
                        Application.Exit();
                    }
                    break;
            }
                
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Z:
                    clutchIn = false;
                    break;
                case Keys.X:
                    breakIn = false;
                    break;
                case Keys.C:
                    throttleIn = false;
                    break;
                case Keys.K:
                    shiftUp = true;
                    break;
                case Keys.M:
                    shiftDown = true;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            acceleration = 100 / gear;
            if(goodLaunch == true && gear == 1)
            {
                acceleration += 5;
            }
            if (shiftUp == true && clutchIn == true && gear <= 4)
            {
                gear++;
                shiftUp = false;
            }
            else
            {
                shiftUp = false;
            }
            if (shiftDown == true && clutchIn == true && gear >= 2)
            {
                gear--;
                shiftDown = false;
            }
            else
            {
                shiftDown = false;
            }
            
            if (clutchIn == true && throttleIn == true && engineRPM <= 8000)
            {
                engineRPM += 125;
                clutchUsed = true;
            }
            else if (clutchIn == true && engineRPM >= 900)
            {
                engineRPM -= 125;
                clutchUsed = true;
            }

            if (clutchUsed == true && clutchIn == false)
            {
                if(speed == 0 && gear == 1 && engineRPM < 6000 && engineRPM > 4000)
                {
                    goodLaunch = true;
                }

                engineRPM = speed * gearRatio[gear];
                clutchUsed = false;
            }
            
            if(clutchIn == false)
            {
                speed = engineRPM / gearRatio[gear];
            }
            if (clutchIn == false && throttleIn == true && engineRPM <= 8000)
            {
                engineRPM += acceleration;
            }
            else if (clutchIn == false && engineRPM >= 900)
            {
                engineRPM -= acceleration;
            }

            if(breakIn == true)
            {
                speed -= 2;
            }
            if(speed < 0)
            {
                speed = 0;
            }

            if(gameStart == true)
            {
                if(greenLight == true)
                {
                    gameStart = false;
                }
                else if(speed > 0)
                {
                    falseStart = true;
                    gameState = "end";
                    timer1.Enabled = false;
                }
                else
                {
                    i++;
                }
                if(i >= lightTiming + 40)
                {
                    greenLight = true;
                }
            }

            distancePerFrame = speed / 2.77 / 50;
            position += distancePerFrame;
            if (position >= distance)
            {
                gameState = "end";
                timer1.Enabled = false;
            }
     
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(grayBrush, 50, 100, roadLength, roadWidth);
            e.Graphics.FillRectangle(grayBrush, 50, 250, roadLength, roadWidth);
            e.Graphics.DrawString($"{gear}\n{engineRPM.ToString("0000")} rpm\n{speed.ToString("00.0")} kph\n{position.ToString("000.00")}m", smallFont, whiteBrush, 10, 10);
            if (gameState == "running")
            {
                if (greenLight == true)
                {
                    e.Graphics.FillEllipse(greenBrush, 100, 210, 30, 30);
                }
                else
                {
                    e.Graphics.FillEllipse(darkGreenBrush, 100, 210, 30, 30);
                }
                if (i >= lightTiming + 30)
                {
                    e.Graphics.FillEllipse(yellowBrush, 150, 210, 30, 30);
                }
                else
                {
                    e.Graphics.FillEllipse(darkYellowBrush, 150, 210, 30, 30);
                }
                if (i >= lightTiming + 20)
                {
                    e.Graphics.FillEllipse(yellowBrush, 200, 210, 30, 30);
                }
                else
                {
                    e.Graphics.FillEllipse(darkYellowBrush, 200, 210, 30, 30);
                }
                if (i >= lightTiming + 10)
                {
                    e.Graphics.FillEllipse(yellowBrush, 250, 210, 30, 30);
                }
                else
                {
                    e.Graphics.FillEllipse(darkYellowBrush, 250, 210, 30, 30);
                }
                e.Graphics.FillEllipse(yellowBrush, 300, 210, 30, 30);
                if (falseStart == true)
                {
                    e.Graphics.FillEllipse(redBrush, 50, 210, 30, 30);
                }
                else
                {
                    e.Graphics.FillEllipse(darkRedBrush, 50, 210, 30, 30);
                }
            }
            



            if (gameState == "start")
            {
                e.Graphics.DrawString("Drag Race", bigFont, whiteBrush, 330, 150);
                e.Graphics.DrawString("press space to start or esc to exit", smallFont, whiteBrush, 270, 200);
            }
            if (gameState == "end")
            {
                if (falseStart == true)
                {
                    e.Graphics.FillEllipse(redBrush, 50, 210, 30, 30);
                }
                e.Graphics.DrawString("Game Over", bigFont, whiteBrush, 330, 150);
                e.Graphics.DrawString("press space to start or esc to exit", smallFont, whiteBrush, 270, 200);
            }
           
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }
        public void gameInitialize()
        {
            timer1.Enabled = true;
        }
    }
}
