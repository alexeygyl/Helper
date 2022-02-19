using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Helper
{
    class Analyzer
    {

        private static List<string> partyColors = new List<string>();

        public static List<Types.Stats> UpdatePartyInfo(ref Bitmap bmp)
        {
            Color pixel = new Color();
            int sX = -1, sY = -1;
            int eX = -1, eY = -1;

            Rectangle rectangle = new Rectangle();
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = 0;
            rectangle.Width = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 100; y < bmp.Height; y++)
                {
                    pixel = bmp.GetPixel(x, y);
                    if (pixel.R == 201 && pixel.G == 192 && pixel.B == 177)
                    {
                        if (sY == -1)
                        {
                            sX = x;
                            sY = y;
                        }
                        else
                        {
                            eY = y;
                            break;
                        }
                    }
                }

                if (eY >= 0)
                {
                    for (eX = sX + 170; eX < bmp.Width; eX++)
                    {
                        pixel = bmp.GetPixel(eX, sY + 3);
                        if (pixel.R == 132 && pixel.G == 122 && pixel.B == 101)
                        {
                            rectangle.X = sX;
                            rectangle.Y = sY;
                            rectangle.Height = eY - sY;
                            rectangle.Width = eX - sX;
                            return UpdatePartyStats(ref rectangle, ref bmp);
                        }
                    }
                }

            }

            return new List<Types.Stats>();
        }

        public static int UpdatePetHp(ref Bitmap bmp)
        {
            Color pixel = new Color();
            int sX = -1, sY = -1;
            int eX = -1, eY = -1;

            Rectangle rectangle = new Rectangle();
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = 0;
            rectangle.Width = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 500; y < bmp.Height; y++)
                {
                    pixel = bmp.GetPixel(x, y);
                    if (pixel.R == 201 && pixel.G == 192 && pixel.B == 177)
                    {
                        if (sY == -1)
                        {
                            sX = x;
                            sY = y;
                        }
                        else
                        {
                            eY = y;
                            break;
                        }
                    }
                }

                if (eY >= 0)
                {
                    for (eX = sX + 170; eX < bmp.Width; eX++)
                    {
                        pixel = bmp.GetPixel(eX, sY + 3);
                        if (pixel.R == 132 && pixel.G == 122 && pixel.B == 101)
                        {
                            rectangle.X = sX;
                            rectangle.Y = sY;
                            rectangle.Height = eY - sY;
                            rectangle.Width = eX - sX;
                            return GetPetHp(ref rectangle, ref bmp);
                        }
                    }
                }

            }

            return -1;
        }

        private static int GetPetHp(ref Rectangle rectangle, ref Bitmap bmp)
        {
            if (rectangle.Width == 0)
            {
                return -1;
            }

            Rectangle cpRect = new Rectangle(rectangle.X + 17, rectangle.Y + 28, rectangle.Width - 21, 1);
            Bitmap healBmp = new Bitmap(cpRect.Width, cpRect.Height);
            healBmp = bmp.Clone(cpRect, healBmp.PixelFormat);
            return CalculatePartyHP(ref healBmp);
        }

        public static Types.Stats UpdateMyStats(ref Bitmap bmp)
        {
            Color pixel = new Color();
            Color pixel2 = new Color();
            int sX = -1, sY = -1;
            int eX = -1, eY = -1;

            Rectangle rectangle = new Rectangle();

            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = 0;
            rectangle.Width = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    pixel = bmp.GetPixel(x, y);
                    if (pixel.R == 201 && pixel.G == 192 && pixel.B == 177)
                    {
                        pixel2 = bmp.GetPixel(x, y + 79);
                        if (pixel2.R == 201 && pixel2.G == 192 && pixel2.B == 177)
                        {
                            sX = x;
                            sY = y;
                            eY = y + 79;
                            break;
                        }
                    }
                }

                if (eY >= 0)
                {
                    for (eX = sX; eX < bmp.Width; eX++)
                    {
                        pixel = bmp.GetPixel(eX, sY + 3);
                        if (pixel.R == 132 && pixel.G == 122 && pixel.B == 101)
                        {
                            rectangle.X = sX;
                            rectangle.Y = sY;
                            rectangle.Height = eY - sY;
                            rectangle.Width = eX - sX;
                            return GetStats(ref rectangle, ref bmp);
                        }
                    }
                }

            }

            return new Types.Stats();
        }

        public static int UpdateTargetHp(ref Bitmap bmp)
        {
            Color pixel = new Color();
            int sX = -1, sY = -1;
            int eX = -1, eY = -1;

            Rectangle rectangle = new Rectangle();
            rectangle.X = 0;
            rectangle.Y = 0;
            rectangle.Height = 0;
            rectangle.Width = 0;

            for (int x = 500; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    pixel = bmp.GetPixel(x, y);
                    if (pixel.R == 201 && pixel.G == 192 && pixel.B == 177)
                    {
                        if (sY == -1)
                        {
                            sX = x;
                            sY = y;
                        }
                        else
                        {
                            eY = y;
                            break;
                        }
                    }
                }

                if (eY >= 0)
                {
                    for (eX = sX + 170; eX < bmp.Width; eX++)
                    {
                        pixel = bmp.GetPixel(eX, sY + 3);
                        if (pixel.R == 132 && pixel.G == 122 && pixel.B == 101)
                        {
                            rectangle.X = sX;
                            rectangle.Y = sY;
                            rectangle.Height = eY - sY;
                            rectangle.Width = eX - sX;
                            
                            return GetTargetHP(ref rectangle, ref bmp);
                        }
                    }
                }

            }

            return -1;
        }

        public static List<Bitmap> UpdateBuffs(ref Bitmap bmp)
        {
            List<Bitmap> buffs = new List<Bitmap>();
            Color pixel = new Color();
            int sX = -1, sY = -1;
            int eX = -1, eY = -1;

            if (bmp.Width < 500)
            {
                return buffs;
            }

            for (int x = 200; x < 500; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    pixel = bmp.GetPixel(x, y);
                    if (pixel.R == 201 && pixel.G == 192 && pixel.B == 177)
                    {
                        if (sY == -1)
                        {
                            sX = x;
                            sY = y;
                        }
                        else
                        {
                            eY = y;
                            break;
                        }
                    }
                }

                if (eY >= 0)
                {
                    
                    for (int yy = sY; yy <= eY - sY; yy += 26)
                    {
                        for (int xx = sX + 13; xx <= sX + 312; xx += 26)
                        {
                            Rectangle buffRect = new Rectangle(xx, yy, 23, 23);
                            Bitmap buffBmp = new Bitmap(23, 23);
                            buffBmp = bmp.Clone(buffRect, buffBmp.PixelFormat);
                            buffs.Add(buffBmp);
                        }
                    }

                    return buffs;
                }

            }

            return buffs;
        }

        public static Types.Stats GetStats(ref Rectangle rectangle, ref Bitmap bmp)
        {
            Types.Stats stats = new Types.Stats();
            Types.StatColor mpColor = new Types.StatColor();
            mpColor.Set(125, 125, 125);

            Rectangle cpRect = new Rectangle(rectangle.X + rectangle.Width / 3, rectangle.Y + 26, rectangle.Width - rectangle.Width / 2, 12);
            Bitmap cpBmp = new Bitmap(cpRect.Width, cpRect.Height);
            cpBmp = bmp.Clone(cpRect, cpBmp.PixelFormat);

            Rectangle hpRect = new Rectangle(rectangle.X + rectangle.Width / 3, rectangle.Y + 39, rectangle.Width - rectangle.Width / 2, 12);
            Bitmap hpBmp = new Bitmap(hpRect.Width, hpRect.Height);
            hpBmp = bmp.Clone(hpRect, hpBmp.PixelFormat);

            Rectangle mpRect = new Rectangle(rectangle.X + rectangle.Width / 3, rectangle.Y + 52, rectangle.Width - rectangle.Width / 2, 12);
            Bitmap mpBmp = new Bitmap(mpRect.Width, mpRect.Height);
            mpBmp = bmp.Clone(mpRect, mpBmp.PixelFormat);

            stats.cp = GetStatByImage(ref cpBmp, ref mpColor);
            stats.hp = GetStatByImage(ref hpBmp, ref mpColor);
            stats.mp = GetStatByImage(ref mpBmp, ref mpColor);
            cpBmp = null;
            hpBmp = null;
            mpBmp = null;
            return stats;
        }

        private static Types.Stat GetStatByImage(ref Bitmap bmp, ref Types.StatColor statColor)
        {
            Types.Stat stat = new Types.Stat();
            int start = -1;
            int stop = 0;
            bool found = false;
            Color p = new Color();
            int w = 0, h = 0;

            string number = "";
            try
            {
                for (w = 0; w < bmp.Width; w++)
                {
                    found = false;
                    for (h = 0; h < bmp.Height; h++)
                    {
                        p = bmp.GetPixel(w, h);
                        if (p.R > statColor.R && p.G > statColor.G && p.B > statColor.B)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found && start == -1)
                    {
                        start = w;
                        //Console.WriteLine("Start {0}", start);
                    }

                    if (found == false && start != -1)
                    {
                        stop = w - 1;
                        //Console.WriteLine("Stop {0}", stop);
                        number += GetNumber(start, stop, ref bmp, ref statColor);
                        start = -1;
                    }
                }
                int pos = number.IndexOf("/");
                stat.current = Int32.Parse(number.Substring(0, pos));
                stat.total = Int32.Parse(number.Substring(pos + 1, number.Length - pos - 1));
            }
            catch
            {
                Console.WriteLine("GetStatByImage Exeption {0} {1} {2}", w, h, number);
            }
            return stat;
        }

        private static string GetNumber(int start, int stop, ref Bitmap bmp, ref Types.StatColor statColor)
        {
            int len = stop - start;
            Color p = new Color();
            int countTotal = 0;
            int countFront = 0;
            int countTil = 0;
            string number = "X";


            for (int w = start; w <= stop; w++)
            {
                for (int h = 0; h < bmp.Height; h++)
                {
                    p = bmp.GetPixel(w, h);
                    if (p.R > statColor.R && p.G > statColor.G && p.B > statColor.B)
                    {
                        if (w == start)
                        {
                            countFront++;
                        }
                        else if (w == stop)
                        {
                            countTil++;
                        }

                        countTotal++;
                    }
                }
            }

            //Console.WriteLine("GetNumber {0} {1} {2}", countTotal, countFront, countTil);

            if (countTotal == 9 && countFront == 1 && countTil == 4) number = "/";
            else if (countTotal == 11 && countFront == 2 && countTil == 1) number = "1";
            else if (countTotal == 15 && countFront == 3 && countTil == 3) number = "2";
            else if (countTotal == 15 && countFront == 2 && countTil == 5) number = "3";
            else if ((countTotal == 17 || countTotal == 18) && countFront == 1 && countTil == 1) number = "4";
            else if (countTotal == 17 && countFront == 4 && countTil == 4) number = "5";
            else if (countTotal == 16 && countFront == 3 && countTil == 3) number = "6";
            else if (countTotal == 14 && countFront == 2 && countTil == 3) number = "7";
            else if (countTotal == 21 && countFront == 6 && countTil == 4) number = "8";
            else if (countTotal == 17 && countFront == 3 && countTil == 4) number = "9";
            else if (countTotal == 18 && countFront == 5 && countTil == 4) number = "0";

            return number;
        }

        private static int GetTargetHP(ref Rectangle rectangle, ref Bitmap bmp)
        {
            if (rectangle.Width <= 0 || rectangle.Height <= -1)
            {
                return -1;
            }

            Rectangle cpRect = new Rectangle(rectangle.X + 17, rectangle.Y + 27, rectangle.Width - 21, 1);
            Bitmap healBmp = new Bitmap(cpRect.Width, cpRect.Height);
            healBmp = bmp.Clone(cpRect, healBmp.PixelFormat);
            int hp;
            //healBmp.Save("C:\\img.jpg", ImageFormat.Jpeg);

            Color p = new Color();
            Color p1 = new Color();

            p1 = bmp.GetPixel(rectangle.X + 25, rectangle.Y + 37);
            if (p1.R == 26 && p1.G == 92 && p1.B == 186)
            {
                return 0;
            }

            for (hp = 0; hp < healBmp.Width; hp++)
            {
                p = healBmp.GetPixel(hp, 0);
                if (p.R < 110)
                {
                    break;
                }
            }
            healBmp = null;
            return (100 * hp) / (rectangle.Width - 21);
        }

        public static List<Types.Stats> UpdatePartyStats(ref Rectangle rectangle, ref Bitmap bmp)
        {
            partyColors.Add("ff18518c");
            partyColors.Add("ff29527b");
            partyColors.Add("ff426129");

            int offsetY = 41;
            int offsetX = 51;

            List<Types.Stats> party = new List<Types.Stats>();
            for (int pos = rectangle.Y; pos < rectangle.Y + rectangle.Height; pos++)
            {
                Color p = new Color();
                p = bmp.GetPixel(rectangle.X + 17, pos);
                if (partyColors.Exists(x => x == p.Name) == true)
                {
                    Types.Stats member = new Types.Stats();
                    if (party.Count > 0)
                    {
                        offsetY = 43;
                    }

                    Rectangle cpRect = new Rectangle(rectangle.X + 17, pos + 25, rectangle.Width - 21, 1);
                    Bitmap healBmp = new Bitmap(cpRect.Width, cpRect.Height);
                    healBmp = bmp.Clone(cpRect, healBmp.PixelFormat);
                    member.hp.current = CalculatePartyHP(ref healBmp);

                    member.pet = -1;
                    Color p1 = new Color();
                    p1 = bmp.GetPixel(rectangle.X + offsetX, pos + offsetY);
                    if (p1.Name == "ff495a55")
                    {

                        Rectangle cp1Rect = new Rectangle(rectangle.X + offsetX + 13, pos + offsetY +2, rectangle.Width - 68, 1);
                        Bitmap heal1Bmp = new Bitmap(cp1Rect.Width, cpRect.Height);
                        heal1Bmp = bmp.Clone(cp1Rect, heal1Bmp.PixelFormat);
                        //heal1Bmp.Save("C:\\tmp\\" + pos + ".jpg", ImageFormat.Jpeg);
                        member.pet = CalculatePartyHP(ref heal1Bmp);

                        pos += 10;
                    }
                    
                    pos +=10;
                    party.Add(member);
                }

            }

            return party;
        }

        private static int CalculatePartyHP(ref Bitmap bmp) 
        {
            int hp = 0;

            Color p = new Color();

            for (hp = 0; hp < bmp.Width; hp++)
            {
                p = bmp.GetPixel(hp, 0);
                if (p.R < 110)
                {
                    break;
                }
            }

            return (100 * hp) / bmp.Width;
        }
    }
}
