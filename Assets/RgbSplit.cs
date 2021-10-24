using UnityEngine;
using UnityEngine.UI;

public class RgbSplit : MonoBehaviour

{
    [SerializeField] private ComputeShader _computeShader;
    [SerializeField] private Texture2D _tex;
    [SerializeField] private RawImage _renderer;
    private RenderTexture _result;
    struct ThreadSize
    {
        public uint x;
        public uint y;
        public uint z;

        public ThreadSize(uint x, uint y, uint z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    void Start()
    {
        if(!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("Comppute Shader is not supported.");
            return;
        }

        _result = new RenderTexture(_tex.width, _tex.height, 0, RenderTextureFormat.ARGB32);
        _result.enableRandomWrite = true;
        _result.Create();

        var kernelIndex = _computeShader.FindKernel("CMain");
        ThreadSize threadSize = new ThreadSize();
        _computeShader.GetKernelThreadGroupSizes(kernelIndex, out threadSize.x, out threadSize.y, out threadSize.z);
        _computeShader.SetTexture(kernelIndex, "Texture", _tex);
        _computeShader.SetTexture(kernelIndex, "Result", _result);
    }

    // Update is called once per frame
    void Update()
    {
        if(!SystemInfo.supportsComputeShaders)
        {
            Debug.LogError("Comppute Shader is not supported.");
            return;
        }

        var kernelIndex = _computeShader.FindKernel("CMain");
        ThreadSize threadSize = new ThreadSize();
        _computeShader.GetKernelThreadGroupSizes(kernelIndex, out threadSize.x, out threadSize.y, out threadSize.z);
        _computeShader.SetFloat("distance", Mathf.Sin(Time.time) * 100);
        _computeShader.Dispatch(kernelIndex, _tex.width / (int)threadSize.x, _tex.height / (int)threadSize.y, (int)threadSize.z);
        _renderer.texture = _result;
    }

    private void OnDestroy()
    {
        _result = null;
    }
}