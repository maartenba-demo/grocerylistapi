﻿using System.Runtime.CompilerServices;
using VerifyTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void  Init()
    {
        VerifyHttp.Initialize();
        VerifierSettings.IgnoreMember("Authorization");
    }
}