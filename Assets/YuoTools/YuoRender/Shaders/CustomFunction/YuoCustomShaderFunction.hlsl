void Test_half(half3 In, out half3 Out)
{
    Out = In * 0.2f;
}

void ACESFilm_half(half3 In, out half3 Out)
{
    const half a = 2.51f;
    const half b = 0.03f;
    const half c = 2.43f;
    const half d = 0.59f;
    const half e = 0.14f;
    Out = saturate(In * (a * In + b) / (In * (c * In + d) + e));
}
