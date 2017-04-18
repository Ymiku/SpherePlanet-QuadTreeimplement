using UnityEngine;
using System.Collections;

public static class MathExtra {
	static float[] sinArray;
	static float[] cosArray;
	static MathExtra()
	{
		sinArray = new float[360];
		cosArray = new float[360];
		for (int i = 0; i < 360; i++) {
			sinArray [i] = Mathf.Sin (i*Mathf.Deg2Rad);
			cosArray [i] = Mathf.Cos (i*Mathf.Deg2Rad);
		}
	}
	public static float FastSin(float f)
	{
		float deg = f*Mathf.Rad2Deg;
		while (deg>=360f) {
			deg -= 360f;
		}
		while (deg<0f) {
			deg += 360f;
		}
		int di = (int)deg;
		return Mathf.Lerp(sinArray [di],sinArray [di+1],deg-di);
	}
	public static float FastCos(float f)
	{
		float deg = f*Mathf.Rad2Deg;
		while (deg>=360f) {
			deg -= 360f;
		}
		while (deg<0f) {
			deg += 360f;
		}
		int di = (int)deg;
		return Mathf.Lerp(cosArray [di],cosArray [di+1],deg-di);
	}
	public static float Noise(int x)
	{
		x = (x << 13) ^ x;
		return (1f - ((x * (x * x * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824f);
	}
	public static float Noise(int x,int y)
	{
		x ^= x << 13;
		return Noise (x + y);
	}
	public static float PerlinNoise(float x,float y)
	{
		int int_x = (int)x;
		int int_y = (int)y;
		float v1 = Noise (int_x, int_y);
		float v2 = Noise (int_x+1,int_y);
		float v3 = Noise (int_x, int_y+1);
		float v4 = Noise (int_x+1, int_y+1);
		float i1 = CosineLerp (v1,v2,x-int_x);
		float i2 = CosineLerp (v3,v4,x-int_x);
		return CosineLerp (i1,i2,y-int_y);
	}
	public static float SmoothNoise_1D(int x)
	{
		return Noise(x)*0.5f+Noise(x-1)*0.25f+Noise(x+1)*0.25f;
	}
	public static float SmoothNoise_2D(int x,int y)
	{
		float corners = (Noise (x - 1, y - 1) + Noise (x + 1, y - 1) + Noise (x - 1, y + 1) + Noise (x + 1, y + 1)) / 16f;
		float sides = (Noise (x - 1, y) + Noise (x + 1, y) + Noise (x, y - 1) + Noise (x, y + 1)) / 8f;
		float center = Noise (x, y) / 4f;
		return corners + sides + center;
	}
	public static Vector3 Verlet(Vector3 lxt,Vector3 xt,Vector3 a,float d = 0.2f,float dt = 0.02f)
	{
		return xt + d * (xt - lxt) + a * dt*dt;
	}
	public static float GetDegree(float d)
	{
		while (d>180f) {
			d -= 360f;
		}
		while (d<-180f) {
			d += 360f;
		}
		return d;
	}

	public static float ConstantLerp(float ori,float target,float speed)
	{
		if (target > ori) {
			return Mathf.Min (ori+speed,target);
		}
		return Mathf.Max (ori-speed,target);
	}

	public static float CosineLerp(float a,float b,float t)
	{
		float f = t * 3.1415927f;
		f = (1f - FastCos (f)) * 0.5f;
		return a * (1f - f) + b * f;
	}
	public static float CubicLerp(float v0,float v1,float v2,float v3,float t)
	{
		float P = (v3-v2)-(v0-v1);
		float Q = (v0 - v1) - P;
		float R = v2 - v0;
		float S = v1;
		return P * Mathf.Pow (t, 3f) + Q * Mathf.Pow (t, 2f) + R * t + S;
	}
	public static float GetV2L(Vector2 v)
	{
		return MathExtra.FastSqrt(Mathf.Pow(v.x,2f)+Mathf.Pow(v.y,2f));
	}
	public static float GetV3L(Vector3 v)
	{
		return MathExtra.FastSqrt(v.x*v.x+v.y*v.y+v.z*v.z);
	}
	public static Vector3 FastNormalize(Vector3 v)
	{
		return v*InverseSqrtFast (v.sqrMagnitude);
	}
	public static float FastSqrt(float x)
	{
		unsafe  
		{  
			int i;
			float x2, y;
			const float threehalfs = 1.5F;
			x2 = x * 0.5F;
			y  = x;
			i  = * ( int * ) &y;     
			i  = 0x5f375a86 - ( i >> 1 ); 
			y  = * ( float * ) &i;
			y  = y * ( threehalfs - ( x2 * y * y ) ); 
			//y  = y * ( threehalfs - ( x2 * y * y ) );  	
			//y  = y * ( threehalfs - ( x2 * y * y ) ); 
			return x*y;
		}
	}
	public static float InverseSqrtFast(float x)  
	{  
		unsafe  
		{  
			float xhalf = 0.5f * x;  
			int i = *(int*)&x;              // Read bits as integer.  
			i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation  
			x = *(float*)&i;                // Convert bits back to float  
			x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.  
			return x;  
		}  
	}  
	public static float Dot (Vector2 v1,Vector2 v2)
	{
		return v1.x * v2.x + v1.y * v2.y;
	}
	public static Vector2 Damping(float time,float swing,float damping,float hz){
		Vector2 pos = Vector2.zero;
		pos.x = time;
		pos.y = swing * Mathf.Pow (damping, -time) * Mathf.Sin (hz * time);
		return pos;
	}
	public static float GetDegree(Vector2 dir)
	{
		if (dir.y == 0) {
			if (dir.x > 0) {
				return 0f;
			} else {
				return 180f;
			}
		}
		float oppo = dir.y;
		float hy = dir.magnitude;
		float degree = Mathf.Asin (oppo / hy) * Mathf.Rad2Deg;
		if (dir.x < 0 && dir.y > 0)
			return 180f - degree;
		if (dir.x <= 0 && dir.y < 0)
			return 180f - degree;
		if (dir.x > 0 && dir.y < 0)
			return 360f + degree;
		return degree;
	} 
	public static bool ApproEquals(float x,float y)
	{
		if (Mathf.Abs (x - y) <= 0.01f)
			return true;
		return false;
	}
	public static float FastDis(Vector3 a,Vector3 b)
	{
		return Mathf.Abs (a.x - b.x) + Mathf.Abs (a.y - b.y) + Mathf.Abs (a.z - b.z);
	}
	public static float Max(float a,float b,float c)
	{
		a = a > b ? a : b;
		return a > c ? a : c;
	}
	public static float Min(float a,float b,float c)
	{
		a = a < b ? a : b;
		return a < c ? a : c;
	}
	public static float Average(float a,float b,float c)
	{
		float f = a + b + c;
		return a * a / f + b * b / f + c * c / f;
	}
}
