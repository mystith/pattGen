#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

Texture2D SpriteTexture;
float DETAIL;
float ZOOM;
float MULTI;
float X_PAN;
float Y_PAN;
float WIDTH;
float HEIGHT;
float MAX_DIST;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float3 HUEtoRGB(in float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float3(R, G, B));
}

float4 BasicDist(VertexShaderOutput input) : COLOR
{
	float x = (input.Position.x - (WIDTH / 2) + X_PAN) / ZOOM;
	float y = (input.Position.y - (HEIGHT / 2) + Y_PAN) / ZOOM;
	if (DETAIL > 1) {
		x = round(x / DETAIL) * DETAIL;
		y = round(y / DETAIL) * DETAIL;
	}

	float f = (((x * x) + (y * y)) / MAX_DIST) * MULTI;
	if (f > 1) f = 1;
	if (f < 0) f = 0;
	float3 abc = HUEtoRGB(f);
	return float4(abc.x, abc.y, abc.z, 1);
}

float4 Sine(VertexShaderOutput input) : COLOR
{
	float x = (input.Position.x - (WIDTH / 2) + X_PAN) / ZOOM;
	float y = (input.Position.y - (HEIGHT / 2) + Y_PAN) / ZOOM;
	if (DETAIL > 1) {
		x = round(x / DETAIL) * DETAIL;
		y = round(y / DETAIL) * DETAIL;
	}

	float f = sin(((x * x) + (y * y))) * MULTI;
	if (f > 1) f = 1;
	if (f < 0) f = 0;
	float3 abc = HUEtoRGB(f);
	return float4(abc.x, abc.y, abc.z, 1);
}

float4 LoopedDist(VertexShaderOutput input) : COLOR
{
	float x = (input.Position.x - (WIDTH / 2) + X_PAN) / ZOOM;
	float y = (input.Position.y - (HEIGHT / 2) + Y_PAN) / ZOOM;
	if (DETAIL > 1) {
		x = round(x / DETAIL) * DETAIL;
		y = round(y / DETAIL) * DETAIL;
	}

	float f = (((x * x) + (y * y)) / MAX_DIST) % 2 * MULTI;
	if (f > 1) f = 1;
	if (f < 0) f = 0;
	float3 abc = HUEtoRGB(f);
	return float4(abc.x, abc.y, abc.z, 1);
}

technique SpriteDrawing
{
	pass BasicDist
	{
		PixelShader = compile PS_SHADERMODEL BasicDist();
	}

	pass LoopedDist
	{
		PixelShader = compile PS_SHADERMODEL LoopedDist();
	}

	pass Sine
	{
		PixelShader = compile PS_SHADERMODEL Sine();
	}
};