using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public static class Math2D
{
	public static float Pi { get { return 3.141592654f;}}
	public static float TwicePi { get { return 6.283185307f;}}
	public static float HalfPi { get { return 1.570796327f;}}                                                                                       

	[Conditional("DEBUG")] static void Assert(bool condition) { if (!condition) throw new Exception(); }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	#region Vector2 extension

	public static readonly Vector2 UnitX = new Vector2 (1f, 0f);
	public static readonly Vector2 UnitY = new Vector2 (0f, 1f);

	public static Vector2 Xx(this Vector2 v2)
	{
		return new Vector2 (v2.x, v2.x);
	}

	public static Vector2 Yy(this Vector2 v2)
	{
		return new Vector2 (v2.y, v2.y);
	}

	public static Vector2 Yx(this Vector2 v2)
	{
		return new Vector2 (v2.y, v2.x);
	}

	public static void Yx(this Vector2 v2, Vector2 value)
	{
		v2.y = value.x;
		v2.x = value.y;
	}

	public static Vector2 Xy(this Vector2 v2)
	{
		return new Vector2 (v2.x, v2.y);
	}

	public static void Xy(this Vector2 v2, Vector2 value)
	{
		v2.x = value.x;
		v2.y = value.y;
	}

	public static Vector3 Xy0(this Vector2 v2)
	{
		return new Vector3 (v2.x, v2.y, 0f);
	}

	public static Vector3 Xy1(this Vector2 v2)
	{
		return new Vector3 (v2.x, v2.y, 1f);
	}

	public static Vector4 Xy00(this Vector2 v2)
	{
		return new Vector4 (v2.x, v2.y, 0f, 0f);
	}

	public static Vector4 Xy01(this Vector2 v2)
	{
		return new Vector4 (v2.x, v2.y, 0f, 1f);
	}

	public static Vector4 Xy10(this Vector2 v2)
	{
		return new Vector4 (v2.x, v2.y, 1f, 0f);
	}

	public static Vector4 Xy11(this Vector2 v2)
	{
		return new Vector4 (v2.x, v2.y, 1f, 1f);
	}

	public static float R(this Vector2 v2)
	{
		return v2.x;
	}

	public static void R(this Vector2 v2, float value)
	{
		v2.x = value;
	}

	public static float G(this Vector2 v2)
	{
		return v2.y;
	}

	public static void G(this Vector2 v2, float value)
	{
		v2.y = value;
	}

	public static float Length (this Vector2 v2)
	{
		return Mathf.Sqrt ((v2.x * v2.x + v2.y * v2.y));
	}

	public static float LengthSquared (this Vector2 v2)
	{
		return v2.x * v2.x + v2.y * v2.y;
	}

	public static float Distance (this Vector2 v2, Vector2 v)
	{
		return v2.Distance (ref v);
	}

	public static float Distance (this Vector2 v2, ref Vector2 v)
	{
		float num = v2.x - v.x;
		float num2 = v2.y - v.y;
		return (float)Math.Sqrt ((double)(num * num + num2 * num2));
	}

	public static float DistanceSquared (this Vector2 v2, Vector2 v)
	{
		return v2.DistanceSquared (ref v);
	}

	public static float DistanceSquared (this Vector2 v2, ref Vector2 v)
	{
		float num = v2.x - v.x;
		float num2 = v2.y - v.y;
		return num * num + num2 * num2;
	}

	public static float Dot (this Vector2 v2, Vector2 v)
	{
		return v2.x * v.x + v2.y * v.y;
	}

	public static float Dot (this Vector2 v2, ref Vector2 v)
	{
		return v2.x * v.x + v2.y * v.y;
	}

	public static float Determinant (this Vector2 v2, Vector2 v)
	{
		return v2.x * v.y - v2.y * v.x;
	}

	public static float Determinant (this Vector2 v2, ref Vector2 v)
	{
		return v2.x * v.y - v2.y * v.x;
	}

	public static Vector2 GetNormalized (this Vector2 v2)
	{
		Vector2 result;
		v2.Normalize (out result);
		return result;
	}

	public static void Normalize (this Vector2 v2, out Vector2 result)
	{
		float num = 1f / v2.Length ();
		result.x = v2.x * num;
		result.y = v2.y * num;
	}

	public static Vector2 Abs (this Vector2 v2)
	{
		Vector2 result;
		v2.Abs (out result);
		return result;
	}

	public static void Abs (this Vector2 v2, out Vector2 result)
	{
		result.x = ((v2.x >= 0f) ? v2.x : (-v2.x));
		result.y = ((v2.y >= 0f) ? v2.y : (-v2.y));
	}

	public static Vector2 Min (this Vector2 v2, Vector2 v)
	{
		Vector2 result;
		v2.Min (ref v, out result);
		return result;
	}

	public static void Min (this Vector2 v2, ref Vector2 v, out Vector2 result)
	{
		result.x = ((v2.x <= v.x) ? v2.x : v.x);
		result.y = ((v2.y <= v.y) ? v2.y : v.y);
	}

	public static Vector2 Min (this Vector2 v2, float f)
	{
		Vector2 result;
		v2.Min (f, out result);
		return result;
	}

	public static void Min (this Vector2 v2, float f, out Vector2 result)
	{
		result.x = ((v2.x <= f) ? v2.x : f);
		result.y = ((v2.y <= f) ? v2.y : f);
	}

	public static Vector2 Max (this Vector2 v2, Vector2 v)
	{
		Vector2 result;
		v2.Max (ref v, out result);
		return result;
	}
	public static void Max (this Vector2 v2, ref Vector2 v, out Vector2 result)
	{
		result.x = ((v2.x >= v.x) ? v2.x : v.x);
		result.y = ((v2.y >= v.y) ? v2.y : v.y);
	}

	public static Vector2 Max (this Vector2 v2, float f)
	{
		Vector2 result;
		v2.Max (f, out result);
		return result;
	}

	public static void Max (this Vector2 v2, float f, out Vector2 result)
	{
		result.x = ((v2.x >= f) ? v2.x : f);
		result.y = ((v2.y >= f) ? v2.y : f);
	}

	public static Vector2 Clamp (this Vector2 v2, Vector2 min, Vector2 max)
	{
		Vector2 result;
		v2.Clamp (ref min, ref max, out result);
		return result;
	}

	public static void Clamp (this Vector2 v2, ref Vector2 min, ref Vector2 max, out Vector2 result)
	{
		result.x = ((v2.x <= min.x) ? min.x : ((v2.x >= max.x) ? max.x : v2.x));
		result.y = ((v2.y <= min.y) ? min.y : ((v2.y >= max.y) ? max.y : v2.y));
	}

	public static Vector2 Clamp (this Vector2 v2, float min, float max)
	{
		Vector2 result;
		v2.Clamp (min, max, out result);
		return result;
	}

	public static void Clamp (this Vector2 v2, float min, float max, out Vector2 result)
	{
		result.x = ((v2.x <= min) ? min : ((v2.x >= max) ? max : v2.x));
		result.y = ((v2.y <= min) ? min : ((v2.y >= max) ? max : v2.y));
	}

	public static Vector2 Repeat (this Vector2 v2, Vector2 min, Vector2 max)
	{
		Vector2 result;
		v2.Repeat (ref min, ref max, out result);
		return result;
	}

	public static void Repeat (this Vector2 v2, ref Vector2 min, ref Vector2 max, out Vector2 result)
	{
		result.x = Math2D.Repeat (v2.x, min.x, max.x);
		result.y = Math2D.Repeat (v2.y, min.y, max.y);
	}

	public static Vector2 Repeat (this Vector2 v2, float min, float max)
	{
		Vector2 result;
		v2.Repeat (min, max, out result);
		return result;
	}

	public static void Repeat (this Vector2 v2, float min, float max, out Vector2 result)
	{
		result.x = Math2D.Repeat (v2.x, min, max);
		result.y = Math2D.Repeat (v2.y, min, max);
	}

	public static Vector2 Lerp (this Vector2 v2, Vector2 v, float f)
	{
		Vector2 result;
		v2.Lerp (ref v, f, out result);
		return result;
	}

	public static void Lerp (this Vector2 v2, ref Vector2 v, float f, out Vector2 result)
	{
		float num = 1f - f;
		result.x = v2.x * num + v.x * f;
		result.y = v2.y * num + v.y * f;
	}

	public static Vector2 Slerp (this Vector2 v2, Vector2 v, float f)
	{
		Vector2 result;
		v2.Slerp (ref v, f, out result);
		return result;
	}

	public static void Slerp (this Vector2 v2, ref Vector2 v, float f, out Vector2 result)
	{
		result = v2.Rotate (v2.Angle (v) * f) * Mathf.Lerp (1f, v.Length () / v2.Length (), f);
	}

	public static Vector2 MoveTo (this Vector2 v2, Vector2 v, float length)
	{
		Vector2 result;
		v2.MoveTo (ref v, length, out result);
		return result;
	}

	public static void MoveTo (this Vector2 v2, ref Vector2 v, float length, out Vector2 result)
	{
		float num = v2.Distance (v);
		result = ((length >= num) ? v : v2.Lerp (v, length / num));
	}

	public static Vector2 TurnTo (this Vector2 v2, Vector2 v, float angle)
	{
		Vector2 result;
		v2.TurnTo (ref v, angle, out result);
		return result;
	}

	public static void TurnTo (this Vector2 v2, ref Vector2 v, float angle, out Vector2 result)
	{
		float num = v2.Angle (v);
		if (num < 0f)
		{
			num = -num;
		}
		result = ((angle >= num) ? v : v2.Slerp (v, angle / num));
	}

	public static float Angle (this Vector2 v2, Vector2 v)
	{
		return v2.Angle (ref v);
	}

	public static float Angle (this Vector2 v2, ref Vector2 v)
	{
		float num = v2.Dot (v) / (v2.Length () * v.Length ());
		if (num < -1f)
		{
			num = -1f;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		float num2 = (float)Math.Acos ((double)num);
		return (v2.x * v.y - v2.y * v.x >= 0f) ? num2 : (-num2);
	}

	public static Vector2 Rotate (this Vector2 v2, float angle)
	{
		Vector2 result;
		v2.Rotate (angle, out result);
		return result;
	}

	public static void Rotate (this Vector2 v2, float angle, out Vector2 result)
	{
		Vector2 vector;
		Math2D.Rotation (angle, out vector);
		v2.Rotate (ref vector, out result);
	}

	public static Vector2 Rotate (this Vector2 v2, Vector2 rotation)
	{
		Vector2 result;
		v2.Rotate (ref rotation, out result);
		return result;
	}

	public static void Rotate (this Vector2 v2, ref Vector2 rotation, out Vector2 result)
	{
		result.x = rotation.x * v2.x - rotation.y * v2.y;
		result.y = rotation.x * v2.y + rotation.y * v2.x;
	}

	public static Vector2 Reflect (this Vector2 v2, Vector2 normal)
	{
		Vector2 result;
		v2.Reflect (ref normal, out result);
		return result;
	}

	public static void Reflect (this Vector2 v2, ref Vector2 normal, out Vector2 result)
	{
		float num = v2.Dot (normal) / normal.LengthSquared ();
		Vector2 vector;
		normal.Multiply (2f * num, out vector);
		v2.Subtract (ref vector, out result);
	}

	public static Vector2 Perpendicular (this Vector2 v2)
	{
		Vector2 result;
		v2.Perpendicular (out result);
		return result;
	}

	public static void Perpendicular (this Vector2 v2, out Vector2 result)
	{
		result.x = -v2.y;
		result.y = v2.x;
	}

	public static Vector2 ProjectOnLine (this Vector2 v2, Vector2 point, Vector2 direction)
	{
		Vector2 result;
		v2.ProjectOnLine (ref point, ref direction, out result);
		return result;
	}

	public static void ProjectOnLine (this Vector2 v2, ref Vector2 point, ref Vector2 direction, out Vector2 result)
	{
		Vector2 vector;
		v2.Subtract (ref point, out vector);
		float f = direction.Dot (ref vector) / direction.LengthSquared ();
		direction.Multiply (f, out vector);
		point.Add (ref vector, out result);
	}

	public static Vector2 Add (this Vector2 v2, Vector2 v)
	{
		Vector2 result;
		v2.Add (ref v, out result);
		return result;
	}

	public static void Add (this Vector2 v2, ref Vector2 v, out Vector2 result)
	{
		result.x = v2.x + v.x;
		result.y = v2.y + v.y;
	}

	public static Vector2 Subtract (this Vector2 v2, Vector2 v)
	{
		Vector2 result;
		v2.Subtract (ref v, out result);
		return result;
	}

	public static void Subtract (this Vector2 v2, ref Vector2 v, out Vector2 result)
	{
		result.x = v2.x - v.x;
		result.y = v2.y - v.y;
	}

	public static Vector2 Multiply (this Vector2 v2, Vector2 v)
	{
		Vector2 result;
		v2.Multiply (ref v, out result);
		return result;
	}

	public static void Multiply (this Vector2 v2, ref Vector2 v, out Vector2 result)
	{
		result.x = v2.x * v.x;
		result.y = v2.y * v.y;
	}

	public static Vector2 Multiply (this Vector2 v2, float f)
	{
		Vector2 result;
		v2.Multiply (f, out result);
		return result;
	}

	public static void Multiply (this Vector2 v2, float f, out Vector2 result)
	{
		result.x = v2.x * f;
		result.y = v2.y * f;
	}

	public static Vector2 Divide (this Vector2 v2, Vector2 v)
	{
		Vector2 result;
		v2.Divide (ref v, out result);
		return result;
	}

	public static void Divide (this Vector2 v2, ref Vector2 v, out Vector2 result)
	{
		result.x = v2.x / v.x;
		result.y = v2.y / v.y;
	}

	public static Vector2 Divide (this Vector2 v2, float f)
	{
		Vector2 result;
		v2.Divide (f, out result);
		return result;
	}

	public static void Divide (this Vector2 v2, float f, out Vector2 result)
	{
		float num = 1f / f;
		result.x = v2.x * num;
		result.y = v2.y * num;
	}

	public static Vector2 Negate (this Vector2 v2)
	{
		Vector2 result;
		v2.Negate (out result);
		return result;
	}

	public static void Negate (this Vector2 v2, out Vector2 result)
	{
		result.x = -v2.x;
		result.y = -v2.y;
	}

	public static bool IsUnit (this Vector2 v2, float epsilon)
	{
		return Math.Abs (v2.Length () - 1f) <= epsilon;
	}

	public static bool IsZero (this Vector2 v2)
	{
		return v2.x == 0f && v2.y == 0f;
	}

	public static bool IsOne (this Vector2 v2)
	{
		return v2.x == 1f && v2.y == 1f;
	}

	public static bool IsInfinity (this Vector2 v2)
	{
		return float.IsInfinity (v2.x) || float.IsInfinity (v2.y);
	}

	public static bool IsNaN (this Vector2 v2)
	{
		return float.IsNaN (v2.x) || float.IsNaN (v2.y);
	}

	#endregion
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static Vector2 Rotation (float angle)
	{
		Vector2 result;
		Math2D.Rotation (angle, out result);
		return result;
	}

	public static void Rotation (float angle, out Vector2 result)
	{
		result.x = (float)Math.Cos ((double)angle);
		result.y = (float)Math.Sin ((double)angle);
	}

	public static float Repeat (float x, float min, float max)
	{
		float num = max - min;
		float num2 = (num == 0f) ? 0f : ((x - min) % num);
		return min + ((num2 >= 0f) ? num2 : (num2 + num));
	}

	public static float Det( Vector2 value1, Vector2 value2 ) 
	{
		return value1.x * value2.y - value1.y * value2.x;
	}
	
	public static float Sign( float x ) 
	{
		if ( x < 0.0f ) return -1.0f;
		if ( x > 0.0f ) return 1.0f;
		return 0.0f;
	}

	public static float Cross(Vector2 vec1, Vector2 vec2)
	{
		return vec1.x * vec2.y - vec1.y * vec2.x;
	}	
	
	public static Vector2 Cross(float val, Vector2 vec)
	{
		Vector2 result = new Vector2(-val * vec.y, val * vec.x);
		return result;
	}
	
	public static Vector2 Cross(Vector2 vec, float val)
	{
		Vector2 result = new Vector2(val * vec.y, -val * vec.x);
		return result;
	}

	public static Vector2 Perp( Vector2 value )
	{
		return new Vector2( -value.y, value.x );
	}
	
	public static Vector4 SetAlpha( Vector4 value, float w )
	{
		value.w = w;
		return value;
	}
	
	
	public static float SafeAcos( float x )
	{
		Math2D.Assert( Mathf.Abs( x ) - 1.0f < 1.0e-5f );
		return Mathf.Acos( Mathf.Clamp( x, -1.0f, 1.0f ) );	// clamp if necessary (we have checked that we are in in [-1,1] by an epsilon)
	}
	
	/// @if LANG_EN
	/// <summary>Return the absolute 2d angle formed by (1,0) and value, in range -pi,pi</summary>
	/// @endif
	public static float Angle( Vector2 value )
	{
		float angle = SafeAcos( value.normalized.x );
		return value.y < 0.0f ? -angle : angle;
	}
	
	
	public static Vector2 Rotate( Vector2 point, float angle, Vector2 pivot )
	{
		return pivot + ( point - pivot ).Rotate( angle );
	}

	public static float Deg2Rad( float value )
	{
		return value * 0.01745329251f;
	}

	public static float Rad2Deg( float value )
	{
		return value * 57.29577951308f;
	}

	public static Vector2 Deg2Rad( Vector2 value )
	{
		return value * 0.01745329251f;
	}

	public static Vector2 Rad2Deg( Vector2 value )
	{
		return value * 57.29577951308f;
	}

	public static float Lerp( float a, float b, float x )
	{
		return a + x * ( b - a );
	}

//	public static Vector2 Lerp( Vector2 a, Vector2 b, float x )
//	{
//		return a + x * ( b - a );
//	}

	public static Vector3 Lerp( Vector3 a, Vector3 b, float x )
	{
		return a + x * ( b - a );
	}

	public static Vector4 Lerp( Vector4 a, Vector4 b, float x )
	{
		return a + x * ( b - a );
	}

	public static Vector2 LerpUnitVectors( Vector2 va, Vector2 vb, float x )
	{
		return va.Rotate( va.Angle( vb ) * x );
	}

	public static float LerpAngles( float a, float b, float x )
	{
		return Angle( LerpUnitVectors( Math2D.Rotation( a ), Math2D.Rotation( b ), x )  );
	}

	public static float Sin( uint period, float phase, uint mstime )
	{
		return Mathf.Sin( ( ( ( ( mstime % period ) ) / period ) + phase ) * Pi * 2.0f );
	}

	public static float Sin( ulong period, float phase, ulong mstime )
	{
		return Mathf.Sin( ( ( ( ( mstime % period ) ) / period ) + phase ) * Pi * 2.0f );
	}
	
	/// @if LANG_EN
	/// <summary>
	/// A very controlable s curve, lets you do polynomial ease in/out curves
	/// with little code.
	/// </summary>
	/// <param name="x">Asssumed to be in 0,1.</param>
	/// <param name="p1">Controls the ease in exponent (if >1).</param>
	/// <param name="p2">Controls the ease out exponent (if >1.,(p1,p2)=(1,1) just gives f(x)=x</param>
	/// @endif
	public static float PowerfulScurve( float x, float p1, float p2 )
	{
		return Mathf.Pow( 1.0f - Mathf.Pow( 1.0f - x, p2 ), p1 );
	}
	
	/// @if LANG_EN
	/// <summary>Ease in curve using Pow.</summary>
	/// @endif
	public static float PowEaseIn( float x, float p )
	{
		return Mathf.Pow(x,p);
	}
	
	/// @if LANG_EN
	/// <summary>Ease out curve using Pow.</summary>
	/// @endif
	public static float PowEaseOut( float x, float p )
	{
		return 1.0f - PowEaseIn( 1.0f - x, p );
	}
	
	/// @if LANG_EN
	/// <summary>
	/// PowEaseIn/PowEaseOut mirrored around 0.5,0.5.
	/// Same exponent in and out.
	/// </summary>
	/// @endif
	public static float PowEaseInOut( float x, float p )
	{
		if ( x < 0.5f )	return 0.5f * PowEaseIn( x * 2.0f, p );
		return 0.5f + 0.5f * PowEaseOut( ( x - 0.5f ) * 2.0f, p );
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Ease out curve using a 1-exp(-a*x) exponential,
	/// but normalized so that we reach 1 when x=1.
	/// </summary>
	/// @endif
	public static float ExpEaseOut( float x, float a )
	{
		return( 1.0f - Mathf.Exp( - x * a ) ) / ( 1.0f - Mathf.Exp( - a ) );
	}
	
	/// @if LANG_EN
	/// <summary>Ease in curve using an exponential.</summary>
	/// @endif
	public static float ExpEaseIn( float x, float a )
	{
		return 1.0f - ExpEaseOut( 1.0f - x, a );
	}
	
	/// @if LANG_EN
	/// <summary>BackEaseIn function (see  http://www.robertpenner.com)</summary>
	/// @endif
	public static float BackEaseIn( float x, float a )
	{
		return x * x * ( ( a + 1.0f ) * x - a );
	}
	
	/// @if LANG_EN
	/// <summary>BackEaseOut function (see  http://www.robertpenner.com)</summary>
	/// @endif
	public static float BackEaseOut( float x, float a )
	{
		return 1.0f - BackEaseIn( 1.0f - x, a );
	}
	
	/// @if LANG_EN
	/// <summary>BackEaseIn/BackEaseOut mirrored around 0.5,0.5.</summary>
	/// @endif
	public static float BackEaseInOut( float x, float p )
	{
		if ( x < 0.5f )	return 0.5f * BackEaseIn( x * 2.0f, p );
		return 0.5f + 0.5f * BackEaseOut( ( x - 0.5f ) * 2.0f, p );
	}
	
	/// @if LANG_EN
	/// <summary>Impulse function (source Inigo Quilez).</summary>
	/// @endif
	public static float Impulse( float x, float b )
	{
		float h = b * x;
		return h * Mathf.Exp( 1.0f - h );
	}
	
	/// @if LANG_EN
	/// <summary>Travelling wave function.</summary>
	/// @endif
	public static float ShockWave( float d
		                , float time
		                , float wave_half_width
		                , float wave_speed
		                , float wave_fade
		                , float d_scale )
	{
		d *= d_scale;
		float travelled = time * wave_speed;
		float x = Mathf.Clamp( d - travelled, -wave_half_width, wave_half_width ) / wave_half_width; // -1,1 parameter
		float wave = ( 1.0f + Mathf.Cos( Pi * x ) ) * 0.5f;
		return wave * Mathf.Exp( -d * wave_fade );
	}
	
	/// @if LANG_EN
	/// <summary>Return the log of v in base 2.</summary>
	/// @endif
	public static int Log2( int v )
	{
		int r;
		int shift;
		r =     (v > 0xFFFF?1:0) << 4; v >>= r;
		shift = (v > 0xFF  ?1:0) << 3; v >>= shift; r |= shift;
		shift = (v > 0xF   ?1:0) << 2; v >>= shift; r |= shift;
		shift = (v > 0x3   ?1:0) << 1; v >>= shift; r |= shift;
		r |= (v >> 1);
		return r;
	}
	
	/// @if LANG_EN
	/// <summary>Return true if 'i' is a power of 2.</summary>
	/// @endif
	public static bool IsPowerOf2( int i )
	{
		return ( 1 << Log2(i) ) == i;
	}
	
	/// @if LANG_EN
	/// <summary>Return the closest greater or equal power of 2.</summary>
	/// @endif
	public static int GreatestOrEqualPowerOf2( int i )
	{
		int p = ( 1 << Log2(i) );
		return p < i ? 2 * p : p;
	}

	public static float LineLineIntersection(Vector2 PosA, Vector2 PosA2, Vector2 PosB, Vector2 PosB2)
	{
		
		Vector2 dirA = PosA2 - PosA;
		Vector2 dirB = PosB2 - PosB;
		Vector2 normalB = new Vector2(-dirB.y, dirB.x);
		
		float dot = Vector2.Dot (dirA, normalB);
		
		if(dot == 0.0f)
			return -1.0f;
		
		float ratioA = Vector2.Dot(PosB - PosA, normalB)/dot;
		
		if((ratioA < 0.0f) || (ratioA > 1.0f))
			return -1.0f;
		
		Vector2 crossPos = PosA + ratioA * dirA;
		
		float ratioB = Vector2.Dot (crossPos - PosB, dirB)/Vector2.Dot (dirB, dirB);
		
		if((ratioB < 0.0f) || (ratioB > 1.0f))
			return -1.0f;
		else
			return ratioA;
		
	}
	
	// some constants

	public static Vector2 _00 = new Vector2(0.0f,0.0f);
	public static Vector2 _10 = new Vector2(1.0f,0.0f);
	public static Vector2 _01 = new Vector2(0.0f,1.0f);
	public static Vector2 _11 = new Vector2(1.0f,1.0f);
	
	public static Vector3 _000 = new Vector3(0.0f,0.0f,0.0f);
	public static Vector3 _100 = new Vector3(1.0f,0.0f,0.0f);
	public static Vector3 _010 = new Vector3(0.0f,1.0f,0.0f);
	public static Vector3 _110 = new Vector3(1.0f,1.0f,0.0f);
	public static Vector3 _001 = new Vector3(0.0f,0.0f,1.0f);
	public static Vector3 _101 = new Vector3(1.0f,0.0f,1.0f);
	public static Vector3 _011 = new Vector3(0.0f,1.0f,1.0f);
	public static Vector3 _111 = new Vector3(1.0f,1.0f,1.0f);
	
	public static Vector4 _0000 = new Vector4(0.0f,0.0f,0.0f,0.0f);
	public static Vector4 _1000 = new Vector4(1.0f,0.0f,0.0f,0.0f);
	public static Vector4 _0100 = new Vector4(0.0f,1.0f,0.0f,0.0f);
	public static Vector4 _1100 = new Vector4(1.0f,1.0f,0.0f,0.0f);
	public static Vector4 _0010 = new Vector4(0.0f,0.0f,1.0f,0.0f);
	public static Vector4 _1010 = new Vector4(1.0f,0.0f,1.0f,0.0f);
	public static Vector4 _0110 = new Vector4(0.0f,1.0f,1.0f,0.0f);
	public static Vector4 _1110 = new Vector4(1.0f,1.0f,1.0f,0.0f);
	public static Vector4 _0001 = new Vector4(0.0f,0.0f,0.0f,1.0f);
	public static Vector4 _1001 = new Vector4(1.0f,0.0f,0.0f,1.0f);
	public static Vector4 _0101 = new Vector4(0.0f,1.0f,0.0f,1.0f);
	public static Vector4 _1101 = new Vector4(1.0f,1.0f,0.0f,1.0f);
	public static Vector4 _0011 = new Vector4(0.0f,0.0f,1.0f,1.0f);
	public static Vector4 _1011 = new Vector4(1.0f,0.0f,1.0f,1.0f);
	public static Vector4 _0111 = new Vector4(0.0f,1.0f,1.0f,1.0f);
	public static Vector4 _1111 = new Vector4(1.0f,1.0f,1.0f,1.0f);
	
	/// @if LANG_EN
	/// <summary>Return the closest point to P that's on segment [A,B].</summary>
	/// @endif
	public static Vector2 ClosestSegmentPoint( Vector2 P, Vector2 A, Vector2 B )
	{
		Vector2 AB = B - A;
		
		if ( ( P - A ).Dot( AB ) <= 0.0f ) return A;
		if ( ( P - B ).Dot( AB ) >= 0.0f ) return B;
		
		return P.ProjectOnLine( A, AB );
	}
}

/// @if LANG_EN
/// <summary>Common interface for Bounds2, Sphere2, ConvexPoly2.</summary>
/// @endif
public interface ICollisionBasics
{
	/// @if LANG_EN
	/// <summary>
	/// Return true if 'point' is inside the primitive (in its negative space).
	/// </summary>
	/// @endif
	bool IsInside( Vector2 point );
	/// @if LANG_EN
	/// <summary>
	/// Return the closest point to 'point' that lies on the surface of the primitive.
	/// If that point is inside the primitive, sign is negative.
	/// </summary>
	/// @endif
	void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign );	
	/// @if LANG_EN
	/// <summary>
	/// Return the signed distance (penetration distance) from 'point' 
	/// to the surface of the primitive.
	/// </summary>
	/// @endif
	float SignedDistance( Vector2 point );
	/// @if LANG_EN
	/// <summary>
	/// Assuming the primitive is convex, clip the segment AB against the primitive.
	/// Return false if AB is entirely in positive halfspace,
	/// else clip against negative space and return true.
	/// </summary>
	/// @endif
	bool NegativeClipSegment( ref Vector2 A, ref Vector2 B );
}

/// @if LANG_EN
/// <summary>
/// An axis aligned box class in 2D.
/// </summary>
/// @endif
public struct Bounds2 : ICollisionBasics
{
	/// @if LANG_EN
	/// <summary>Minimum point (lower left).</summary>
	/// @endif
	public Vector2 Min;
	
	/// @if LANG_EN
	/// <summary>Maximum point (upper right)</summary>
	/// @endif
	public Vector2 Max;
	
	/// @if LANG_EN
	/// <summary>The Width/Height ratio.</summary>
	/// @endif
	public float Aspect { get { Vector2 size = Size; return size.x / size.y;}} 
	
	/// @if LANG_EN
	/// <summary>The center of the bounds (Max+Min)/2.</summary>
	/// @endif
	public Vector2 Center { get { return( Max + Min ) * 0.5f;}} 
	
	/// @if LANG_EN
	/// <summary>The Size the bounds (Max-Min).</summary>
	/// @endif
	public Vector2 Size { get { return( Max - Min );}} 
	
	/// @if LANG_EN
	/// <summary>Return true if the size is (0,0).</summary>
	/// @endif
	public bool IsEmpty()
	{
		return Max == Min;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Bounds2 constructor.
	/// All functions in Bounds2 assume that Min is less or equal Max. If it is not the case, the user takes responsability for it.
	/// SafeBounds will ensure this is the case whatever the input is, but the default constructor will just blindly
	/// takes anything the user passes without trying to fix it.
	/// </summary>
	/// <param name="min">The bottom left point. Min is set to that value without further checking.</param>
	/// <param name="max">The top right point. Max is set to that value without further checking.</param>
	/// @endif
	public Bounds2( Vector2 min, Vector2 max )
	{
		Min = min; 
		Max = max;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Bounds2 constructor. 
	/// Return a zero size bounds. You can then use Add to expand it. 
	/// </summary>
	/// <param name="point">Location of the Bounds2.</param>
	/// @endif
	public Bounds2( Vector2 point )
	{
		Min = point; 
		Max = point;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Create a Bounds2 that goes through 2 points, the min and max are recalculated.
	/// </summary>
	/// <param name="min">First point.</param>
	/// <param name="max">Second point.</param>
	/// @endif
	static public Bounds2 SafeBounds( Vector2 min, Vector2 max )
	{
		return new Bounds2( min.Min( max ), min.Max( max ) );
	}
	
	/// @if LANG_EN
	/// <summary>(0,0) -> (0,0) box.</summary>
	/// @endif
	static public Bounds2 Zero = new Bounds2( new Vector2(0.0f,0.0f), new Vector2(0.0f,0.0f) );
	
	/// @if LANG_EN
	/// <summary>(0,0) -> (1,1) box.</summary>
	/// @endif
	static public Bounds2 Quad0_1 = new Bounds2( new Vector2(0.0f,0.0f), new Vector2(1.0f,1.0f) );
	
	/// @if LANG_EN
	/// <summary>(-1,-1) -> (1,1) box.</summary>
	/// @endif
	static public Bounds2 QuadMinus1_1 = new Bounds2( new Vector2(-1.0f,-1.0f), new Vector2(1.0f,1.0f) );
	
	/// @if LANG_EN
	/// <summary>
	/// Return a box that goes from (-h,-h) to (h,h).
	/// We don't check for sign.
	/// </summary>
	/// <param name="h">Half size of the square.</param>
	/// @endif
	static public Bounds2 CenteredSquare( float h )
	{
		Vector2 half_vec = new Vector2( h, h );
		return new Bounds2( -half_vec, half_vec );
	}
	
	/// @if LANG_EN
	/// <summary>Translate bounds.</summary>
	/// @endif
	public static Bounds2 operator + ( Bounds2 bounds, Vector2 value )
	{
		return new Bounds2( bounds.Min + value, 
		                   bounds.Max + value );
	}
	
	/// @if LANG_EN
	/// <summary>Translate bounds.</summary>
	/// @endif
	public static Bounds2 operator - ( Bounds2 bounds, Vector2 value )
	{
		return new Bounds2( bounds.Min - value, bounds.Max - value );
	}
	
	/// @if LANG_EN
	/// <summary>Return true if this and 'bounds' overlap.</summary>
	/// @endif
	public bool Overlaps( Bounds2 bounds )
	{
		if ( Min.x > bounds.Max.x || bounds.Min.x > Max.x )	return false;
		if ( Min.y > bounds.Max.y || bounds.Min.y > Max.y )	return false;
		
		return true;
	}
	
	/// @if LANG_EN
	/// <summary>Return the Bounds2 resulting from the intersection of 2 bounds.</summary>
	/// @endif
	public Bounds2 Intersection( Bounds2 bounds ) 
	{
		Vector2 mi = Min.Max( bounds.Min );
		Vector2 ma = Max.Min( bounds.Max );
		
		Vector2 dim = ma - mi;
		
		if ( dim.x < 0.0f || dim.y < 0.0f )
			return Zero;
		
		return new Bounds2( mi, ma );
	} 
	
	/// @if LANG_EN
	/// <summary>
	/// Scale bounds around a given pivot.
	/// </summary>
	/// <param name="scale">Amount of scale.</param>
	/// <param name="center">Scale center.</param>
	/// @endif
	public Bounds2 Scale( Vector2 scale, Vector2 center )
	{
		return new Bounds2( ( Min - center ).Multiply(scale) + center, ( Max - center ).Multiply(scale) + center );
	}
	
	/// @if LANG_EN
	/// <summary>Add the contribution of 'point' to this Bounds2.</summary>
	/// @endif
	public void Add( Vector2 point )
	{
		Min = Min.Min( point );
		Max = Max.Max( point );
	}
	
	/// @if LANG_EN
	/// <summary>Add the contribution of 'bounds' to this Bounds2.</summary>
	/// @endif
	public void Add( Bounds2 bounds )
	{
		Add( bounds.Min );
		Add( bounds.Max );
	}
	
	// Note about PointXX: first column is x, second is y (0 means min, 1 means max, you can also see those as 'uv')
	
	/// @if LANG_EN
	/// <summary>The bottom left point (which is also Min).</summary>
	/// @endif
	public Vector2 Point00 { get { return Min;}}
	/// @if LANG_EN
	/// <summary>The top right point (which is also Max).</summary>
	/// @endif
	public Vector2 Point11 { get { return Max;}}
	/// @if LANG_EN
	/// <summary>The bottom right point.</summary>
	/// @endif
	public Vector2 Point10 { get { return new Vector2(Max.x,Min.y);}}
	/// @if LANG_EN
	/// <summary>The top left point.</summary>
	/// @endif
	public Vector2 Point01 { get { return new Vector2(Min.x,Max.y);}}
	
	/// @if LANG_EN
	/// <summary>Return the string representation of this Bounds2.</summary>
	/// @endif
	public override string ToString()
	{
		return Min.ToString() + " " + Max.ToString();
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return true if 'point' is inside the primitive (in its negative space).
	/// </summary>
	/// @endif
	public bool IsInside( Vector2 point )
	{
		return point == point.Max( Min ).Min( Max );
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return the closest point to 'point' that lies on the surface of the primitive.
	/// If that point is inside the primitive, sign is negative.
	/// </summary>
	/// @endif
	public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
	{
		Vector2 closest = point.Max( Min ).Min( Max );
		
		if ( closest != point )
		{
			ret = closest;
			sign = 1.0f;
			return;
		}
		
		Vector2 l = closest; l.x = Min.x; float dl = closest.x - Min.x; 
		Vector2 r = closest; r.x = Max.x; float dr = Max.x - closest.x; 
		Vector2 t = closest; t.y = Min.y; float dt = closest.y - Min.y; 
		Vector2 b = closest; b.y = Max.y; float db = Max.y - closest.y; 
		
		ret = l; 
		float d = dl;
		
		if ( d > dr )
		{
			ret = r; 
			d = dr;
		}
		
		if ( d > dt )
		{
			ret = t; 
			d = dt;
		}
		
		if ( d > db )
		{
			ret = b; 
			d = db;
		}
		
		sign = -1.0f;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return the signed distance (penetration distance) from 'point' 
	/// to the surface of the primitive.
	/// </summary>
	/// @endif
	public float SignedDistance( Vector2 point )
	{
		Vector2 p;
		float s = 0.0f;
		ClosestSurfacePoint( point, out p, out s );
		return s * ( p - point ).Length();
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Assuming the primitive is convex, clip the segment AB against the primitive.
	/// Return false if AB is entirely in positive halfspace,
	/// else clip against negative space and return true.
	/// </summary>
	/// @endif
	public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
	{
		bool ret = true;
		
		ret &= ( new Plane2( Min, -Math2D._10 ) ).NegativeClipSegment( ref A, ref B );
		ret &= ( new Plane2( Min, -Math2D._01 ) ).NegativeClipSegment( ref A, ref B );
		ret &= ( new Plane2( Max,  Math2D._10 ) ).NegativeClipSegment( ref A, ref B );
		ret &= ( new Plane2( Max,  Math2D._01 ) ).NegativeClipSegment( ref A, ref B );
		
		return ret;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Swap y coordinates for top and bottom, handy for hacking uvs
	/// in system that use 0,0 as top left. Also, this will generate
	/// an invalid Bounds2 and all functions in that class will break
	/// (intersections, add etc.)
	/// 
	/// Functions like Point00, Point10 etc can still be used.
	/// </summary>
	/// @endif
	public Bounds2 OutrageousYTopBottomSwap()
	{
		Bounds2 ret = this;
		float y = Min.y;
		ret.Min.y = ret.Max.y;
		ret.Max.y = y;
		return ret;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Similar to OutrageousYTopBottomSwap, but instead of
	/// swapping top and bottom y, it just does y=1-y. Same
	/// comment as OutrageousYTopBottomSwap.
	/// </summary>
	/// @endif
	public Bounds2 OutrageousYVCoordFlip()
	{
		Bounds2 ret = this;
		ret.Min.y = 1.0f - ret.Min.y;
		ret.Max.y = 1.0f - ret.Max.y;
		return ret;
	}
}

/// @if LANG_EN
/// <summary>A plane class in 2D.</summary>
/// @endif
public struct Plane2 : ICollisionBasics
{
	/// @if LANG_EN
	/// <summary>A base point on the plane.</summary>
	/// @endif
	public Vector2 Base;
	
	/// @if LANG_EN
	/// <summary>The plane normal vector, assumed to be unit length. If this is not the case, some functions will have undefined behaviour.</summary>
	/// @endif
	public Vector2 UnitNormal;
	
	/// @if LANG_EN
	/// <summary>Plane2 constructor</summary>
	/// @endif
	public Plane2( Vector2 a_base, Vector2 a_unit_normal )
	{
		Base = a_base;
		UnitNormal = a_unit_normal;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return true if 'point' is inside the primitive (in its negative space).
	/// </summary>
	/// @endif
	public bool IsInside( Vector2 point )
	{
		return SignedDistance( point ) <= 0.0f;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return the closest point to 'point' that lies on the surface of the primitive.
	/// If that point is inside the primitive, sign is negative.
	/// </summary>
	/// @endif
	public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
	{
		float d = SignedDistance( point );
		ret = point - d * UnitNormal;
		sign = d > 0.0f ? 1.0f : -1.0f;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return the signed distance (penetration distance) from 'point' 
	/// to the surface of the primitive.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// Return the signed distance (penetration distance) from 'point' 
	/// to the surface of the primitive.
	/// </summary>
	/// @endif
	public float SignedDistance( Vector2 point )
	{
		return( point - Base ).Dot( UnitNormal );
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Project a point on this plane.
	/// </summary>
	/// @endif
	/// @if LANG_JA
	/// <summary>
	/// Project a point on this plane.
	/// </summary>
	/// @endif
	public Vector2 Project( Vector2 point )
	{
		return point - SignedDistance( point ) * UnitNormal;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Assuming the primitive is convex, clip the segment AB against the primitive.
	/// Return false if AB is entirely in positive halfspace,
	/// else clip against negative space and return true.
	/// </summary>
	/// @endif
	public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
	{
		float dA = SignedDistance( A );
		float dB = SignedDistance( B );
		
		bool Ain = ( dA >= 0.0f );
		bool Bin = ( dB >= 0.0f );
		
		if ( Ain && Bin )
			return false;
		
		if ( Ain && (!Bin) )
		{
			Vector2 AB = B - A;
			float alpha = -dA / AB.Dot( UnitNormal );
			Vector2 I = A + alpha * AB;
			A = I;
		}
		else if ( (!Ain) && Bin )
		{
			Vector2 AB = B - A;
			float alpha = -dA / AB.Dot( UnitNormal );
			Vector2 I = A + alpha * AB;
			B = I;
		}
		
		return true;
	}
}

/// @if LANG_EN
/// <summary>A sphere class in 2D.</summary>
/// @endif
public struct Sphere2 : ICollisionBasics
{
	/// @if LANG_EN
	/// <summary>Sphere center.</summary>
	/// @endif
	public Vector2 Center;
	
	/// @if LANG_EN
	/// <summary>Sphere radius.</summary>
	/// @endif
	public float Radius; 
	
	/// @if LANG_EN
	/// <summary>Sphere2 constructor.</summary>
	/// @endif
	public Sphere2( Vector2 center, float radius )
	{
		Center = center;
		Radius = radius;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return true if 'point' is inside the primitive (in its negative space).
	/// </summary>
	/// @endif
	public bool IsInside( Vector2 point )
	{
		return( point - Center ).LengthSquared() <= Radius * Radius;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return the closest point to 'point' that lies on the surface of the primitive.
	/// If that point is inside the primitive, sign is negative.
	/// </summary>
	/// @endif
	public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
	{
		Vector2 r = point - Center;
		float len = r.Length();
		float d = len - Radius;
		
		if ( len < 0.00001f )
		{
			ret = Center + new Vector2( 0.0f, Radius );	// degenerate case, pick any separation direction
			sign = -1.0f;
			return;
		}
		
		ret = point - d * ( r / len );
		sign = d > 0.0f ? 1.0f : -1.0f;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return the signed distance (penetration distance) from 'point' 
	/// to the surface of the primitive.
	/// </summary>
	/// @endif
	public float SignedDistance( Vector2 point )
	{
		return( point - Center ).Length() - Radius;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Assuming the primitive is convex, clip the segment AB against the primitive.
	/// Return false if AB is entirely in positive halfspace,
	/// else clip against negative space and return true.
	/// </summary>
	/// @endif
	public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B )
	{
		Vector2 AB = B - A;
		float r_sqr = Radius * Radius;
		
		float epsilon = 0.00000001f;
		if ( AB.LengthSquared() <= epsilon )
		{
			// A and B are the same point
			if ( ( A - Center ).LengthSquared() >= r_sqr )
				return false;
		}
		
		Vector2 p = Center.ProjectOnLine( A, AB );
		float d_sqr = ( p - Center ).LengthSquared();
		
		if ( d_sqr >= r_sqr )
			return false;
		
		float e = Mathf.Sqrt( Mathf.Max( 0.0f, r_sqr - d_sqr ) );
		Vector2 v = AB.GetNormalized();
		Vector2 A2 = p - e * v;
		Vector2 B2 = p + e * v;
		
		if ( ( A - B2 ).Dot( AB ) >= 0.0f ) return false;
		if ( ( B - A2 ).Dot( AB ) <= 0.0f ) return false;
		
		if ( ( A - A2 ).Dot( AB ) < 0.0f ) A = A2;
		if ( ( B - B2 ).Dot( AB ) > 0.0f ) B = B2;
		
		return true;
	}
}

/// @if LANG_EN
/// <summary>A convex polygon class in 2D.</summary>
/// @endif
public struct ConvexPoly2 : ICollisionBasics
{
	/// @if LANG_EN
	/// <summary>
	/// The convex poly is stored as a list of planes assumed to define a 
	/// convex region. Plane base points are also polygon vertices.
	/// </summary>
	/// @endif
	public Plane2[] Planes; 
	
	Sphere2 m_sphere;
	
	/// @if LANG_EN
	/// <summary>Bounding sphere, centered at center of mass.</summary>
	/// @endif
	public Sphere2 Sphere{ get { return m_sphere; } }
	
	/// @if LANG_EN
	/// <summary>
	/// ConvexPoly2 constructor.
	/// Assumes input points define a convex region.
	/// </summary>
	/// @endif
	public ConvexPoly2( Vector2[] points )
	{
		Vector2 center = Math2D._00;
		Planes = new Plane2[ points.Length ];
		
		for ( int n=points.Length,i=n-1,i_next=0; i_next < n; i=i_next++ )
		{
			Vector2 p1 = points[i];
			Vector2 p2 = points[i_next];
			
			Planes[i] = new Plane2( p1, Math2D.Perp( p2 - p1 ).GetNormalized() );
			
			center += p1;
		}
		
		center /= (float)points.Length;
		
		float radius = 0.0f;
		for ( int i = 0; i != points.Length; ++i )
			radius = Mathf.Max( radius, ( points[i] - center ).Length() );
		
		m_sphere = new Sphere2( center, radius );
	}
	
	public void MakeBox( Bounds2 bounds )
	{
		Planes = new Plane2[ 4 ];
		
		Planes[0] = new Plane2( bounds.Point00, -Math2D._10 );
		Planes[1] = new Plane2( bounds.Point10, -Math2D._01 );
		Planes[2] = new Plane2( bounds.Point11,  Math2D._10 );
		Planes[3] = new Plane2( bounds.Point01,  Math2D._01 );
		
		m_sphere = new Sphere2( bounds.Center, bounds.Size.Length() * 0.5f );
	}
	
	public void MakeRegular( uint num, float r )
	{
		Planes = new Plane2[ num ];
		
		float a2 = Math2D.TwicePi * 0.5f / (float)num;
		
		for ( uint i = 0; i != num; ++i )
		{
			float a = Math2D.TwicePi * (float)i / (float)num;
			Vector2 p = Math2D.Rotation( a + a2 );
			Vector2 n = Math2D.Rotation( a );
			Planes[i] = new Plane2( p * r, n );
		}
		
		m_sphere = new Sphere2( Math2D._00, r );
	}
	
	/// @if LANG_EN
	/// <summary>Return the number of vertices (or faces)</summary>
	/// @endif
	public uint Size()
	{
		return(uint)Planes.Length;
	}
	
	/// @if LANG_EN
	/// <summary>Get a vertex position.</summary>
	/// <param name="index">The vertex index.</param>
	/// @endif
	public Vector2 GetPoint( int index )
	{
		return Planes[index].Base;
	}
	
	/// @if LANG_EN
	/// <summary>Get the normal vector of a face of this poly.</summary>
	/// <param name="index">The face index.</param>
	/// @endif
	public Vector2 GetNormal( int index )
	{
		return Planes[index].UnitNormal;
	}
	
	/// @if LANG_EN
	/// <summary>Get the plane formed by a face of this poly.</summary>
	/// <param name="index">The face index.</param>
	/// @endif
	public Plane2 GetPlane( int index )
	{
		return Planes[index];
	}
	
	/// @if LANG_EN
	/// <summary>Calculate the bounds of this poly.</summary>
	/// @endif
	public Bounds2 CalcBounds()
	{
		if ( Size() == 0 )
			return Bounds2.Zero;
		
		Bounds2 retval = new Bounds2( GetPoint( 0 ), GetPoint( 0 ) );
		for ( int i = 1; i != (int)Size(); ++i )
			retval.Add( GetPoint( i ) );
		return retval;
	}
	
	/// @if LANG_EN
	/// <summary>Calculate the gravity center of this poly.</summary>
	/// @endif
	public Vector2 CalcCenter()
	{
		Vector2 center = Math2D._00;
		float area = 0.0f;
		
		for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
		{
			Vector2 A = GetPoint( i );
			Vector2 B = GetPoint( i_next );
			float det = Math2D.Det( A, B );
			area += det;
			center += det * ( A + B );
		}
		
		area /= 2.0f;
		center /= ( 6.0f * area );
		
		return center;
	}
	
	/// @if LANG_EN
	/// <summary>Calculate the area of this convex poly.</summary>
	/// @endif
	public float CalcArea()
	{
		float area = 0.0f;
		
		for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
			area += Math2D.Det( GetPoint( i ), GetPoint( i_next ) );
		
		area /= 2.0f;
		
		return area;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return true if 'point' is inside the primitive (in its negative space).
	/// </summary>
	/// @endif
	public bool IsInside(  Vector2 point ) 
	{
		foreach ( Plane2 plane in Planes )
		{
			if ( plane.SignedDistance( point ) > 0.0f )
				return false;
		}
		
		return true;
	}

	public bool IntersectLine(Vector2 p0, Vector2 p1, out Vector2 result)
	{
		result = Vector2.zero;

		bool haveIntersection = false;
		for (uint j = Size() - 1, i = 0; i < Size(); j = i, i++)   // process polygon edge V[i]V[i+1]
		{
			float ratio = Math2D.LineLineIntersection(p0, p1, Planes[j].Base, Planes[i].Base);

			if(ratio != -1f){
				result = p0 + (p1 - p0) * ratio;
				haveIntersection = true;
				break;
			}
		}

		return haveIntersection;
	}

	public bool ContainsPoint (Vector2 point)
	{ 
		bool result = false;
		for (uint i = 0, j = Size() - 1; i < Size(); j = i++) {
			if ((Planes[i].Base.y > point.y) != (Planes[j].Base.y > point.y) &&
			    (point.x < (Planes[j].Base.x - Planes[i].Base.x) * (point.y - Planes[i].Base.y) / (Planes[j].Base.y-Planes[i].Base.y) + Planes[i].Base.x)) {
				result = !result;
			}
		}
		return result;
	}
	
	public float ClosestSurfacePoint( Vector2 point)
	{
		float sign = 1;
		Vector2 ret = Vector2.zero;
		ClosestSurfacePoint( point, out ret, out sign );
		return sign;
	}

	/// @if LANG_EN
	/// <summary>
	/// Return the closest point to 'point' that lies on the surface of the primitive.
	/// If that point is inside the primitive, sign is negative.
	/// </summary>
	/// @endif
	public void ClosestSurfacePoint( Vector2 point, out Vector2 ret, out float sign )
	{
		ret = Math2D._00;
		
		float max_neg_d = -100000.0f;
		int max_neg_d_plane_index = -1;
		bool outside = false;
		
		for ( int i=0; i != Planes.Length; ++i )
		{
			float d = Planes[i].SignedDistance( point );
			
			if ( d > 0.0f )	outside = true;
			else if ( max_neg_d < d )
			{
				max_neg_d = d;
				max_neg_d_plane_index = i;
			}
		}
		
		if ( !outside )
		{
			sign = -1.0f;
			ret = point - max_neg_d * Planes[max_neg_d_plane_index].UnitNormal;
			return;
		}
		
		// brute force
		
		float d_sqr_min = 0.0f;
		
		for ( int n=(int)Size(),i=n-1,i_next=0; i_next < n; i=i_next++ )
		{
			Vector2 p = Math2D.ClosestSegmentPoint( point, GetPoint( i ), GetPoint( i_next ) );
			float d_sqr = ( p - point ).LengthSquared();
			
			if ( i==n-1 || d_sqr < d_sqr_min )
			{
				ret = p;
				d_sqr_min = d_sqr;
			}
		}
		
		sign = 1.0f;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Return the signed distance (penetration distance) from 'point' 
	/// to the surface of the primitive.
	/// </summary>
	/// @endif
	public float SignedDistance( Vector2 point )
	{
		Vector2 p;
		float s = 0.0f;
		ClosestSurfacePoint( point, out p, out s );
		return s * ( p - point ).Length();
	}
	
	public void Translate( Vector2 dx, ConvexPoly2 poly )
	{
		Planes = new Plane2[ poly.Planes.Length ];
		
		for ( int i=0; i != poly.Planes.Length; ++i )
		{
			Planes[i] = poly.Planes[i];
			Planes[i].Base += dx;
		}
		
		m_sphere = poly.m_sphere;
		m_sphere.Center += dx;
	}
	
	/// @if LANG_EN
	/// <summary>
	/// Assuming the primitive is convex, clip the segment AB against the primitive.
	/// Return false if AB is entirely in positive halfspace,
	/// else clip against negative space and return true.
	/// </summary>
	/// @endif
	public bool NegativeClipSegment( ref Vector2 A, ref Vector2 B ) 
	{
		for ( int i=0; i != Planes.Length; ++i )
		{
			if ( !Planes[i].NegativeClipSegment( ref A, ref B ) )
				return false;
		}
		
		return true;
	}
}