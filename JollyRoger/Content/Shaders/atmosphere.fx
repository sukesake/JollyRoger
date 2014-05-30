//--------------------------------------------------------------------------------------
// Global variables
//--------------------------------------------------------------------------------------
// matrixes
float4x4  g_MatrixViewProj;
float4x4  g_MatrixWorld;

// light
float3    g_LightDir;
float4    g_LightDiffuse = float4( 0.8f, 0.8f, 0.8f,0.8f );;

// camera
float3    g_EyePos;

// atmospheric scattering

// planet inner radius, planet outer radius, height min, max
float4    g_RadiusInOutHeightMinMax;
// scattering coefficients : KrESun, KmESun, Kr4PI, Km4PI
float4    g_ScatterFactors;
// optical depth : scale, scaleDepth, ScaleOverScaleDepth, Iterations
float4    g_ScaleIter;
// gravity and Exposure : g, g ^ 2, exposure, 
float4    g_Gravity;
// wave length
float4    g_InvWaveLength;

//--------------------------------------------------------------------------------------
// Structures
//--------------------------------------------------------------------------------------
struct vertexInput 
{
    float3 position        : POSITION;
    //float3 normal        : NORMAL;
};

struct vertexOutput 
{
    float4 hPosition      : POSITION;
	float4 ColorRayleigh    : TEXCOORD0;
	float4 ColorMie        : TEXCOORD1;
	float3 Direction      : TEXCOORD2;
};

//--------------------------------------------------------------------------------------
// Vertex support functions
//--------------------------------------------------------------------------------------
float RayIntersection( float3 f3Pos, float3 f3Dir, float fOuterRadius ) 
{
  float A = dot(f3Dir, f3Dir);
  float B = 2.0f *  dot(f3Pos, f3Dir);
  float C = dot( f3Pos, f3Pos ) - fOuterRadius * fOuterRadius;
  
  float t0 = (-B + sqrt(B*B - 4.0f * A * C) ) / 2.0f * A;
  float t1 = (-B - sqrt(B*B - 4.0f * A * C) ) / 2.0f * A;

  return t0 >= 0.0f ? t0 : t1;
}

float scale(float cos)
{
  float x = 1.0f - cos;
  return g_ScaleIter.y * exp(-0.00287f + x*(0.459f + x*(3.83f + x*(-6.80f + x*5.25f))));
}

