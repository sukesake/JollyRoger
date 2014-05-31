using SharpDX.Direct3D11;

namespace SharpHelper
{
    public static class DeviceExtensions
    {
        /// <summary>
        ///     Set current rasterizer state to default
        /// </summary>
        public static Device SetDefaultRasterState(this Device device)
        {
            var rasterDescription = RasterizerStateDescription.Default();

            device.ImmediateContext.Rasterizer.State = new RasterizerState(device, rasterDescription);

            return device;
        }

        /// <summary>
        ///     Set current rasterizer state to wireframe
        /// </summary>
        public static Device SetWireframeRasterState(this Device device)
        {
            var rasterDescription = RasterizerStateDescription.Default();
            rasterDescription.FillMode = FillMode.Wireframe;

            device.ImmediateContext.Rasterizer.State = new RasterizerState(device, rasterDescription);

            return device;
        }

        ///// <summary>
        ///// Clear backbuffer and zbuffer
        ///// </summary>
        ///// <param name="color">background color</param>
        //public static Device Clear(this Device device, Color color)
        //{
        //    device.ImmediateContext.ClearRenderTargetView(_backbufferView, color);
        //    device.ImmediateContext.ClearDepthStencilView(_zbufferView, DepthStencilClearFlags.Depth, 1.0F, 0);
        //}
    }
}