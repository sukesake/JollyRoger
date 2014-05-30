float3 eyePos : VIEWPOS;
float3 sunPos : SUN;
float  turbidity : CUSTOMFLOAT;
float4x4 matView : VIEW;
float4x4 matWVP : WORLDVIEWPROJECTION;
float4x4 matWorld : WORLD;
float4x4 matProjection : PROJECTION;

struct VS_INPUT 
{
   float3 Position : POSITION;
   float3 Normal : NORMAL;
   //float2 TextureCoordinate : TEXCOORD;
};

struct VS_OUTPUT 
{
   float4 Position : POSITION;
   float3 l : TEXCOORD0;
   float3 v : TEXCOORD1;
   float3 Color : COLOR;
};

float3 perezZenith ( float t, float thetaSun )
{
  float  pi = 3.1415926;
  float4  cx1 = float4( 0,       0.00209, -0.00375, 0.00165  );
  float4  cx2 = float4( 0.00394, -0.03202, 0.06377, -0.02903 );
  float4  cx3 = float4( 0.25886, 0.06052, -0.21196, 0.11693  );
  float4  cy1 = float4( 0.0,     0.00317, -0.00610, 0.00275  );
  float4  cy2 = float4( 0.00516, -0.04153, 0.08970, -0.04214 );
  float4  cy3 = float4( 0.26688, 0.06670, -0.26756, 0.15346  );

  float  t2    = t*t;
  float  chi   = (4.0 / 9.0 - t / 120.0 ) * (pi - 2.0 * thetaSun );
  float4  theta = float4( 1, thetaSun, thetaSun*thetaSun, thetaSun*thetaSun*thetaSun );

  float  Y = (4.0453 * t - 4.9710) * tan ( chi ) - 0.2155 * t + 2.4192;
  float  x = t2 * dot ( cx1, theta ) + t * dot ( cx2, theta ) + dot ( cx3, theta );
  float  y = t2 * dot ( cy1, theta ) + t * dot ( cy2, theta ) + dot ( cy3, theta );

  return float3( Y, x, y );
}


float3 perezFunc ( float t, float cosTheta, float cosGamma )
{
    float  gamma      = acos ( cosGamma );
    float  cosGammaSq = cosGamma * cosGamma;
    float  aY =  0.17872 * t - 1.46303;
    float  bY = -0.35540 * t + 0.42749;
    float  cY = -0.02266 * t + 5.32505;
    float  dY =  0.12064 * t - 2.57705;
    float  eY = -0.06696 * t + 0.37027;
    float  ax = -0.01925 * t - 0.25922;
    float  bx = -0.06651 * t + 0.00081;
    float  cx = -0.00041 * t + 0.21247;
    float  dx = -0.06409 * t - 0.89887;
    float  ex = -0.00325 * t + 0.04517;
    float  ay = -0.01669 * t - 0.26078;
    float  by = -0.09495 * t + 0.00921;
    float  cy = -0.00792 * t + 0.21023;
    float  dy = -0.04405 * t - 1.65369;
    float  ey = -0.01092 * t + 0.05291;

    return float3 ( (1.0 + aY * exp(bY/cosTheta)) * (1.0 + cY * exp(dY * gamma) + eY*cosGammaSq),
                  (1.0 + ax * exp(bx/cosTheta)) * (1.0 + cx * exp(dx * gamma) + ex*cosGammaSq),
                  (1.0 + ay * exp(by/cosTheta)) * (1.0 + cy * exp(dy * gamma) + ey*cosGammaSq) );
}



float3 perezSky ( float t, float cosTheta, float cosGamma, float cosThetaSun )
{
    float thetaSun = acos        ( cosThetaSun );
    float3 zenith   = perezZenith ( t, thetaSun );
    float3 clrYxy   = zenith * perezFunc ( t, cosTheta, cosGamma ) / perezFunc ( t, 1.0, cosThetaSun );
    clrYxy [0] *= smoothstep ( 0.0, 0.1, cosThetaSun );      // make sure when thetaSun > PI/2 we have black color
  
    return clrYxy;
}

float3 convertColor (float3 colorYxy)
{
    float3 clrYxy = float3( colorYxy );
                                            // now rescale Y component
  //  clrYxy [0] = 1.0 - exp ( -clrYxy [0] / 25.0 );
 clrYxy [0] = 1.0 - exp ( -clrYxy [0] / turbidity );

    float ratio = clrYxy [0] / clrYxy [2];  // Y / y = X + Y + Z
    float3 XYZ;

    XYZ.x = clrYxy [1] * ratio;             // X = x * ratio
    XYZ.y = clrYxy [0];                     // Y = Y
    XYZ.z = ratio - XYZ.x - XYZ.y;          // Z = ratio - X - Y

    float3 rCoeffs = float3 ( 3.240479, -1.53715, -0.49853  );
    float3 gCoeffs = float3 ( -0.969256, 1.875991, 0.041556 );
    float3 bCoeffs = float3 ( 0.055684, -0.204043, 1.057311 );

    return float3 ( dot ( rCoeffs, XYZ ), dot ( gCoeffs, XYZ ), dot ( bCoeffs, XYZ ) );
}

float3 Test(float3 inp)
{
return float3(inp.x * 2,inp.y * 2, inp.z * 2);
}


VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;


   float3 l = normalize(float3(sunPos.x,sunPos.z,sunPos.y));
   float3 v = normalize(float3(Input.Position.x,Input.Position.z,Input.Position.y) - eyePos);

	Output.v = v;
	Output.l = l;

   float3 Col  = perezSky ( turbidity, max ( v.z, 0.0 ) + 0.05, dot (l, v ), l.z );


	Output.Color = Col;

	Output.Color=  float3(0.5, 0.5, 0.5);

   Output.Position  = mul( Input.Position, matWVP );

   return( Output );
}


float3 ps_main( VS_OUTPUT Input ) : COLOR
{  
	return Input.Color;

  float3 convColor = convertColor(Input.Color);

  return float4 ( clamp ( convColor, 0.0, 1.0 ), 1.0 );
}

technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, vs_main() ) );
		SetPixelShader( CompileShader( ps_4_0, ps_main() ) );
	}
}