//--------------------------------------------------------------------------------------
// Vertex shader
//--------------------------------------------------------------------------------------
vertexOutput VS_Scattering( vertexInput IN ) 
{
    vertexOutput OUT;

  // world space vertex position without camera position
  float3 f3WorldPos  = mul( float4( IN.position, 1.0f ), g_MatrixWorld );

  // camera always in atmosphere
  
  // camera always in center of sphere, only height change
  // position camera in world space
  float3 v3Camera  = float3( 0.0f, g_EyePos.y, 0.0f );
  // ray from camera to current vertex (sky pos) in begin world space
  float3 v3Ray  = normalize(f3WorldPos - v3Camera);
  
  // camera height part between Min and Max
  float fCameraHeight  = (g_EyePos.y  - g_RadiusInOutHeightMinMax.z) / (g_RadiusInOutHeightMinMax.w - g_RadiusInOutHeightMinMax.z);
  // camera height in planet space
  fCameraHeight    = g_RadiusInOutHeightMinMax.x + fCameraHeight * (g_RadiusInOutHeightMinMax.y - g_RadiusInOutHeightMinMax.x);
  
  // star position in planet space
  float3 v3Start  = float3( 0.0f, fCameraHeight, 0.0f );
  
  float fDistance  = RayIntersection( v3Start, v3Ray, g_RadiusInOutHeightMinMax.y );
    
  // star angle
  float fStartAngle  = dot(v3Ray, v3Start) / fCameraHeight;
    
  // depth = exp(ScaleOverScaleDepth * ( inner radius - eye height )
  float fStartDepth  = exp(g_ScaleIter.z * ( g_RadiusInOutHeightMinMax.x - fCameraHeight ) );
  float fStartOffset = fStartDepth * scale(fStartAngle);

  // Initialize the scattering loop variables

  float fSampleLength = fDistance / g_ScaleIter.w;
  float fScaledLength = fSampleLength * g_ScaleIter.x;

  float3 v3SampleRay    = v3Ray * fSampleLength;
  float3 v3SamplePoint  = v3Start + v3SampleRay * 0.5;

  // Now loop through the sample points

  float3 v3FrontColor = float3(0.0, 0.0, 0.0);
  
  for(int i=0; i< (int)g_ScaleIter.w; i++) 
  {

              float fHeight = length(v3SamplePoint);
    // depth = exp(ScaleOverScaleDepth * ( inner radius - cur height )
              float fDepth = exp(g_ScaleIter.z * (g_RadiusInOutHeightMinMax.x - fHeight) );

              float fLightAngle = dot(-g_LightDir.xyz, v3SamplePoint) / fHeight;

              float fCameraAngle = dot(v3Ray, v3SamplePoint) / fHeight;

              float fScatter = (fStartOffset + fDepth * (scale(fLightAngle) - scale(fCameraAngle)));

              float3 v3Attenuate = exp(-fScatter * (g_InvWaveLength.xyz * g_ScatterFactors.z + g_ScatterFactors.w));

              v3FrontColor += v3Attenuate * (fDepth * fScaledLength);

              v3SamplePoint += v3SampleRay;
  }

  float4x4 mWorldAlignCamera  = g_MatrixWorld;
  mWorldAlignCamera._41_43  += g_EyePos.xz;
  // view space pos
        OUT.hPosition    = mul( float4(  IN.position, 1.0f ), mul( mWorldAlignCamera, g_MatrixViewProj ) );

  // Finally, scale the Mie and Rayleigh colors
  OUT.ColorRayleigh.xyz  = v3FrontColor * (g_InvWaveLength.xyz * g_ScatterFactors.x);
  OUT.ColorRayleigh.w    = 1.0f;
  
  OUT.ColorMie.xyz    = v3FrontColor * g_ScatterFactors.y;
  OUT.ColorMie.w      = 1.0f;

  OUT.Direction      = v3Camera - f3WorldPos;
    
    return OUT;
}

//--------------------------------------------------------------------------------------
// Pixel support functions
//--------------------------------------------------------------------------------------

float getRayleighPhase( float fCos2)
{
  return 0.75 * (1.0 + fCos2);
}

float getMiePhase( float fCos, float fCos2 )
{
  float3 t3;
  t3.x = 1.5f * ( (1.0f - g_Gravity.y) / (2.0f + g_Gravity.y) );
  t3.y = 1.0f + g_Gravity.y;
  t3.z = 2.0f * g_Gravity.x;
  
  return t3.x * (1.0f + fCos2) / pow(t3.y - t3.z * fCos, 1.5f);
}

//--------------------------------------------------------------------------------------
// Pixel shader
//--------------------------------------------------------------------------------------
float4 PS_Scattering( vertexOutput IN ):SV_Target
{
  
  float4 Color = float4( 1.0f, 1.0f, 1.0f, 1.0f );
  
  float2 fCos;
  fCos.x = dot(-g_LightDir.xyz, IN.Direction ) / length(IN.Direction);
  fCos.y = fCos.x * fCos.x;

//  Color.rgb = getRayleighPhase(fCos2) * IN.ColorRayleigh.rgb + getMiePhase(fCos, fCos2) * IN.ColorMie.rgb;
  // getRayleighPhase - HDR
  // getMiePhase - sun disk
  Color.rgb = getRayleighPhase(fCos.y) * IN.ColorRayleigh.rgb + getMiePhase(fCos.x, fCos.y) * IN.ColorMie.rgb;
  

  Color.a = 1.0f;
  
  return Color;
}

technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS_Scattering() ) );
		SetPixelShader( CompileShader( ps_4_0, PS_Scattering() ) );
	}
}