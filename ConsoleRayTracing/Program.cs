using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleRayTracing
{
    class Program
    {
        static void Main(string[] args)
        {
            int w = 120;
            int h = 30;

            char[] gradient = "░.':,\"!/r(l1Z4H9W8$@".ToCharArray();


            char[] screen = new char[w * h + 1];
            screen[w * h] = '\0';
            double ratio = 1.8;
            for (int t = 0; t < int.MaxValue; t++)
            {
                vec3 spherePos = new vec3(0, 3, 0);
                vec3 light = norm(new vec3(-0.5, 0.5, -1.0));
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        vec2 uv = new vec2(i, j) / new vec2(w, h) - new vec2(0.5);
                        uv.x *= ratio;
                        vec3 ro = new vec3(-7, 0, 0);
                        vec3 rd = norm(new vec3(1, uv + new vec2(0.1)));
                        ro = rotateZ(ro, t * 0.01);
                        rd = rotateZ(rd, t * 0.01);
                        ro = rotateY(ro, t * 0.01);
                        rd = rotateY(rd, t * 0.01);
                        double diff = 0;
                        vec2 minIt = new vec2(99999);
                        vec3 n = new vec3(0);
                        vec2 it = sphIntersect(ro, rd, spherePos, 1);
                        if (it.x > 0 && it.x < minIt.x)
                        {
                            minIt = it;
                            n = norm(ro - spherePos + rd * new vec3(it.x));
                        }
                        vec3 boxN = new vec3(0);

                        vec2 it2 = boxIntersection(ro, rd, new vec3(1), ref boxN);
                        if (it2.x > 0 && it2.x < minIt.x)
                        {
                            minIt = it2;
                            n = boxN;
                        }
                        diff = dot(n, light);
                        int col = 0;
                        if (minIt.x < 99999)
                        {
                            diff = diff * 0.5 + 0.5;
                            col = (int)(diff * 20.0);
                        }
                        if (col < 0) col = 0;
                        if (col > 19) col = 19;
                        screen[i + j * w] = gradient[col];
                    }
                }
                Console.SetCursorPosition(0, 0);
                Thread.Sleep(1000);
                Console.WriteLine(screen);
            }
            Console.ReadLine();
        }

       static  double sign(double a) { return Convert.ToDouble(Convert.ToInt16(0 < a) - Convert.ToInt16(a < 0)); }
        static double step(double edge, double x) { return Convert.ToDouble(x > edge); }
        static double length(vec2 v) { return Math.Sqrt(v.x * v.x + v.y * v.y); }
        static double length(vec3 v) { return Math.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z); }
        static vec2 norm(vec2 v) { return v / new vec2(length(v)); }
        static vec3 norm(vec3 v) { return v / new vec3(length(v)); }
        static double dot(vec3 a, vec3 b) { return a.x * b.x + a.y * b.y + a.z * b.z; }
        static vec3 abs(vec3 v) { return new vec3(Math.Abs(v.x), Math.Abs(v.y), Math.Abs(v.z)); }
        static vec3 sign(vec3 v) { return new vec3(sign(v.x), sign(v.y), sign(v.z)); }
        static vec3 step(vec3 edge, vec3 v) { return  new vec3(step(edge.x, v.x), step(edge.y, v.y), step(edge.z, v.z)); }

        static vec3 rotateX(vec3 a, double angle)
        {
            vec3 b = a;
            b.z = a.z * Math.Cos(angle) - a.y * Math.Sin(angle);
            b.y = a.z * Math.Sin(angle) + a.y * Math.Cos(angle);
            return b;
        }

        static vec3 rotateY(vec3 a, double angle)
        {
            vec3 b = a;
            b.x = a.x * Math.Cos(angle) - a.z * Math.Sin(angle);
            b.z = a.x * Math.Sin(angle) + a.z * Math.Cos(angle);
            return b;
        }

        static vec3 rotateZ(vec3 a, double angle)
        {
            vec3 b = a;
            b.x = a.x * Math.Cos(angle) - a.y * Math.Sin(angle);
            b.y = a.x * Math.Sin(angle) + a.y * Math.Cos(angle);
            return b;
        }

        static vec2 sphIntersect(vec3 ro, vec3 rd, vec3 ce, double ra)
        {
            vec3 oc = ro - ce;
            double b = dot(oc, rd);
            double c = dot(oc, oc) - ra * ra;
            double h = b * b - c;
            if (h < 0.0) return new vec2(-1.0); // no intersection
            h = Math.Sqrt(h);
            return new vec2(-b - h, -b + h);
        }

        static vec2 boxIntersection(vec3 ro, vec3 rd, vec3 boxSize, ref vec3 outNormal)
        {
            vec3 m = new vec3(1.0) / rd; // can precompute if traversing a set of aligned boxes
            vec3 n = m * ro;   // can precompute if traversing a set of aligned boxes
            vec3 k = abs(m) * boxSize;
            vec3 t1 = -n - k;
            vec3 t2 = -n + k;
            double tN = Math.Max(Math.Max(t1.x, t1.y), t1.z);
            double tF = Math.Min(Math.Min(t2.x, t2.y), t2.z);
            if (tN > tF || tF < 0.0) return new vec2(-1.0); // no intersection
            vec3 yzx = new vec3(t1.y, t1.z, t1.x);
            vec3 zxy = new vec3(t1.z, t1.x, t1.y);
            outNormal = -sign(rd) * step(yzx, t1) * step(zxy, t1);
            return new vec2(tN, tF);
        }
    }

    struct vec2
    {
        public double x, y;

        public vec2(double _value) 
        {
            x = _value;
            y = _value;
        }
        public vec2(double _x, double _y) {
            x = _x;
            y = _y;
        }

        public static vec2 operator + (vec2 vec1 ,vec2 vec2)
        {
            return new vec2(vec1.x + vec2.x, vec1.y + vec2.y);
        }

        public static vec2 operator -(vec2 vec1, vec2 vec2)
        {
            return new vec2(vec1.x - vec2.x, vec1.y - vec2.y);
        }

        public static vec2 operator /(vec2 vec1, vec2 vec2)
        {
            return new vec2(vec1.x / vec2.x, vec1.y / vec2.y);
        }
    };


    struct vec3
    {
        public double x, y, z;

        public vec3(double _value)
        {
            x = _value;
            y = _value;
            z = _value;
        }
        public vec3(vec2 v, double _z) {
            x = v.x;
            y = v.y;
            z = _z;             
        }
        public vec3(double _x, vec2 v) {
            x = _x;
            y = v.x;
            z = v.y;
        }
        public vec3(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public static vec3 operator +(vec3 vec, vec3 other) { return new vec3(vec.x + other.x, vec.y + other.y, vec.z + other.z); }
        public static vec3 operator -(vec3 vec, vec3 other) { return new vec3(vec.x - other.x, vec.y - other.y, vec.z - other.z); }
        public static vec3 operator *(vec3 vec, vec3 other) { return new vec3(vec.x * other.x, vec.y * other.y, vec.z * other.z); }
        public static vec3 operator /(vec3 vec, vec3 other) { return new vec3(vec.x / other.x, vec.y / other.y, vec.z / other.z); }
        public static vec3 operator -(vec3 vec) { return new vec3(-vec.x, -vec.y, -vec.z); }

        vec3 xxx() { return new vec3(x, x, x); }
        vec3 xxy() { return new vec3(x, x, y); }
        vec3 xxz() { return new vec3(x, x, z); }
        vec3 xyx() { return new vec3(x, y, x); }
        vec3 xyy() { return new vec3(x, y, y); }
        vec3 xyz() { return new vec3(x, y, z); }
        vec3 xzx() { return new vec3(x, z, x); }
        vec3 xzy() { return new vec3(x, z, y); }
        vec3 xzz() { return new vec3(x, z, z); }
        vec3 yxx() { return new vec3(y, x, x); }
        vec3 yxy() { return new vec3(y, x, y); }
        vec3 yxz() { return new vec3(y, x, z); }
        vec3 yyx() { return new vec3(y, y, x); }
        vec3 yyy() { return new vec3(y, y, y); }
        vec3 yyz() { return new vec3(y, y, z); }
        vec3 yzx() { return new vec3(y, z, x); }
        vec3 yzy() { return new vec3(y, z, y); }
        vec3 yzz() { return new vec3(y, z, z); }
        vec3 zxx() { return new vec3(z, x, x); }
        vec3 zxy() { return new vec3(z, x, y); }
        vec3 zxz() { return new vec3(z, x, z); }
        vec3 zyx() { return new vec3(z, y, x); }
        vec3 zyy() { return new vec3(z, y, y); }
        vec3 zyz() { return new vec3(z, y, z); }
        vec3 zzx() { return new vec3(z, z, x); }
        vec3 zzy() { return new vec3(z, z, y); }
        vec3 zzz() { return new vec3(z, z, z); }

    };
}
