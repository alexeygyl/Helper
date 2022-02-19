using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Helper
{
    class Keyboard
    {
        [DllImport("KeySender.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void KeyboardKeyPress(short key);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("User32.dll")]
        static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);


        static int SM_CXSCREEN = 0;
        static int SM_CYSCREEN = 1;

        enum MouseFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            Absolute = 0x8000
        };


        public enum KeyCode : ushort
        {
            MEDIA_NEXT_TRACK = 0xb0,
            MEDIA_PLAY_PAUSE = 0xb3,
            MEDIA_PREV_TRACK = 0xb1,
            MEDIA_STOP = 0xb2,
            ADD = 0x6b,
            MULTIPLY = 0x6a,
            DIVIDE = 0x6f,
            SUBTRACT = 0x6d,
            BROWSER_BACK = 0xa6,
            BROWSER_FAVORITES = 0xab,
            BROWSER_FORWARD = 0xa7,
            BROWSER_HOME = 0xac,
            BROWSER_REFRESH = 0xa8,
            BROWSER_SEARCH = 170,
            BROWSER_STOP = 0xa9,
            NUMPAD0 = 0x60,
            NUMPAD1 = 0x61,
            NUMPAD2 = 0x62,
            NUMPAD3 = 0x63,
            NUMPAD4 = 100,
            NUMPAD5 = 0x65,
            NUMPAD6 = 0x66,
            NUMPAD7 = 0x67,
            NUMPAD8 = 0x68,
            NUMPAD9 = 0x69,
            F1 = 0x70,
            F10 = 0x79,
            F11 = 0x7a,
            F12 = 0x7b,
            F13 = 0x7c,
            F14 = 0x7d,
            F15 = 0x7e,
            F16 = 0x7f,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 130,
            F2 = 0x71,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 120,
            OEM_1 = 0xba,
            OEM_102 = 0xe2,
            OEM_2 = 0xbf,
            OEM_3 = 0xc0,
            OEM_4 = 0xdb,
            OEM_5 = 220,
            OEM_6 = 0xdd,
            OEM_7 = 0xde,
            OEM_8 = 0xdf,
            OEM_CLEAR = 0xfe,
            OEM_COMMA = 0xbc,
            OEM_MINUS = 0xbd,
            OEM_PERIOD = 190,
            OEM_PLUS = 0xbb,
            KEY_0 = 0x30,
            KEY_1 = 0x31,
            KEY_2 = 50,
            KEY_3 = 0x33,
            KEY_4 = 0x34,
            KEY_5 = 0x35,
            KEY_6 = 0x36,
            KEY_7 = 0x37,
            KEY_8 = 0x38,
            KEY_9 = 0x39,
            KEY_A = 0x41,
            KEY_B = 0x42,
            KEY_C = 0x43,
            KEY_D = 0x44,
            KEY_E = 0x45,
            KEY_F = 70,
            KEY_G = 0x47,
            KEY_H = 0x48,
            KEY_I = 0x49,
            KEY_J = 0x4a,
            KEY_K = 0x4b,
            KEY_L = 0x4c,
            KEY_M = 0x4d,
            KEY_N = 0x4e,
            KEY_O = 0x4f,
            KEY_P = 80,
            KEY_Q = 0x51,
            KEY_R = 0x52,
            KEY_S = 0x53,
            KEY_T = 0x54,
            KEY_U = 0x55,
            KEY_V = 0x56,
            KEY_W = 0x57,
            KEY_X = 0x58,
            KEY_Y = 0x59,
            KEY_Z = 90,
            VOLUME_DOWN = 0xae,
            VOLUME_MUTE = 0xad,
            VOLUME_UP = 0xaf,
            SNAPSHOT = 0x2c,
            RightClick = 0x5d,
            BACKSPACE = 8,
            CANCEL = 3,
            CAPS_LOCK = 20,
            CONTROL = 0x11,
            ALT = 18,
            DECIMAL = 110,
            DELETE = 0x2e,
            DOWN = 40,
            END = 0x23,
            ESC = 0x1b,
            HOME = 0x24,
            INSERT = 0x2d,
            LAUNCH_APP1 = 0xb6,
            LAUNCH_APP2 = 0xb7,
            LAUNCH_MAIL = 180,
            LAUNCH_MEDIA_SELECT = 0xb5,
            LCONTROL = 0xa2,
            LEFT = 0x25,
            LSHIFT = 160,
            LWIN = 0x5b,
            PAGEDOWN = 0x22,
            NUMLOCK = 0x90,
            PAGE_UP = 0x21,
            RCONTROL = 0xa3,
            ENTER = 13,
            RIGHT = 0x27,
            RSHIFT = 0xa1,
            RWIN = 0x5c,
            SHIFT = 0x10,
            SPACE_BAR = 0x20,
            TAB = 9,
            UP = 0x26,
        }

        static public short GetKeyCodeByString(String str)
        {
            short keyCode = 0;
            switch (str.ToUpper())
            {
                case "/": return 111;
                case "F1": return 0x70;
                case "F2": return 0x71;
                case "F3": return 0x72;
                case "F4": return 0x73;
                case "F5": return 0x74;
                case "F6": return 0x75;
                case "F7": return 0x76;
                case "F8": return 0x77;
                case "F9": return 0x78;
                case "F10": return 0x79;
                case "F11": return 0x7A;
                case "F12": return 0x7B;
                case "ESC": return 0x1b;
                case "INSERT": return 45;
                case "DELETE": return 46;
                case "PAUSE": return 19;
                case "SCROLL": return 145;
                case "ENTER": return 13;
                case "ESCAPE": return 27;
                case " ": return 32;
                case "0": return 0x30;
                case "1": return 0x31;
                case "2": return 0x32;
                case "3": return 0x33;
                case "4": return 0x34;
                case "5": return 0x35;
                case "6": return 0x36;
                case "7": return 0x37;
                case "8": return 0x38;
                case "9": return 0x39;
                case "-": return 189;
                case "=": return 187;
                case "A": return 0x41;
                case "B": return 0x42;
                case "C": return 0x43;
                case "D": return 0x44;
                case "E": return 0x45;
                case "F": return 70;
                case "G": return 0x47;
                case "H": return 0x48;
                case "I": return 0x49;
                case "J": return 0x4a;
                case "K": return 0x4b;
                case "L": return 0x4c;
                case "M": return 0x4d;
                case "N": return 0x4e;
                case "O": return 0x4f;
                case "P": return 80;
                case "Q": return 0x51;
                case "R": return 0x52;
                case "S": return 0x53;
                case "T": return 0x54;
                case "U": return 0x55;
                case "V": return 0x56;
                case "W": return 0x57;
                case "X": return 0x58;
                case "Y": return 0x59;
                case "Z": return 90;
                case "Ф": return 0x41;
                case "И": return 0x42;
                case "С": return 0x43;
                case "В": return 0x44;
                case "У": return 0x45;
                case "А": return 70;
                case "П": return 0x47;
                case "Р": return 0x48;
                case "Ш": return 0x49;
                case "О": return 0x4a;
                case "Л": return 0x4b;
                case "Д": return 0x4c;
                case "Ь": return 0x4d;
                case "Т": return 0x4e;
                case "Щ": return 0x4f;
                case "З": return 80;
                case "Й": return 0x51;
                case "К": return 0x52;
                case "Ы": return 0x53;
                case "Е": return 0x54;
                case "Г": return 0x55;
                case "М": return 0x56;
                case "Ц": return 0x57;
                case "Ч": return 0x58;
                case "Н": return 0x59;
                case "Я": return 90;
                case "Ж": return 186;
                case "Э": return 222;
                case "Б": return 188;
                case "Ю": return 190;
                case "Х": return 219;
                case "Ъ": return 221;
            }

            return keyCode;
        }

        static public void PressKey(String str)
        {
            if (System.Environment.OSVersion.Version.Major < 10)
            {
                KeyboardKeyPress(GetKeyCodeByString(str));
            }
            else 
            {
                keybd_event((byte)GetKeyCodeByString(str), 0x58, 0, 0);
                keybd_event((byte)GetKeyCodeByString(str), 0xd8, 2, 0);
            }
        }

        static public void PressKey(char c)
        {

            string str = new string(c,1);

            if (System.Environment.OSVersion.Version.Major < 10)
            {
                KeyboardKeyPress(GetKeyCodeByString(str));
            }
            else
            {
                keybd_event((byte)GetKeyCodeByString(str), 0x58, 0, 0);
                keybd_event((byte)GetKeyCodeByString(str), 0xd8, 2, 0);
            }
        }

        static public void ShiftPressKey(String str)
        {
            keybd_event(0x10, 0x1d, 0, 0); 
            keybd_event((byte)GetKeyCodeByString(str), 0x58, 0, 0);
            keybd_event((byte)GetKeyCodeByString(str), 0xd8, 2, 0); 
            keybd_event(0x10, 0x9d, 2, 0);
        }

        static public void AcceptParty()
        {
            int sx = GetSystemMetrics(SM_CXSCREEN);
            int sy = GetSystemMetrics(SM_CYSCREEN);

            Rectangle window = AsteriosManager.GetWindowRect();

            int x = (window.X + 470) * 65536 / sx;
            int y = (window.Y + window.Height + 10) * 65536 / sy;
        
            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftDown, x, y, 0, 0);
            Thread.Sleep(300);
            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftUp, x, y, 0, 0);
            Thread.Sleep(100);
        
            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftDown, x, y, 0, 0);
            Thread.Sleep(300);
            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftUp, x, y, 0, 0);
            Thread.Sleep(100);
        }

        static public void StartType()
        {
            int sx = GetSystemMetrics(SM_CXSCREEN);
            int sy = GetSystemMetrics(SM_CYSCREEN);

            Rectangle window = AsteriosManager.GetWindowRect();

            int x = (window.X + 250) * 65536 / sx;
            int y = (window.Y + window.Height + 20) * 65536 / sy;

            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftDown, x, y, 0, 0);
            Thread.Sleep(300);
            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftUp, x, y, 0, 0);
            Thread.Sleep(100);

            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftDown, x, y, 0, 0);
            Thread.Sleep(300);
            mouse_event(MouseFlags.Absolute | MouseFlags.Move | MouseFlags.LeftUp, x, y, 0, 0);
            Thread.Sleep(100);
        }

        static public void EndType()
        {
            PressKey("ENTER");
            PressKey("ESCAPE");
        }

        static public void Type(string str, string lang)
        {
            AsteriosManager.SetLang(lang);

            for (int i = 0; i < str.Length; i++)
            {
                PressKey(str.ElementAt(i));
            }
            Thread.Sleep(300);
        }

        static public void Invite(Types.MemberInfo member)
        {
            StartType();
            Type("INVITE ", "eng");
            Type(member.name, member.lang);
            EndType();
        }
    }
}
