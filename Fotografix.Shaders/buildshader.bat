fxc %1.hlsl /nologo /T lib_4_0 /D D2D_FUNCTION /D D2D_ENTRY=main /Fl %1.fxlib /I %2
fxc %1.hlsl /nologo /T ps_4_0 /D D2D_FULL_SHADER /D D2D_ENTRY=main /E main /setprivate %1.fxlib /Fo:%1.cso /I %2
