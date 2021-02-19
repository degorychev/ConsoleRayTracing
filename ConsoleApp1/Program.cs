using System;
class P
{
    static void Main()
    {
        Func<string> r = Console.ReadLine;
        int x = int.Parse(r()), y = int.Parse(r()); var c = r(); for (int j = 0; j < 25; j++) { for (int i = 0; i < 70; i++) { var l = c.Length; Console.Write(c[(int)Math.Min(l * Math.Sqrt(Math.Pow(Math.Abs(x - i) / 2.0, 2) + Math.Pow(Math.Abs(y - j), 2)) / 35, l - 1)]); } Console.Write("\n"); } }
}