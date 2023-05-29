using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerrariaClassSwitcher
{
    public partial class MainScreen : UserControl
    {
        // Boss HP Variables
        Bitmap bmp = new Bitmap(456, 26);
        Rectangle bounds = new Rectangle(-1188, 1015, 456, 26);
        float bossHp = 0;
        float lastSavedBossHP = 0;
        bool isBossHealthBar = false;

        bool isPaused = true;
        float timeTillNextClass = 300000; // In Milliseconds
        int currentClass = 0; // 0 = Melee, 1 = Ranger, 2 = Mage, 3 = Summoner

        // Class stuff
        List<Bitmap> classBitmaps = new List<Bitmap>()
        {
            new Bitmap(Application.StartupPath + "/Images/Melee.png"),
            new Bitmap(Application.StartupPath + "/Images/Ranger.png"),
            new Bitmap(Application.StartupPath + "/Images/Mage.png"),
            new Bitmap(Application.StartupPath + "/Images/Summoner.png")
        };
        Bitmap arrowHead = new Bitmap(Application.StartupPath + "/Images/arrowhead.png");
        SolidBrush meleeBrush = new SolidBrush(Color.FromArgb(246, 131, 14));
        SolidBrush rangerBrush = new SolidBrush(Color.FromArgb(34, 221, 151));
        SolidBrush mageBrush = new SolidBrush(Color.FromArgb(132, 37, 228));
        SolidBrush summonerBrush = new SolidBrush(Color.FromArgb(22, 173, 254));

        SolidBrush fadedMeleeBrush = new SolidBrush(Color.FromArgb(246 / 3, 131 / 3, 14 / 3));
        SolidBrush fadedRangerBrush = new SolidBrush(Color.FromArgb(34 / 3, 221 / 3, 151 / 3));
        SolidBrush fadedMageBrush = new SolidBrush(Color.FromArgb(132 / 3, 37 / 3, 228 / 3));
        SolidBrush fadedSummonerBrush = new SolidBrush(Color.FromArgb(22 / 3, 173 / 3, 254 / 3));

        SoundPlayer classChangeSound = new SoundPlayer(Application.StartupPath + "/Images/Research_0.wav");

        StringFormat str = new();

        /** FPS RELATED VARIABLES **/
        int framesSinceLastSecond = 0;
        int accurateSec = -1;
        int lastSec = -1;
        int fps = 0;

        // Millisecond diff
        int lastMillis = 0;
        int lastSecond = 0;

        int getMillisSinceLast()
        {
            int theTime;
            if(DateTime.Now.Second < lastSecond)
            {
                theTime = 60000 + ((DateTime.Now.Second - lastSecond) * 1000) + (DateTime.Now.Millisecond - lastMillis);
            }
            else{
                if(DateTime.Now.Millisecond < lastMillis)
                {
                    theTime = 1000 + (DateTime.Now.Millisecond - lastMillis);
                }
                else
                {
                    theTime = (DateTime.Now.Millisecond - lastMillis);
                }
            }
            return theTime;
        }

        string numToClass(int num)
        {
            switch(num)
            {
                case 0:
                    return "Melee";
                case 1:
                    return "Ranger";
                case 2:
                    return "Mage";
                case 3:
                    return "Summoner";
            }
            return "";
        }

        public MainScreen()
        {
            InitializeComponent();

            lastMillis = DateTime.Now.Millisecond;
            lastSecond = DateTime.Now.Second;

            str.LineAlignment = StringAlignment.Center;
            str.Alignment = StringAlignment.Center;
        }

        // Function used to determine the current boss hp and whether or not a player is fighting a boss
        void DeterminePercent()
        {
            // Get current pixel and store it in a variable
            Color bmpPixel;
            Color bmpPixel2;

            // Check for boss bar "box"
            bmpPixel = bmp.GetPixel(226, 0);
            bmpPixel2 = bmp.GetPixel(142, 0);
            if (!(bmpPixel.R == 113 && bmpPixel.G == 114 && bmpPixel.B == 147) && !(bmpPixel2.R == 113 && bmpPixel2.G == 114 && bmpPixel2.B == 147))
            {
                if (isBossHealthBar)
                {
                    classChangeSound.Play();
                }
                isBossHealthBar = false;
                bossHp = 0;
                return;
            }
            bmpPixel = bmp.GetPixel(226, 24);
            bmpPixel2 = bmp.GetPixel(142, 24);
            if (!(bmpPixel.R == 159 && bmpPixel.G == 160 && bmpPixel.B == 183) && !(bmpPixel2.R == 113 && bmpPixel2.G == 114 && bmpPixel2.B == 147))
            {
                if (isBossHealthBar)
                {
                    classChangeSound.Play();
                }
                isBossHealthBar = false;
                bossHp = 0;
                return;
            }

            // If it is boss health bar then approximate the amount of hp left by going through the boss hp bar

            if (!isBossHealthBar)
            {
                classChangeSound.Play();
            }
            isBossHealthBar = true;
            bossHp = lastSavedBossHP + 0;
            for (int i = 0;i < 456;i += 1)
            {
                for(int j = 3;j < 20;j += 1)
                {
                    bmpPixel = bmp.GetPixel(i, j);
                    if (bmpPixel.R == 198 && bmpPixel.G == 91 && bmpPixel.B == 91)
                    {
                        bossHp = Convert.ToSingle(i+1) / 456f;
                        if(bossHp < 0.75f && lastSavedBossHP >= 0.75f)
                        {
                            classChangeSound.Play();
                        }
                        if (bossHp < 0.5f && lastSavedBossHP >= 0.5f)
                        {
                            classChangeSound.Play();
                        }
                        if (bossHp < 0.25f && lastSavedBossHP >= 0.25f)
                        {
                            classChangeSound.Play();
                        }
                        lastSavedBossHP = bossHp + 0;
                        return;
                    }
                }
            }

            // If the boss bar is still there but we cannot tell exactly how much hp is left due to the text
            return;
        }

        private void updateTick_Tick(object sender, EventArgs e)
        {
            framesSinceLastSecond++;
            accurateSec = DateTime.Now.Second;
            if(lastSec != accurateSec)
            {
                lastSec = accurateSec;
                fps = framesSinceLastSecond;
                framesSinceLastSecond = 0;
            }
            if (!isPaused)
            {
                timeTillNextClass -= getMillisSinceLast();
            }
            if(timeTillNextClass <= 0)
            {
                if (!isBossHealthBar)
                {
                    classChangeSound.Play();
                }
                timeTillNextClass += 300000;
                currentClass++;
                if(currentClass > 3)
                {
                    currentClass = 0;
                }
            }
            lastMillis = DateTime.Now.Millisecond;
            lastSecond = DateTime.Now.Second;
            this.Refresh();
        }

        private void MainScreen_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            // Class visual
            if (isBossHealthBar)
            {
                // Stroke 
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(19, 19, 31)), Width / 2 - 85, Height - 100 - 70, 170, 70);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(19, 19, 31)), Width / 2 - 75, Height - 100 - 80, 150, 80);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(19, 19, 31)), Width / 2 - 85, Height - 180, 20, 20);
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(19, 19, 31)), Width / 2 + 65, Height - 180, 20, 20);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(19, 19, 31)), Width / 2 - 230 - 6, Height - 100 - 6, 472, 17);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(62, 61, 88)), Width / 2 - 230 - 4, Height - 100 - 4, 468, 13);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(113, 114, 147)), Width / 2 - 170, Height - 100 - 4, 340, 13);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(62, 61, 88)), Width / 2 - 160, Height - 100 - 4, 320, 13);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(113, 114, 147)), Width / 2 - 150, Height - 100 - 4, 300, 13);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(159, 160, 183)), Width / 2 - 90, Height - 100 - 4, 180, 13);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(113, 114, 147)), Width / 2 - 80, Height - 100 - 4, 160, 13);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(159, 160, 183)), Width / 2 - 75, Height - 100 - 4, 150, 13);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(19, 19, 31)), Width / 2 - 230 - 2, Height - 100 - 2, 464, 9);

                // Back Colors
                e.Graphics.FillRectangle(fadedMeleeBrush, Width / 2 - 230, Height - 100, 115, 5);
                e.Graphics.FillRectangle(fadedRangerBrush, Width / 2 - 115, Height - 100, 115, 5);
                e.Graphics.FillRectangle(fadedMageBrush, Width / 2, Height - 100, 115, 5);
                e.Graphics.FillRectangle(fadedSummonerBrush, Width / 2 + 115, Height - 100, 115, 5);

                // Front Colors
                e.Graphics.FillRectangle(meleeBrush, Width / 2 - 230, Height - 100, 115 * (bossHp) * 4, 5);
                if(bossHp > 0.25)
                {
                    e.Graphics.FillRectangle(rangerBrush, Width / 2 - 115, Height - 100, 115 * (bossHp - 0.25f) * 4, 5);
                }
                if(bossHp > 0.5)
                {
                    e.Graphics.FillRectangle(mageBrush, Width / 2, Height - 100, 115 * (bossHp - 0.5f) * 4, 5);
                }
                if(bossHp > 0.75)
                {
                    e.Graphics.FillRectangle(summonerBrush, Width / 2 + 115, Height - 100, 115 * (bossHp - 0.75f) * 4, 5);
                }
                //e.Graphics.FillRectangle(new SolidBrush(Color.White), Width / 2 - 230 + bossHp * 460 - 2, Height - 100 - 5, 4, 15);
                e.Graphics.DrawImage(classBitmaps[Convert.ToInt16(Math.Floor(bossHp * 4))], Width / 2 - 230 + bossHp * 460 - 15, Height - 100 - 15, 30, 30);

                e.Graphics.DrawString("Current Class", new Font("Andy", 15), new SolidBrush(Color.White), Width / 2, Height - 160, str);
                e.Graphics.DrawString(numToClass(Convert.ToInt16(Math.Floor(bossHp * 4))), new Font("Andy", 24), new SolidBrush(Color.White), Width / 2, Height - 135, str);
            }
            else
            {
                int wheelSize = 200;
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(15, 15, 30)), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize);

                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(19, 19, 31), 42), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize);
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(62, 61, 88), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(113, 114, 147), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, -70, 60);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(113, 114, 147), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 20, 40);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(113, 114, 147), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 80, 5);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(113, 114, 147), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 90, 15);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(113, 114, 147), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 120, 130);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(159, 160, 183), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, -50, 30);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(159, 160, 183), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 25, 30);
                e.Graphics.DrawArc(new Pen(Color.FromArgb(159, 160, 183), 36), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 140, 100);
                e.Graphics.DrawEllipse(new Pen(Color.FromArgb(19, 19, 31), 30), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize);
                float arcMove = ((300000 - timeTillNextClass) / 300000 + currentClass) * 90;
                e.Graphics.DrawArc(new Pen(meleeBrush, 24), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, -90 - arcMove, 91);
                e.Graphics.DrawArc(new Pen(rangerBrush, 24), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 0 - arcMove, 91);
                e.Graphics.DrawArc(new Pen(mageBrush, 24), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 90 - arcMove, 91);
                e.Graphics.DrawArc(new Pen(summonerBrush, 24), Width / 2 - wheelSize / 2, Height / 2, wheelSize, wheelSize, 180 - arcMove, 91);

                e.Graphics.DrawImage(classBitmaps[0], Width / 2 - 15 + MathF.Sin((arcMove - 180) / (180 / MathF.PI)) * wheelSize / 2, Height / 2 + wheelSize / 2 + MathF.Cos((arcMove - 180) / (180 / MathF.PI)) * wheelSize / 2 - 15, 30, 30);
                e.Graphics.DrawImage(classBitmaps[1], Width / 2 - 15 + MathF.Sin((arcMove + 90) / (180 / MathF.PI)) * wheelSize / 2, Height / 2 + wheelSize / 2 + MathF.Cos((arcMove + 90) / (180 / MathF.PI)) * wheelSize / 2 - 15, 30, 30);
                e.Graphics.DrawImage(classBitmaps[2], Width / 2 - 15 + MathF.Sin((arcMove) / (180 / MathF.PI)) * wheelSize / 2, Height / 2 + wheelSize / 2 + MathF.Cos((arcMove) / (180 / MathF.PI)) * wheelSize / 2 - 15, 30, 30);
                e.Graphics.DrawImage(classBitmaps[3], Width / 2 - 15 + MathF.Sin((arcMove - 90) / (180 / MathF.PI)) * wheelSize / 2, Height / 2 + wheelSize / 2 + MathF.Cos((arcMove - 90) / (180 / MathF.PI)) * wheelSize / 2 - 15, 30, 30);

                e.Graphics.DrawImage(arrowHead, Width / 2 - 9, Height / 2 - 30, 18, 62);


                e.Graphics.DrawString("Current Class", new Font("Andy", 15), new SolidBrush(Color.White), Width / 2, Height / 2 + wheelSize / 2 - 40, str);
                e.Graphics.DrawString(numToClass(currentClass), new Font("Andy", 24), new SolidBrush(Color.White), Width / 2, Height / 2 + wheelSize / 2 - 15, str);
                e.Graphics.DrawString("Time Remaining", new Font("Andy", 15), new SolidBrush(Color.White), Width / 2, Height / 2 + wheelSize / 2 + 10, str);
                int mins = Convert.ToInt16(Math.Floor(timeTillNextClass / 1000 / 60));
                int secs = Convert.ToInt16(Math.Floor((timeTillNextClass / 1000) % 60));
                string secPlus = secs < 10 ? "0" : "";
                e.Graphics.DrawString($"{mins}:{secPlus}{secs}", new Font("Andy", 28), new SolidBrush(Color.White), Width / 2, Height / 2 + wheelSize / 2 + 40, str);

                if (isPaused)
                {
                    e.Graphics.DrawString($"Timer Paused", new Font("Andy", 36), new SolidBrush(Color.White), Width / 2, Height / 2 - 50, str);
                }
            }

            // Testing
            using (Graphics g = Graphics.FromImage(bmp))
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

            //e.Graphics.DrawImage(bmp, Width / 2 - 230, Height / 2 - 13);
            DeterminePercent();
            //e.Graphics.DrawString($"Boss HP: {bossHp*100f}%\nIsBossHPBar:{isBossHealthBar}\nTime Left: {timeTillNextClass}\nFPS: {fps}", DefaultFont, new SolidBrush(Color.Black), 10, 10);

        }

        private void MainScreen_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                isPaused = !isPaused;
            }
            if (isPaused)
            {
                if (e.KeyCode == Keys.D1)
                {
                    timeTillNextClass -= 1000;
                }
                if (e.KeyCode == Keys.D2)
                {
                    timeTillNextClass -= 5000;
                }
                if (e.KeyCode == Keys.D3)
                {
                    timeTillNextClass -= 10000;
                }
                if (e.KeyCode == Keys.D4)
                {
                    timeTillNextClass -= 60000;
                }
                if (e.KeyCode == Keys.D5)
                {
                    if (!isBossHealthBar)
                    {
                        classChangeSound.Play();
                    }
                    timeTillNextClass = 300000;
                    currentClass++;
                    if (currentClass > 3)
                    {
                        currentClass = 0;
                    }
                }
            }
        }
    }
}
