Shader "Unlit/Mask"
{
    SubShader
    {
        Tags {"Queue"="Geometry-1"}

        Blend Zero One
        ZWrite Off

        Pass
        {
            Stencil
            {
                Ref 1
                Comp always
                Pass replace
            }
        }
    }
}
