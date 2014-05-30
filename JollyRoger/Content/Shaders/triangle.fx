float4x4 WVP;
float4x4 World;
float4x4 View;
float4x4 Projection;

float3 Index;
 
float timeOfDay = 13;

float3 CameraPosition;
//float FogNear = 250;
//float FogFar = 300;

float3    g_LightDir;

float4 HorizonColor;
float4 SunColor;		
float4 NightColor;

float4 MorningTint;		
float4 EveningTint;	

Texture2D AtlasTexture : register(t0);

SamplerState MeshTextureSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
	MaxLOD = 8;
};

struct VS_IN
{
	float4 pos : POSITION;
	float3 normal : NORMAL;
	float2 tex : TEXCOORD;
};
 
struct VS_OUTPUT
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float3 normal : NORMAL;
	float2 tex : TEXCOORD;
};
 
VS_OUTPUT VS( VS_IN input )
{
	float x = input.pos.x;// int(input.pos & 0xFF);
	float y = input.pos.y;// int((input.pos & 0xFF00)>> 8);
	float z = input.pos.z;// int((input.pos & 0xFF0000)>> 16);
	float w = input.pos.w;// int((input.pos & 0xFF000000)>> 24);
		
	// Пока не реализована смена дня и ночи
	float light = ((float)w)/16;

	VS_OUTPUT output = (VS_OUTPUT)0;

	float4 worldPosition = mul(float4((float)Index.x + (float)x, (float)y, (float)Index.z + (float)z, 1), World);

	//output.normal = normalize(mul(input.normal*-1, World));

	output.pos = mul(worldPosition, View);

    output.pos = mul(output.pos, Projection);

	output.tex = input.tex;

	output.col.rgb = light;
	output.col.a = 1;

	return output;
}
 
float4 PS( VS_OUTPUT input ) : SV_Target
{
	// Ambient light
	//float Ai = 0.8f;
	//float4 Ac = float4(0.075, 0.075, 0.2, 1.0);
	
	//float AixAc = float4(0.06, 0.06, 0.16, 0.8);

	// Diffuse light
	//float Di = 1.0f;
	//float4 Dc = float4(1.0, 1.0, 1.0, 1.0);
	//float DixDc = float4(1.0, 1.0, 1.0, 1.0);

	// return Ambient light * diffuse light. See tutorial if
	// you dont understand this formula	
	//float4 color = AixAc + DixDc;// * normalize(float4(g_LightDir,1));//saturate(dot(g_LightDir, input.normal));

	float4 atlasColor = AtlasTexture.Sample(MeshTextureSampler, input.tex);

	atlasColor.rgb *= input.col;//*saturate(dot(input.normal, g_LightDir*-1));

	return atlasColor;
}
 
  
technique10 Render
{
	pass P0
	{
		SetGeometryShader( 0 );
		SetVertexShader( CompileShader( vs_4_0, VS() ) );
		SetPixelShader( CompileShader( ps_4_0, PS() ) );
	}
}