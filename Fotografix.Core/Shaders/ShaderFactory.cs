using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Shaders
{
    public static class ShaderFactory
    {
        public static PixelShaderEffect CreatePixelShaderEffect(string shaderName)
        {
            using (var stream = typeof(ShaderFactory).Assembly.GetManifestResourceStream(typeof(ShaderFactory), shaderName + ".cso"))
            {
                int length = (int)stream.Length;
                byte[] shaderCode = new byte[stream.Length];
                stream.Read(shaderCode, 0, length);
                return new PixelShaderEffect(shaderCode);
            }
        }
    }
}
