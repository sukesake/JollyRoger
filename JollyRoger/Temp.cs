using SharpDX;
using SharpHelper;

namespace JollyRoger
{
    public class Temp
    {
        private readonly SharpDevice _device;
        private SharpMesh _mesh;

        public Temp(SharpDevice device)
        {
            _device = device;
            _mesh = new Voxel(_device).Mesh;
        }

        public void Draw()
        {
            _mesh.Draw();
        }

        public void Update()
        {

        }
    }
}